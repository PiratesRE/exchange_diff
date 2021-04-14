using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class EnumToRadioButtonAdapter : ExchangeUserControl
	{
		public EnumToRadioButtonAdapter()
		{
			base.Name = "EnumToRadioButtonAdapter";
		}

		public void AddRadioButtonToEnumEntry(RadioButton radioButton, Enum enumValue)
		{
			this.radioButtonToEnumDictionary[radioButton] = enumValue;
			this.enumToRadioButtonDictionary[enumValue] = radioButton;
			radioButton.CheckedChanged += this.RadioButton_CheckedChanged;
		}

		private void RadioButton_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = (RadioButton)sender;
			if (radioButton.Checked)
			{
				this.SelectedValue = this.radioButtonToEnumDictionary[radioButton];
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Enum SelectedValue
		{
			get
			{
				return this.selectedValue;
			}
			set
			{
				if (!object.Equals(value, this.SelectedValue))
				{
					RadioButton radioButton = (this.SelectedValue != null) ? this.enumToRadioButtonDictionary[this.SelectedValue] : null;
					this.selectedValue = value;
					RadioButton radioButton2 = (this.SelectedValue != null) ? this.enumToRadioButtonDictionary[this.SelectedValue] : null;
					if (radioButton2 != null)
					{
						radioButton2.Checked = true;
					}
					if (radioButton != null)
					{
						radioButton.Checked = false;
					}
					this.OnSelectedValueChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[EnumToRadioButtonAdapter.EventSelectedValueChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler SelectedValueChanged
		{
			add
			{
				base.Events.AddHandler(EnumToRadioButtonAdapter.EventSelectedValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(EnumToRadioButtonAdapter.EventSelectedValueChanged, value);
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "SelectedValue";
			}
		}

		private static readonly object EventSelectedValueChanged = new object();

		private Enum selectedValue;

		protected Dictionary<RadioButton, Enum> radioButtonToEnumDictionary = new Dictionary<RadioButton, Enum>();

		private Dictionary<Enum, RadioButton> enumToRadioButtonDictionary = new Dictionary<Enum, RadioButton>();
	}
}
