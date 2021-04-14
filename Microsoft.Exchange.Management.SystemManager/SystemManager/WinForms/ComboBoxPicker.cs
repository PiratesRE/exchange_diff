using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ComboBoxPicker : ExchangeUserControl
	{
		public ComboBoxPicker()
		{
			this.InitializeComponent();
			this.objectTable = new DataTable("objectTable");
			this.objectTable.Columns.Add("ValueColumn", typeof(object));
			this.objectTable.Columns.Add("DisplayColumn", typeof(string));
			this.comboBox.ValueMember = "ValueColumn";
			this.comboBox.DisplayMember = "DisplayColumn";
			this.objectTable.DefaultView.Sort = "DisplayColumn";
			this.comboBox.DataSource = this.objectTable.DefaultView;
			this.comboBox.SelectedIndexChanged += this.comboBox_SelectedIndexChanged;
		}

		private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.suspendChangeNotification)
			{
				this.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		private void DataTableLoader_RefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.suspendChangeNotification = true;
			object selectedValue = this.SelectedValue;
			DataTable table = (sender as DataTableLoader).Table;
			this.objectTable.Rows.Clear();
			foreach (object obj in table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				DataRow dataRow2 = this.objectTable.NewRow();
				ConvertEventArgs convertEventArgs = new ConvertEventArgs(dataRow[this.ValueMember], dataRow[this.ValueMember].GetType());
				this.OnValueConvert(convertEventArgs);
				if (convertEventArgs.Value != null)
				{
					dataRow2["ValueColumn"] = convertEventArgs.Value;
					ValueToDisplayObjectConverter valueToDisplayObjectConverter = (this.ValueMemberConverter != null) ? this.ValueMemberConverter : new ToStringValueToDisplayObjectConverter();
					dataRow2["DisplayColumn"] = (string.IsNullOrEmpty(this.DisplayMember) ? valueToDisplayObjectConverter.Convert(dataRow2["ValueColumn"]) : valueToDisplayObjectConverter.Convert(dataRow[this.DisplayMember]));
					this.objectTable.Rows.Add(dataRow2);
				}
			}
			this.SelectedIndex = -1;
			this.SelectedValue = (this.delaySetSelectedValue ?? selectedValue);
			this.suspendChangeNotification = false;
		}

		private int GetIndexByValueColumn(object value)
		{
			int result = -1;
			DataView dataView = this.comboBox.DataSource as DataView;
			if (value != null && dataView != null && !string.IsNullOrEmpty(this.comboBox.ValueMember))
			{
				for (int i = 0; i < dataView.Count; i++)
				{
					object obj = dataView[i][this.comboBox.ValueMember];
					if (value is string)
					{
						if ((this.IsCaseSensitive && string.Equals(value as string, obj as string, StringComparison.InvariantCulture)) || (!this.IsCaseSensitive && string.Equals(value as string, obj as string, StringComparison.InvariantCultureIgnoreCase)))
						{
							result = i;
							break;
						}
					}
					else if (object.Equals(value, obj))
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		private void InitializeComponent()
		{
			this.comboBox = new ExchangeComboBox();
			this.tableLayoutPanel = new TableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.comboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox.Location = new Point(0, 0);
			this.comboBox.Margin = new Padding(0);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new Size(120, 21);
			this.comboBox.TabIndex = 0;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.comboBox, 0, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(120, 21);
			this.tableLayoutPanel.TabIndex = 1;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "ComboBoxPicker";
			base.Size = new Size(120, 21);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue(null)]
		public ObjectPicker Picker
		{
			get
			{
				return this.picker;
			}
			set
			{
				if (value != this.Picker && value != null)
				{
					if (this.Picker != null)
					{
						this.picker.DataTableLoader.RefreshCompleted -= this.DataTableLoader_RefreshCompleted;
					}
					this.picker = value;
					this.picker.AllowMultiSelect = true;
					this.picker.DataTableLoader.RefreshCompleted += this.DataTableLoader_RefreshCompleted;
					this.picker.PerformQuery(null, this.QueryString);
				}
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedValue
		{
			get
			{
				return this.delaySetSelectedValue ?? this.comboBox.SelectedValue;
			}
			set
			{
				if (this.picker.DataTableLoader.Refreshing)
				{
					this.delaySetSelectedValue = value;
					return;
				}
				this.SelectedIndex = this.GetIndexByValueColumn(value);
				this.comboBox.Refresh();
				this.delaySetSelectedValue = null;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ValueMember
		{
			get
			{
				return this.valueMember;
			}
			set
			{
				this.valueMember = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DisplayMember
		{
			get
			{
				return this.displayMember;
			}
			set
			{
				this.displayMember = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string QueryString
		{
			get
			{
				return this.queryString;
			}
			set
			{
				this.queryString = value;
			}
		}

		[DefaultValue(null)]
		public ValueToDisplayObjectConverter ValueMemberConverter
		{
			get
			{
				return this.converter;
			}
			set
			{
				this.converter = value;
			}
		}

		[DefaultValue(-1)]
		public int SelectedIndex
		{
			get
			{
				return this.comboBox.SelectedIndex;
			}
			set
			{
				this.comboBox.SelectedIndex = value;
			}
		}

		[DefaultValue(0)]
		public int ObjectCount
		{
			get
			{
				return this.objectTable.Rows.Count;
			}
		}

		[DefaultValue(false)]
		public bool IsCaseSensitive
		{
			get
			{
				return this.isCaseSensitive;
			}
			set
			{
				this.isCaseSensitive = value;
			}
		}

		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ComboBoxPicker.EventSelectedValueChanged];
			if (eventHandler != null && !this.suspendChangeNotification)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler SelectedValueChanged
		{
			add
			{
				base.Events.AddHandler(ComboBoxPicker.EventSelectedValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ComboBoxPicker.EventSelectedValueChanged, value);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (this.Picker != null)
			{
				this.Picker.DataTableLoader.RefreshCompleted -= this.DataTableLoader_RefreshCompleted;
			}
		}

		protected virtual void OnValueConvert(ConvertEventArgs e)
		{
			if (this.ValueConvert != null)
			{
				this.ValueConvert(this, e);
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "SelectedValue";
			}
		}

		private const string ValueColumn = "ValueColumn";

		private const string DisplayColumn = "DisplayColumn";

		private ObjectPicker picker;

		private ValueToDisplayObjectConverter converter;

		private string queryString;

		private string valueMember;

		private string displayMember;

		private ComboBox comboBox;

		private DataTable objectTable;

		private bool suspendChangeNotification;

		private bool isCaseSensitive;

		private TableLayoutPanel tableLayoutPanel;

		private object delaySetSelectedValue;

		private static readonly object EventSelectedValueChanged = new object();

		public ConvertEventHandler ValueConvert;
	}
}
