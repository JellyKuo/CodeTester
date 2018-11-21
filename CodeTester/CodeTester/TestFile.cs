using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeTester
{
    public class TestFile
    {
        public string Path { get; set; }
        public string Content { get; set; }

        public string Size
        {
            get
            {
                if (_Size != null)
                    return _Size;
                if (File.Exists(Path))
                {
                    var info = new FileInfo(Path);
                    _Size = FileSize.FileSizeToString(info.Length);
                }
                return _Size;
            }
        }

        private string _Size { get; set; }

        public TestFile(string Path)
        {
            this.Path = Path;
            Content = File.ReadAllText(Path);
        }
    }
}
