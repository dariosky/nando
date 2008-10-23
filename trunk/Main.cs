/*
 * Utente: Dario Varotto
 * Data: 10/08/2008
 * Ora: 19.24
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Ini;
using System.ComponentModel;
using System.Security.Permissions;
using HS.Controls.Tree;
[assembly: FileIOPermission(SecurityAction.RequestMinimum)]


namespace nando
{
    /// <summary>
    /// This is the Mainform of Nando
    /// </summary>
    /// 
    public partial class MainForm : Form
    {

        public string inipath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "nando.ini");
        private DateTime StartTime;
        private nandoSql db = new nandoSql();
        private string _currentFilename;
        private string currentFilename
        {
            get { return _currentFilename; }
            set
            {
                _currentFilename = value;
                this.Text = string.Format("{1} - {0}", FormTitle,
                    System.IO.Path.GetFileName(_currentFilename));
            }
        }
        private string FormTitle = "Nando is a CATalogator";
        private string initalSearchText = "enter a search expression here";

        public MainForm()
        {
            InitializeComponent();
            ActivateOrDeactivateAddBtn();	//check if addBtn can be enabled
            saveBTN.Enabled = false;
            this.db.log -= this.log;
            this.db.log += this.log;
            IniFile ini = new IniFile(inipath);
            cdPath.Text = ini.IniReadValue("Default", "cdPath");
            cdName.Text = ini.IniReadValue("Default", "cdName");
            this.Text = FormTitle + " - v" + Application.ProductVersion;
            this.associateMNU.Checked = Win32.CheckExtensionAssociation(".nnd", Application.ExecutablePath);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                this.LoadFile(args[1]);
            }
        }

        void ActivateOrDeactivateAddBtn()
        {
            /// <summary>Check if Add a disk is allowed (path and name inserted)</summary>
            addBTN.Enabled = (cdName.Text != "" && cdPath.Text != "");
            newDiskBTN.Text = splitHLeft.Panel1Collapsed ? "Add a new disk" : "Hide new catalog panel";
        }

        void NewDiskBTNClick(object sender, EventArgs e)
        {
            splitHLeft.Panel1Collapsed = !splitHLeft.Panel1Collapsed;
            ActivateOrDeactivateAddBtn();
        }

        void BrowseBTNClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (System.IO.Directory.Exists(cdPath.Text)) dialog.SelectedPath = cdPath.Text;	// starting folder
            if (dialog.ShowDialog() == DialogResult.OK)
                cdPath.Text = dialog.SelectedPath;
            dialog.Dispose();
        }

        void OnAfterParsingFunction(object sender, RunWorkerCompletedEventArgs e)
        {
            //treeFiles.EndUpdate();
            DateTime StopTime = DateTime.Now;
            TimeSpan delta = StopTime - StartTime;
            PathParseModel model = treeFiles.Model as PathParseModel;
            if (e.Cancelled)
            {
                StatusLabel.Text = "Parsing cancelled by the user.";
                treeFiles.Model = null;
            }
            else if (e.Error != null)
            {
                StatusLabel.Text = String.Format(String.Format("Error occurred: {0}", e.Error.ToString()));
            }
            else
            {
                StatusLabel.Text = String.Format("Parsed {0} files, {1} folders in {2} s.", db.FileParsed, db.DirectoryParsed, delta.TotalSeconds);
                //TimeSpan toSqlElapsedTimeSpan = db.Add(model);
                //StatusLabel.Text += String.Format(", in SQL with other {0} s.", toSqlElapsedTimeSpan.TotalSeconds);

                SupportNode newSupp = new SupportNode();
                newSupp.CreationDate = DateTime.Now;
                newSupp.SqlId = db.lastParsedRootId;
                newSupp.Name = db.lastParsedName;
                newSupp.Size = db.GetSize(db.lastParsedRootId);
                if (treeSupport.Model == null)
                    treeSupport.Model = new SupportsModel();
                SupportsModel supmodel = treeSupport.Model as SupportsModel;
                supmodel.Nodes.Add(newSupp);
                treeSupport.ClearSelection();
                treeSupport.Select(newSupp.Index);
                treeSupport.ScrollTo(newSupp.Index);

                //DONE: Afterparsing select on suplist the inserted support
                //DONE: Populate treeFiles after parsing

                //treeFiles.ExpandAll();
                //treeFiles.AutoSizeAllColumns();
            }
            treeFiles.Enabled = true;
            addBTN.Text = "Catalog a new disk";
            saveBTN.Enabled = db.Modified;

        }

        void AddBTNClick(object sender, EventArgs e)
        {
            // DONE: Creare il modello in background
            // DONE: Creo una finestra di dialogo durante il processo con la possibilità di fermarlo

            if (addBTN.Text == "Cancel")
            {
                //PathParseModel model = treeFiles.Model as PathParseModel;
                //model.CancelParsing();
                db.cancelParsing();
                //db.deleteSupport(cdName.Text);
                return;
            }
            else
            {
                // DONE: if catalog name already exist ask to cancel/update
                if (db.SupportExist(cdName.Text))
                {
                    DialogResult answer;
                    answer = MessageBox.Show("Would you like to overwrite existing support?",
                                            "A support with this name already exist",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Exclamation);
                    if (answer == DialogResult.Cancel) return;
                    db.deleteSupport(cdName.Text);
                    if (treeSupport.Model != null)
                        foreach (SupportNode root in (treeSupport.Model as SupportsModel).Nodes)
                        {
                            if (root.Name.ToLower() == cdName.Text.ToLower())
                            {
                                (treeSupport.Model as SupportsModel).Nodes.Remove(root);
                                break;
                            }
                        }
                }

                addBTN.Text = "Cancel";
                StatusLabel.Text = "Parsing, please wait...";
                treeFiles.Enabled = false;
                //PathParseModel model = new PathParseModel(cdPath.Text, cdName.Text);
                PathParseModel model = new PathParseModel();
                db.AfterParsing -= OnAfterParsingFunction;
                db.AfterParsing += OnAfterParsingFunction;
                db.Parse(cdPath.Text, cdName.Text);

                treeFiles.Model = model;

                StartTime = DateTime.Now;
                //model.Parse();
                //PathParser pp = new PathParser();
                //pp.Parse(cdPath.Text, 0);
            }

        }
        private object addNode(string path, object parent)
        {
            MessageBox.Show("aggiungo " + path + " con id padre:" + ((int)parent).ToString());
            return 1;
        }
        void CdDescriptionChanged(object sender, EventArgs e)
        {
            ActivateOrDeactivateAddBtn(); // if text is void addability is false
        }

        void SearchTextEnter(object sender, EventArgs e)
        {
            searchText.SelectAll();
            if (searchText.Text == initalSearchText) searchText.Text = "";
        }


        void aboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            AboutNando aboutForm = new AboutNando();
            aboutForm.ShowDialog();
            aboutForm.Dispose();

        }

        void SearchOnTheWholeArchiveToolStripMenuItemClick(object sender, EventArgs e)
        {
            searchText.Focus();
        }

        void SearchTextTextChanged(object sender, EventArgs e)
        {
            searchText.ForeColor = (searchText.Text == this.initalSearchText) ? SystemColors.GrayText : SystemColors.WindowText;
            searchTimer.Stop();
            searchTimer.Start();

        }

        private void searchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            string search = searchText.Text.Trim();
            db.filter(search);

            List<long> oldSelectedId = new List<long>();
            foreach (TreeNodeAdv nodeadv in treeSupport.SelectedNodes)
            {
                SupportNode sup = nodeadv.Tag as SupportNode;
                oldSelectedId.Add(sup.SqlId);
            }

            //treeSupport.BeginUpdate();
            this.refreshSupports(); // update supports:

            foreach (TreeNodeAdv nodeadv in treeSupport.AllNodes)
            {
                SupportNode sup = nodeadv.Tag as SupportNode;
                if (oldSelectedId.Contains(sup.SqlId))
                {
                    treeSupport.Select(nodeadv);
                    treeSupport.ScrollTo(nodeadv);
                }
            }
            //treeSupport.EndUpdate();
            
            /*
             * SupportsModel supps = new SupportsModel();
            treeFiles.Model = null;
            db.putSupsOnModel(supps);
            treeSupport.Model = supps;
             * */
        }

        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (WaitToSave())
            {
                e.Cancel = true;
                return;
            }
            IniFile ini = new IniFile(inipath);
            ini.IniWriteValue("Default", "cdPath", cdPath.Text);
            ini.IniWriteValue("Default", "cdName", cdName.Text);
        }

        void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool WaitToSave()
        {
            if (db.Modified)
            {
                DialogResult result = MessageBox.Show("Would you like to discard unsaved changes?",
                    "Your archive has been modified",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation);
                return (result != DialogResult.OK);
            }
            return false;   // i don't have to wait, we can proceed
        }

        void LoadFile(string filename)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                db.Load(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Maybe you're loading a file from a different version, report me the issue if you would like to import it.\n" + ex.Message,
                            "Cannot load this file",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                return;
            }
            // FUTU: try to open a db, if fail ask for a password
            refreshSupports();
            DateTime endTime = DateTime.Now;
            System.Collections.Hashtable counts = db.itemsCount();
            this.log(string.Format("Loaded from DB {0} supports [{1} files - {2} folders]. Total size: {3}. In {4} seconds.",
                counts["supps"],
                counts["files"], counts["dirs"],
                BaseItem.GetHumanizedSize((long)counts["size"]),
                (endTime - startTime).TotalSeconds));
            currentFilename = filename;
        }

        void LoadBTNClick(object sender, EventArgs e)
        {
            if (WaitToSave()) return;
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.DefaultExt = ".nnd";
            openDialog.Filter = "nando DB file (*.nnd)|*.nnd|All files (*.*)|*.*";
            openDialog.Title = "Choose the Nando archive to open";
            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoadFile(openDialog.FileName);
            }
        }

        void SyncToolStripMenuItemClick(object sender, EventArgs e)
        {
            // FUTU: connect to a server and send differences
        }

        //FUTU: On double click, ask to insert the support somewhere (asking for root folder) and open doubleclicked file
        //FUTU: Select files and split them in a new entry
        //FUTU: Drag and drop files to an entity to move them to another entity
        //FUTU: Ask to remove an entity when it's empty



        public void log(string message)
        {
            StatusLabel.Text = message;
        }

        private void SaveAsClick(object sender, EventArgs e)
        {
            string saveToFilaPath = currentFilename;
            if (saveToFilaPath == null || sender == SaveAsToolStripMenuItem)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = ".nnd";
                saveDialog.Filter = "nando DB file (*.nnd)|*.nnd|All files (*.*)|*.*";
                saveDialog.Title = "Choose where to save your archive";
                DialogResult result = saveDialog.ShowDialog();
                if (result != DialogResult.OK) return;  // we don't have to save, we can exit
                saveToFilaPath = saveDialog.FileName;
            }

            try
            {
                db.Save(saveToFilaPath);
                currentFilename = saveToFilaPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            saveBTN.Enabled = db.Modified;
        }

        private void supps_Select(object sender, EventArgs e)
        {
            treeFiles.Model = null;
            treeFiles.BeginUpdate();
            PathParseModel model = new PathParseModel();
            model.rootId = new List<long>();
            foreach (TreeNodeAdv nodeadv in treeSupport.SelectedNodes)
            {
                SupportNode sup = nodeadv.Tag as SupportNode;
                //model.Nodes.Add(new FolderItem(sup.Name, null, model));  //"name"));
                //db.addSupportToModel(sup.SqlId, model);
                model.rootId.Add(sup.SqlId);
            }
            model.dbConnection = db;
            treeFiles.Model = model;
            treeFiles.EndUpdate();
            //treeFiles.ExpandAll();
            //treeFiles.AutoSizeAllColumns();
        }

        private void cdName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                addBTN.PerformClick();
                e.Handled = true;
            }
        }

        private void supContext_Opening(object sender, CancelEventArgs e)
        {
            int nodes = 0;
            foreach (TreeNodeAdv nodeadv in treeSupport.SelectedNodes)
                nodes += 1;
            supRename.Enabled = supDelete.Enabled = nodes > 0;
        }

        private void supDelete_Click(object sender, EventArgs e)
        {
            int nodes = 0;
            string lastName = "";
            foreach (TreeNodeAdv nodeadv in treeSupport.SelectedNodes)
            {
                lastName = (nodeadv.Tag as SupportNode).Name;
                nodes += 1;
            }
            DialogResult result;
            if (nodes > 1)
                result = MessageBox.Show(String.Format("You're going to delete {0} supports from the archive.\nProceed?", nodes),
                    "Confirm supports deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            else
                result = MessageBox.Show(String.Format("You're going to delete {0} from the archive.\nProceed?", lastName),
                    "Confirm support deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);


            if (result == DialogResult.Cancel) return;
            treeSupport.BeginUpdate();
            SupportsModel model = treeSupport.Model as SupportsModel;
            foreach (TreeNodeAdv nodeadv in treeSupport.SelectedNodes)
            {
                this.db.deleteAllIsRootedAt((nodeadv.Tag as SupportNode).SqlId);
                model.Nodes.Remove(nodeadv.Tag as SupportNode);
            }

            treeSupport.EndUpdate();
            saveBTN.Enabled = db.Modified;
            //refreshSupports(); // DONE: we don't need to refresh all
        }

        private void refreshSupports()
        // update support list from db and empty treefiles
        {
            treeSupport.BeginUpdate();
            SupportsModel supps = new SupportsModel();
            db.putSupsOnModel(supps);
            treeSupport.Model = supps;
            treeSupport.EndUpdate();
            treeFiles.Model = null;
        }

        private void supRename_Click(object sender, EventArgs e)
        {
            int nodes = 0;
            string lastName = "";
            long lastId = 0;
            foreach (TreeNodeAdv nodeadv in treeSupport.SelectedNodes)
            {
                lastName = (nodeadv.Tag as SupportNode).Name;
                lastId = (nodeadv.Tag as SupportNode).SqlId;
                nodes += 1;
            }
            if (lastId == 0) return;
            DialogResult result;
            DialogNewName form = new DialogNewName();
            if (nodes > 1)
                throw new Exception("Non implemented yet"); // FUTU: rename multiple supports at once
            form.SetQuestion(string.Format("Rename '{0}' to ...", lastName));
            form.SetName(lastName);
            result = form.ShowDialog();
            if (result != DialogResult.OK) return;
            string newname = form.getAnswer();
            if (this.db.SupportExist(newname))
            {
                MessageBox.Show("Choose another name or delete the old support.", "Support already exists",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            db.Rename(lastId, newname);
            saveBTN.Enabled = db.Modified;
            foreach (TreeNodeAdv nodeadv in treeSupport.SelectedNodes)
            {
                (nodeadv.Tag as SupportNode).Name = newname;
            }
            //refreshSupports(); //DONE: We don't need full refresh
        }

        private void associateMNU_Click(object sender, EventArgs e)
        {
            if (!associateMNU.Checked)
            {
                //set the association
                Win32.AssociateFile(".nnd", Application.ExecutablePath);
            }
            else
            {
                Win32.DeassociateFile(".nnd");
            }
            associateMNU.Checked = !associateMNU.Checked;
        }

        private void startParsing_Click(object sender, EventArgs e)
        {
            splitHLeft.Panel1Collapsed = false;
            cdName.Focus();
        }

        private void treeSupport_ColumnClicked(object sender, TreeColumnEventArgs e)
        {
            treeSupport.ChangeSort(e.Column);
            //treeSupport.SortedColumn = e.Column;
        }


        //FUTU: Fill everywhere with tests
        //TODO: Add supports->lend list
        //TODO: Add support->place
        //TODO: Sortable support list (add creation date column)
        //DONE: Add supports count on load stats
        //DONE: Add total size to load stats (and add Tera, Peta, Exa suffixes)
        //TODO: Import from a file (adding) choose if creation_date should be now or the originals (eventually select supports to import)
        //TODO: After support selection show the sum of selected item size
    }
}