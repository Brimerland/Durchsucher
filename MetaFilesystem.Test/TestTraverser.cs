using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MetaFilesystem.FileSystemTraverser;
using System.Diagnostics;

namespace MetaFilesystem.Test
{
    [TestClass]
    public class TestTraverser
    {
        [TestMethod]
        public void TraverseUserDirectory()
        {
            //var srcDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.None);
            var srcDir = @"\\mcp\archiv";

            var traverser = new FileSystemTraverser();
            traverser.Traverse(srcDir, new Handler() );

        }

        private class Handler : AbstractTraversalHandlerBase
        {
            public override void ProcessEntry(TraversalEntry traversalEntry)
            {
                if (true)//traversalEntry.Info.FullName.ToLower().Contains("durchsucher"))
                {
                    Debug.WriteLine(traversalEntry.Info.FullName);
                }
            }
        }
    }
}
