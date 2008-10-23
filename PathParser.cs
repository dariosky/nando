using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;


namespace nando
{
    // create the delegates and events to append some action before and after disk parsing
    public delegate object AddNodeEventHandler(string path, int type, object tag);

    class PathParser
    {
        private BackgroundWorker _worker;
        public event RunWorkerCompletedEventHandler AfterParsing;
        public event AddNodeEventHandler AddNode;
        private string _root;
        private int _rootLength;
        private int fileParsed = 0;
        public int FileParsed { get { return fileParsed; } }
        private int directoryParsed = 0;
        public int DirectoryParsed { get { return directoryParsed; } }
        
        public static int FILETYPE = 0;
        public static int DIRTYPE = 1;

        public string Root
        {
            get { return _root; }
            set
            {
                // FIXED: Fix Root path, to avoid ending slashes and get fullpath
                _root = Path.GetFullPath(value).TrimEnd(Path.DirectorySeparatorChar);
                _rootLength = _root.Length + 1;
            }
        }
        private struct ToParseItem
        {
            public string Path;    // the path of the node parsed
            public object Tag;     // some info about the node
            public int type;       // type of item

            public ToParseItem(string path, object tag, int type)
            {
                this.Path = path;
                this.Tag = tag;
                this.type = type;
            }
            public ToParseItem(string path, object tag) : this(path, tag, DIRTYPE) { }

        }
        private List<ToParseItem> _itemToParse;

        public PathParser()
        {
            _worker = new BackgroundWorker();

            _worker.DoWork -= ProcessToDoList;
            _worker.DoWork += ProcessToDoList;
            _worker.WorkerSupportsCancellation = true;
        }

        public void Parse(string path, object tag)
        {
            this.Root = path;
            if (_worker.IsBusy) throw new Exception("Parse in progress, please cancel the pending process before initiate a new one.");
            _itemToParse = new List<ToParseItem>();
            ToParseItem rootNode = new ToParseItem("", tag);// add the root node to the list of nodes to parse
            _itemToParse.Add(rootNode);
            this.directoryParsed = 0;
            this.fileParsed = 0;
            _worker.RunWorkerCompleted -= AfterParsing;
            _worker.RunWorkerCompleted += AfterParsing;
            _worker.RunWorkerAsync();
        }

        public void CancelParsing()
        {
            if (_worker.IsBusy) _worker.CancelAsync();
        }

        void ProcessToDoList(object sender, DoWorkEventArgs e)
        {
            while (_itemToParse.Count > 0)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    e.Result = "Parsing cancelled";
                    return;
                }
                ToParseItem nodeToParse = _itemToParse[0];
                _itemToParse.RemoveAt(0);

                object parent = null;
                if (this.AddNode != null) parent = this.AddNode(nodeToParse.Path, nodeToParse.type, nodeToParse.Tag);

               
                if (nodeToParse.type == DIRTYPE)
                    try // se siamo in una cartella inserisco come da processare tutte le sottocartelle e i sottofile
                    {
                        this.directoryParsed += 1;
                        string fullpath = System.IO.Path.Combine(this.Root, nodeToParse.Path);
                        foreach (string str in Directory.GetDirectories(fullpath))
                            _itemToParse.Add(
                                new ToParseItem(str.Substring(_rootLength), parent)
                            );

                        foreach (string str in Directory.GetFiles(fullpath + Path.DirectorySeparatorChar))
                        {
                            _itemToParse.Add(
                                new ToParseItem(str.Substring(_rootLength), parent, FILETYPE)
                            );
                        }
                        //	_itemToParse.Add(new FileItem(str, parent, this));
                    }
                    catch (IOException) { }
                else
                {
                    this.fileParsed += 1;
                }

            }
            e.Result = "Parsing complete";
        }


    }

}
