using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CheckedTextBox : CaptionedTextBox
	{
		[DefaultValue("")]
		public string DefaultText
		{
			get
			{
				return this.defaultText;
			}
			set
			{
				this.defaultText = value;
			}
		}

		public CheckedTextBox()
		{
			this.InitializeComponent();
			this.exchangeTextBox.DataBindings.Add("ReadOnly", this.checkboxCaption, "Checked", true, DataSourceUpdateMode.Never).Format += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = !(bool)e.Value;
			};
			this.checkboxCaption.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				if (this.checkboxCaption.Checked)
				{
					this.exchangeTextBox.Focus();
					this.Text = this.DefaultText;
					if (!string.IsNullOrEmpty(this.Text))
					{
						this.exchangeTextBox.SelectionStart = this.Text.Length;
						return;
					}
				}
				else
				{
					this.Text = string.Empty;
				}
			};
			base.BindingContextChanged += delegate(object param0, EventArgs param1)
			{
				this.DataBindings_CollectionChanged(null, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			};
			base.DataBindings.CollectionChanged += this.DataBindings_CollectionChanged;
			base.Name = "CheckedTextBox";
		}

		private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			Binding binding = (Binding)e.Element;
			switch (e.Action)
			{
			case CollectionChangeAction.Add:
				if (binding.PropertyName == "Text")
				{
					this.SetUncheckedValueByPropertyType(binding);
					binding.Parse += this.Binding_Parse;
					binding.Format += this.Binding_Format;
					return;
				}
				break;
			case CollectionChangeAction.Remove:
				break;
			case CollectionChangeAction.Refresh:
				binding = base.DataBindings["Text"];
				if (binding != null)
				{
					binding.Parse -= this.Binding_Parse;
					binding.Format -= this.Binding_Format;
					this.SetUncheckedValueByPropertyType(binding);
					binding.Parse += this.Binding_Parse;
					binding.Format += this.Binding_Format;
				}
				break;
			default:
				return;
			}
		}

		private void Binding_Parse(object s, ConvertEventArgs e)
		{
			if (this.checkboxCaption.Checked && string.IsNullOrEmpty(this.exchangeTextBox.Text))
			{
				throw new ArgumentException(Strings.ValueCanNotBeEmpty);
			}
		}

		private void Binding_Format(object s, ConvertEventArgs e)
		{
			this.checkboxCaption.Checked = (e.Value != null && !string.IsNullOrEmpty(e.Value.ToString()) && (this.uncheckedValue == null || !this.uncheckedValue.Equals(e.Value.ToString())));
			if (!this.checkboxCaption.Checked)
			{
				e.Value = string.Empty;
			}
		}

		private void SetUncheckedValueByPropertyType(Binding binding)
		{
			if (binding.BindingManagerBase != null)
			{
				PropertyDescriptorCollection itemProperties = binding.BindingManagerBase.GetItemProperties();
				PropertyDescriptor propertyDescriptor = itemProperties.Find(binding.BindingMemberInfo.BindingField, true);
				if (propertyDescriptor != null)
				{
					FilterValuePropertyDescriptor filterValuePropertyDescriptor = propertyDescriptor as FilterValuePropertyDescriptor;
					Type type = (filterValuePropertyDescriptor != null) ? filterValuePropertyDescriptor.ValuePropertyType : propertyDescriptor.PropertyType;
					if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Unlimited<>) || (type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsGenericType && type.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Unlimited<>))))
					{
						this.uncheckedValue = this.unlimitedString;
						return;
					}
					this.uncheckedValue = null;
				}
			}
		}

		private void InitializeComponent()
		{
			this.checkboxCaption = new AutoHeightCheckBox();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.Controls.Add(this.checkboxCaption, 0, 0);
			this.checkboxCaption.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.checkboxCaption.Checked = true;
			this.checkboxCaption.CheckState = CheckState.Checked;
			this.checkboxCaption.Location = new Point(0, 0);
			this.checkboxCaption.Margin = new Padding(3, 2, 0, 1);
			this.checkboxCaption.Name = "checkboxCaption";
			this.checkboxCaption.TabIndex = 0;
			this.checkboxCaption.UseVisualStyleBackColor = true;
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue("")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Caption
		{
			get
			{
				return this.checkboxCaption.Text;
			}
			set
			{
				this.checkboxCaption.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue("")]
		[Browsable(true)]
		public override string Text
		{
			get
			{
				if (this.checkboxCaption == null || !this.checkboxCaption.Checked)
				{
					return this.uncheckedValue;
				}
				return base.Text;
			}
			set
			{
				base.Text = value;
				this.OnTextChanged(EventArgs.Empty);
			}
		}

		private string defaultText = string.Empty;

		private string unlimitedString = Strings.UnlimitedString;

		private string uncheckedValue;

		private AutoHeightCheckBox checkboxCaption;
	}
}
