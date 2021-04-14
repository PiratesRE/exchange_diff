using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FilterItem : ExchangeUserControl, ISupportInitialize
	{
		internal FilterItem(FilterNode node, IList<FilterablePropertyDescription> propertiesToFilter)
		{
			this.BeginInit();
			this.InitializeComponent();
			this.valueTypeEditors = new Dictionary<FilterablePropertyValueEditor, Control>();
			this.deleteButton.Image = IconLibrary.ToSmallBitmap(Icons.Remove);
			this.deleteButton.ToolTipText = Strings.RemoveExpression;
			List<FilterablePropertyDescription> list = new List<FilterablePropertyDescription>(propertiesToFilter.Count);
			for (int i = 0; i < propertiesToFilter.Count; i++)
			{
				list.Add(propertiesToFilter[i].SurfaceFilterablePropertyDescription ?? propertiesToFilter[i]);
			}
			list.Sort();
			this.FilterNode = node;
			this.surfaceNode = new FilterNode();
			this.SyncFilterNodeToSurfaceNode();
			this.FilterNode.FilterablePropertyDescriptionChanged += delegate(object param0, EventArgs param1)
			{
				this.SyncFilterNodeToSurfaceNode();
			};
			this.FilterNode.OperatorChanged += delegate(object param0, EventArgs param1)
			{
				this.SyncFilterNodeToSurfaceNode();
			};
			this.FilterNode.ValueChanged += delegate(object param0, EventArgs param1)
			{
				this.SyncFilterNodeToSurfaceNode();
			};
			this.surfaceNode.FilterablePropertyDescriptionChanged += delegate(object param0, EventArgs param1)
			{
				this.SyncSurfaceNodeToFilterNode();
			};
			this.surfaceNode.OperatorChanged += delegate(object param0, EventArgs param1)
			{
				this.SyncSurfaceNodeToFilterNode();
			};
			this.surfaceNode.ValueChanged += delegate(object param0, EventArgs param1)
			{
				this.SyncSurfaceNodeToFilterNode();
			};
			this.propertyBindingSource.DataSource = new BindingList<FilterablePropertyDescription>(list);
			this.BindingSource.DataSource = this.surfaceNode;
			this.surfaceNode.OperatorChanged += this.surfaceNode_OperatorChanged;
			this.EndInit();
		}

		private void SyncFilterNodeToSurfaceNode()
		{
			this.FilterNode.FilterablePropertyDescription.SurfaceFilterNodeSynchronizer.Synchronize(this.FilterNode, this.surfaceNode);
		}

		private void SyncSurfaceNodeToFilterNode()
		{
			this.surfaceNode.FilterablePropertyDescription.UnderlyingFilterNodeSynchronizer.Synchronize(this.surfaceNode, this.FilterNode);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.valueTypeEditors != null)
			{
				foreach (FilterablePropertyValueEditor key in this.valueTypeEditors.Keys)
				{
					this.valueTypeEditors[key].Dispose();
				}
				this.valueTypeEditors.Clear();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			base.SuspendLayout();
			this.tableLayoutPanel.SuspendLayout();
			this.bindingSource = new BindingSource();
			this.bindingSource.DataSource = typeof(FilterNode);
			this.propertyBindingSource = new BindingSource();
			this.propertyBindingSource.DataSource = typeof(List<FilterablePropertyDescription>);
			this.operatorBindingSource = new BindingSource();
			this.operatorBindingSource.DataSource = this.propertyBindingSource;
			this.operatorBindingSource.DataMember = "SupportedOperators";
			this.operatorBindingSource.ListChanged += this.operatorBindingSource_ListChanged;
			this.comboProperty = new ExchangeComboBox();
			this.comboProperty.Name = "comboProperty";
			this.comboProperty.FlatStyle = FlatStyle.System;
			this.comboProperty.Dock = DockStyle.Fill;
			this.comboProperty.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboProperty.DataSource = this.propertyBindingSource;
			this.comboProperty.DisplayMember = "DisplayName";
			Binding binding = this.comboProperty.DataBindings.Add("SelectedIndex", this.BindingSource, "FilterablePropertyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = this.comboProperty.Items.IndexOf(e.Value);
			};
			binding.Parse += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = this.comboProperty.SelectedItem;
			};
			this.comboProperty.TabIndex = 0;
			this.comboOperator = new ExchangeComboBox();
			this.comboOperator.Name = "comboOperator";
			this.comboOperator.FlatStyle = FlatStyle.System;
			this.comboOperator.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboOperator.Dock = DockStyle.Fill;
			this.comboOperator.DataSource = this.operatorBindingSource;
			this.comboOperator.DisplayMember = "Text";
			this.comboOperator.ValueMember = "Value";
			this.comboOperator.DataBindings.Add("SelectedValue", this.BindingSource, "Operator", true, DataSourceUpdateMode.OnPropertyChanged);
			this.comboOperator.TabIndex = 1;
			this.deleteButton = new ToolStripButton();
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.deleteButton.Click += this.deleteButton_Click;
			this.deleteButtonStrip = new TabbableToolStrip();
			this.deleteButtonStrip.BackColor = Color.Transparent;
			this.deleteButtonStrip.GripStyle = ToolStripGripStyle.Hidden;
			this.deleteButtonStrip.Name = "deleteButtonStrip";
			this.deleteButtonStrip.Items.Add(this.deleteButton);
			this.deleteButtonStrip.TabStop = true;
			this.deleteButtonStrip.TabIndex = 7;
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.ColumnCount = 4;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			this.tableLayoutPanel.Controls.Add(this.comboProperty, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.comboOperator, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.deleteButtonStrip, 3, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = Color.Transparent;
			base.Controls.Add(this.tableLayoutPanel);
			this.Dock = DockStyle.Top;
			base.Margin = new Padding(0);
			base.TabStop = true;
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		internal BindingSource BindingSource
		{
			get
			{
				return this.bindingSource;
			}
		}

		private void operatorBindingSource_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (!this.initializing && e.ListChangedType == ListChangedType.Reset && typeof(ObjectListSourceItem) == this.operatorBindingSource.Current.GetType())
			{
				PropertyFilterOperator propertyFilterOperator = (PropertyFilterOperator)((ObjectListSourceItem)this.operatorBindingSource[0]).Value;
				if (this.surfaceNode.Operator == propertyFilterOperator)
				{
					this.DeployValueControl();
					return;
				}
				this.surfaceNode.Operator = propertyFilterOperator;
			}
		}

		internal FilterNode FilterNode { get; private set; }

		private void surfaceNode_OperatorChanged(object sender, EventArgs e)
		{
			if (this.initializing)
			{
				return;
			}
			this.DeployValueControl();
		}

		private void DeployValueControl()
		{
			base.SuspendLayout();
			this.SetControlAsValueControl(this.GetValueEditorForFilterableProperty(this.surfaceNode.FilterablePropertyDescription, this.surfaceNode.Operator));
			base.ResumeLayout(true);
		}

		private void SetControlAsValueControl(Control newValueControl)
		{
			if (!object.ReferenceEquals(this.previousValueControl, newValueControl))
			{
				if (this.previousValueControl != null)
				{
					this.previousValueControl.Visible = false;
					this.previousValueControl.DataBindings.Clear();
					this.tableLayoutPanel.Controls.Remove(this.previousValueControl);
				}
				this.previousValueControl = newValueControl;
			}
			if (!this.tableLayoutPanel.Controls.Contains(newValueControl))
			{
				newValueControl.Visible = true;
				this.tableLayoutPanel.Controls.Add(newValueControl, 2, 0);
			}
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			base.Parent.Controls.Remove(this);
			base.Dispose();
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool result;
			if (keyData == Keys.Escape)
			{
				this.deleteButton_Click(this, EventArgs.Empty);
				result = true;
			}
			else
			{
				result = base.ProcessDialogKey(keyData);
			}
			return result;
		}

		private Control GetComboBoxEditor(FilterablePropertyDescription filterableProperty)
		{
			ExchangeComboBox exchangeComboBox;
			if (!this.valueTypeEditors.ContainsKey(FilterablePropertyValueEditor.ComboBox))
			{
				exchangeComboBox = new ExchangeComboBox();
				exchangeComboBox.Name = "comboBoxValue";
				exchangeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				exchangeComboBox.FlatStyle = FlatStyle.System;
				exchangeComboBox.Dock = DockStyle.Fill;
				exchangeComboBox.TabIndex = 2;
				this.valueTypeEditors.Add(FilterablePropertyValueEditor.ComboBox, exchangeComboBox);
			}
			else
			{
				exchangeComboBox = (this.valueTypeEditors[FilterablePropertyValueEditor.ComboBox] as ExchangeComboBox);
				exchangeComboBox.DataBindings.Clear();
			}
			ObjectListSource filterableListSource = filterableProperty.FilterableListSource;
			exchangeComboBox.DataSource = new ArrayList(filterableListSource.GetList());
			exchangeComboBox.DisplayMember = "Text";
			exchangeComboBox.ValueMember = "Value";
			Binding binding = new Binding("SelectedValue", this.BindingSource, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
			Type valueType = filterableProperty.ValueType;
			if (typeof(Enum).IsAssignableFrom(valueType))
			{
				binding.Format += delegate(object sender, ConvertEventArgs e)
				{
					if (e.Value != null && e.Value.GetType() != valueType)
					{
						e.Value = Enum.Parse(valueType, e.Value.ToString());
					}
				};
			}
			exchangeComboBox.DataBindings.Add(binding);
			return exchangeComboBox;
		}

		private Control GetTextBoxEditor(FilterablePropertyDescription filterableProperty)
		{
			ExchangeTextBox exchangeTextBox = this.GetDisabledTextBoxEditor(filterableProperty) as ExchangeTextBox;
			exchangeTextBox.Enabled = true;
			exchangeTextBox.FormatMode = filterableProperty.FormatMode;
			Binding binding = new Binding("Text", this.BindingSource, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.BindingComplete += this.TextboxEditor_BindingCompleted;
			exchangeTextBox.DataBindings.Add(binding);
			return exchangeTextBox;
		}

		private Control GetDisabledTextBoxEditor(FilterablePropertyDescription filterableProperty)
		{
			ExchangeTextBox exchangeTextBox;
			if (!this.valueTypeEditors.ContainsKey(FilterablePropertyValueEditor.DisabledTextBox))
			{
				exchangeTextBox = new ExchangeTextBox();
				exchangeTextBox.Name = "textBoxValue";
				exchangeTextBox.Dock = DockStyle.Fill;
				exchangeTextBox.TabIndex = 2;
				this.valueTypeEditors.Add(FilterablePropertyValueEditor.DisabledTextBox, exchangeTextBox);
			}
			else
			{
				exchangeTextBox = (this.valueTypeEditors[FilterablePropertyValueEditor.DisabledTextBox] as ExchangeTextBox);
				exchangeTextBox.DataBindings.Clear();
				exchangeTextBox.Clear();
			}
			exchangeTextBox.Enabled = false;
			return exchangeTextBox;
		}

		private void TextboxEditor_BindingCompleted(object sender, BindingCompleteEventArgs e)
		{
			e.Cancel = false;
			if (e.BindingCompleteState != BindingCompleteState.Success)
			{
				if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate)
				{
					((FilterNode)this.BindingSource.DataSource).ValueParsingError = e.ErrorText;
					e.Cancel = true;
					TextBox textBox = e.Binding.Control as TextBox;
					if (textBox != null && string.IsNullOrEmpty(textBox.Text))
					{
						((FilterNode)this.BindingSource.DataSource).Value = null;
						return;
					}
				}
			}
			else
			{
				((FilterNode)this.BindingSource.DataSource).ValueParsingError = null;
			}
		}

		private Control GetDateTimePickerEditor(FilterablePropertyDescription filterableProperty)
		{
			DateTimePicker dateTimePicker;
			if (!this.valueTypeEditors.ContainsKey(FilterablePropertyValueEditor.DateTimePicker))
			{
				dateTimePicker = new DateTimePicker();
				dateTimePicker.Name = "dateTimePicker";
				dateTimePicker.Dock = DockStyle.Fill;
				dateTimePicker.TabIndex = 2;
				this.valueTypeEditors.Add(FilterablePropertyValueEditor.DateTimePicker, dateTimePicker);
			}
			else
			{
				dateTimePicker = (this.valueTypeEditors[FilterablePropertyValueEditor.DateTimePicker] as DateTimePicker);
				dateTimePicker.DataBindings.Clear();
			}
			dateTimePicker.Format = DateTimePickerFormat.Custom;
			dateTimePicker.CustomFormat = string.Format("{0} - {1}", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
			Binding binding = new Binding("Value", this.BindingSource, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.NullValue = (DateTime)ExDateTime.Now;
			dateTimePicker.DataBindings.Add(binding);
			return dateTimePicker;
		}

		private Control GetPickerLauncherTextBoxEditor(FilterablePropertyDescription filterableProperty)
		{
			PickerLauncherTextBox pickerLauncherTextBox;
			if (!this.valueTypeEditors.ContainsKey(FilterablePropertyValueEditor.PickerLauncherTextBox))
			{
				pickerLauncherTextBox = new PickerLauncherTextBox();
				pickerLauncherTextBox.Name = "textboxObjectPicker";
				pickerLauncherTextBox.Dock = DockStyle.Fill;
				pickerLauncherTextBox.TabIndex = 2;
				this.valueTypeEditors.Add(FilterablePropertyValueEditor.PickerLauncherTextBox, pickerLauncherTextBox);
			}
			else
			{
				pickerLauncherTextBox = (this.valueTypeEditors[FilterablePropertyValueEditor.PickerLauncherTextBox] as PickerLauncherTextBox);
				pickerLauncherTextBox.DataBindings.Clear();
			}
			if (filterableProperty.ObjectPicker != null)
			{
				pickerLauncherTextBox.Picker = filterableProperty.ObjectPicker;
				pickerLauncherTextBox.ValueMember = (filterableProperty.ObjectPickerValueMember ?? "Name");
				pickerLauncherTextBox.DisplayMember = (filterableProperty.ObjectPickerDisplayMember ?? "Name");
				pickerLauncherTextBox.ValueMemberPropertyDefinition = filterableProperty.ObjectPickerValueMemberPropertyDefinition;
				pickerLauncherTextBox.SelectedValue = null;
				Binding binding = new Binding("SelectedValue", this.BindingSource, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
				binding.DataSourceNullValue = null;
				pickerLauncherTextBox.DataBindings.Add(binding);
			}
			return pickerLauncherTextBox;
		}

		private Control GetValueEditorForFilterableProperty(FilterablePropertyDescription filterableProperty, PropertyFilterOperator currentOperator)
		{
			Control result;
			switch (filterableProperty.GetPropertyFilterEditor(currentOperator))
			{
			case FilterablePropertyValueEditor.DisabledTextBox:
				result = this.GetDisabledTextBoxEditor(filterableProperty);
				break;
			case FilterablePropertyValueEditor.ComboBox:
				result = this.GetComboBoxEditor(filterableProperty);
				break;
			case FilterablePropertyValueEditor.DateTimePicker:
				result = this.GetDateTimePickerEditor(filterableProperty);
				break;
			case FilterablePropertyValueEditor.PickerLauncherTextBox:
				result = this.GetPickerLauncherTextBoxEditor(filterableProperty);
				break;
			default:
				result = this.GetTextBoxEditor(filterableProperty);
				break;
			}
			return result;
		}

		public void BeginInit()
		{
			this.initializing = true;
		}

		public void EndInit()
		{
			this.initializing = false;
			this.DeployValueControl();
		}

		private AutoTableLayoutPanel tableLayoutPanel;

		private ExchangeComboBox comboProperty;

		private ExchangeComboBox comboOperator;

		private TabbableToolStrip deleteButtonStrip;

		private ToolStripButton deleteButton;

		private BindingSource bindingSource;

		private BindingSource propertyBindingSource;

		private BindingSource operatorBindingSource;

		private FilterNode surfaceNode;

		private Control previousValueControl;

		private Dictionary<FilterablePropertyValueEditor, Control> valueTypeEditors;

		private bool initializing;
	}
}
