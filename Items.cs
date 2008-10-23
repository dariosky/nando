/*
 * Utente: Dario Varotto
 * Data: 18/08/2008
 * Ora: 2.39
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using HS.Controls.Tree.Base;
using System.Collections;
using System.IO;
using TagLib;

namespace nando
{

    public abstract class BaseItem : HS.Controls.Tree.Node
    {
        // this item could be indifferently a folder or a file

        public BaseItem(string path, BaseItem parent, PathParseModel model)
        {
            Path = path;
            ParentItem = parent;
            Name = System.IO.Path.GetFileName(path);
        }
        public BaseItem()
        //create the item, empty like a saturday tv show
        {
        }

        private long _sqlid = 0; // when in a db this could be useful to store object id
        public long SqlId
        {
            get { return _sqlid; }
            set { _sqlid = value; }
        }

        public static string GetHumanizedSize(long bytes)
        {
            string[] suffixes = { "", " KB", " MB", " GB", "TB", "PetaB", "ExaB" };
            float newSize = bytes;	// get size in byte
            int choosenSuffix = 0;
            for (; choosenSuffix < suffixes.Length - 1; choosenSuffix++)
            {
                if (newSize < 1024) break;
                newSize /= 1024;
            }

            return Math.Round(newSize, 1).ToString() + suffixes[choosenSuffix];
        }

        private string _path;	// this is the path relative to root
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private string _name;	// this is the name shown on the treeviw
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private System.Drawing.Image _icon; // the system icon of the file
        public System.Drawing.Image Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }


        private long _size;	// the size in byte of the current file (or of all the folder with subfolders)
        public long Size
        {
            get { return _size; }
            set
            {
                // propagate size to parents
                if (this.ParentItem != null) this.ParentItem.Size = this.ParentItem.Size - _size + value;
                _size = value;
            }
        }
        public string HumanizedSize
        {
            get
            {
                return BaseItem.GetHumanizedSize(this.Size);
            }
        }

        private BaseItem _parentItem;
        public BaseItem ParentItem
        {
            get { return _parentItem; }
            set { _parentItem = value; }
        }

        private PathParseModel _model;
        public PathParseModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private string[] _artists;
        protected string[] Artists
        {
            get { return _artists; }
            set { _artists = value; }
        }


        public string Artist
        {
            get
            {
                if (this.Artists != null) return String.Join(", ", this.Artists);
                else return null;
            }
            set
            {
                this.Artists = new string[1] { value };
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _album;
        public string Album
        {
            get { return _album; }
            set { _album = value; }
        }

        private long _duration;	//duration in secs
        public long Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        public string HumanizedDuration
        {
            get
            {
                if (Duration == 0) return "";
                int hours = (int)Duration / 3600;
                int mins = (int)(Duration % 3600) / 60;
                if (hours > 0) return String.Format("{0}h:{1}m", hours, mins);
                if (mins > 0)
                {
                    int secs = (int)(Duration % 60);
                    return String.Format("{0}m:{1}s", mins, secs);
                }
                return String.Format("{0}s", Duration);
            }
            set
            {
                throw new ApplicationException("Don't set Humanized duration, set the Duration in seconds instead.");
            }
        }

        public SupportNode ToSupportNode()
        {
            SupportNode support = new SupportNode();
            support.SqlId = this.SqlId;
            support.Name = this.Name;
            support.Size = this.Size;
            return support;
        }
    }

    public class FolderItem : BaseItem
    {
        public FolderItem(string path, BaseItem parent, PathParseModel model)
            : base(path, parent, model)
        {
            Icon = Win32.GetSmallFolderIcon().ToBitmap();
        }
        public FolderItem()
        {
            Icon = Win32.GetSmallFolderIcon().ToBitmap();
        }
    }

    public class FileItem : BaseItem
    {
        public FileItem(string path, BaseItem parent, PathParseModel model)
            : base(path, parent, model)
        {
            //string fullpath = System.IO.Path.Combine(model.RootPath, path);
            //Icon = Win32.GetSmallAssociatedIcon(fullpath).ToBitmap();
            //FileInfo info = new System.IO.FileInfo(fullpath);
            //Size = info.Length;
            //// senza il controllo estensioni impiego 183 secondi per dariosky, con solo 55 sec
            //if (Array.Exists<string>(_supportedExtension,
            //                         delegate(string totest)
            //                         {
            //                             return info.Extension.Equals(totest, StringComparison.InvariantCultureIgnoreCase);
            //                         }))
            //{


            //    try
            //    {
            //        TagLib.File tagFile = TagLib.File.Create(fullpath);
            //        this.Artists = tagFile.Tag.Performers;
            //        this.Title = tagFile.Tag.Title;
            //        this.Album = tagFile.Tag.Album;
            //        this.Duration = (int)Math.Ceiling(tagFile.Properties.Duration.TotalSeconds);

            //    }
            //    catch (TagLib.UnsupportedFormatException)
            //    {
            //        // unsupported file won't have tag
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
        }

        public FileItem()
        {
        }

    }
}



