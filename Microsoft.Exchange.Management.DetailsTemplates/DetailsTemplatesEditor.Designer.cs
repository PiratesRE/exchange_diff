namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal partial class DetailsTemplatesEditor : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.mainMenu = new global::System.Windows.Forms.MainMenu();
			this.fileMenuItem = new global::System.Windows.Forms.MenuItem();
			this.saveMenuItem = new global::System.Windows.Forms.MenuItem();
			this.menuItemSeparator = new global::System.Windows.Forms.MenuItem();
			this.exitMenuItem = new global::System.Windows.Forms.MenuItem();
			this.editMenuItem = new global::System.Windows.Forms.MenuItem();
			this.undoMenuItem = new global::System.Windows.Forms.MenuItem();
			this.redoMenuItem = new global::System.Windows.Forms.MenuItem();
			this.menuItemSeparatorBetweenRedoAndCut = new global::System.Windows.Forms.MenuItem();
			this.cutMenuItem = new global::System.Windows.Forms.MenuItem();
			this.copyMenuItem = new global::System.Windows.Forms.MenuItem();
			this.pasteMenuItem = new global::System.Windows.Forms.MenuItem();
			this.deleteMenuItem = new global::System.Windows.Forms.MenuItem();
			this.selectAllMenuItemSeparator = new global::System.Windows.Forms.MenuItem();
			this.selectAllMenuItem = new global::System.Windows.Forms.MenuItem();
			this.menuItemSeparatorBetweenDelAndRemove = new global::System.Windows.Forms.MenuItem();
			this.addTabPageMenuItem = new global::System.Windows.Forms.MenuItem();
			this.removeTabPageMenuItem = new global::System.Windows.Forms.MenuItem();
			this.editorContextMenu = new global::System.Windows.Forms.ContextMenu();
			this.cutContextMenuItem = new global::System.Windows.Forms.MenuItem();
			this.copyContextMenuItem = new global::System.Windows.Forms.MenuItem();
			this.pasteContextMenuItem = new global::System.Windows.Forms.MenuItem();
			this.deleteContextMenuItem = new global::System.Windows.Forms.MenuItem();
			this.selectAllContextMenuItemSeparator = new global::System.Windows.Forms.MenuItem();
			this.selectAllContextMenuItem = new global::System.Windows.Forms.MenuItem();
			this.contextMenuItemSeparatorBetweenDelAndRemove = new global::System.Windows.Forms.MenuItem();
			this.addTabPageContextMenuItem = new global::System.Windows.Forms.MenuItem();
			this.removeTabPageContextMenuItem = new global::System.Windows.Forms.MenuItem();
			this.helpMenuItem = new global::System.Windows.Forms.MenuItem();
			this.splitContainer1 = new global::System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new global::System.Windows.Forms.SplitContainer();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			base.SuspendLayout();
			this.mainMenu.MenuItems.AddRange(new global::System.Windows.Forms.MenuItem[]
			{
				this.fileMenuItem,
				this.editMenuItem,
				this.helpMenuItem
			});
			this.fileMenuItem.MenuItems.AddRange(new global::System.Windows.Forms.MenuItem[]
			{
				this.saveMenuItem,
				this.menuItemSeparator,
				this.exitMenuItem,
				this.undoMenuItem
			});
			this.fileMenuItem.Name = "fileMenuItem";
			this.fileMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.File;
			this.saveMenuItem.Name = "saveMenuItem";
			this.saveMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Save;
			this.saveMenuItem.Click += new global::System.EventHandler(this.saveToolStripMenuItem_Click);
			this.menuItemSeparator.Name = "menuItemSeparator";
			this.menuItemSeparator.Text = "-";
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Exit;
			this.exitMenuItem.Click += new global::System.EventHandler(this.exitToolStripMenuItem_Click);
			this.editMenuItem.Name = "editMenuItem";
			this.editMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.EditText;
			this.editMenuItem.MenuItems.AddRange(new global::System.Windows.Forms.MenuItem[]
			{
				this.addTabPageMenuItem,
				this.removeTabPageMenuItem,
				this.menuItemSeparatorBetweenDelAndRemove,
				this.undoMenuItem,
				this.redoMenuItem,
				this.menuItemSeparatorBetweenRedoAndCut,
				this.cutMenuItem,
				this.copyMenuItem,
				this.pasteMenuItem,
				this.deleteMenuItem,
				this.selectAllMenuItemSeparator,
				this.selectAllMenuItem
			});
			this.undoMenuItem.Name = "undoMenuItem";
			this.undoMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Undo;
			this.undoMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Undo;
			this.undoMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.CtrlZ;
			this.undoMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.redoMenuItem.Name = "redoMenuItem";
			this.redoMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Redo;
			this.redoMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Redo;
			this.redoMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.CtrlY;
			this.redoMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.menuItemSeparatorBetweenRedoAndCut.Name = "menuItemSeparatorBetweenRedoAndCut";
			this.menuItemSeparatorBetweenRedoAndCut.Text = "-";
			this.cutMenuItem.Name = "cutMenuItem";
			this.cutMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Cut;
			this.cutMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Cut;
			this.cutMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.CtrlX;
			this.cutMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.copyMenuItem.Name = "copyMenuItem";
			this.copyMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Copy;
			this.copyMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Copy;
			this.copyMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.CtrlC;
			this.copyMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.pasteMenuItem.Name = "pasteMenuItem";
			this.pasteMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Paste;
			this.pasteMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Paste;
			this.pasteMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.CtrlV;
			this.pasteMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.deleteMenuItem.Name = "deleteMenuItem";
			this.deleteMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Delete;
			this.deleteMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Delete;
			this.deleteMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.Del;
			this.deleteMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.selectAllMenuItemSeparator.Name = "selectAllMenuItemSeparator";
			this.selectAllMenuItemSeparator.Text = "-";
			this.selectAllMenuItem.Name = "selectAllMenuItem";
			this.selectAllMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.SelectAll;
			this.selectAllMenuItem.Tag = global::Microsoft.Exchange.Management.DetailsTemplates.DetailsTemplatesMenuService.SelectAllCommandId;
			this.selectAllMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.CtrlA;
			this.selectAllMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.menuItemSeparatorBetweenDelAndRemove.Name = "menuItemSeparatorBetweenDelAndRemove";
			this.menuItemSeparatorBetweenDelAndRemove.Text = "-";
			this.addTabPageMenuItem.Name = "addTabPageMenuItem";
			this.addTabPageMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.AddTab;
			this.addTabPageMenuItem.Tag = global::Microsoft.Exchange.Management.DetailsTemplates.DetailsTemplatesMenuService.AddTabPageCommandId;
			this.addTabPageMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.removeTabPageMenuItem.Name = "removeTabPageMenuItem";
			this.removeTabPageMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.RemoveTab;
			this.removeTabPageMenuItem.Tag = global::Microsoft.Exchange.Management.DetailsTemplates.DetailsTemplatesMenuService.RemoveTabPageCommandId;
			this.removeTabPageMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.editorContextMenu.Name = "editorContextMenu";
			this.editorContextMenu.MenuItems.AddRange(new global::System.Windows.Forms.MenuItem[]
			{
				this.addTabPageContextMenuItem,
				this.removeTabPageContextMenuItem,
				this.contextMenuItemSeparatorBetweenDelAndRemove,
				this.cutContextMenuItem,
				this.copyContextMenuItem,
				this.pasteContextMenuItem,
				this.deleteContextMenuItem,
				this.selectAllContextMenuItemSeparator,
				this.selectAllContextMenuItem
			});
			this.cutMenuItem.Name = "cutContextMenuItem";
			this.cutContextMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Cut;
			this.cutContextMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Cut;
			this.cutContextMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.copyContextMenuItem.Name = "copyContextMenuItem";
			this.copyContextMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Copy;
			this.copyContextMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Copy;
			this.copyContextMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.pasteContextMenuItem.Name = "pasteContextMenuItem";
			this.pasteContextMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Paste;
			this.pasteContextMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Paste;
			this.pasteContextMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.deleteContextMenuItem.Name = "deleteContextMenuItem";
			this.deleteContextMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Delete;
			this.deleteContextMenuItem.Tag = global::System.ComponentModel.Design.StandardCommands.Delete;
			this.deleteContextMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.selectAllContextMenuItemSeparator.Name = "selectAllContextMenuItemSeparator";
			this.selectAllContextMenuItemSeparator.Text = "-";
			this.selectAllContextMenuItem.Name = "selectAllContextMenuItem";
			this.selectAllContextMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.SelectAll;
			this.selectAllContextMenuItem.Tag = global::Microsoft.Exchange.Management.DetailsTemplates.DetailsTemplatesMenuService.SelectAllCommandId;
			this.selectAllContextMenuItem.Shortcut = global::System.Windows.Forms.Shortcut.CtrlA;
			this.selectAllContextMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.contextMenuItemSeparatorBetweenDelAndRemove.Name = "contextMenuItemSeparatorBetweenDelAndRemove";
			this.contextMenuItemSeparatorBetweenDelAndRemove.Text = "-";
			this.addTabPageContextMenuItem.Name = "addTabPageContextMenuItem";
			this.addTabPageContextMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.AddTab;
			this.addTabPageContextMenuItem.Tag = global::Microsoft.Exchange.Management.DetailsTemplates.DetailsTemplatesMenuService.AddTabPageCommandId;
			this.addTabPageContextMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.removeTabPageContextMenuItem.Name = "removeTabPageContextMenuItem";
			this.removeTabPageContextMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.RemoveTab;
			this.removeTabPageContextMenuItem.Tag = global::Microsoft.Exchange.Management.DetailsTemplates.DetailsTemplatesMenuService.RemoveTabPageCommandId;
			this.removeTabPageContextMenuItem.Click += new global::System.EventHandler(this.editMenuItems_Click);
			this.helpMenuItem.Name = "helpMenuItem";
			this.helpMenuItem.Text = global::Microsoft.Exchange.ManagementGUI.Resources.Strings.Help;
			this.helpMenuItem.Click += new global::System.EventHandler(this.helpToolStripMenuItem_Click);
			this.splitContainer1.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new global::System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new global::System.Drawing.Size(792, 392);
			this.splitContainer1.SplitterDistance = 662;
			this.splitContainer1.TabIndex = 1;
			this.splitContainer2.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new global::System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Size = new global::System.Drawing.Size(662, 392);
			this.splitContainer2.SplitterDistance = 140;
			this.splitContainer2.TabIndex = 0;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(792, 416);
			base.Controls.Add(this.splitContainer1);
			base.Menu = this.mainMenu;
			base.Name = "DetailsTemplatesEditor";
			this.Text = "DetailsTemplatesEditor";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.MainMenu mainMenu;

		private global::System.Windows.Forms.MenuItem fileMenuItem;

		private global::System.Windows.Forms.MenuItem saveMenuItem;

		private global::System.Windows.Forms.MenuItem menuItemSeparator;

		private global::System.Windows.Forms.MenuItem exitMenuItem;

		private global::System.Windows.Forms.MenuItem helpMenuItem;

		private global::System.Windows.Forms.MenuItem editMenuItem;

		private global::System.Windows.Forms.MenuItem undoMenuItem;

		private global::System.Windows.Forms.MenuItem redoMenuItem;

		private global::System.Windows.Forms.MenuItem menuItemSeparatorBetweenRedoAndCut;

		private global::System.Windows.Forms.MenuItem cutMenuItem;

		private global::System.Windows.Forms.MenuItem copyMenuItem;

		private global::System.Windows.Forms.MenuItem pasteMenuItem;

		private global::System.Windows.Forms.MenuItem deleteMenuItem;

		private global::System.Windows.Forms.MenuItem selectAllMenuItemSeparator;

		private global::System.Windows.Forms.MenuItem selectAllMenuItem;

		private global::System.Windows.Forms.MenuItem menuItemSeparatorBetweenDelAndRemove;

		private global::System.Windows.Forms.MenuItem removeTabPageMenuItem;

		private global::System.Windows.Forms.MenuItem addTabPageMenuItem;

		private global::System.Windows.Forms.MenuItem cutContextMenuItem;

		private global::System.Windows.Forms.MenuItem copyContextMenuItem;

		private global::System.Windows.Forms.MenuItem pasteContextMenuItem;

		private global::System.Windows.Forms.MenuItem deleteContextMenuItem;

		private global::System.Windows.Forms.MenuItem selectAllContextMenuItemSeparator;

		private global::System.Windows.Forms.MenuItem selectAllContextMenuItem;

		private global::System.Windows.Forms.MenuItem contextMenuItemSeparatorBetweenDelAndRemove;

		private global::System.Windows.Forms.MenuItem removeTabPageContextMenuItem;

		private global::System.Windows.Forms.MenuItem addTabPageContextMenuItem;

		private global::System.Windows.Forms.ContextMenu editorContextMenu;

		private global::System.Windows.Forms.SplitContainer splitContainer1;

		private global::System.Windows.Forms.SplitContainer splitContainer2;
	}
}
