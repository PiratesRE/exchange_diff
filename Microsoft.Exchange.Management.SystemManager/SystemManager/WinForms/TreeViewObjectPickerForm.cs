using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class TreeViewObjectPickerForm : ExchangeForm, ISelectedObjectsProvider
	{
		public TreeViewObjectPickerForm()
		{
			this.InitializeComponent();
			base.Icon = Icons.ObjectPicker;
			this.fileToolStripMenuItem.Text = Strings.ObjectPickerFile;
			this.closeToolStripMenuItem.Text = Strings.ObjectPickerClose;
			this.helpButton.Text = Strings.Help;
			this.helpButton.Visible = false;
			this.cancelButton.Text = Strings.Cancel;
			this.okButton.Text = Strings.Ok;
			base.AcceptButton = this.okButton;
			this.closeToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				base.DialogResult = DialogResult.Cancel;
			};
		}

		public TreeViewObjectPickerForm(ObjectPicker objectPicker) : this()
		{
			this.ObjectPicker = objectPicker;
			DataTableLoader dataTableLoader = this.ObjectPicker.DataTableLoader;
			this.resultDataTable = dataTableLoader.Table.Clone();
			this.rootNodes = new BindingList<TreeViewObjectPickerForm.DataTreeNodeModel>();
			this.queryResults = dataTableLoader.Table;
			this.resultTreeView.LazyExpandAll = true;
			this.resultTreeView.ImageList = ObjectPicker.ObjectClassIconLibrary.SmallImageList;
			this.resultTreeView.DataSource = this.rootNodes;
			this.resultTreeView.NodePropertiesMapping.Add("Text", "Name");
			this.resultTreeView.NodePropertiesMapping.Add("ImageKey", "ImageKey");
			this.resultTreeView.NodePropertiesMapping.Add("SelectedImageKey", "ImageKey");
			this.resultTreeView.ChildRelation = "Children";
			base.Load += delegate(object param0, EventArgs param1)
			{
				this.RetriveChildrenNodes(null);
			};
			this.resultTreeView.BeforeExpand += this.resultTreeView_BeforeExpand;
			this.resultTreeView.AfterSelect += this.resultTreeView_AfterSelect;
		}

		[DefaultValue(null)]
		protected ObjectPicker ObjectPicker
		{
			get
			{
				return this.objectPicker;
			}
			set
			{
				this.objectPicker = value;
				this.Text = this.ObjectPicker.Caption;
				if (this.ObjectPicker.AllowMultiSelect)
				{
					ExTraceGlobals.DataFlowTracer.TraceDebug((long)this.GetHashCode(), "TreeViewObjectPickerForm only support single selection, ignore the AllowMultiSelect setting of the ObjectPicker");
				}
				this.ObjectPicker.DataTableLoader.RefreshCompleted += this.DataTableLoader_RefreshCompleted;
				this.ObjectPicker.DataTableLoader.ProgressChanged += this.DataTableLoader_ProgressChanged;
			}
		}

		public DataTable SelectedObjects
		{
			get
			{
				return this.selectedObjects;
			}
		}

		private void DataTableLoader_ProgressChanged(object sender, RefreshProgressChangedEventArgs e)
		{
			this.UpdateStatusLabelText();
		}

		private void DataTableLoader_RefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (ExceptionHelper.IsUICriticalException(e.Error))
			{
				throw new ObjectPickerException(e.Error.Message, e.Error);
			}
			if (!this.ObjectPicker.DataTableLoader.Refreshing)
			{
				if (e.Error != null && !(e.Error is RootObjectNotFoundException))
				{
					string message;
					if (e.Error is SearchTooLargeException)
					{
						message = Strings.SearchTooLargeRefineYourSearch(e.Error.Message);
					}
					else if (ExceptionHelper.IsWellknownExceptionFromServer(e.Error.InnerException))
					{
						message = e.Error.InnerException.Message;
					}
					else
					{
						message = e.Error.Message;
					}
					base.ShowError(message);
				}
				this.OnQueryCompleted(e);
				this.InProgress = this.ObjectPicker.DataTableLoader.Refreshing;
			}
		}

		private bool InProgress
		{
			get
			{
				return this.inProgress;
			}
			set
			{
				if (this.inProgress != value)
				{
					this.inProgress = value;
					this.loadProgressBar.Visible = this.InProgress;
					this.resultTreeView.UseWaitCursor = this.InProgress;
					if (this.InProgress)
					{
						this.loadStatusLabel.Text = Strings.Searching;
						this.Cursor = Cursors.AppStarting;
						return;
					}
					this.loadStatusLabel.Text = string.Empty;
					this.Cursor = Cursors.Default;
				}
			}
		}

		private void UpdateStatusLabelText()
		{
			this.loadStatusLabel.Text = Strings.ObjectsFound(this.queryResults.Rows.Count);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			this.selectedObjects = this.resultDataTable.Clone();
			TreeViewObjectPickerForm.DataTreeNodeModel dataTreeNodeModel = this.resultTreeView.SelectedObject as TreeViewObjectPickerForm.DataTreeNodeModel;
			if (dataTreeNodeModel != null)
			{
				this.selectedObjects.Rows.Add(dataTreeNodeModel.InnerDataRow.ItemArray);
			}
			this.ObjectPicker.DataTableLoader.CancelRefresh();
			this.queryResults.Clear();
		}

		private void resultTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (this.ObjectPicker.DataTableLoader.Refreshing)
			{
				e.Cancel = true;
				return;
			}
			DataTreeNode dataTreeNode = e.Node as DataTreeNode;
			TreeViewObjectPickerForm.DataTreeNodeModel dataTreeNodeModel = dataTreeNode.DataSource as TreeViewObjectPickerForm.DataTreeNodeModel;
			if (!dataTreeNodeModel.IsChildrenReady)
			{
				e.Cancel = true;
				this.RetriveChildrenNodes(dataTreeNode);
			}
		}

		private void RetriveChildrenNodes(DataTreeNode node)
		{
			this.nodeToExpand = node;
			object rootId = null;
			if (this.nodeToExpand != null)
			{
				rootId = (this.nodeToExpand.DataSource as TreeViewObjectPickerForm.DataTreeNodeModel).InnerDataRow;
			}
			this.ObjectPicker.PerformQuery(rootId, string.Empty);
			this.InProgress = true;
		}

		private void OnQueryCompleted(RunWorkerCompletedEventArgs e)
		{
			this.resultTreeView.Select();
			TreeViewObjectPickerForm.DataTreeNodeModel parent = (this.nodeToExpand != null) ? (this.nodeToExpand.DataSource as TreeViewObjectPickerForm.DataTreeNodeModel) : null;
			this.CreateTreeNodes(parent);
			if (this.nodeToExpand != null)
			{
				this.nodeToExpand.Expand();
				return;
			}
			if (this.resultTreeView.Nodes.Count > 0)
			{
				this.resultTreeView.SelectedNode = this.resultTreeView.Nodes[0];
				this.resultTreeView.SelectedNode.Expand();
			}
		}

		private void CreateTreeNodes(TreeViewObjectPickerForm.DataTreeNodeModel parent)
		{
			this.resultDataTable.Merge(this.queryResults);
			BindingList<TreeViewObjectPickerForm.DataTreeNodeModel> bindingList = (parent == null) ? this.rootNodes : parent.Children;
			foreach (object obj in this.queryResults.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				DataRow dataRow2 = this.resultDataTable.Rows.Find(dataRow[this.queryResults.PrimaryKey[0]]);
				if (this.ObjectPicker.ObjectPickerProfile == null || !(this.ObjectPicker.ObjectPickerProfile.Scope is DataRow) || !object.Equals(dataRow2[this.queryResults.PrimaryKey[0].ColumnName] as ADObjectId, (this.ObjectPicker.ObjectPickerProfile.Scope as DataRow)[this.queryResults.PrimaryKey[0].ColumnName]))
				{
					bindingList.Add(new TreeViewObjectPickerForm.DataTreeNodeModel(dataRow2, this.ObjectPicker));
				}
			}
			if (parent != null)
			{
				parent.IsChildrenReady = true;
			}
		}

		private void resultTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (!this.ObjectPicker.CanSelectRootObject && this.resultTreeView.SelectedNode.Parent == null)
			{
				this.okButton.Enabled = false;
				this.selectedCountLabel.Text = Strings.CannotSelectRootObject;
				return;
			}
			int num = (this.resultTreeView.SelectedNode != null) ? 1 : 0;
			this.okButton.Enabled = (num == 1);
			this.selectedCountLabel.Text = Strings.ObjectsSelected(num);
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled)
			{
				ExchangeHelpService.ShowHelpFromHelpTopicId(this, this.ObjectPicker.HelpTopic);
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		private DataTable queryResults;

		private DataTable resultDataTable;

		private BindingList<TreeViewObjectPickerForm.DataTreeNodeModel> rootNodes;

		private ObjectPicker objectPicker;

		private DataTable selectedObjects;

		private bool inProgress;

		private DataTreeNode nodeToExpand;

		internal class DataTreeNodeModel
		{
			public DataTreeNodeModel(DataRow dataRow, ObjectPicker objectPicker)
			{
				this.InnerDataRow = dataRow;
				this.ObjectPicker = objectPicker;
			}

			public DataRow InnerDataRow { get; private set; }

			public ObjectPicker ObjectPicker { get; private set; }

			public string Name
			{
				get
				{
					string result = string.Empty;
					if (this.InnerDataRow.Table.Columns.Contains("Type") && string.Compare((string)this.InnerDataRow["Type"], "Domain", true, CultureInfo.InvariantCulture) == 0 && this.InnerDataRow["CanonicalName"] != DBNull.Value)
					{
						result = this.InnerDataRow["CanonicalName"].ToString().TrimEnd(new char[]
						{
							'/'
						});
					}
					else
					{
						result = this.InnerDataRow[this.ObjectPicker.NameProperty].ToString();
					}
					return result;
				}
			}

			public object ImageKey
			{
				get
				{
					return this.InnerDataRow[this.ObjectPicker.ImageProperty];
				}
			}

			public BindingList<TreeViewObjectPickerForm.DataTreeNodeModel> Children
			{
				get
				{
					return this.children;
				}
			}

			public bool IsChildrenReady { get; set; }

			private BindingList<TreeViewObjectPickerForm.DataTreeNodeModel> children = new BindingList<TreeViewObjectPickerForm.DataTreeNodeModel>();
		}
	}
}
