using GdaTools;
using MetaFilesystem.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MetaFilesystem
{
    public class FileEntryRepository
    {
        private List<FileEntry> _entities = new List<FileEntry>();
        private static XmlSerializer _serializer = new XmlSerializer(typeof(List<FileEntry>));


        public List<FileEntry> GetAll()
        {
            return _entities.ToList();
        }

        public List<FileEntry> GetByFilter(Predicate<FileEntry> shouldInclude)
        {
            List<FileEntry> retValue = new List<FileEntry>();
            foreach (var element in _entities)
            {
                if (shouldInclude(element))
                {
                    retValue.Add(element);
                }
            }
            return retValue;
        }

        public void LoadFromFile(String fileName)
        {
            var sr = new StringReader(File.ReadAllText(fileName));
            var newEntries = (List<FileEntry>)_serializer.Deserialize(sr);
            _entities.Clear();
            _entities.AddRange(newEntries);
        }

        public void SaveToFile(string dataFile)
        {
            using (var sw = new StringWriter())
            {
                _serializer.Serialize(sw, _entities);
                File.WriteAllText(dataFile, sw.ToString());
            }
        }

        public void LoadFromDirectory(string srcDir)
        {
            var newEntries =  CollectFileEntries(srcDir);
            _entities.Clear();
            _entities.AddRange(newEntries);
        }

        private class TraversalHandler : AbstractTraversalHandlerBase
        {
            public List<FileEntry> Entries = new List<FileEntry>();
            private ActionFilter _actionFilter = new ActionFilter();

            public override void ProcessEntry(FileSystemTraverser.TraversalEntry traversalEntry)
            {
                FileInfo fileInfo = traversalEntry.Info as FileInfo;
                if (null != fileInfo)
                {
                    _actionFilter.MaybeExecute(()=> { Debug.WriteLine(fileInfo.FullName); });
                    
                    String name = fileInfo.Name;
                    String location = fileInfo.DirectoryName;
                    long length = fileInfo.Length;
                    Entries.Add(new FileEntry()
                    {
                        Name = name,
                        Location = location,
                        Size = length
                    });
                }
            }

            private void LogError(Exception ex)
            {
                File.AppendAllText(@"c:\temp\error.log", ex.ToString() + "\n");
                Debug.WriteLine(ex);
            }
        }

        private void LogError(Exception ex)
        {
            File.AppendAllText(@"c:\temp\error.log", ex.ToString() + "\n");
            Debug.WriteLine(ex);
        }

        public void CalculateHash(FileEntry entry)
        {
            entry.Hash = ByteTools.ToByteString(FileTools.CreateMD5(Path.Combine(entry.Location, entry.Name)));
        }

        private List<FileEntry> CollectFileEntries(string directory)
        {
            var handler = new TraversalHandler();
            var traverser = new FileSystemTraverser();
            traverser.Traverse(directory, handler);
            return handler.Entries;
        }
    }
}
