using MetaFilesystem.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var newEntries = new List<FileEntry>();
            CollectFileEntries(srcDir, newEntries);
            _entities.Clear();
            _entities.AddRange(newEntries);
        }

        private List<FileEntry> CollectFileEntries(string directory, List<FileEntry> results)
        {
            var ignoreCaseComparison = new Comparison<string>((a, b) =>
            {
                return String.Compare(a, b, ignoreCase: true);
            });

            System.Diagnostics.Debug.WriteLine(directory);

            try
            {
                var filenames = new List<string>(System.IO.Directory.GetFiles(directory));
                filenames.Sort(ignoreCaseComparison);
                foreach (var fileName in filenames)
                {
                    results.Add(new FileEntry() { Name = System.IO.Path.GetFileName(fileName), Location = directory });
                }
            }
            catch(Exception ex)
            {
                LogError(ex);
            }

            try
            {
                var dirnames = new List<string>(System.IO.Directory.GetDirectories(directory));
                dirnames.Sort(ignoreCaseComparison);
                foreach (var dirName in dirnames)
                {
                    CollectFileEntries(System.IO.Path.Combine(directory, dirName), results);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return results;
        }

        private void LogError(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
