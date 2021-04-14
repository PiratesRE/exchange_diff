using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class PublicFolderPermissionControl : ExchangeUserControl
	{
		public PublicFolderPermissionControl()
		{
			this.InitializeComponent();
			this.permissionLevelLabel.Text = Strings.PublicFolderPermissionLevelLabel;
			this.createItemsCheckBox.Text = Strings.PublicFolderPermissionCreateItemsText;
			this.readItemsCheckBox.Text = Strings.PublicFolderPermissionReadItemsText;
			this.createSubfoldersCheckBox.Text = Strings.PublicFolderPermissionCreateSubfoldersText;
			this.folderOwnerCheckBox.Text = Strings.PublicFolderPermissionFolderOwnerText;
			this.folderContactCheckBox.Text = Strings.PublicFolderPermissionFolderContactText;
			this.folderVisibleCheckBox.Text = Strings.PublicFolderPermissionFolderVisibleText;
			this.editOwnCheckBox.Text = Strings.PublicFolderPermissionEditOwnText;
			this.editAllCheckBox.Text = Strings.PublicFolderPermissionEditAllText;
			this.deleteOwnCheckBox.Text = Strings.PublicFolderPermissionDeleteOwnText;
			this.deleteAllCheckBox.Text = Strings.PublicFolderPermissionDeleteAllText;
			new PublicFolderPermissionLevelComboBoxAdapter(this.permissionLevelComboBox).BindPermissionValue(this, "PermissionValue");
			FlagsEnumBinder<PublicFolderPermission> flagsEnumBinder = new FlagsEnumBinder<PublicFolderPermission>(this, "PermissionValue");
			flagsEnumBinder.BindCheckBoxToFlag(this.createItemsCheckBox, PublicFolderPermission.CreateItems);
			flagsEnumBinder.BindCheckBoxToFlag(this.readItemsCheckBox, PublicFolderPermission.ReadItems | PublicFolderPermission.FolderVisible);
			flagsEnumBinder.BindCheckBoxToFlag(this.createSubfoldersCheckBox, PublicFolderPermission.CreateSubfolders);
			flagsEnumBinder.BindCheckBoxToFlag(this.folderOwnerCheckBox, PublicFolderPermission.FolderOwner);
			flagsEnumBinder.BindCheckBoxToFlag(this.folderContactCheckBox, PublicFolderPermission.FolderContact);
			flagsEnumBinder.BindCheckBoxToFlag(this.folderVisibleCheckBox, PublicFolderPermission.FolderVisible);
			flagsEnumBinder.BindCheckBoxToFlag(this.editOwnCheckBox, PublicFolderPermission.EditOwnedItems);
			flagsEnumBinder.BindCheckBoxToFlag(this.editAllCheckBox, PublicFolderPermission.EditOwnedItems | PublicFolderPermission.EditAllItems);
			flagsEnumBinder.BindCheckBoxToFlag(this.deleteOwnCheckBox, PublicFolderPermission.DeleteOwnedItems);
			flagsEnumBinder.BindCheckBoxToFlag(this.deleteAllCheckBox, PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.DeleteAllItems);
			this.folderVisibleCheckBox.DataBindings.Add("Enabled", this.readItemsCheckBox, "Checked", true, DataSourceUpdateMode.Never).Format += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = false.Equals(e.Value);
			};
			this.editOwnCheckBox.DataBindings.Add("Enabled", this.editAllCheckBox, "Checked", true, DataSourceUpdateMode.Never).Format += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = false.Equals(e.Value);
			};
			this.deleteOwnCheckBox.DataBindings.Add("Enabled", this.deleteAllCheckBox, "Checked", true, DataSourceUpdateMode.Never).Format += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = false.Equals(e.Value);
			};
		}

		public event EventHandler PermissionValueChanged;

		public PublicFolderPermission? PermissionValue
		{
			get
			{
				return this.permissionValue;
			}
			set
			{
				if (this.permissionValue != value)
				{
					this.permissionValue = value;
					this.OnPermissionValueChanged(EventArgs.Empty);
				}
			}
		}

		private void OnPermissionValueChanged(EventArgs args)
		{
			if (this.PermissionValueChanged != null)
			{
				this.PermissionValueChanged(this, args);
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "PermissionValue";
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size preferredSize = this.topTableLayoutPanel.GetPreferredSize(proposedSize);
			Size preferredSize2 = this.tableLayoutPanel.GetPreferredSize(proposedSize);
			return new Size(Math.Max(preferredSize.Width, preferredSize2.Width), preferredSize.Height + preferredSize2.Height);
		}

		private void InitializeComponent()
		{
			this.permissionLevelLabel = new Label();
			this.permissionLevelComboBox = new ExchangeComboBox();
			this.createItemsCheckBox = new AutoHeightCheckBox();
			this.readItemsCheckBox = new AutoHeightCheckBox();
			this.createSubfoldersCheckBox = new AutoHeightCheckBox();
			this.folderOwnerCheckBox = new AutoHeightCheckBox();
			this.folderContactCheckBox = new AutoHeightCheckBox();
			this.folderVisibleCheckBox = new AutoHeightCheckBox();
			this.editOwnCheckBox = new AutoHeightCheckBox();
			this.editAllCheckBox = new AutoHeightCheckBox();
			this.deleteOwnCheckBox = new AutoHeightCheckBox();
			this.deleteAllCheckBox = new AutoHeightCheckBox();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.topTableLayoutPanel = new AutoTableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			this.topTableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.permissionLevelLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.permissionLevelLabel.AutoSize = true;
			this.permissionLevelLabel.ImageAlign = ContentAlignment.TopLeft;
			this.permissionLevelLabel.Location = new Point(0, 3);
			this.permissionLevelLabel.Margin = new Padding(0, 3, 0, 5);
			this.permissionLevelLabel.Name = "permissionLevelLabel";
			this.permissionLevelLabel.Size = new Size(108, 13);
			this.permissionLevelLabel.TabIndex = 1;
			this.permissionLevelLabel.Text = "permissionLevelLabel";
			this.permissionLevelComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.permissionLevelComboBox.Location = new Point(111, 0);
			this.permissionLevelComboBox.Margin = new Padding(3, 0, 0, 0);
			this.permissionLevelComboBox.Name = "permissionLevelComboBox";
			this.permissionLevelComboBox.Size = new Size(343, 21);
			this.permissionLevelComboBox.TabIndex = 2;
			this.permissionLevelComboBox.Text = "permissionLevelComboBox";
			this.createItemsCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.createItemsCheckBox.Location = new Point(3, 8);
			this.createItemsCheckBox.Margin = new Padding(3, 0, 0, 0);
			this.createItemsCheckBox.Name = "createItemsCheckBox";
			this.createItemsCheckBox.Size = new Size(224, 17);
			this.createItemsCheckBox.TabIndex = 3;
			this.createItemsCheckBox.Text = "createItemsCheckBox";
			this.readItemsCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.readItemsCheckBox.Location = new Point(3, 28);
			this.readItemsCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.readItemsCheckBox.Name = "readItemsCheckBox";
			this.readItemsCheckBox.Size = new Size(224, 17);
			this.readItemsCheckBox.TabIndex = 5;
			this.readItemsCheckBox.Text = "readItemsCheckBox";
			this.createSubfoldersCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.createSubfoldersCheckBox.Location = new Point(3, 48);
			this.createSubfoldersCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.createSubfoldersCheckBox.Name = "createSubfoldersCheckBox";
			this.createSubfoldersCheckBox.Size = new Size(224, 17);
			this.createSubfoldersCheckBox.TabIndex = 7;
			this.createSubfoldersCheckBox.Text = "createSubfoldersCheckBox";
			this.folderOwnerCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.folderOwnerCheckBox.Location = new Point(230, 8);
			this.folderOwnerCheckBox.Margin = new Padding(3, 0, 0, 0);
			this.folderOwnerCheckBox.Name = "folderOwnerCheckBox";
			this.folderOwnerCheckBox.Size = new Size(224, 17);
			this.folderOwnerCheckBox.TabIndex = 4;
			this.folderOwnerCheckBox.Text = "folderOwnerCheckBox";
			this.folderContactCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.folderContactCheckBox.Location = new Point(230, 28);
			this.folderContactCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.folderContactCheckBox.Name = "folderContactCheckBox";
			this.folderContactCheckBox.Size = new Size(224, 17);
			this.folderContactCheckBox.TabIndex = 6;
			this.folderContactCheckBox.Text = "folderContactCheckBox";
			this.folderVisibleCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.folderVisibleCheckBox.Location = new Point(230, 48);
			this.folderVisibleCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.folderVisibleCheckBox.Name = "folderVisibleCheckBox";
			this.folderVisibleCheckBox.Size = new Size(224, 17);
			this.folderVisibleCheckBox.TabIndex = 8;
			this.folderVisibleCheckBox.Text = "folderVisibleCheckBox";
			this.editOwnCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.editOwnCheckBox.ImageAlign = ContentAlignment.TopLeft;
			this.editOwnCheckBox.Location = new Point(3, 68);
			this.editOwnCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.editOwnCheckBox.Name = "editOwnCheckBox";
			this.editOwnCheckBox.Size = new Size(224, 17);
			this.editOwnCheckBox.TabIndex = 9;
			this.editOwnCheckBox.Text = "editOwnCheckBox";
			this.editAllCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.editAllCheckBox.ImageAlign = ContentAlignment.TopLeft;
			this.editAllCheckBox.Location = new Point(3, 88);
			this.editAllCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.editAllCheckBox.Name = "editAllCheckBox";
			this.editAllCheckBox.Size = new Size(224, 17);
			this.editAllCheckBox.TabIndex = 11;
			this.editAllCheckBox.Text = "editAllCheckBox";
			this.deleteOwnCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.deleteOwnCheckBox.ImageAlign = ContentAlignment.TopLeft;
			this.deleteOwnCheckBox.Location = new Point(230, 68);
			this.deleteOwnCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.deleteOwnCheckBox.Name = "deleteOwnCheckBox";
			this.deleteOwnCheckBox.Size = new Size(224, 17);
			this.deleteOwnCheckBox.TabIndex = 10;
			this.deleteOwnCheckBox.Text = "editOwnCheckBox";
			this.deleteAllCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.deleteAllCheckBox.ImageAlign = ContentAlignment.TopLeft;
			this.deleteAllCheckBox.Location = new Point(230, 88);
			this.deleteAllCheckBox.Margin = new Padding(3, 3, 0, 0);
			this.deleteAllCheckBox.Name = "deleteAllCheckBox";
			this.deleteAllCheckBox.Size = new Size(224, 17);
			this.deleteAllCheckBox.TabIndex = 12;
			this.deleteAllCheckBox.Text = "editAllCheckBox";
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel.Controls.Add(this.createItemsCheckBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.folderOwnerCheckBox, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.readItemsCheckBox, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.folderContactCheckBox, 1, 1);
			this.tableLayoutPanel.Controls.Add(this.createSubfoldersCheckBox, 0, 2);
			this.tableLayoutPanel.Controls.Add(this.folderVisibleCheckBox, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.editOwnCheckBox, 0, 3);
			this.tableLayoutPanel.Controls.Add(this.deleteOwnCheckBox, 1, 3);
			this.tableLayoutPanel.Controls.Add(this.editAllCheckBox, 0, 4);
			this.tableLayoutPanel.Controls.Add(this.deleteAllCheckBox, 1, 4);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 21);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(0, 8, 0, 0);
			this.tableLayoutPanel.RowCount = 5;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(454, 105);
			this.tableLayoutPanel.TabIndex = 1;
			this.topTableLayoutPanel.AutoLayout = true;
			this.topTableLayoutPanel.AutoSize = true;
			this.topTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.topTableLayoutPanel.ColumnCount = 2;
			this.topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.topTableLayoutPanel.Controls.Add(this.permissionLevelLabel, 0, 0);
			this.topTableLayoutPanel.Controls.Add(this.permissionLevelComboBox, 1, 0);
			this.topTableLayoutPanel.Dock = DockStyle.Top;
			this.topTableLayoutPanel.Location = new Point(0, 0);
			this.topTableLayoutPanel.Margin = new Padding(0);
			this.topTableLayoutPanel.Name = "topTableLayoutPanel";
			this.topTableLayoutPanel.RowCount = 1;
			this.topTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.topTableLayoutPanel.Size = new Size(454, 21);
			this.topTableLayoutPanel.TabIndex = 0;
			base.Controls.Add(this.tableLayoutPanel);
			base.Controls.Add(this.topTableLayoutPanel);
			base.Name = "PublicFolderPermissionControl";
			base.Size = new Size(454, 128);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			this.topTableLayoutPanel.ResumeLayout(false);
			this.topTableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private const string PermissionValuePropertyName = "PermissionValue";

		private const string EnabledPropertyName = "Enabled";

		private const string CheckedPropertyName = "Checked";

		private Label permissionLevelLabel;

		private ExchangeComboBox permissionLevelComboBox;

		private AutoHeightCheckBox createItemsCheckBox;

		private AutoHeightCheckBox readItemsCheckBox;

		private AutoHeightCheckBox createSubfoldersCheckBox;

		private AutoHeightCheckBox folderOwnerCheckBox;

		private AutoHeightCheckBox folderContactCheckBox;

		private AutoHeightCheckBox folderVisibleCheckBox;

		private AutoHeightCheckBox editOwnCheckBox;

		private AutoHeightCheckBox editAllCheckBox;

		private AutoHeightCheckBox deleteOwnCheckBox;

		private AutoHeightCheckBox deleteAllCheckBox;

		private AutoTableLayoutPanel topTableLayoutPanel;

		private AutoTableLayoutPanel tableLayoutPanel;

		private PublicFolderPermission? permissionValue = null;
	}
}
