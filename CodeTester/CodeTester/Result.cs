using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CodeTester
{
    public class Result
    {
        public Test Test { get; set; }
        public ResultTypes ResultType { get; set; }
        public BitmapImage Image
        {
            get
            {
                switch (ResultType)
                {
                    case ResultTypes.Running:
                        return new BitmapImage(new Uri("pack://application:,,,/CodeTester;component/Resources/Gear.png"));
                    case ResultTypes.AC:
                        return new BitmapImage(new Uri("pack://application:,,,/CodeTester;component/Resources/Check.png"));
                    case ResultTypes.WA:
                        return new BitmapImage(new Uri("pack://application:,,,/CodeTester;component/Resources/Cross.jpg"));
                    case ResultTypes.TLE:
                        return new BitmapImage(new Uri("pack://application:,,,/CodeTester;component/Resources/Time.png"));
                }
                return new BitmapImage(new Uri("pack://application:,,,/CodeTester;component/Resources/Question.png"));
            }
        }

        public string ResultString
        {
            get
            {
                switch (ResultType)
                {
                    case ResultTypes.Running:
                        return "Running";
                    case ResultTypes.AC:
                        return "Correct";
                    case ResultTypes.WA:
                        return "Wrong Answer";
                    case ResultTypes.TLE:
                        return "Time Limit Exceeded";
                }
                return "Unknown";
            }
        }

        public TimeSpan TimeTaken { get; set; }
        public bool IsCompleted { get; set; }
        public string Output { get; set; }

    }

    public enum ResultTypes { Running, AC, WA, TLE }
}
