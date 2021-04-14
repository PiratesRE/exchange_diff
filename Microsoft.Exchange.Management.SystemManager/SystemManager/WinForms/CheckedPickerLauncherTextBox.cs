using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CheckedPickerLauncherTextBox : ExchangeUserControl, IFormatModeProvider, IBindableComponent, IComponent, IDisposable
	{
		public CheckedPickerLauncherTextBox()
		{
			this.InitializeComponent();
			this.checkBox.CheckedChanged += this.checkBox_CheckedChanged;
			base.DataBindings.CollectionChanged += this.DataBindings_CollectionChanged;
			this.pickerLauncherTextBox.Validating += this.pickerLauncherTextBox_Validating;
			this.pickerLauncherTextBox.FormatModeChanged += delegate(object param0, EventArgs param1)
			{
				this.OnFormatModeChanged(EventArgs.Empty);
			};
		}

		private void pickerLauncherTextBox_Validating(object sender, CancelEventArgs e)
		{
			this.RaiseValidatingEvent();
		}

		private void RaiseValidatingEvent()
		{
			this.OnValidating(new CancelEventArgs());
		}

		[DefaultValue(true)]
		public bool TextBoxReadOnly
		{
			get
			{
				return this.pickerLauncherTextBox.TextBoxReadOnly;
			}
			set
			{
				this.pickerLauncherTextBox.InitTextBoxReadOnly(value, this, this.ExposedPropertyName);
			}
		}

		private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			Binding binding = (Binding)e.Element;
			CollectionChangeAction action = e.Action;
			if (action != CollectionChangeAction.Add)
			{
				return;
			}
			if (binding.PropertyName == this.ExposedPropertyName)
			{
				binding.DataSourceNullValue = null;
				(binding.DataSource as BindingSource).DataSourceChanged += delegate(object param0, EventArgs param1)
				{
					if (!this.checkBox.Checked)
					{
						this.pickerLauncherTextBox.SelectedValue = null;
					}
					this.pickerLauncherTextBox.UpdateDisplay();
				};
			}
		}

		private void checkBox_CheckedChanged(object sender, EventArgs e)
		{
			bool @checked = this.checkBox.Checked;
			this.pickerLauncherTextBox.Enabled = @checked;
			this.OnCheckedChanged(EventArgs.Empty);
			if (!@checked || !CheckedPickerLauncherTextBox.isInitializedValueOfSelectedValue(this.pickerLauncherTextBox.SelectedValue))
			{
				this.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		private void pickerLauncherTextBox_SelectedValueChanged(object sender, EventArgs e)
		{
			if (this.Checked && !CheckedPickerLauncherTextBox.isInitializedValueOfSelectedValue(this.pickerLauncherTextBox.SelectedValue))
			{
				this.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		public override Control ErrorProviderAnchor
		{
			get
			{
				return this.pickerLauncherTextBox;
			}
		}

		protected override UIValidationError[] GetValidationErrors()
		{
			List<UIValidationError> list = new List<UIValidationError>();
			if (this.checkBox.Checked && CheckedPickerLauncherTextBox.isInitializedValueOfSelectedValue(this.SelectedValue))
			{
				UIValidationError item = new UIValidationError(Strings.SelectValueErrorMessage, this.pickerLauncherTextBox);
				list.Add(item);
			}
			return list.ToArray();
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.checkBox = new AutoHeightCheckBox();
			this.pickerLauncherTextBox = new PickerLauncherTextBox();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.checkBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.pickerLauncherTextBox, 1, 1);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(0, 0, 16, 0);
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(397, 46);
			this.tableLayoutPanel.TabIndex = 0;
			this.checkBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.checkBox, 2);
			this.checkBox.Location = new Point(3, 0);
			this.checkBox.Margin = new Padding(3, 0, 0, 0);
			this.checkBox.Name = "checkBox";
			this.checkBox.Size = new Size(378, 17);
			this.checkBox.TabIndex = 0;
			this.checkBox.Text = "checkBox";
			this.checkBox.UseVisualStyleBackColor = true;
			this.pickerLauncherTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.pickerLauncherTextBox.AutoSize = true;
			this.pickerLauncherTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pickerLauncherTextBox.Enabled = false;
			this.pickerLauncherTextBox.Location = new Point(16, 23);
			this.pickerLauncherTextBox.Margin = new Padding(0, 6, 0, 0);
			this.pickerLauncherTextBox.Name = "pickerLauncherTextBox";
			this.pickerLauncherTextBox.Size = new Size(365, 23);
			this.pickerLauncherTextBox.TabIndex = 1;
			this.pickerLauncherTextBox.SelectedValueChanged += this.pickerLauncherTextBox_SelectedValueChanged;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "CheckedPickerLauncherTextBox";
			base.Size = new Size(397, 46);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Caption
		{
			get
			{
				return this.checkBox.Text;
			}
			set
			{
				this.checkBox.Text = value;
			}
		}

		[DefaultValue(false)]
		public bool Checked
		{
			get
			{
				return this.checkBox.Checked;
			}
			set
			{
				this.checkBox.Checked = value;
			}
		}

		[DefaultValue(null)]
		public ObjectPickerBase Picker
		{
			get
			{
				return this.pickerLauncherTextBox.Picker;
			}
			set
			{
				this.pickerLauncherTextBox.Picker = value;
			}
		}

		[DefaultValue(null)]
		public object SelectedValue
		{
			get
			{
				if (this.checkBox.Checked)
				{
					return this.pickerLauncherTextBox.SelectedValue;
				}
				return null;
			}
			set
			{
				if (value != this.pickerLauncherTextBox.SelectedValue)
				{
					this.suspendChangeNotification = true;
					this.checkBox.Checked = !CheckedPickerLauncherTextBox.isInitializedValueOfSelectedValue(value);
					if (this.checkBox.Checked)
					{
						this.pickerLauncherTextBox.SelectedValue = value;
					}
					this.suspendChangeNotification = false;
					this.OnSelectedValueChanged(EventArgs.Empty);
				}
			}
		}

		private static bool isInitializedValueOfSelectedValue(object value)
		{
			if (!(value is string))
			{
				return null == value;
			}
			return string.IsNullOrEmpty((string)value);
		}

		[DefaultValue(null)]
		public string ValueMember
		{
			get
			{
				return this.pickerLauncherTextBox.ValueMember;
			}
			set
			{
				this.pickerLauncherTextBox.ValueMember = value;
			}
		}

		[DefaultValue(null)]
		internal ADPropertyDefinition ValueMemberPropertyDefinition
		{
			get
			{
				return this.pickerLauncherTextBox.ValueMemberPropertyDefinition;
			}
			set
			{
				this.pickerLauncherTextBox.ValueMemberPropertyDefinition = value;
			}
		}

		[DefaultValue(null)]
		public ValueToDisplayObjectConverter ValueMemberConverter
		{
			get
			{
				return this.pickerLauncherTextBox.ValueMemberConverter;
			}
			set
			{
				this.pickerLauncherTextBox.ValueMemberConverter = value;
			}
		}

		[DefaultValue(null)]
		public string DisplayMember
		{
			get
			{
				return this.pickerLauncherTextBox.DisplayMember;
			}
			set
			{
				this.pickerLauncherTextBox.DisplayMember = value;
			}
		}

		[DefaultValue(null)]
		public ValueToDisplayObjectConverter DisplayMemberConverter
		{
			get
			{
				return this.pickerLauncherTextBox.DisplayMemberConverter;
			}
			set
			{
				this.pickerLauncherTextBox.DisplayMemberConverter = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LocalizedString ErrorObjectType
		{
			get
			{
				return this.errorObjectType;
			}
			set
			{
				this.errorObjectType = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ButtonText
		{
			get
			{
				return this.pickerLauncherTextBox.ButtonText;
			}
			set
			{
				this.pickerLauncherTextBox.ButtonText = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanBrowse
		{
			get
			{
				return this.pickerLauncherTextBox.CanBrowse;
			}
			set
			{
				this.pickerLauncherTextBox.CanBrowse = value;
				this.checkBox.Enabled = value;
			}
		}

		protected virtual void OnCheckedChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[CheckedPickerLauncherTextBox.EventCheckedChanged];
			if (eventHandler != null && !this.suspendChangeNotification)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler CheckedChanged
		{
			add
			{
				base.Events.AddHandler(CheckedPickerLauncherTextBox.EventCheckedChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(CheckedPickerLauncherTextBox.EventCheckedChanged, value);
			}
		}

		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			base.UpdateError();
			EventHandler eventHandler = (EventHandler)base.Events[CheckedPickerLauncherTextBox.EventSelectedValueChanged];
			if (eventHandler != null && !this.suspendChangeNotification)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler SelectedValueChanged
		{
			add
			{
				base.Events.AddHandler(CheckedPickerLauncherTextBox.EventSelectedValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(CheckedPickerLauncherTextBox.EventSelectedValueChanged, value);
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		public void ClearContent()
		{
			this.suspendChangeNotification = true;
			this.checkBox.Checked = false;
			this.pickerLauncherTextBox.SelectedValue = null;
			this.pickerLauncherTextBox.UpdateDisplay();
			this.suspendChangeNotification = false;
			this.OnSelectedValueChanged(EventArgs.Empty);
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "SelectedValue";
			}
		}

		[DefaultValue(0)]
		public DisplayFormatMode FormatMode
		{
			get
			{
				return this.pickerLauncherTextBox.FormatMode;
			}
			set
			{
				this.pickerLauncherTextBox.FormatMode = value;
			}
		}

		protected virtual void OnFormatModeChanged(EventArgs e)
		{
			if (this.FormatModeChanged != null)
			{
				this.FormatModeChanged(this, e);
			}
		}

		public event EventHandler FormatModeChanged;

		void IFormatModeProvider.add_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged += A_1;
		}

		void IFormatModeProvider.remove_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged -= A_1;
		}

		private AutoTableLayoutPanel tableLayoutPanel;

		private AutoHeightCheckBox checkBox;

		private PickerLauncherTextBox pickerLauncherTextBox;

		private bool suspendChangeNotification;

		private LocalizedString errorObjectType;

		private static readonly object EventCheckedChanged = new object();

		private static readonly object EventSelectedValueChanged = new object();
	}
}
