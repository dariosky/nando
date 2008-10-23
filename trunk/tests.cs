using System;
using System.Collections.Generic;
using System.Text;

namespace nando
{
    using NUnit.Framework;
    using System.ComponentModel;
    [TestFixture]
    public class NandoSqlTest
    {
        private nandoSql db = new nandoSql();
        private string tempFilename = System.IO.Path.GetTempFileName();
        [Test]
        public void EmptySave()
        {
            db.Save(tempFilename);
        }

        [Test]
        public void EmptyLoad()
        {
            db.Load(tempFilename);
        }


        void OnAfterParsingFunction(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine(System.IO.Path.GetFullPath(@"../../mp3test"));
            Console.WriteLine("dirs: {0} files: {1}", db.DirectoryParsed, db.FileParsed);

        }

        [Test]
        public void ParseMp3Test() {
            db.AfterParsing += OnAfterParsingFunction;
            db.Parse(@"../../mp3test", "mp3test");
        }
    }
}
