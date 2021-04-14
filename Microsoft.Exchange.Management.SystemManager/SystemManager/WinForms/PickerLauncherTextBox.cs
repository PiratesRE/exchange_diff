using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class PickerLauncherTextBox : PickerTextBoxBase
	{
		public PickerLauncherTextBox()
		{
			base.UpdateBrowseButtonState();
			base.Name = "PickerLauncherTextBox";
			base.TextChanged += this.PickerLauncherTextBox_TextChanged;
		}

		private void PickerLauncherTextBox_TextChanged(object sender, EventArgs e)
		{
			if (!base.TextBoxReadOnly)
			{
				this.SelectedValue = (string.IsNullOrEmpty(this.Text) ? null : this.Text);
			}
		}

		protected override void OnBrowseButtonClick(CancelEventArgs e)
		{
			base.OnBrowseButtonClick(e);
			bool allowMultiSelect = this.Picker.AllowMultiSelect;
			this.Picker.AllowMultiSelect = false;
			try
			{
				if (this.Picker.ShowDialog() == DialogResult.OK)
				{
					if (!string.IsNullOrEmpty(this.ValueMember))
					{
						base.NotifyExposedPropertyIsModified();
						this.SelectedValue = this.Picker.SelectedObjects.Rows[0][this.ValueMember];
					}
				}
				else
				{
					e.Cancel = true;
				}
			}
			catch (ObjectPickerException ex)
			{
				base.ShowError(ex.Message);
			}
			this.Picker.AllowMultiSelect = allowMultiSelect;
		}

		[DefaultValue(null)]
		public ObjectPickerBase Picker
		{
			get
			{
				return this.picker;
			}
			set
			{
				if (value != this.Picker)
				{
					if (this.Picker != null)
					{
						base.Components.Remove(this.Picker);
					}
					this.picker = value;
					if (this.Picker != null)
					{
						base.Components.Add(this.Picker);
					}
					if (this.resolver != null)
					{
						this.resolver.ResolveObjectIdsCompleted -= this.resolver_ResolveObjectIdsCompleted;
					}
					if (value != null && value is ObjectPicker)
					{
						this.resolver = new ObjectResolver(value as ObjectPicker);
						this.resolver.ResolveObjectIdsCompleted += this.resolver_ResolveObjectIdsCompleted;
					}
					else
					{
						this.resolver = null;
					}
					base.UpdateBrowseButtonState();
				}
			}
		}

		private void resolver_ResolveObjectIdsCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			base.UpdateBrowseButtonState();
			ValueToDisplayObjectConverter valueToDisplayObjectConverter = (this.DisplayMemberConverter != null) ? this.DisplayMemberConverter : new ToStringValueToDisplayObjectConverter();
			object obj = null;
			if (this.resolver.ResolvedObjects != null && this.resolver.ResolvedObjects.Rows.Count > 0)
			{
				obj = this.resolver.ResolvedObjects.Rows[0][this.DisplayMember];
			}
			obj = (obj ?? this.SelectedValue);
			if (obj != null)
			{
				obj = valueToDisplayObjectConverter.Convert(obj);
			}
			this.Text = ((obj == null) ? string.Empty : obj.ToString());
		}

		protected override bool ButtonAvailable()
		{
			return base.ButtonAvailable() && this.Picker != null && (this.resolver == null || !this.resolver.IsResolving);
		}

		[DefaultValue(null)]
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

		[DefaultValue(null)]
		internal ADPropertyDefinition ValueMemberPropertyDefinition
		{
			get
			{
				return this.valueMemberPropertyDefinition;
			}
			set
			{
				this.valueMemberPropertyDefinition = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ValueToDisplayObjectConverter ValueMemberConverter
		{
			get
			{
				return this.valueMemberConverter;
			}
			set
			{
				this.valueMemberConverter = value;
			}
		}

		[DefaultValue(null)]
		public ValueToDisplayObjectConverter DisplayMemberConverter
		{
			get
			{
				return this.displayMemberConverter;
			}
			set
			{
				this.displayMemberConverter = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedValue
		{
			get
			{
				return this.selectedValue;
			}
			set
			{
				if ((this.selectedValue == null) ? (value != null) : (!this.selectedValue.Equals(value)))
				{
					this.selectedValue = value;
					this.picker.InputObject = value;
					this.UpdateDisplay();
					this.OnSelectedValueChanged(EventArgs.Empty);
				}
			}
		}

		internal void UpdateDisplay()
		{
			if (!base.TextBoxReadOnly || this.DisplayMember == null || string.Equals(this.ValueMember, this.DisplayMember))
			{
				ValueToDisplayObjectConverter valueToDisplayObjectConverter = (this.ValueMemberConverter != null) ? this.ValueMemberConverter : new ToStringValueToDisplayObjectConverter();
				this.Text = ((this.SelectedValue == null || DBNull.Value == this.SelectedValue) ? string.Empty : valueToDisplayObjectConverter.Convert(this.SelectedValue).ToString());
				return;
			}
			if (this.SelectedValue != null && this.Picker != null)
			{
				object obj = null;
				ValueToDisplayObjectConverter valueToDisplayObjectConverter2 = (this.DisplayMemberConverter != null) ? this.DisplayMemberConverter : new ToStringValueToDisplayObjectConverter();
				if (this.Picker.SelectedObjects != null && this.Picker.SelectedObjects.Rows.Count > 0)
				{
					obj = this.Picker.SelectedObjects.Rows[0][this.DisplayMember];
				}
				else if (this.resolver != null && (this.SelectedValue is ADObjectId || this.ValueMemberPropertyDefinition != null) && !string.IsNullOrEmpty(this.SelectedValue.ToString()))
				{
					ADPropertyDefinition property = this.ValueMemberPropertyDefinition ?? ADObjectSchema.Id;
					this.resolver.ResolveObjectIds(property, new List<object>(new object[]
					{
						this.SelectedValue
					}));
					base.UpdateBrowseButtonState();
				}
				else
				{
					obj = this.SelectedValue;
				}
				this.Text = ((obj == null) ? string.Empty : valueToDisplayObjectConverter2.Convert(obj).ToString());
				return;
			}
			this.Text = string.Empty;
		}

		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[PickerLauncherTextBox.EventSelectedValueChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler SelectedValueChanged
		{
			add
			{
				base.Events.AddHandler(PickerLauncherTextBox.EventSelectedValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(PickerLauncherTextBox.EventSelectedValueChanged, value);
			}
		}

		[DefaultValue(null)]
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

		internal string DisplayedText
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text = ((value == null) ? string.Empty : value);
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "SelectedValue";
			}
		}

		private ObjectPickerBase picker;

		private ObjectResolver resolver;

		private string displayMember;

		private string valueMember;

		private object selectedValue;

		private ValueToDisplayObjectConverter valueMemberConverter;

		private ValueToDisplayObjectConverter displayMemberConverter;

		private ADPropertyDefinition valueMemberPropertyDefinition;

		private static readonly object EventSelectedValueChanged = new object();
	}
}
