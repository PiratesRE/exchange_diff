using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal sealed class CheckboxControl : DetailsTemplateControl
	{
		public string AttributeName
		{
			get
			{
				return this.m_AttributeName;
			}
			set
			{
				DetailsTemplateControl.ValidateAttributeName(value, this.GetAttributeControlType(), base.GetType().Name);
				this.m_AttributeName = value;
			}
		}

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

		internal override DetailsTemplateControl.AttributeControlTypes GetAttributeControlType()
		{
			return DetailsTemplateControl.AttributeControlTypes.Checkbox;
		}

		internal override DetailsTemplateControl.ControlTypes GetControlType()
		{
			return DetailsTemplateControl.ControlTypes.Checkbox;
		}

		internal override bool ValidateAttribute(MAPIPropertiesDictionary propertiesDictionary)
		{
			return base.ValidateAttributeHelper(propertiesDictionary);
		}

		internal override DetailsTemplateControl.MapiPrefix GetMapiPrefix()
		{
			return DetailsTemplateControl.MapiPrefix.Checkbox;
		}

		public override string ToString()
		{
			return "Check Box";
		}
	}
}
