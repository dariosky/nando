/*
 * Creato da SharpDevelop.
 * Utente: dariosky
 * Data: 10/08/2008
 * Ora: 19.24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace nando
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savetoolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.startParsing = new System.Windows.Forms.ToolStripMenuItem();
            this.searchOnTheWholeArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.syncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.associateMNU = new System.Windows.Forms.ToolStripMenuItem();
            this.aiutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolbar = new System.Windows.Forms.ToolStrip();
            this.newDiskBTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.searchText = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.saveBTN = new System.Windows.Forms.ToolStripButton();
            this.nodeIcon = new HS.Controls.Tree.NodeControls.NodeIcon();
            this.nodeName = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeFileSize = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitHMain = new System.Windows.Forms.SplitContainer();
            this.splitHLeft = new System.Windows.Forms.SplitContainer();
            this.addBTN = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cdPath = new System.Windows.Forms.TextBox();
            this.browseBTN = new System.Windows.Forms.Button();
            this.cdName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.treeSupport = new HS.Controls.Tree.TreeViewAdv();
            this.sn_Name = new HS.Controls.Tree.TreeColumn();
            this.sn_Size = new HS.Controls.Tree.TreeColumn();
            this.supContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.supRename = new System.Windows.Forms.ToolStripMenuItem();
            this.supDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ssn_Icon = new HS.Controls.Tree.NodeControls.NodeIcon();
            this.ssn_Name = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.ssn_Size = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.labelNomeArchivio = new System.Windows.Forms.Label();
            this.treeFiles = new HS.Controls.Tree.TreeViewAdv();
            this.ft_name = new HS.Controls.Tree.TreeColumn();
            this.ft_size = new HS.Controls.Tree.TreeColumn();
            this.ft_artist = new HS.Controls.Tree.TreeColumn();
            this.ft_album = new HS.Controls.Tree.TreeColumn();
            this.ft_title = new HS.Controls.Tree.TreeColumn();
            this.ft_duration = new HS.Controls.Tree.TreeColumn();
            this.ft_usertags = new HS.Controls.Tree.TreeColumn();
            this.ft_filetags = new HS.Controls.Tree.TreeColumn();
            this.fsN_Icon = new HS.Controls.Tree.NodeControls.NodeIcon();
            this.fsN_Name = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.fsN_Size = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.fsN_Artist = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.fsN_Album = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.fsN_Title = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.fsN_Duration = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.fsN_UserTags = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.fsN_FileTags = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this.st_Name = new HS.Controls.Tree.TreeColumn();
            this.st_Size = new HS.Controls.Tree.TreeColumn();
            this.searchTimer = new System.Windows.Forms.Timer(this.components);
            this.mainMenu.SuspendLayout();
            this.mainToolbar.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.splitHMain.Panel1.SuspendLayout();
            this.splitHMain.Panel2.SuspendLayout();
            this.splitHMain.SuspendLayout();
            this.splitHLeft.Panel1.SuspendLayout();
            this.splitHLeft.Panel2.SuspendLayout();
            this.splitHLeft.SuspendLayout();
            this.supContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem3,
            this.toolStripMenuItem2,
            this.aiutoToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(704, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadToolStripMenuItem,
            this.savetoolStrip,
            this.SaveAsToolStripMenuItem,
            this.ExportToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // LoadToolStripMenuItem
            // 
            this.LoadToolStripMenuItem.Image = global::nando.Properties.Resources.folder_database;
            this.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem";
            this.LoadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.LoadToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.LoadToolStripMenuItem.Text = "&Load a CATalog...";
            this.LoadToolStripMenuItem.Click += new System.EventHandler(this.LoadBTNClick);
            // 
            // savetoolStrip
            // 
            this.savetoolStrip.Image = global::nando.Properties.Resources.disc;
            this.savetoolStrip.Name = "savetoolStrip";
            this.savetoolStrip.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.savetoolStrip.Size = new System.Drawing.Size(220, 22);
            this.savetoolStrip.Text = "Save";
            this.savetoolStrip.Click += new System.EventHandler(this.SaveAsClick);
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.SaveAsToolStripMenuItem.Text = "&Save as...";
            this.SaveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsClick);
            // 
            // ExportToolStripMenuItem
            // 
            this.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem";
            this.ExportToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.ExportToolStripMenuItem.Text = "E&xport...";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(217, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startParsing,
            this.searchOnTheWholeArchiveToolStripMenuItem});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(51, 20);
            this.toolStripMenuItem3.Text = "&Modify";
            // 
            // startParsing
            // 
            this.startParsing.Name = "startParsing";
            this.startParsing.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.startParsing.Size = new System.Drawing.Size(267, 22);
            this.startParsing.Text = "&Parse a new support";
            this.startParsing.Click += new System.EventHandler(this.startParsing_Click);
            // 
            // searchOnTheWholeArchiveToolStripMenuItem
            // 
            this.searchOnTheWholeArchiveToolStripMenuItem.Name = "searchOnTheWholeArchiveToolStripMenuItem";
            this.searchOnTheWholeArchiveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchOnTheWholeArchiveToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.searchOnTheWholeArchiveToolStripMenuItem.Text = "Search on the whole archive";
            this.searchOnTheWholeArchiveToolStripMenuItem.Click += new System.EventHandler(this.SearchOnTheWholeArchiveToolStripMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.syncToolStripMenuItem,
            this.associateMNU});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem2.Text = "&Tools";
            // 
            // syncToolStripMenuItem
            // 
            this.syncToolStripMenuItem.Name = "syncToolStripMenuItem";
            this.syncToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.syncToolStripMenuItem.Text = "&Online sync";
            this.syncToolStripMenuItem.Visible = false;
            this.syncToolStripMenuItem.Click += new System.EventHandler(this.SyncToolStripMenuItemClick);
            // 
            // associateMNU
            // 
            this.associateMNU.Name = "associateMNU";
            this.associateMNU.Size = new System.Drawing.Size(224, 22);
            this.associateMNU.Text = "Associate nando to .nnd files";
            this.associateMNU.Click += new System.EventHandler(this.associateMNU_Click);
            // 
            // aiutoToolStripMenuItem
            // 
            this.aiutoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.aiutoToolStripMenuItem.Name = "aiutoToolStripMenuItem";
            this.aiutoToolStripMenuItem.Size = new System.Drawing.Size(24, 20);
            this.aiutoToolStripMenuItem.Text = "?";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.aboutToolStripMenuItem.Text = "&About Nando...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItemClick);
            // 
            // mainToolbar
            // 
            this.mainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newDiskBTN,
            this.toolStripSeparator1,
            this.searchText,
            this.toolStripButton1,
            this.saveBTN});
            this.mainToolbar.Location = new System.Drawing.Point(0, 24);
            this.mainToolbar.Name = "mainToolbar";
            this.mainToolbar.Size = new System.Drawing.Size(704, 25);
            this.mainToolbar.TabIndex = 1;
            // 
            // newDiskBTN
            // 
            this.newDiskBTN.Image = ((System.Drawing.Image)(resources.GetObject("newDiskBTN.Image")));
            this.newDiskBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newDiskBTN.Name = "newDiskBTN";
            this.newDiskBTN.Size = new System.Drawing.Size(99, 22);
            this.newDiskBTN.Text = "Add a new disk";
            this.newDiskBTN.Click += new System.EventHandler(this.NewDiskBTNClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // searchText
            // 
            this.searchText.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchText.ForeColor = System.Drawing.SystemColors.GrayText;
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(180, 25);
            this.searchText.Text = "enter a search expression here";
            this.searchText.Enter += new System.EventHandler(this.SearchTextEnter);
            this.searchText.TextChanged += new System.EventHandler(this.SearchTextTextChanged);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::nando.Properties.Resources.folder_database;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(50, 22);
            this.toolStripButton1.Text = "Load";
            this.toolStripButton1.Click += new System.EventHandler(this.LoadBTNClick);
            // 
            // saveBTN
            // 
            this.saveBTN.Image = global::nando.Properties.Resources.disc;
            this.saveBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveBTN.Name = "saveBTN";
            this.saveBTN.Size = new System.Drawing.Size(51, 22);
            this.saveBTN.Text = "Save";
            this.saveBTN.Click += new System.EventHandler(this.SaveAsClick);
            // 
            // nodeIcon
            // 
            this.nodeIcon.DataPropertyName = "Icon";
            this.nodeIcon.ParentColumn = null;
            // 
            // nodeName
            // 
            this.nodeName.DataPropertyName = "Name";
            this.nodeName.ParentColumn = null;
            this.nodeName.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // nodeFileSize
            // 
            this.nodeFileSize.DataPropertyName = "HumanizedSize";
            this.nodeFileSize.ParentColumn = null;
            this.nodeFileSize.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 409);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(704, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // splitHMain
            // 
            this.splitHMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitHMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitHMain.Location = new System.Drawing.Point(0, 49);
            this.splitHMain.Name = "splitHMain";
            // 
            // splitHMain.Panel1
            // 
            this.splitHMain.Panel1.Controls.Add(this.splitHLeft);
            // 
            // splitHMain.Panel2
            // 
            this.splitHMain.Panel2.Controls.Add(this.treeFiles);
            this.splitHMain.Size = new System.Drawing.Size(704, 360);
            this.splitHMain.SplitterDistance = 219;
            this.splitHMain.TabIndex = 3;
            // 
            // splitHLeft
            // 
            this.splitHLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitHLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitHLeft.IsSplitterFixed = true;
            this.splitHLeft.Location = new System.Drawing.Point(0, 0);
            this.splitHLeft.Name = "splitHLeft";
            this.splitHLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitHLeft.Panel1
            // 
            this.splitHLeft.Panel1.Controls.Add(this.addBTN);
            this.splitHLeft.Panel1.Controls.Add(this.label2);
            this.splitHLeft.Panel1.Controls.Add(this.cdPath);
            this.splitHLeft.Panel1.Controls.Add(this.browseBTN);
            this.splitHLeft.Panel1.Controls.Add(this.cdName);
            this.splitHLeft.Panel1.Controls.Add(this.label1);
            // 
            // splitHLeft.Panel2
            // 
            this.splitHLeft.Panel2.Controls.Add(this.treeSupport);
            this.splitHLeft.Panel2.Controls.Add(this.labelNomeArchivio);
            this.splitHLeft.Size = new System.Drawing.Size(219, 360);
            this.splitHLeft.SplitterDistance = 120;
            this.splitHLeft.TabIndex = 0;
            // 
            // addBTN
            // 
            this.addBTN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.addBTN.Location = new System.Drawing.Point(45, 93);
            this.addBTN.Name = "addBTN";
            this.addBTN.Size = new System.Drawing.Size(121, 23);
            this.addBTN.TabIndex = 4;
            this.addBTN.Text = "Catalog a new disk";
            this.addBTN.UseVisualStyleBackColor = true;
            this.addBTN.Click += new System.EventHandler(this.AddBTNClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Path of the disc to catalog:";
            // 
            // cdPath
            // 
            this.cdPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cdPath.Location = new System.Drawing.Point(12, 65);
            this.cdPath.Name = "cdPath";
            this.cdPath.Size = new System.Drawing.Size(113, 20);
            this.cdPath.TabIndex = 2;
            this.cdPath.TextChanged += new System.EventHandler(this.CdDescriptionChanged);
            // 
            // browseBTN
            // 
            this.browseBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseBTN.Location = new System.Drawing.Point(131, 63);
            this.browseBTN.Name = "browseBTN";
            this.browseBTN.Size = new System.Drawing.Size(75, 23);
            this.browseBTN.TabIndex = 3;
            this.browseBTN.Text = "Browse...";
            this.browseBTN.UseVisualStyleBackColor = true;
            this.browseBTN.Click += new System.EventHandler(this.BrowseBTNClick);
            // 
            // cdName
            // 
            this.cdName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cdName.Location = new System.Drawing.Point(12, 17);
            this.cdName.Name = "cdName";
            this.cdName.Size = new System.Drawing.Size(195, 20);
            this.cdName.TabIndex = 1;
            this.cdName.TextChanged += new System.EventHandler(this.CdDescriptionChanged);
            this.cdName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cdName_KeyDown);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name of the disc:";
            // 
            // treeSupport
            // 
            this.treeSupport.BackColor = System.Drawing.SystemColors.Window;
            this.treeSupport.Columns.Add(this.sn_Name);
            this.treeSupport.Columns.Add(this.sn_Size);
            this.treeSupport.ContextMenuStrip = this.supContext;
            this.treeSupport.DefaultToolTipProvider = null;
            this.treeSupport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeSupport.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeSupport.FullRowSelect = true;
            this.treeSupport.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeSupport.Location = new System.Drawing.Point(0, 13);
            this.treeSupport.Model = null;
            this.treeSupport.Name = "treeSupport";
            this.treeSupport.NodeControls.Add(this.ssn_Icon);
            this.treeSupport.NodeControls.Add(this.ssn_Name);
            this.treeSupport.NodeControls.Add(this.ssn_Size);
            this.treeSupport.SelectedNode = null;
            this.treeSupport.Size = new System.Drawing.Size(219, 223);
            this.treeSupport.TabIndex = 4;
            this.treeSupport.SelectionChanged += new System.EventHandler(this.supps_Select);
            this.treeSupport.ColumnClicked += new System.EventHandler<HS.Controls.Tree.TreeColumnEventArgs>(this.treeSupport_ColumnClicked);
            // 
            // sn_Name
            // 
            this.sn_Name.Header = "Name";
            this.sn_Name.Width = 150;
            // 
            // sn_Size
            // 
            this.sn_Size.Header = "Size";
            // 
            // supContext
            // 
            this.supContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.supRename,
            this.supDelete});
            this.supContext.Name = "supContext";
            this.supContext.Size = new System.Drawing.Size(137, 48);
            this.supContext.Opening += new System.ComponentModel.CancelEventHandler(this.supContext_Opening);
            // 
            // supRename
            // 
            this.supRename.Name = "supRename";
            this.supRename.Size = new System.Drawing.Size(136, 22);
            this.supRename.Text = "Rename...";
            this.supRename.Click += new System.EventHandler(this.supRename_Click);
            // 
            // supDelete
            // 
            this.supDelete.Name = "supDelete";
            this.supDelete.Size = new System.Drawing.Size(136, 22);
            this.supDelete.Text = "Delete";
            this.supDelete.Click += new System.EventHandler(this.supDelete_Click);
            // 
            // ssn_Icon
            // 
            this.ssn_Icon.DataPropertyName = "Icon";
            this.ssn_Icon.ParentColumn = this.sn_Name;
            // 
            // ssn_Name
            // 
            this.ssn_Name.DataPropertyName = "Name";
            this.ssn_Name.ParentColumn = this.sn_Name;
            this.ssn_Name.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // ssn_Size
            // 
            this.ssn_Size.DataPropertyName = "HumanizedSize";
            this.ssn_Size.ParentColumn = this.sn_Size;
            this.ssn_Size.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // labelNomeArchivio
            // 
            this.labelNomeArchivio.AutoSize = true;
            this.labelNomeArchivio.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelNomeArchivio.Location = new System.Drawing.Point(0, 0);
            this.labelNomeArchivio.Name = "labelNomeArchivio";
            this.labelNomeArchivio.Size = new System.Drawing.Size(67, 13);
            this.labelNomeArchivio.TabIndex = 3;
            this.labelNomeArchivio.Text = "Your archive";
            // 
            // treeFiles
            // 
            this.treeFiles.AllowColumnReorder = true;
            this.treeFiles.BackColor = System.Drawing.SystemColors.Window;
            this.treeFiles.Columns.Add(this.ft_name);
            this.treeFiles.Columns.Add(this.ft_size);
            this.treeFiles.Columns.Add(this.ft_artist);
            this.treeFiles.Columns.Add(this.ft_album);
            this.treeFiles.Columns.Add(this.ft_title);
            this.treeFiles.Columns.Add(this.ft_duration);
            this.treeFiles.Columns.Add(this.ft_usertags);
            this.treeFiles.Columns.Add(this.ft_filetags);
            this.treeFiles.DefaultToolTipProvider = null;
            this.treeFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFiles.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeFiles.FullRowSelect = true;
            this.treeFiles.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeFiles.Location = new System.Drawing.Point(0, 0);
            this.treeFiles.Model = null;
            this.treeFiles.Name = "treeFiles";
            this.treeFiles.NodeControls.Add(this.fsN_Icon);
            this.treeFiles.NodeControls.Add(this.fsN_Name);
            this.treeFiles.NodeControls.Add(this.fsN_Size);
            this.treeFiles.NodeControls.Add(this.fsN_Artist);
            this.treeFiles.NodeControls.Add(this.fsN_Album);
            this.treeFiles.NodeControls.Add(this.fsN_Title);
            this.treeFiles.NodeControls.Add(this.fsN_Duration);
            this.treeFiles.NodeControls.Add(this.fsN_UserTags);
            this.treeFiles.NodeControls.Add(this.fsN_FileTags);
            this.treeFiles.SelectedNode = null;
            this.treeFiles.Size = new System.Drawing.Size(481, 360);
            this.treeFiles.TabIndex = 1;
            // 
            // ft_name
            // 
            this.ft_name.Header = "Name";
            this.ft_name.Width = 250;
            // 
            // ft_size
            // 
            this.ft_size.Header = "Size";
            // 
            // ft_artist
            // 
            this.ft_artist.Header = "Artist";
            // 
            // ft_album
            // 
            this.ft_album.Header = "Album";
            // 
            // ft_title
            // 
            this.ft_title.Header = "Title";
            // 
            // ft_duration
            // 
            this.ft_duration.Header = "Duration";
            // 
            // ft_usertags
            // 
            this.ft_usertags.Header = "User TAGS";
            // 
            // ft_filetags
            // 
            this.ft_filetags.Header = "File TAGS";
            // 
            // fsN_Icon
            // 
            this.fsN_Icon.DataPropertyName = "Icon";
            this.fsN_Icon.ParentColumn = this.ft_name;
            // 
            // fsN_Name
            // 
            this.fsN_Name.DataPropertyName = "Name";
            this.fsN_Name.ParentColumn = this.ft_name;
            this.fsN_Name.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // fsN_Size
            // 
            this.fsN_Size.DataPropertyName = "HumanizedSize";
            this.fsN_Size.ParentColumn = this.ft_size;
            this.fsN_Size.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // fsN_Artist
            // 
            this.fsN_Artist.DataPropertyName = "Artist";
            this.fsN_Artist.ParentColumn = this.ft_artist;
            this.fsN_Artist.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // fsN_Album
            // 
            this.fsN_Album.DataPropertyName = "Album";
            this.fsN_Album.ParentColumn = this.ft_album;
            this.fsN_Album.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // fsN_Title
            // 
            this.fsN_Title.DataPropertyName = "Title";
            this.fsN_Title.ParentColumn = this.ft_title;
            this.fsN_Title.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // fsN_Duration
            // 
            this.fsN_Duration.DataPropertyName = "HumanizedDuration";
            this.fsN_Duration.ParentColumn = this.ft_duration;
            this.fsN_Duration.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // fsN_UserTags
            // 
            this.fsN_UserTags.DataPropertyName = "UserTags";
            this.fsN_UserTags.ParentColumn = this.ft_usertags;
            this.fsN_UserTags.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // fsN_FileTags
            // 
            this.fsN_FileTags.DataPropertyName = "FileTags";
            this.fsN_FileTags.ParentColumn = this.ft_filetags;
            this.fsN_FileTags.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // st_Name
            // 
            this.st_Name.Header = "Support name";
            this.st_Name.Width = 150;
            // 
            // st_Size
            // 
            this.st_Size.Header = "Size";
            // 
            // searchTimer
            // 
            this.searchTimer.Interval = 300;
            this.searchTimer.Tick += new System.EventHandler(this.searchTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 431);
            this.Controls.Add(this.splitHMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainToolbar);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Nando is a CATalogator - easily archive cd, dvd and so on.";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.mainToolbar.ResumeLayout(false);
            this.mainToolbar.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitHMain.Panel1.ResumeLayout(false);
            this.splitHMain.Panel2.ResumeLayout(false);
            this.splitHMain.ResumeLayout(false);
            this.splitHLeft.Panel1.ResumeLayout(false);
            this.splitHLeft.Panel1.PerformLayout();
            this.splitHLeft.Panel2.ResumeLayout(false);
            this.splitHLeft.Panel2.PerformLayout();
            this.splitHLeft.ResumeLayout(false);
            this.supContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem savetoolStrip;
		private System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton saveBTN;
		private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
		private System.Windows.Forms.ToolStripMenuItem LoadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem syncToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private HS.Controls.Tree.NodeControls.NodeTextBox nodeFileSize;
		private HS.Controls.Tree.NodeControls.NodeTextBox nodeName;
		private HS.Controls.Tree.NodeControls.NodeIcon nodeIcon;
		private System.Windows.Forms.ToolStripMenuItem searchOnTheWholeArchiveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private HS.Controls.Tree.TreeViewAdv treeFiles;
		private System.Windows.Forms.Button addBTN;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolStripTextBox searchText;
		private System.Windows.Forms.ToolStripButton newDiskBTN;
		private System.Windows.Forms.TextBox cdName;
		private System.Windows.Forms.Button browseBTN;
		private System.Windows.Forms.TextBox cdPath;
		private System.Windows.Forms.ToolStrip mainToolbar;
		private System.Windows.Forms.Label labelNomeArchivio;
		private System.Windows.Forms.SplitContainer splitHLeft;
		private System.Windows.Forms.SplitContainer splitHMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem aiutoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private HS.Controls.Tree.TreeColumn ft_size;
        private HS.Controls.Tree.TreeColumn ft_artist;
        private HS.Controls.Tree.TreeColumn ft_album;
        private HS.Controls.Tree.TreeColumn ft_title;
        private HS.Controls.Tree.TreeColumn ft_duration;
        private HS.Controls.Tree.TreeColumn ft_usertags;
        private HS.Controls.Tree.TreeColumn ft_filetags;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_Name;
        private HS.Controls.Tree.NodeControls.NodeIcon fsN_Icon;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_Artist;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_Album;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_Title;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_UserTags;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_FileTags;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_Duration;
        private HS.Controls.Tree.NodeControls.NodeTextBox fsN_Size;
        private HS.Controls.Tree.TreeColumn st_Name;
        private HS.Controls.Tree.TreeColumn st_Size;
        private HS.Controls.Tree.TreeColumn ft_name;
        private HS.Controls.Tree.TreeViewAdv treeSupport;
        private HS.Controls.Tree.TreeColumn sn_Name;
        private HS.Controls.Tree.TreeColumn sn_Size;
        private HS.Controls.Tree.NodeControls.NodeTextBox ssn_Name;
        private HS.Controls.Tree.NodeControls.NodeTextBox ssn_Size;
        private HS.Controls.Tree.NodeControls.NodeIcon ssn_Icon;
        private System.Windows.Forms.Timer searchTimer;
        private System.Windows.Forms.ContextMenuStrip supContext;
        private System.Windows.Forms.ToolStripMenuItem supRename;
        private System.Windows.Forms.ToolStripMenuItem supDelete;
        private System.Windows.Forms.ToolStripMenuItem associateMNU;
        private System.Windows.Forms.ToolStripMenuItem startParsing;
	}
}
