using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderConsole
{
    public class PhotoFile
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PathToSave { get; set; }
    }
}
