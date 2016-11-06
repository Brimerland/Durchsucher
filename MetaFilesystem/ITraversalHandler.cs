using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaFilesystem.FileSystemTraverser;

namespace MetaFilesystem
{
    public interface ITraversalHandler
    {
        void ProcessEntry(TraversalEntry traversalEntry);
        void HandleException(String directoryName, Exception ex);
        bool ShouldTraverseDirectory(String directoryName);
    }
}
