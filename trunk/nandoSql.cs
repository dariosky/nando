using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;


namespace nando
{
    public delegate void LogEventHandler(string message);

    /// <summary>
    /// nandoSql is the bridge to access a sql db storing nando archive.
    /// So this can create an archive from a treemodel parsed from files
    /// or can read from the DB to some useful format.
    /// </summary>
    public class nandoSql
    {
        //private static string[] _supportedExtension = {".asf", ".wma", ".wmv",
        //    ".avi", ".flac", ".mpc", ".mp+", ".mpp",
        //    ".mpeg", ".mpg", ".mpe", ".mpv2", ".mp3", ".mp4", ".m4a", ".m4v", ".m4p", ".ogg", ".wav", ".wv"
        //};
        public static bool ExtensionSupportTags(string ext)
        {
            return (ext == ".asf" || ext == ".wma" || ext == ".wmv" ||
                ext == ".avi" || ext == ".flac" || ext == ".mpc" || ext == ".mp+" || ext == ".mpp" ||
                ext == ".mpeg" || ext == ".mpg" || ext == ".mpe" || ext == ".mpv2" || ext == ".mp3" || ext == ".mp4" || ext == ".m4a" || ext == ".m4v" || ext == ".m4p" || ext == ".ogg" || ext == ".wav" || ext == ".wv"
                );
        }
        //if (Array.Exists<string>(_supportedExtension,
        //                         delegate(string totest)
        //                         {
        //                             return info.Extension.Equals(totest, StringComparison.InvariantCultureIgnoreCase);
        //                         }))

        SQLiteConnection cnn;
        public bool Modified = false;

        private PathParser myParser = new PathParser();
        private bool modifiedBeforeParser = false;
        public int FileParsed { get { return myParser.FileParsed; } }
        public int DirectoryParsed { get { return myParser.DirectoryParsed; } }
        public string lastParsedName;  // remember the name of the last parsed Tree
        public long lastParsedRootId;   // rember the id of last parsed root
        private SQLiteTransaction parseTransaction;
        private SQLiteCommand parseCreateItem;
        private SQLiteCommand parseCreateFile;
        private Stack<long> parseDirectoryStack;    // a stack to remember folder which size has to be fixed

        private SQLiteCommand getChildrenCommand;
        private SQLiteCommand getFilteredChildrenCommand;

        private bool isFiltered = false;    // if it's filtered join with filter table of items rowid
        private string filterQuery = "";    // the string who filtered the archive


        private bool IsKnownTable(string tableName)
        {
            tableName = tableName.ToLower();
            return (
                    tableName == "items" ||
                    tableName == "fileprops" ||
                    tableName == "usertags" ||
                    tableName == "supportdetails"
                    );
        }

        private bool IsKnownIndex(string indexName)
        {
            return false; //(indexName == "id_UserTags");
        }

        public event LogEventHandler log;

        private string QueryCreateFTS(string dbName)
        {
            return string.Format(@"
                        CREATE VIRTUAL TABLE {0}.Items_text USING FTS3( name, note );
                        CREATE VIRTUAL TABLE {0}.FileProps_text USING FTS3( path, artist, title, album, file_tags );
                        --CREATE VIRTUAL TABLE {0}.UserTags_text USING FTS3( tag );

                        -- Trigger Items ******************
                        CREATE TRIGGER {0}.update_items AFTER UPDATE OF name, note ON Items
                        BEGIN
                            update Items_text set name=new.name, note=new.note where rowid=old.rowid;
                        END;
                        CREATE TRIGGER {0}.insert_items AFTER INSERT ON Items
                        BEGIN
                            insert into Items_text(rowid, name, note) VALUES (new.rowid, new.name, new.note);
                        END;
                        CREATE TRIGGER {0}.delete_items AFTER DELETE ON Items
                        BEGIN
                            DELETE FROM Items_text WHERE rowid=old.rowid;
                        END;
                        -- Trigger FileProps ******************
                        CREATE TRIGGER {0}.update_fileprops AFTER UPDATE OF path, artist, title, album, file_tags ON FileProps
                        BEGIN
                            update FileProps_text set path=new.path, artist=new.artist, title=new.title, album=new.album, file_tags=new.file_tags where rowid=old.rowid;
                        END;
                        CREATE TRIGGER {0}.insert_fileprops AFTER INSERT ON FileProps
                        BEGIN
                            insert into FileProps_text(rowid, path, artist, title, album, file_tags) VALUES (new.rowid, new.path, new.artist, new.title, new.album, new.file_tags);
                        END;
                        CREATE TRIGGER {0}.delete_fileprops AFTER DELETE ON FileProps
                        BEGIN
                            DELETE FROM FileProps_text WHERE rowid=old.rowid;
                        END;
                    ", dbName);
        }
        private string QueryCreateFTS() { return QueryCreateFTS("main"); }

        private string QueryCreateString(string dbName)
        {
            string createDBQuery = @"
                CREATE TABLE {0}.[Items] (
                  --[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                    parent INTEGER, 
                    name TEXT COLLATE NOCASE NOT NULL,
                    size INTEGER NOT NULL,
                    note TEXT
                );

                CREATE TABLE {0}.[FileProps] (
                   --id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                   
                   path text,
                   title text, artist text, album text, file_tags text,
                   duration INTEGER
                );


                CREATE TABLE {0}.[UserTags] (
                    id integer not null,
                    tag text
                );

                CREATE TABLE {0}.[SupportDetails] (
                    --[id] INTEGER NOT NULL PRIMARY KEY,
                    [creation_date] DATETIME,
                    [file_count] INTEGER,
                    [dir_count] INTEGER
                );

            ";
            return String.Format(createDBQuery, dbName);
        }
        private string QueryCreateString() { return QueryCreateString("main"); }

        private string QueryDeleteString(string dbName)
        {
            /*
             * Get the list of all table and indexes and delete all the known one (that will be recreated)
             * */
            string deleteString = "";
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = String.Format(@" SELECT name from {0}.sqlite_master where type=?; ", dbName);
                cmd.Parameters.Add(new SQLiteParameter());
                cmd.Parameters[0].Value = "table";

                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string tableName = reader.GetString(0) as string;
                    if (IsKnownTable(tableName))
                    {
                        deleteString += String.Format(@"
                                DROP TABLE {0}.[{1}];
                            ", dbName, tableName);
                    }
                }
                reader.Close();
            }
            return String.Format(deleteString, dbName);
        }

        private string QueryCopyString(string dbSource, string dbTarget)
        {
            string copyString = @"
                insert into {1}.Items(rowid, parent, name, size, note)
                               select rowid, parent, name, size, note from {0}.Items;

                insert into {1}.FileProps(rowid, path, title, artist, album, file_tags, duration)
                                   select rowid, path, title, artist, album, file_tags, duration from {0}.FileProps;

                insert into {1}.UserTags(rowid, id, tag)
                                  select rowid, id, tag from {0}.UserTags;

                insert into {1}.SupportDetails(rowid, creation_date, file_count, dir_count)
                                        select rowid, creation_date, file_count, dir_count from {0}.SupportDetails;

            ";
            copyString = String.Format(copyString, dbSource, dbTarget);

            return copyString;
        }

        private void OpenConnection(string path)
        {
            if (string.IsNullOrEmpty(path)) path = @"Data source=:memory:";
            cnn = new SQLiteConnection(path);
            cnn.Open();

            if (cnn.GetSchema("Tables", new string[] { null, null, "Items", null }).Rows.Count == 0)
            {
                using (SQLiteTransaction dbTrans = cnn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = QueryCreateString();
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = QueryCreateFTS(); // we're in :memory: create FTS
                        cmd.ExecuteNonQuery();
                    }
                    dbTrans.Commit();
                }
            }

            this.isFiltered = false; filterQuery = "";  // we lose the "filtered" table, so we have to reset filtered status

            // create some useful command
            getChildrenCommand = cnn.CreateCommand();
            getChildrenCommand.CommandText = @"
                                    select  items.rowid as id, Items.size,
                                            Items.name,
                                            Fileprops.duration,
                                            Fileprops.path, Fileprops.rowid as fid, Fileprops.artist,
                                            Fileprops.title, Fileprops.album
                                    from items
                                            left join Fileprops on items.rowid=fileprops.rowid
                                    where Items.parent=@parentId
                ";
            getChildrenCommand.Parameters.Add(new SQLiteParameter("@parentId"));

            getFilteredChildrenCommand = cnn.CreateCommand();
            getFilteredChildrenCommand.CommandText = @"
                                    select  Items.rowid as id, Items.size,
                                            Items.name,
                                            Fileprops.duration,
                                            Fileprops.path, Fileprops.rowid as fid, Fileprops.artist,
                                            Fileprops.title, Fileprops.album
                                    from items
                                            join filtered on Items.rowid=filtered.rowid
                                            left join Fileprops on items.rowid=fileprops.rowid
                                    where Items.parent=@parentId
                ";
            getFilteredChildrenCommand.Parameters.Add(new SQLiteParameter("@parentId"));
        }

        // <summary> Create a bridge with a sqllite model file.</summary>
        public nandoSql()
        {
            OpenConnection(null);
        }

        public void Load(string filepath)
        {
            /* 
             * clear all memory
             * attach file to memory
             * copy all to memory db
             * detach
             */
            // DONE: load from file
            cnn.Close();    //clear memory connection
            getChildrenCommand = null;
            getFilteredChildrenCommand = null;

            OpenConnection(null);   // reopen a brand new memory db
            using (SQLiteCommand cmd = cnn.CreateCommand())     // ******************** Attach the file
            {
                cmd.CommandText = "ATTACH DATABASE @filename AS Target;";
                SQLiteParameter targetFileName = new SQLiteParameter("@filename");
                targetFileName.Value = filepath;
                cmd.Parameters.Add(targetFileName);
                cmd.ExecuteNonQuery();
            }
            using (SQLiteTransaction tran = cnn.BeginTransaction())
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = QueryCopyString("target", "main");
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
            }
            using (SQLiteCommand cmd = cnn.CreateCommand())// ******************** Detach the file
            {
                cmd.CommandText = "DETACH DATABASE Target;";
                cmd.ExecuteNonQuery();
            }
            this.Modified = false;
        }

        public void Save(string filepath)
        {
            //DONE: If filepath already exist save it to bak
            if (System.IO.File.Exists(filepath))
                try
                {
                    string bakfiledir = Path.GetDirectoryName(filepath);
                    string bakfilepath = Path.Combine(bakfiledir, Path.GetFileNameWithoutExtension(filepath) + ".bak");
                    System.IO.File.Copy(filepath, bakfilepath);
                }
                catch
                {
                }


            DateTime start = DateTime.Now;
            SQLiteConnection.CreateFile(filepath);  // empty target file

            using (SQLiteCommand cmd = cnn.CreateCommand())     // ******************** Attach the file
            {
                cmd.CommandText = "ATTACH DATABASE @filename AS Target;";
                SQLiteParameter targetFileName = new SQLiteParameter("@filename");
                targetFileName.Value = filepath;
                cmd.Parameters.Add(targetFileName);
                cmd.ExecuteNonQuery();
            }
            using (SQLiteTransaction tran = cnn.BeginTransaction()) // in a transaction: delete, create and copy
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandText = QueryCreateString("Target");
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = QueryCopyString("main", "target");
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
            }
            using (SQLiteCommand cmd = cnn.CreateCommand())// ******************** Detach the file
            {
                cmd.CommandText = "DETACH DATABASE Target;";
                cmd.ExecuteNonQuery();
            }

            DateTime end = DateTime.Now;
            if (this.log != null) this.log(string.Format("Saved on DB in {0}s.", (end - start).TotalSeconds));

            this.Modified = false;
        }


        public bool SupportExist(string name)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
                                    select 1 from SupportDetails sup join Items on sup.rowid=Items.rowid
                                    where name=@name;
                                   ";
                SQLiteParameter supportName = new SQLiteParameter("@name");
                cmd.Parameters.Add(supportName);
                supportName.Value = name;
                return cmd.ExecuteScalar() != null;
            }
        }




        public void deleteSupport(string name)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
                                    select SupportDetails.rowid from SupportDetails join Items on SupportDetails.rowid=Items.rowid
                                    where name=@name;
                                   ";
                SQLiteParameter supportName = new SQLiteParameter("@name");
                supportName.Value = name;
                cmd.Parameters.Add(supportName);

                long id = Convert.ToInt64(cmd.ExecuteScalar());
                this.deleteAllIsRootedAt(id);
            }

        }

        public void deleteAllIsRootedAt(long id)
        {
            /*
             * Seleziona tutti i figli del nodo da cancellare e lo cancella
             * Seleziona tutti i figli dei figli e cancella i figli... e così via
             * Cancello contemporaneamente anche in FileProps e UserTags
             * */
            // DONE: Delete everything in the existing support
            List<string> ids = new List<string>();
            ids.Add(id.ToString());

            using (SQLiteTransaction tran = cnn.BeginTransaction())
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    while (ids.Count > 0)
                    {
                        string commaIds = string.Join(", ", ids.ToArray());
                        cmd.CommandText = String.Format(@"SELECT rowid from Items where parent in ({0})", commaIds);
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        ids.Clear();    // preapare
                        while (reader.Read())
                        {
                            ids.Add(reader.GetInt64(0).ToString());
                        }
                        reader.Close();
                        cmd.CommandText = String.Format(@"
                                                            DELETE FROM Items where rowid in ({0});
                                                            DELETE FROM Fileprops where rowid in ({0});
                                                            DELETE FROM UserTags where rowid in ({0});
                                                            DELETE FROM SupportDetails where rowid in ({0});
                                                        ", commaIds);
                        cmd.ExecuteNonQuery();
                    }
                }
                tran.Commit();
                this.Modified = true;
            }
        }

        internal void putSupsOnModel(SupportsModel supps)
        {
            // put all root supports on the tree, we will load them dinamically on request
            using (SQLiteTransaction tran = cnn.BeginTransaction())
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandText = @"
                                        SELECT SupportDetails.rowid, name, size, creation_date, file_count, dir_count
                                        FROM SupportDetails
                                            JOIN Items on SupportDetails.rowid=Items.rowid
                                       ";
                    if (isFiltered) cmd.CommandText += " JOIN filtered on Items.rowid=filtered.rowid ";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SupportNode newSupp = new SupportNode();
                        newSupp.SqlId = reader.GetInt64(0);
                        newSupp.Name = reader.GetString(1);
                        newSupp.Size = reader.GetInt64(2);
                        newSupp.CreationDate = reader.GetDateTime(3);
                        supps.Nodes.Add(newSupp);
                    }
                    reader.Close();
                }
            }
        }



        public event RunWorkerCompletedEventHandler AfterParsing;

        private void CloseTransactionAtTheEnd(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled || e.Error != null)
            {
                this.Modified = this.modifiedBeforeParser;
                this.deleteSupport(this.lastParsedName);
                this.lastParsedName = null;
            }
            else
            {
                // parse completed
                // FUTU: Calculate size in a separate backgroundworker
                using (SQLiteCommand sizeUpdateCommand = cnn.CreateCommand())
                {
                    sizeUpdateCommand.Transaction = parseTransaction;
                    sizeUpdateCommand.CommandText = @"
                                UPDATE Items
                                SET size= (SELECT IFNULL(SUM(size),0)FROM Items WHERE parent=@parentId)
                                WHERE rowid=@parentId
                    ";
                    sizeUpdateCommand.Parameters.Add(new SQLiteParameter("@parentId"));
                    while (parseDirectoryStack.Count > 0)
                    {
                        lastParsedRootId = parseDirectoryStack.Pop();
                        sizeUpdateCommand.Parameters["@parentId"].Value = lastParsedRootId;
                        //MessageBox.Show("Update di " + sizeUpdateCommand.Parameters["@parentId"].Value.ToString());
                        sizeUpdateCommand.ExecuteNonQuery();

                    }
                }
                using (SQLiteCommand createSupportDetailCommand = cnn.CreateCommand())
                {
                    createSupportDetailCommand.Transaction = parseTransaction;
                    createSupportDetailCommand.CommandText = @"
                        INSERT INTO [SupportDetails] (rowid, file_count, dir_count, creation_date)
                        VALUES (@supportId, @fileCount, @dirCount, @creationDate)
                    ";
                    createSupportDetailCommand.Parameters.Add(new SQLiteParameter("@supportId"));
                    createSupportDetailCommand.Parameters.Add(new SQLiteParameter("@fileCount"));
                    createSupportDetailCommand.Parameters.Add(new SQLiteParameter("@dirCount"));
                    createSupportDetailCommand.Parameters.Add(new SQLiteParameter("@creationDate"));
                    createSupportDetailCommand.Parameters["@supportId"].Value = lastParsedRootId;
                    createSupportDetailCommand.Parameters["@fileCount"].Value = myParser.FileParsed;
                    createSupportDetailCommand.Parameters["@dirCount"].Value = myParser.DirectoryParsed;
                    createSupportDetailCommand.Parameters["@creationDate"].Value = DateTime.Now;
                    createSupportDetailCommand.ExecuteNonQuery();
                }

            }
            if (this.parseTransaction != null)
            {
                this.parseTransaction.Commit();
                this.parseCreateItem.Dispose();
                this.parseCreateFile.Dispose();
                this.parseTransaction.Dispose();
            }
        }

        internal void Parse(string path, string name)
        {
            this.myParser.AfterParsing -= CloseTransactionAtTheEnd;
            this.myParser.AfterParsing += CloseTransactionAtTheEnd;
            this.myParser.AfterParsing -= this.AfterParsing;
            this.myParser.AfterParsing += this.AfterParsing;
            modifiedBeforeParser = this.Modified;
            parseDirectoryStack = new Stack<long>();
            lastParsedName = name;
            myParser.AddNode -= this.addNode;
            myParser.AddNode += this.addNode;

            this.parseTransaction = cnn.BeginTransaction();
            parseCreateItem = cnn.CreateCommand();
            parseCreateItem.Transaction = parseTransaction;
            parseCreateItem.CommandText = @"
                                insert into Items (parent, name, size, note) VALUES (@parent, @name, @size, @note);
                                SELECT last_insert_rowid() AS [NodeID];
                            ";
            parseCreateItem.Parameters.Add(new SQLiteParameter("@name"));
            parseCreateItem.Parameters.Add(new SQLiteParameter("@parent"));
            parseCreateItem.Parameters.Add(new SQLiteParameter("@size"));
            parseCreateItem.Parameters.Add(new SQLiteParameter("@note"));

            parseCreateFile = cnn.CreateCommand();
            parseCreateFile.Transaction = parseTransaction;
            parseCreateFile.CommandText = @"
                        insert into FileProps (rowid, path, title, artist, album, file_tags, duration) values (@id, @path, @title, @artist, @album, @file_tags, @duration);
                    ";
            parseCreateFile.Parameters.Add(new SQLiteParameter("@id"));
            parseCreateFile.Parameters.Add(new SQLiteParameter("@path"));
            parseCreateFile.Parameters.Add(new SQLiteParameter("@artist"));
            parseCreateFile.Parameters.Add(new SQLiteParameter("@title"));
            parseCreateFile.Parameters.Add(new SQLiteParameter("@album"));
            parseCreateFile.Parameters.Add(new SQLiteParameter("@file_tags"));
            parseCreateFile.Parameters.Add(new SQLiteParameter("@duration"));

            myParser.Parse(path, 0);
        }

        private object addNode(string path, int type, object parent)
        {
            long parentId = Convert.ToInt64(parent);
            if (parentId == 0)
            {
                this.Modified = true;
                parseCreateItem.Parameters["@name"].Value = lastParsedName;  // name of the root
            }
            else parseCreateItem.Parameters["@name"].Value = System.IO.Path.GetFileName(path);

            string fullpath = System.IO.Path.Combine(myParser.Root, path);
            string extension = null;
            if (type == PathParser.FILETYPE)        // get size
            {
                FileInfo info = new System.IO.FileInfo(fullpath);
                parseCreateItem.Parameters["@size"].Value = info.Length;
                extension = info.Extension.ToLower();
            }
            else
                parseCreateItem.Parameters["@size"].Value = 0;  // directory without size, I'll get this at the end

            parseCreateItem.Parameters["@parent"].Value = Convert.ToInt64(parent);
            long id = (long)parseCreateItem.ExecuteScalar();    // create the item

            if (type == PathParser.FILETYPE)
            {
                parseCreateFile.Parameters["@id"].Value = id;
                parseCreateFile.Parameters["@path"].Value = path;
                if (nandoSql.ExtensionSupportTags(extension))
                {
                    try
                    {
                        TagLib.File tagFile = TagLib.File.Create(fullpath);
                        parseCreateFile.Parameters["@artist"].Value = String.Join(", ", tagFile.Tag.Performers);
                        parseCreateFile.Parameters["@title"].Value = tagFile.Tag.Title;
                        parseCreateFile.Parameters["@album"].Value = tagFile.Tag.Album;
                        parseCreateFile.Parameters["@file_tags"].Value = path;
                        parseCreateFile.Parameters["@duration"].Value = (int)Math.Ceiling(tagFile.Properties.Duration.TotalSeconds);
                    }
                    catch
                    {
                        parseCreateFile.Parameters["@artist"].Value = null;
                        parseCreateFile.Parameters["@title"].Value = null;
                        parseCreateFile.Parameters["@album"].Value = null;
                        parseCreateFile.Parameters["@file_tags"].Value = null;
                        parseCreateFile.Parameters["@duration"].Value = null;
                    }
                }
                else
                {   // untaggable file
                    parseCreateFile.Parameters["@artist"].Value = null;
                    parseCreateFile.Parameters["@title"].Value = null;
                    parseCreateFile.Parameters["@album"].Value = null;
                    parseCreateFile.Parameters["@file_tags"].Value = null;
                    parseCreateFile.Parameters["@duration"].Value = null;
                }

                parseCreateFile.ExecuteNonQuery();
            }
            else
            {
                parseDirectoryStack.Push(id);   // remember the folder for later use
            }
            //MessageBox.Show(string.Format("aggiungo {0}[{1}] con id padre: {2}.", path, id, parentId));
            return id;   // restituisco l'id del nodo
        }

        public void cancelParsing()
        {
            myParser.CancelParsing();
        }

        internal long GetSize(long id)
        {
            SQLiteCommand getSizeCommand = cnn.CreateCommand();
            getSizeCommand.CommandText = @"SELECT IFNULL(size,0) FROM Items WHERE rowid=@id";
            getSizeCommand.Parameters.Add(new SQLiteParameter("@id"));
            getSizeCommand.Parameters["@id"].Value = id;
            try
            {
                return (long)getSizeCommand.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
        }


        internal IEnumerable<BaseItem> getChildren(long id)
        {
            if (!isFiltered && getChildrenCommand == null)  // create getchildren command (it' used ofter)
            {

                //if (this.isFiltered)
                //{
                //    filterJoin = " JOIN filtered ON Items.rowid=filtered.rowid ";
                //}

            }
            SQLiteCommand usedCommand = isFiltered ? getFilteredChildrenCommand : getChildrenCommand;

            usedCommand.Parameters["@parentId"].Value = id;
            List<BaseItem> children = new List<BaseItem>();
            SQLiteDataReader reader = usedCommand.ExecuteReader();
            while (reader.Read())
            {
                BaseItem item;
                if (!reader["fid"].Equals(DBNull.Value))
                {
                    item = new FileItem();
                    item.Path = reader["path"] as string;
                    item.Icon = Win32.GetSmallAssociatedIcon(item.Path).ToBitmap();
                    item.Artist = reader["artist"] as string;
                    item.Title = reader["title"] as string;
                    item.Album = reader["album"] as string;
                    if (reader["duration"] != DBNull.Value)
                        item.Duration = Convert.ToInt64(reader["duration"]);
                    else item.Duration = 0;
                    //item. = reader[""] as string;
                }
                else
                {
                    item = new FolderItem();
                    //item.Path = System.IO.Path.Combine(parent.Path, reader["name"] as string);
                }
                item.Name = reader["name"] as string;
                item.Size = Convert.ToInt64(reader["size"]);
                item.SqlId = Convert.ToInt64(reader["id"]);
                children.Add(item);
            }
            reader.Close();
            return children;
        }

        public void filter(string searchPhrase)
        {
            if (string.IsNullOrEmpty(searchPhrase))
            {
                this.isFiltered = false;
            }
            else
            {
                if (searchPhrase != filterQuery)
                {
                    using (SQLiteTransaction filterTransaction = cnn.BeginTransaction())
                    {
                        using (SQLiteCommand createTempTable = cnn.CreateCommand())
                        {
                            createTempTable.Transaction = filterTransaction;
                            createTempTable.CommandText = @"
                                    CREATE TEMP TABLE IF NOT EXISTS filtered (
                                        rowid INTEGER PRIMARY KEY
                                    );
                                    DELETE FROM filtered;
                                    ";
                            createTempTable.ExecuteNonQuery();
                        }
                        using (SQLiteCommand FilterCommand = cnn.CreateCommand())
                        {
                            FilterCommand.Transaction = filterTransaction;

                            string cleanlike = '%' + searchPhrase + '%';    // search items name more than the other tokenized filters

                            FilterCommand.CommandText = @"
                                INSERT INTO filtered
                                SELECT Items.rowid FROM Items
                                WHERE
                                    rowid in (SELECT rowid FROM Items_text WHERE items_text MATCH @filter)
                                 OR rowid in (SELECT rowid FROM FileProps_text WHERE FileProps_text MATCH @filter)
                                 OR Items.name like @cleanlike
                            ";
                            FilterCommand.Parameters.Add(new SQLiteParameter("@filter"));
                            FilterCommand.Parameters.Add(new SQLiteParameter("@cleanlike"));
                            FilterCommand.Parameters["@filter"].Value = searchPhrase;
                            FilterCommand.Parameters["@cleanlike"].Value = cleanlike;
                            FilterCommand.ExecuteNonQuery();
                            FilterCommand.CommandText = "select count(*) from filtered;";
                            long filteredCount = Convert.ToInt64(FilterCommand.ExecuteScalar());
                            this.log(string.Format("{1} record matchano '{0}'", searchPhrase, filteredCount));
                        }
                        using (SQLiteCommand addFilteredParentCommand = cnn.CreateCommand())
                        {
                            addFilteredParentCommand.Transaction = filterTransaction;
                            addFilteredParentCommand.CommandText = @"
                                INSERT INTO filtered
                                SELECT DISTINCT parent
                                FROM Items JOIN filtered on Items.rowid=filtered.rowid
                                WHERE (parent<>0) and (parent not in (select rowid from filtered));
                            ";

                            while(true) { 
                                int genitoriInseriti=addFilteredParentCommand.ExecuteNonQuery();
                                //MessageBox.Show("genitori inseriti: " + genitoriInseriti.ToString());
                                if (genitoriInseriti == 0) break;
                            }
                        }
                        filterTransaction.Commit();
                        this.isFiltered = true;
                        filterQuery = searchPhrase;

                    }
                }
            }
        }

        public System.Collections.Hashtable itemsCount()
        // return the number of items on all the support for the current connection
        {
            SQLiteCommand getCountCommand = cnn.CreateCommand();
            getCountCommand.CommandText = @"
                SELECT
                    ifnull(sum(file_count),0),
                    ifnull(sum(dir_count),0),
                    COUNT(*),
                    SUM(size)
                FROM SupportDetails
                    JOIN Items on SupportDetails.rowid=Items.rowid
            ";
            SQLiteDataReader reader = getCountCommand.ExecuteReader();
            System.Collections.Hashtable dict = new System.Collections.Hashtable();
            dict["files"] = 0;
            dict["dirs"] = 0;
            dict["supports"] = 0;
            while (reader.Read())
            {
                dict["files"] = reader.GetInt64(0);
                dict["dirs"] = reader.GetInt64(1);
                dict["supps"] = reader.GetInt64(2);
                dict["size"] = reader.GetInt64(3);
            }
            reader.Close();
            return dict;
        }


        public void Rename(long id, string newname)
        {
            using (SQLiteTransaction tran = cnn.BeginTransaction())
            {
                using (SQLiteCommand renameCommand = cnn.CreateCommand())
                {
                    renameCommand.Transaction = tran;
                    renameCommand.CommandText = "UPDATE Items SET name=@newname WHERE rowId=@id";
                    renameCommand.Parameters.Add(new SQLiteParameter("@newname"));
                    renameCommand.Parameters.Add(new SQLiteParameter("@id"));
                    renameCommand.Parameters["@id"].Value=id;
                    renameCommand.Parameters["@newname"].Value=newname;
                    renameCommand.ExecuteNonQuery();
                }
                tran.Commit();
                this.Modified = true;
            }
        }
    }   //class nandoSql
}   // namespace


