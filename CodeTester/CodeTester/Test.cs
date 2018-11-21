using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeTester
{
    public class Test
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Size
        {
            get
            {
                return Input?.Size;
            }
        }
        public TestFile Input { get; set; }
        public TestFile Answer { get; set; }

    }
}
