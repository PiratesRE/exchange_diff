using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DataListControl : BindableUserControl
	{
		[DefaultValue(false)]
		public bool SuspendDuplicateErrorMessage
		{
			get
			{
				return this.suspendDuplicateErrorMessage;
			}
			set
			{
				this.suspendDuplicateErrorMessage = value;
			}
		}

		public DataListControl()
		{
			this.InitializeComponent();
			base.BindingSource.DataSourceChanged += delegate(object param0, EventArgs param1)
			{
				this.OnDataSourceChanged(EventArgs.Empty);
			};
			base.BindingSource.ListChanged += delegate(object sender, ListChangedEventArgs e)
			{
				if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemChanged || e.ListChangedType == ListChangedType.ItemDeleted || (base.BindingSource.DataSource is SortedDataList && e.ListChangedType == ListChangedType.ItemMoved))
				{
					this.OnDataSourceChanged(EventArgs.Empty);
				}
			};
			this.DataListView.ContextMenu = this.dataListViewContextMenu;
			base.HandleCreated += delegate(object param0, EventArgs param1)
			{
				this.SetVisibilityForListLabel();
				this.SetVisibilityForPageLabel();
			};
			this.DataListView.HandleCreated += delegate(object param0, EventArgs param1)
			{
				this.AdjustColumWidthForDataListView();
			};
			this.DataListView.SizeChanged += delegate(object param0, EventArgs param1)
			{
				if (this.DataListView.IsHandleCreated)
				{
					this.AdjustColumWidthForDataListView();
				}
			};
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(389, 372);
			}
		}

		private void AdjustColumWidthForDataListView()
		{
			if (1 == this.DataListView.Columns.Count && this.DataListView.HeaderStyle != ColumnHeaderStyle.None && View.Details == this.DataListView.View && this.previousWidthOfDataListView != this.DataListView.Width)
			{
				this.previousWidthOfDataListView = this.DataListView.Width;
				if (this.DataListView.Width - this.DataListView.ClientRectangle.Width >= SystemInformation.VerticalScrollBarWidth)
				{
					this.DataListView.Columns[0].Width = this.DataListView.ClientRectangle.Width;
					return;
				}
				this.DataListView.Columns[0].Width = this.DataListView.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
			}
		}

		private void toolStrip_ItemAdded(object sender, ToolStripItemEventArgs e)
		{
			ToolStripDropDownItem toolStripDropDownItem = e.Item as ToolStripDropDownItem;
			if (toolStripDropDownItem != null)
			{
				ToolStripDropDownMenu toolStripDropDownMenu = toolStripDropDownItem.DropDown as ToolStripDropDownMenu;
				if (toolStripDropDownMenu != null)
				{
					toolStripDropDownMenu.ShowImageMargin = false;
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ExchangeTextBox EditTextBox
		{
			get
			{
				return this.exchangeTextBoxEdit;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataListView DataListView
		{
			get
			{
				return this.dataListView;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public IBindingList DataList
		{
			get
			{
				return base.BindingSource;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		protected Type ItemType
		{
			get
			{
				return ListBindingHelper.GetListItemType(this.DataSource);
			}
		}

		[DefaultValue(null)]
		public object DataSource
		{
			get
			{
				return this.originDataSource;
			}
			set
			{
				if (value != this.DataSource)
				{
					this.originDataSource = value;
					base.BindingSource.DataSource = this.CreateWrappedDataSource(value);
				}
			}
		}

		protected virtual object CreateWrappedDataSource(object value)
		{
			if (!this.IsSimpleList(value))
			{
				return value;
			}
			return new SortedDataList((IList)value);
		}

		private bool IsSimpleList(object list)
		{
			bool result = false;
			if (list is IList)
			{
				if (list is Array)
				{
					result = true;
				}
				else
				{
					Type type = list.GetType();
					if (type.IsGenericType)
					{
						Type genericTypeDefinition = type.GetGenericTypeDefinition();
						result = (Array.IndexOf<Type>(DataListControl.SimpleGenericListTypes, genericTypeDefinition) >= 0);
					}
				}
			}
			return result;
		}

		protected virtual void OnDataSourceChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[DataListControl.EventDataSourceChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler DataSourceChanged
		{
			add
			{
				base.Events.AddHandler(DataListControl.EventDataSourceChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataListControl.EventDataSourceChanged, value);
			}
		}

		[DefaultValue("")]
		public string ListLabelText
		{
			get
			{
				return this.listLabel.Text;
			}
			set
			{
				this.listLabel.Text = value;
				if (base.IsHandleCreated)
				{
					this.SetVisibilityForListLabel();
				}
			}
		}

		private void SetVisibilityForListLabel()
		{
			this.listLabel.Visible = !string.IsNullOrEmpty(this.listLabel.Text);
		}

		[DefaultValue("")]
		public string PageLabelText
		{
			get
			{
				return this.pageLabel.Text;
			}
			set
			{
				this.pageLabel.Text = value;
				if (base.IsHandleCreated)
				{
					this.SetVisibilityForPageLabel();
				}
			}
		}

		private void SetVisibilityForPageLabel()
		{
			this.pageLabel.Visible = !string.IsNullOrEmpty(this.pageLabel.Text);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripItemCollection ToolStripItems
		{
			get
			{
				return this.toolStrip.Items;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected TabbableToolStrip ToolStrip
		{
			get
			{
				return this.toolStrip;
			}
		}

		[DefaultValue(null)]
		public string IdentityProperty
		{
			get
			{
				return this.identityProperty;
			}
			set
			{
				this.identityProperty = value;
			}
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			this.DataListView.Focus();
		}

		public override void Refresh()
		{
			base.BindingSource.ResetBindings(false);
			base.Refresh();
		}

		public void ShowErrorAsync(string message)
		{
			base.BeginInvoke(new DataListControl.ShowErrorDelegate(base.ShowError), new object[]
			{
				message
			});
		}

		private void control_VisibleChanged(object sender, EventArgs e)
		{
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.listLabel.Margin = (this.pageLabel.Visible ? new Padding(0, 12, 0, 0) : new Padding(0));
			this.exchangeTextBoxEdit.Margin = ((this.pageLabel.Visible || this.listLabel.Visible) ? new Padding(3, 3, 0, 0) : new Padding(3, 0, 0, 0));
			this.toolStrip.Margin = ((!this.pageLabel.Visible && !this.listLabel.Visible && !this.exchangeTextBoxEdit.Visible) ? new Padding(3, 0, 0, 0) : new Padding(3, 3, 0, 0));
			if (!this.toolStrip.Visible)
			{
				this.dataListView.Margin = ((!this.pageLabel.Visible && !this.listLabel.Visible && !this.exchangeTextBoxEdit.Visible) ? new Padding(3, 0, 0, 0) : new Padding(3, 3, 0, 0));
			}
			else
			{
				this.dataListView.Margin = new Padding(3, 0, 0, 0);
			}
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected void InternalAddRange(ICollection collection)
		{
			int num = 0;
			try
			{
				foreach (object obj in collection)
				{
					int num2 = -1;
					if (!string.IsNullOrEmpty(this.IdentityProperty))
					{
						PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(obj)[this.IdentityProperty];
						object value = propertyDescriptor.GetValue(obj);
						for (int i = 0; i < this.DataList.Count; i++)
						{
							object component = this.DataList[i];
							object value2 = propertyDescriptor.GetValue(component);
							if (value.Equals(value2))
							{
								num2 = i;
								break;
							}
						}
					}
					else
					{
						num2 = this.DataList.IndexOf(obj);
					}
					num++;
					this.suppressFocusOnRow = (num != collection.Count);
					if (num2 < 0)
					{
						this.InternalAddValue(obj);
					}
					else
					{
						this.InternalEditValue(num2, obj);
					}
				}
			}
			finally
			{
				this.suppressFocusOnRow = false;
			}
		}

		protected virtual bool InternalAddValue(object value)
		{
			bool result = false;
			try
			{
				base.NotifyExposedPropertyIsModified();
				this.AddDataItemToDataSource(value);
				result = true;
				if (!this.suppressFocusOnRow)
				{
					this.DataListView.FocusOnDataRow(value);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (!(this.DataSource is MultiValuedPropertyBase) && !(this.DataSource is DataTable) && !(this.DataSource is DataView))
				{
					throw;
				}
				if (!this.SuspendDuplicateErrorMessage)
				{
					base.ShowError(ex.Message);
				}
			}
			catch (DataValidationException ex2)
			{
				base.ShowError(ex2.Message);
			}
			return result;
		}

		private void AddDataItemToDataSource(object item)
		{
			DataRow dataRow = item as DataRow;
			if (dataRow != null && this.Table != null)
			{
				try
				{
					this.Table.Rows.Add(dataRow);
					return;
				}
				catch (ConstraintException)
				{
					if (this.Table.Constraints.Count == 1 && this.Table.Constraints[0] is UniqueConstraint && (this.Table.Constraints[0] as UniqueConstraint).Columns != null && (this.Table.Constraints[0] as UniqueConstraint).Columns.Length > 0)
					{
						throw new InvalidOperationException(DataStrings.ErrorValueAlreadyPresent(dataRow[(this.Table.Constraints[0] as UniqueConstraint).Columns[0]].ToString()));
					}
					throw;
				}
			}
			this.DataList.Add(item);
		}

		protected bool InternalEditValue(int index, object value, bool isShowErrorAsync)
		{
			bool result = false;
			DataListControl.ShowErrorDelegate showErrorDelegate = isShowErrorAsync ? new DataListControl.ShowErrorDelegate(this.ShowErrorAsync) : new DataListControl.ShowErrorDelegate(base.ShowError);
			try
			{
				this.EditDataItemInDataSource(index, value);
				result = true;
				if (!this.suppressFocusOnRow)
				{
					this.DataListView.FocusOnDataRow(value);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (!(this.DataSource is MultiValuedPropertyBase) && !(this.DataSource is DataTable) && !(this.DataSource is DataView))
				{
					throw ex;
				}
				showErrorDelegate(ex.Message);
			}
			catch (DataValidationException ex2)
			{
				showErrorDelegate(ex2.Message);
			}
			return result;
		}

		private DataTable Table
		{
			get
			{
				DataTable dataTable = this.DataSource as DataTable;
				if (dataTable == null && this.DataSource is DataView)
				{
					dataTable = (this.DataSource as DataView).Table;
				}
				return dataTable;
			}
		}

		private void EditDataItemInDataSource(int index, object value)
		{
			if (value is DataRow && this.Table != null)
			{
				DataRow dataRow = value as DataRow;
				DataRow dest = this.Table.Rows[index];
				try
				{
					DataListControl.CopyDataRow(dataRow, dest);
					return;
				}
				catch (ConstraintException)
				{
					if (this.Table.Constraints.Count == 1 && this.Table.Constraints[0] is UniqueConstraint && (this.Table.Constraints[0] as UniqueConstraint).Columns != null && (this.Table.Constraints[0] as UniqueConstraint).Columns.Length > 0)
					{
						throw new InvalidOperationException(DataStrings.ErrorValueAlreadyPresent(dataRow[(this.Table.Constraints[0] as UniqueConstraint).Columns[0]].ToString()));
					}
					throw;
				}
			}
			this.DataList[index] = value;
		}

		protected virtual bool InternalEditValue(int index, object value)
		{
			base.NotifyExposedPropertyIsModified();
			return this.InternalEditValue(index, value, false);
		}

		protected static void CopyDataRow(DataRow source, DataRow dest)
		{
			dest.BeginEdit();
			for (int i = 0; i < source.Table.Columns.Count; i++)
			{
				if (!dest.Table.Columns[i].ReadOnly)
				{
					dest[i] = source[i];
				}
			}
			dest.EndEdit();
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "DataSource";
			}
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
			this.components = new Container();
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.pageLabel = new Label();
			this.listLabel = new Label();
			this.exchangeTextBoxEdit = new ExchangeTextBox();
			this.toolStrip = new TabbableToolStrip();
			this.dataListView = new DataListView();
			this.dataListViewContextMenu = new ContextMenu();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.pageLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.listLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.exchangeTextBoxEdit, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.toolStrip, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.dataListView, 0, 4);
			this.tableLayoutPanel1.Dock = DockStyle.Fill;
			this.tableLayoutPanel1.Location = new Point(0, 0);
			this.tableLayoutPanel1.Margin = new Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.Size = new Size(418, 396);
			this.tableLayoutPanel1.TabIndex = 4;
			this.pageLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.pageLabel.AutoSize = true;
			this.pageLabel.Location = new Point(0, 0);
			this.pageLabel.Margin = new Padding(0);
			this.pageLabel.Name = "pageLabel";
			this.pageLabel.Size = new Size(418, 13);
			this.pageLabel.TabIndex = 0;
			this.pageLabel.Tag = "";
			this.pageLabel.Visible = false;
			this.pageLabel.VisibleChanged += this.control_VisibleChanged;
			this.listLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.listLabel.AutoSize = true;
			this.listLabel.Location = new Point(0, 25);
			this.listLabel.Margin = new Padding(0, 12, 0, 0);
			this.listLabel.Name = "listLabel";
			this.listLabel.Size = new Size(418, 13);
			this.listLabel.TabIndex = 3;
			this.listLabel.Visible = false;
			this.listLabel.VisibleChanged += this.control_VisibleChanged;
			this.exchangeTextBoxEdit.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exchangeTextBoxEdit.Location = new Point(3, 41);
			this.exchangeTextBoxEdit.Margin = new Padding(3, 3, 0, 0);
			this.exchangeTextBoxEdit.Name = "exchangeTextBoxEdit";
			this.exchangeTextBoxEdit.Size = new Size(415, 20);
			this.exchangeTextBoxEdit.TabIndex = 1;
			this.exchangeTextBoxEdit.Visible = false;
			this.exchangeTextBoxEdit.VisibleChanged += this.control_VisibleChanged;
			this.toolStrip.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.toolStrip.BackColor = Color.Transparent;
			this.toolStrip.Dock = DockStyle.None;
			this.toolStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
			this.toolStrip.Location = new Point(3, 64);
			this.toolStrip.Margin = new Padding(3, 3, 0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new Size(415, 19);
			this.toolStrip.Stretch = true;
			this.toolStrip.TabIndex = 2;
			this.toolStrip.TabStop = true;
			this.toolStrip.Text = "toolStrip";
			this.toolStrip.ItemAdded += this.toolStrip_ItemAdded;
			this.toolStrip.VisibleChanged += this.control_VisibleChanged;
			this.dataListView.ContextMenu = this.dataListViewContextMenu;
			this.dataListView.DataSource = base.BindingSource;
			this.dataListView.Dock = DockStyle.Fill;
			this.dataListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.dataListView.Location = new Point(3, 83);
			this.dataListView.Margin = new Padding(3, 0, 0, 0);
			this.dataListView.Name = "dataListView";
			this.dataListView.Size = new Size(415, 313);
			this.dataListView.TabIndex = 4;
			this.dataListView.UseCompatibleStateImageBehavior = false;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "DataListControl";
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private static Type[] SimpleGenericListTypes = new Type[]
		{
			typeof(DagNetMultiValuedProperty<>),
			typeof(MultiValuedProperty<>),
			typeof(BindingList<>),
			typeof(List<>)
		};

		private bool suppressFocusOnRow;

		private bool suspendDuplicateErrorMessage;

		private int previousWidthOfDataListView;

		private object originDataSource;

		private static readonly object EventDataSourceChanged = new object();

		private string identityProperty;

		private IContainer components;

		private TableLayoutPanel tableLayoutPanel1;

		private Label pageLabel;

		private Label listLabel;

		private TabbableToolStrip toolStrip;

		private DataListView dataListView;

		private ContextMenu dataListViewContextMenu;

		private ExchangeTextBox exchangeTextBoxEdit;

		private delegate void ShowErrorDelegate(string message);
	}
}
