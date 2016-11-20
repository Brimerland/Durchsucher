using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetaFilesystem.Test
{
    [TestClass]
    public class TestFileEntryRepository
    {
        [TestMethod]
        public void SaveToFile()
        {
            //Arrange
            var repository = new FileEntryRepository();
            var srcDir = @"\\mcp\archiv";
            //var srcDir = @"c:\temp";

            //Act
            repository.LoadFromDirectory(srcDir);
            repository.SaveToFile(@"c:\temp\output.dat");


        }
    }
}
