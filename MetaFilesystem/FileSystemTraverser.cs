using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFilesystem
{
    public class FileSystemTraverser
    {
        public class TraversalEntry
        {
            public FileSystemInfo Info { get; set; }
        }

        public void Traverse(String directoryName, ITraversalHandler handler)
        {
            if (handler.ShouldTraverseDirectory(directoryName))
            {
                handler.ProcessEntry(new TraversalEntry() { Info = new DirectoryInfo(directoryName) });
                try
                {
                    foreach (var fileNameString in Sort(Directory.GetFiles(directoryName)))
                    {
                        handler.ProcessEntry(new TraversalEntry() { Info = new FileInfo(fileNameString) });
                    }
                }
                catch (Exception ex)
                {
                    handler.HandleException(directoryName, ex);
                }

                try
                {
                    foreach (var subDirectoryName in Sort(Directory.GetDirectories(directoryName)))
                    {
                        Traverse(subDirectoryName, handler);
                    }
                }
                catch (Exception ex)
                {
                    handler.HandleException(directoryName, ex);
                }
            }
        }


        private String[] Sort(String[] names)
        {
            Array.Sort(names, (a, b) => { return String.Compare(a, b, ignoreCase: true); });
            return names;
        }
    }
}
