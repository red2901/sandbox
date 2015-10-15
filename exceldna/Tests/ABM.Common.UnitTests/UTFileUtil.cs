using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ABM.Common.UnitTests
{
    [TestClass]
    public class UTFileUtil
    {
        [TestMethod]
        public void ReadFileIntoString()
        {
            var stringList = FileUtil.ReadFileIntoStringList(@"C:\Users\Kin\Documents\Work\Subversion\QMA\Tests\NG.csv");

            Assert.IsTrue(stringList.Count > 0);
        }
    }
}
