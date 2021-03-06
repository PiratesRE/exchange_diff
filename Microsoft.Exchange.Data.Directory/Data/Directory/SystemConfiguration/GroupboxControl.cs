using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal sealed class GroupboxControl : DetailsTemplateControl
	{
		public string Text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (this.m_Text != value)
				{
					DetailsTemplateControl.ValidateText(value, DetailsTemplateControl.TextLengths.Label);
					this.m_Text = value;
					base.NotifyPropertyChanged("Text");
				}
			}
		}

		internal override DetailsTemplateControl.ControlTypes GetControlType()
		{
			return DetailsTemplateControl.ControlTypes.Groupbox;
		}

		public override string ToString()
		{
			return "Group Box";
		}
	}
}
