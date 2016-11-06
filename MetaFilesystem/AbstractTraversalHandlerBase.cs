using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaFilesystem.FileSystemTraverser;

namespace MetaFilesystem
{
    public abstract class AbstractTraversalHandlerBase : ITraversalHandler
    {
        public void HandleException(string directoryName, Exception ex)
        {
        }

        public abstract void ProcessEntry(TraversalEntry traversalEntry);

        public bool ShouldTraverseDirectory(string directoryName)
        {
            return true;
        }


    }
}
