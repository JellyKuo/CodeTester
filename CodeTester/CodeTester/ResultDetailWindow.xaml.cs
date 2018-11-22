using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using DiffMatchPatch;

namespace CodeTester
{
    /// <summary>
    /// ResultDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ResultDetailWindow : Window
    {
        Result result;

        public ResultDetailWindow(Result result)
        {
            InitializeComponent();
            this.result = result;
            rtbAnswer.AppendText(result.Test.Answer.Content);
            rtbOutput.AppendText(result.Output);
            DataContext = result;
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(result.Test.Input.Content.Replace("\r", Environment.NewLine));
            MessageBox.Show("Content copied!", result.Test.Name, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDiff_Click(object sender, RoutedEventArgs e)
        {
            diff_match_patch DIFF = new diff_match_patch();

            List<Diff> diffs = DIFF.diff_main(result.Test.Answer.Content, result.Output);
            DIFF.diff_cleanupSemanticLossless(diffs);

            rtbOutput.Document = new FlowDocument();
            foreach (var d in diffs)
            {
                var range = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                if (d.operation == Operation.DELETE)
                    continue;
                range.Text = d.text.Replace("\n", Environment.NewLine);
                if (d.operation == Operation.INSERT)
                    range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightSalmon);
                else
                    range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);

            }
        }
    }
}
