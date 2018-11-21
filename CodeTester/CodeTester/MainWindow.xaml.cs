using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeTester
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DateFormat = "yyyy/MM/dd hh:mm:ss";
        const string AppName = "NPSC";
        const string ExeExtension = ".exe";
        const string InputExtension = ".in";
        const string AnswerExtension = ".ans";
        const int ParallelTasks = 4;
        int _TimeLimit;
        int TimeLimit
        {
            get
            {
                return _TimeLimit;
            }
            set
            {
                _TimeLimit = value;
                if (IsLoaded)
                    mItemTimeLimit.Header = $"Time Limit: {_TimeLimit} ms";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Tests = new ObservableCollection<Test>();
            Results = new ObservableCollection<Result>();
            dgTestData.ItemsSource = Tests;
            dgResult.ItemsSource = Results;
        }

        ObservableCollection<Test> Tests;
        ObservableCollection<Result> Results;

        private void SelectExe(string Path)
        {
            txtExePath.Text = Path;
            if (!File.Exists(Path))
            {
                MessageBox.Show("Cannot find the selected file!", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var fInfo = new FileInfo(Path);
            txtExeName.Text = fInfo.Name;
            txtExeCreatedAt.Text = fInfo.CreationTime.ToString(DateFormat);
            txtExeModifiedAt.Text = fInfo.LastWriteTime.ToString(DateFormat);
            txtExeSize.Text = FileSize.FileSizeToString(fInfo.Length);
        }

        private void AddTestFile(string Path)
        {
            if (!File.Exists(Path))
            {
                MessageBox.Show($"Cannot find {Path}! File will be ignored.", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var fInfo = new FileInfo(Path);
            AddTestFile(fInfo);
        }

        private void AddTestFile(FileInfo fileInfo)
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name);
            Test test = Tests.FirstOrDefault(t => t.Name == fileName);
            bool newTest = test == null;
            if (newTest)
                test = new Test { ID = Tests.Count + 1, Name = fileName };
            switch (fileInfo.Extension)
            {
                case InputExtension:
                    test.Input = new TestFile(fileInfo.FullName);
                    break;
                case AnswerExtension:
                    test.Answer = new TestFile(fileInfo.FullName);
                    break;
            }
            if (newTest)
                Tests.Add(test);
        }

        private void ProcessInputFile(string Path)
        {
            if (!File.Exists(Path))
            {
                MessageBox.Show($"Cannot find {Path}! File will be ignored.", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var fileInfo = new FileInfo(Path);
            ProcessInputFile(fileInfo);
        }
        private void ProcessInputFile(FileInfo fileInfo)
        {
            switch (fileInfo.Extension)
            {
                case ExeExtension:
                    SelectExe(fileInfo.FullName);
                    break;
                case InputExtension:
                case AnswerExtension:
                    AddTestFile(fileInfo);
                    break;
            }
        }

        private Result Run(Test test)
        {
            string ExePath;
            if (!txtExePath.Dispatcher.CheckAccess())
                ExePath = txtExePath.Dispatcher.Invoke(() => ExePath = txtExePath.Text);
            else
                ExePath = txtExePath.Text;
            var result = new Result();
            result.Test = test;
            var stopwatch = new Stopwatch();
            var startInfo = new ProcessStartInfo(ExePath);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.Domain =null;
            startInfo.UserName = null;
            startInfo.Password = null;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            Process process =new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;
            stopwatch.Start();
            process.Start();
            process.StandardInput.WriteLine(test.Input.Content);
            if (!process.WaitForExit(TimeLimit))
            {
                stopwatch.Stop();
                result.IsCompleted = true;
                result.ResultType = ResultTypes.TLE;
                result.TimeTaken = stopwatch.Elapsed;
                result.Output = process.StandardOutput.ReadToEnd();
                return result;
            }
            stopwatch.Stop();
            var output = process.StandardOutput.ReadToEnd().Replace("\r", "");
            result.IsCompleted = true;
            if (output == test.Answer.Content)
                result.ResultType = ResultTypes.AC;
            else
                result.ResultType = ResultTypes.WA;
            result.Output = output;
            result.TimeTaken = stopwatch.Elapsed;
            return result;
        }

        private async void BeginRunTests(List<Test> tests)
        {
            Results.Clear();
            await Task.Run(() =>
            {
                for (int i = 0; i < tests.Count; i++)
                {
                    var result = Run(tests[i]);
                    if (Application.Current.Dispatcher.CheckAccess())
                        Results.Add(result);
                    else
                        Application.Current.Dispatcher.Invoke(() => Results.Add(result));
                }
            });
        }

        private void BtnExePathBrowse_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Executables (*.exe)|*.exe|All FIles (*.*)|*.*"
            };
            if (ofd.ShowDialog() != true)
                return;
            SelectExe(ofd.FileName);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = e.AllowedEffects;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files)
                {
                    if (File.Exists(f))
                    {
                        ProcessInputFile(f);
                    }
                    else if (Directory.Exists(f))
                    {
                        var dir = new DirectoryInfo(f);
                        foreach (var subFile in dir.EnumerateFiles())
                        {
                            ProcessInputFile(subFile);
                        }
                    }

                }
            }
        }

        private DataGridRow GetDgRowBySender(object sender)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                    return (DataGridRow)vis;
            return null;
        }

        private void dgTests_Details_Click(object sender, RoutedEventArgs e)
        {
            var row = GetDgRowBySender(sender);
            var testData = row.Item as Test;
            var testDetailWindow = new TestDetailWindow(testData);
            testDetailWindow.Show();
        }

        private void dgResults_Details_Click(object sender, RoutedEventArgs e)
        {
            var row = GetDgRowBySender(sender);
            var resultData = row.Item as Result;
            var resultDetailWindow = new ResultDetailWindow(resultData);
            resultDetailWindow.Show();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Tests.Clear();
            Results.Clear();
            txtExeCreatedAt.Text = "";
            txtExeModifiedAt.Text = "";
            txtExeName.Text = "";
            txtExePath.Text = "";
            txtExeSize.Text = "";
        }

        private void BtnRunAll_Click(object sender, RoutedEventArgs e)
        {
            BeginRunTests(Tests.ToList());
        }

        private void BtnRunID_Click(object sender, RoutedEventArgs e)
        {
            var inpW = new InputWindow("Test ID", TimeLimit.ToString());
            if (inpW.ShowDialog() != true)
                return;
            int id = inpW.Value;
            var test = Tests.FirstOrDefault(t => t.ID == id);
            if (test == null)
            {
                MessageBox.Show("Cannot find selected test id!", AppName, MessageBoxButton.OK, MessageBoxImage.Error); return;
            }
            BeginRunTests(new List<Test> { test });
        }

        private void BtnRunFailed_Click(object sender, RoutedEventArgs e)
        {
            var tests = Results.Where(r => r.ResultType == ResultTypes.WA || r.ResultType == ResultTypes.TLE).Select(r => r.Test).ToList();
            if (tests.Count == 0)
            {
                MessageBox.Show("No failed tests found! Make sure results contains WA or TLE results.", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            BeginRunTests(tests);
        }

        private void MItemTimeLimit_Click(object sender, RoutedEventArgs e)
        {
            var inpW = new InputWindow("Time Limit", TimeLimit.ToString());
            if (inpW.ShowDialog() != true)
                return;
            TimeLimit = inpW.Value;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimeLimit = 1000;
        }
    }
}
