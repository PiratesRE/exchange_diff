using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ColumnPickerDialog : ExchangePropertyPageControl
	{
		public ColumnPickerDialog()
		{
			this.InitializeComponent();
			base.HelpTopic = HelpId.ColumnPickerDialog.ToString();
			this.tableLayoutPanel.SuspendLayout();
			this.availableColumnLabel.Text = Strings.AvailableColumnsLabel;
			this.displayedColumnsLabel.Text = Strings.DisplayedColumnsLabel;
			this.addButton.Text = Strings.AddButton;
			this.removeButton.Text = Strings.RemoveButton;
			this.addAllButton.Text = Strings.AddAllButton;
			this.moveUpButton.Text = Strings.MoveUpButton;
			this.moveDownButton.Text = Strings.MoveDownButton;
			this.restoreDefaultsButton.Text = Strings.RestoreDefaultsButton;
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
		}

		public ColumnPickerDialog(DataListView owner) : this()
		{
			this.list = owner;
			this.InitializeLists();
		}

		protected override Size DefaultMaximumSize
		{
			get
			{
				return new Size(570, 316);
			}
		}

		protected override Size DefaultMinimumSize
		{
			get
			{
				return new Size(570, 316);
			}
		}

		[DefaultValue(true)]
		public new bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}

		[DefaultValue(AutoSizeMode.GrowAndShrink)]
		public new AutoSizeMode AutoSizeMode
		{
			get
			{
				return base.AutoSizeMode;
			}
			set
			{
				base.AutoSizeMode = value;
			}
		}

		public StringCollection DisplayedColumnNames
		{
			get
			{
				StringCollection stringCollection = new StringCollection();
				for (int i = 0; i < this.displayedColumnsListView.Items.Count; i++)
				{
					stringCollection.Add(this.displayedColumnsListView.Items[i].SubItems[1].Text);
				}
				return stringCollection;
			}
		}

		private void InitializeLists()
		{
			this.availableColumnsListView.BeginUpdate();
			this.displayedColumnsListView.BeginUpdate();
			List<ListViewItem> list = new List<ListViewItem>();
			foreach (ExchangeColumnHeader exchangeColumnHeader in this.list.AvailableColumns)
			{
				ListViewItem listViewItem = new ListViewItem(exchangeColumnHeader.Text);
				listViewItem.Name = exchangeColumnHeader.Name;
				listViewItem.SubItems.Add(exchangeColumnHeader.Name);
				listViewItem.SubItems.Add(exchangeColumnHeader.DisplayIndex.ToString());
				if (exchangeColumnHeader.Visible)
				{
					list.Add(listViewItem);
				}
				else
				{
					this.availableColumnsListView.Items.Add(listViewItem);
				}
			}
			list.Sort(new ColumnPickerDialog.DisplayIndexItemComparer());
			this.displayedColumnsListView.Items.AddRange(list.ToArray());
			this.ResizeLists();
			this.displayedColumnsListView.EndUpdate();
			this.availableColumnsListView.EndUpdate();
			this.SelectFirstItemInBothLists();
			this.UpdateButtonStates();
		}

		private ExchangeColumnHeader GetColumnFromListItem(ListViewItem item)
		{
			return this.list.AvailableColumns[item.SubItems[1].Text];
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			this.AddColumns(this.availableColumnsListView.SelectedItems);
		}

		private void addAllButton_Click(object sender, EventArgs e)
		{
			this.AddColumns(this.availableColumnsListView.Items);
		}

		private void AddColumns(IList itemCollection)
		{
			this.displayedColumnsListView.SelectedItems.Clear();
			int index = ((ListViewItem)itemCollection[0]).Index;
			foreach (object obj in itemCollection)
			{
				ListViewItem listViewItem = (ListViewItem)obj;
				this.availableColumnsListView.Items.Remove(listViewItem);
				this.displayedColumnsListView.Items.Add(listViewItem);
				listViewItem.Selected = true;
			}
			this.PreserveSelectionAfterAddRemove(index, this.availableColumnsListView, this.displayedColumnsListView);
			this.ResizeLists();
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			this.UpdateRightToLeftLayout();
		}

		private void UpdateRightToLeftLayout()
		{
			if (base.IsHandleCreated)
			{
				this.availableColumnsListView.RightToLeftLayout = LayoutHelper.IsRightToLeft(this);
				this.displayedColumnsListView.RightToLeftLayout = LayoutHelper.IsRightToLeft(this);
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			this.UpdateRightToLeftLayout();
		}

		private void PreserveSelectionAfterAddRemove(int selectionIndexAfterAdd, ListView fromList, ListView toList)
		{
			if (fromList.Items.Count > 0)
			{
				selectionIndexAfterAdd = ((fromList.Items.Count > selectionIndexAfterAdd) ? selectionIndexAfterAdd : (fromList.Items.Count - 1));
				fromList.Items[selectionIndexAfterAdd].Selected = true;
			}
			toList.SelectedItems[0].Focused = true;
			toList.Focus();
		}

		private void removeButton_Click(object sender, EventArgs e)
		{
			this.availableColumnsListView.SelectedItems.Clear();
			int index = this.displayedColumnsListView.SelectedItems[0].Index;
			foreach (object obj in this.displayedColumnsListView.SelectedItems)
			{
				ListViewItem listViewItem = (ListViewItem)obj;
				this.displayedColumnsListView.Items.Remove(listViewItem);
				this.availableColumnsListView.Items.Add(listViewItem);
				listViewItem.Selected = true;
			}
			this.PreserveSelectionAfterAddRemove(index, this.displayedColumnsListView, this.availableColumnsListView);
			this.ResizeLists();
		}

		private void moveUpButton_Click(object sender, EventArgs e)
		{
			this.MoveItem(true);
		}

		private void moveDownButton_Click(object sender, EventArgs e)
		{
			this.MoveItem(false);
		}

		private void MoveItem(bool moveUp)
		{
			ListViewItem listViewItem = this.displayedColumnsListView.SelectedItems[0];
			int index = moveUp ? (listViewItem.Index - 1) : (listViewItem.Index + 1);
			this.displayedColumnsListView.Items.Remove(listViewItem);
			this.displayedColumnsListView.Items.Insert(index, listViewItem);
			this.displayedColumnsListView.Focus();
			this.displayedColumnsListView.SelectedItems[0].Focused = true;
			this.UpdateButtonStates();
		}

		private void restoreDefaultsButton_Click(object sender, EventArgs e)
		{
			this.availableColumnsListView.BeginUpdate();
			this.displayedColumnsListView.BeginUpdate();
			List<ListViewItem> list = new List<ListViewItem>();
			foreach (ExchangeColumnHeader exchangeColumnHeader in this.list.AvailableColumns)
			{
				if (!exchangeColumnHeader.Default && this.displayedColumnsListView.Items.ContainsKey(exchangeColumnHeader.Name))
				{
					ListViewItem listViewItem = this.displayedColumnsListView.Items[exchangeColumnHeader.Name];
					this.displayedColumnsListView.Items.Remove(listViewItem);
					this.availableColumnsListView.Items.Add(listViewItem);
				}
				else if (exchangeColumnHeader.Default)
				{
					ListViewItem listViewItem2;
					if (!this.displayedColumnsListView.Items.ContainsKey(exchangeColumnHeader.Name))
					{
						listViewItem2 = this.availableColumnsListView.Items[exchangeColumnHeader.Name];
						this.availableColumnsListView.Items.Remove(listViewItem2);
					}
					else
					{
						listViewItem2 = this.displayedColumnsListView.Items[exchangeColumnHeader.Name];
						this.displayedColumnsListView.Items.Remove(listViewItem2);
					}
					listViewItem2.SubItems[2].Text = exchangeColumnHeader.DefaultDisplayIndex.ToString();
					list.Add(listViewItem2);
				}
			}
			list.Sort(new ColumnPickerDialog.DisplayIndexItemComparer());
			this.displayedColumnsListView.Items.AddRange(list.ToArray());
			this.SelectFirstItemInBothLists();
			this.ResizeLists();
			this.displayedColumnsListView.EndUpdate();
			this.availableColumnsListView.EndUpdate();
		}

		private void ResizeLists()
		{
			this.displayedColumnsListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			this.availableColumnsListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void SelectFirstItemInBothLists()
		{
			this.SelectFirstItem(this.availableColumnsListView);
			this.SelectFirstItem(this.displayedColumnsListView);
		}

		private void SelectFirstItem(ListView list)
		{
			if (list.Items.Count > 0)
			{
				list.SelectedItems.Clear();
				list.Items[0].Selected = true;
			}
		}

		private void availableColumnsListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateButtonStates();
		}

		private void displayedColumnsListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateButtonStates();
		}

		private void UpdateButtonStates()
		{
			bool flag = this.displayedColumnsListView.SelectedItems.Count == 1;
			bool flag2 = this.displayedColumnsListView.SelectedItems.Count != 0;
			bool flag3 = false;
			if (flag2)
			{
				foreach (object obj in this.displayedColumnsListView.SelectedItems)
				{
					ListViewItem item = (ListViewItem)obj;
					ExchangeColumnHeader columnFromListItem = this.GetColumnFromListItem(item);
					if (columnFromListItem.Name == this.list.SelectionNameProperty || (columnFromListItem.Name.Equals("Name") && columnFromListItem.Default))
					{
						flag3 = true;
						break;
					}
				}
			}
			bool flag4 = true;
			foreach (ExchangeColumnHeader exchangeColumnHeader in this.list.AvailableColumns)
			{
				if (exchangeColumnHeader.Default != this.displayedColumnsListView.Items.ContainsKey(exchangeColumnHeader.Name) || (exchangeColumnHeader.Default && exchangeColumnHeader.DefaultDisplayIndex != this.displayedColumnsListView.Items.IndexOfKey(exchangeColumnHeader.Name)))
				{
					flag4 = false;
					break;
				}
			}
			bool enabled = flag && this.list.AllowColumnReorder && this.displayedColumnsListView.SelectedIndices[0] != 0 && this.GetColumnFromListItem(this.displayedColumnsListView.SelectedItems[0]).IsReorderable && this.GetColumnFromListItem(this.displayedColumnsListView.Items[this.displayedColumnsListView.SelectedIndices[0] - 1]).IsReorderable;
			bool enabled2 = flag && this.list.AllowColumnReorder && this.displayedColumnsListView.SelectedIndices[0] != this.displayedColumnsListView.Items.Count - 1 && this.GetColumnFromListItem(this.displayedColumnsListView.SelectedItems[0]).IsReorderable && this.GetColumnFromListItem(this.displayedColumnsListView.Items[this.displayedColumnsListView.SelectedIndices[0] + 1]).IsReorderable;
			this.restoreDefaultsButton.Enabled = !flag4;
			this.moveUpButton.Enabled = enabled;
			this.moveDownButton.Enabled = enabled2;
			this.removeButton.Enabled = (flag2 && !flag3);
			this.addButton.Enabled = (this.availableColumnsListView.SelectedItems.Count != 0);
			this.addAllButton.Enabled = (this.availableColumnsListView.Items.Count != 0);
		}

		private void availableColumnsListView_ItemActivate(object sender, EventArgs e)
		{
			this.addButton.PerformClick();
		}

		private void displayedColumnsListView_ItemActivate(object sender, EventArgs e)
		{
			if (this.removeButton.Enabled)
			{
				this.removeButton.PerformClick();
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			proposedSize.Width = base.Width;
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

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
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.availableColumnLabel = new Label();
			this.displayedColumnsLabel = new Label();
			this.availableColumnsListView = new ListView();
			this.availableColumnsHeader = new ColumnHeader();
			this.displayedColumnsListView = new ListView();
			this.displayedColumnsHeader = new ColumnHeader();
			this.addButton = new ExchangeButton();
			this.addAllButton = new ExchangeButton();
			this.moveUpButton = new ExchangeButton();
			this.moveDownButton = new ExchangeButton();
			this.restoreDefaultsButton = new ExchangeButton();
			this.removeButton = new ExchangeButton();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			this.tableLayoutPanel.AutoLayout = false;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 7;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.ContainerType = ContainerType.Control;
			this.tableLayoutPanel.Controls.Add(this.availableColumnLabel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.displayedColumnsLabel, 4, 0);
			this.tableLayoutPanel.Controls.Add(this.availableColumnsListView, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.displayedColumnsListView, 4, 1);
			this.tableLayoutPanel.Controls.Add(this.addButton, 2, 1);
			this.tableLayoutPanel.Controls.Add(this.addAllButton, 2, 3);
			this.tableLayoutPanel.Controls.Add(this.moveUpButton, 6, 1);
			this.tableLayoutPanel.Controls.Add(this.moveDownButton, 6, 2);
			this.tableLayoutPanel.Controls.Add(this.restoreDefaultsButton, 6, 3);
			this.tableLayoutPanel.Controls.Add(this.removeButton, 2, 2);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(13, 12, 16, 12);
			this.tableLayoutPanel.RowCount = 5;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(570, 312);
			this.tableLayoutPanel.TabIndex = 0;
			this.availableColumnLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.availableColumnLabel.AutoSize = true;
			this.availableColumnLabel.Location = new Point(13, 12);
			this.availableColumnLabel.Margin = new Padding(0);
			this.availableColumnLabel.Name = "availableColumnLabel";
			this.availableColumnLabel.Size = new Size(155, 13);
			this.availableColumnLabel.TabIndex = 0;
			this.availableColumnLabel.Text = "labelAvailableColumns";
			this.displayedColumnsLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.displayedColumnsLabel.AutoSize = true;
			this.displayedColumnsLabel.Location = new Point(268, 12);
			this.displayedColumnsLabel.Margin = new Padding(0);
			this.displayedColumnsLabel.Name = "displayedColumnsLabel";
			this.displayedColumnsLabel.Size = new Size(155, 13);
			this.displayedColumnsLabel.TabIndex = 5;
			this.displayedColumnsLabel.Text = "labelDisplayedColumns";
			this.availableColumnsListView.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.availableColumnsListView.Columns.AddRange(new ColumnHeader[]
			{
				this.availableColumnsHeader
			});
			this.availableColumnsListView.HeaderStyle = ColumnHeaderStyle.None;
			this.availableColumnsListView.HideSelection = false;
			this.availableColumnsListView.Location = new Point(16, 28);
			this.availableColumnsListView.Margin = new Padding(3, 3, 0, 0);
			this.availableColumnsListView.Name = "availableColumnsListView";
			this.tableLayoutPanel.SetRowSpan(this.availableColumnsListView, 4);
			this.availableColumnsListView.Size = new Size(152, 272);
			this.availableColumnsListView.Sorting = SortOrder.Ascending;
			this.availableColumnsListView.TabIndex = 1;
			this.availableColumnsListView.UseCompatibleStateImageBehavior = false;
			this.availableColumnsListView.View = View.Details;
			this.availableColumnsListView.ItemActivate += this.availableColumnsListView_ItemActivate;
			this.availableColumnsListView.SelectedIndexChanged += this.availableColumnsListView_SelectedIndexChanged;
			this.availableColumnsHeader.Text = "DisplayName";
			this.availableColumnsHeader.Width = 148;
			this.displayedColumnsListView.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.displayedColumnsListView.Columns.AddRange(new ColumnHeader[]
			{
				this.displayedColumnsHeader
			});
			this.displayedColumnsListView.HeaderStyle = ColumnHeaderStyle.None;
			this.displayedColumnsListView.HideSelection = false;
			this.displayedColumnsListView.Location = new Point(271, 28);
			this.displayedColumnsListView.Margin = new Padding(3, 3, 0, 0);
			this.displayedColumnsListView.Name = "displayedColumnsListView";
			this.tableLayoutPanel.SetRowSpan(this.displayedColumnsListView, 4);
			this.displayedColumnsListView.Size = new Size(152, 272);
			this.displayedColumnsListView.TabIndex = 6;
			this.displayedColumnsListView.UseCompatibleStateImageBehavior = false;
			this.displayedColumnsListView.View = View.Details;
			this.displayedColumnsListView.ItemActivate += this.displayedColumnsListView_ItemActivate;
			this.displayedColumnsListView.SelectedIndexChanged += this.displayedColumnsListView_SelectedIndexChanged;
			this.displayedColumnsHeader.Text = "DisplayName";
			this.displayedColumnsHeader.Width = 148;
			this.addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.addButton.AutoSize = true;
			this.addButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.addButton.Enabled = false;
			this.addButton.Location = new Point(176, 28);
			this.addButton.Margin = new Padding(3, 3, 0, 0);
			this.addButton.MinimumSize = new Size(75, 23);
			this.addButton.Name = "addButton";
			this.addButton.Size = new Size(87, 23);
			this.addButton.TabIndex = 2;
			this.addButton.Text = "buttonAdd";
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += this.addButton_Click;
			this.addAllButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.addAllButton.AutoSize = true;
			this.addAllButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.addAllButton.Location = new Point(176, 98);
			this.addAllButton.Margin = new Padding(3, 12, 0, 0);
			this.addAllButton.MinimumSize = new Size(75, 23);
			this.addAllButton.Name = "addAllButton";
			this.addAllButton.Size = new Size(87, 23);
			this.addAllButton.TabIndex = 4;
			this.addAllButton.Text = "buttonAddAll";
			this.addAllButton.UseVisualStyleBackColor = true;
			this.addAllButton.Click += this.addAllButton_Click;
			this.moveUpButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.moveUpButton.AutoSize = true;
			this.moveUpButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.moveUpButton.Enabled = false;
			this.moveUpButton.Location = new Point(431, 28);
			this.moveUpButton.Margin = new Padding(3, 3, 0, 0);
			this.moveUpButton.Name = "moveUpButton";
			this.moveUpButton.Size = new Size(123, 23);
			this.moveUpButton.TabIndex = 7;
			this.moveUpButton.Text = "buttonMoveUp";
			this.moveUpButton.UseVisualStyleBackColor = true;
			this.moveUpButton.Click += this.moveUpButton_Click;
			this.moveDownButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.moveDownButton.AutoSize = true;
			this.moveDownButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.moveDownButton.Enabled = false;
			this.moveDownButton.Location = new Point(431, 63);
			this.moveDownButton.Margin = new Padding(3, 12, 0, 0);
			this.moveDownButton.Name = "moveDownButton";
			this.moveDownButton.Size = new Size(123, 23);
			this.moveDownButton.TabIndex = 8;
			this.moveDownButton.Text = "buttonMoveDown";
			this.moveDownButton.UseVisualStyleBackColor = true;
			this.moveDownButton.Click += this.moveDownButton_Click;
			this.restoreDefaultsButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.restoreDefaultsButton.AutoSize = true;
			this.restoreDefaultsButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.restoreDefaultsButton.Location = new Point(431, 98);
			this.restoreDefaultsButton.Margin = new Padding(3, 12, 0, 0);
			this.restoreDefaultsButton.Name = "restoreDefaultsButton";
			this.restoreDefaultsButton.Size = new Size(123, 23);
			this.restoreDefaultsButton.TabIndex = 9;
			this.restoreDefaultsButton.Text = "buttonRestoreDefaults";
			this.restoreDefaultsButton.UseVisualStyleBackColor = true;
			this.restoreDefaultsButton.Click += this.restoreDefaultsButton_Click;
			this.removeButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.removeButton.AutoSize = true;
			this.removeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.removeButton.Enabled = false;
			this.removeButton.Location = new Point(176, 63);
			this.removeButton.Margin = new Padding(3, 12, 0, 0);
			this.removeButton.MinimumSize = new Size(75, 23);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new Size(87, 23);
			this.removeButton.TabIndex = 3;
			this.removeButton.Text = "buttonRemove";
			this.removeButton.UseVisualStyleBackColor = true;
			this.removeButton.Click += this.removeButton_Click;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			this.MinimumSize = new Size(570, 316);
			base.Name = "ColumnPickerDialog";
			base.Size = new Size(570, 316);
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private const int columnNameSubItem = 1;

		private const int columnDisplayIndexSubItem = 2;

		private DataListView list;

		private IContainer components;

		private AutoTableLayoutPanel tableLayoutPanel;

		private Label availableColumnLabel;

		private Label displayedColumnsLabel;

		private ListView availableColumnsListView;

		private ListView displayedColumnsListView;

		private ExchangeButton addButton;

		private ExchangeButton removeButton;

		private ExchangeButton addAllButton;

		private ExchangeButton moveUpButton;

		private ExchangeButton moveDownButton;

		private ExchangeButton restoreDefaultsButton;

		private ColumnHeader availableColumnsHeader;

		private ColumnHeader displayedColumnsHeader;

		private class DisplayIndexItemComparer : IComparer<ListViewItem>
		{
			public int Compare(ListViewItem x, ListViewItem y)
			{
				int result = 0;
				if (x == null && y != null)
				{
					result = -1;
				}
				else if (x != null && y == null)
				{
					result = 1;
				}
				else if (x != null && y != null)
				{
					int num = int.Parse(x.SubItems[2].Text);
					int num2 = int.Parse(y.SubItems[2].Text);
					result = num - num2;
				}
				return result;
			}
		}
	}
}
