using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFilesystem.Data
{
    public class FileEntry
    {
        public String Name { get; set; }
        public long Size { get; set; }
        public String Location { get; set; }
        public String Hash { get; set; }
    }
}
