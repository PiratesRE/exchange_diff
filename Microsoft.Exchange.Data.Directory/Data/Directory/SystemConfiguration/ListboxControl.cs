using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal sealed class ListboxControl : DetailsTemplateControl
	{
		public ScrollBars ScrollBars
		{
			get
			{
				return this.scrollBars;
			}
			set
			{
				if (this.scrollBars != value)
				{
					this.scrollBars = value;
					base.NotifyPropertyChanged("ScrollBars");
				}
			}
		}

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

		internal override DetailsTemplateControl.ControlTypes GetControlType()
		{
			return DetailsTemplateControl.ControlTypes.Listbox;
		}

		internal override DetailsTemplateControl.AttributeControlTypes GetAttributeControlType()
		{
			return DetailsTemplateControl.AttributeControlTypes.Listbox;
		}

		internal override bool ValidateAttribute(MAPIPropertiesDictionary propertiesDictionary)
		{
			return base.ValidateAttributeHelper(propertiesDictionary);
		}

		internal override DetailsTemplateControl.ControlFlags GetControlFlags()
		{
			DetailsTemplateControl.ControlFlags originalFlags = this.OriginalFlags;
			bool flag = this.scrollBars == ScrollBars.Both || this.scrollBars == ScrollBars.Vertical;
			bool flag2 = this.scrollBars == ScrollBars.Both || this.scrollBars == ScrollBars.Horizontal;
			DetailsTemplateControl.SetBitField(!flag, DetailsTemplateControl.ControlFlags.ReadOnly, ref originalFlags);
			DetailsTemplateControl.SetBitField(!flag2, DetailsTemplateControl.ControlFlags.Multiline, ref originalFlags);
			return originalFlags;
		}

		internal ListboxControl(DetailsTemplateControl.ControlFlags controlFlags)
		{
			bool flag = (controlFlags & DetailsTemplateControl.ControlFlags.ReadOnly) == (DetailsTemplateControl.ControlFlags)0U;
			bool flag2 = (controlFlags & DetailsTemplateControl.ControlFlags.Multiline) == (DetailsTemplateControl.ControlFlags)0U;
			if (flag2 && flag)
			{
				this.scrollBars = ScrollBars.Both;
				return;
			}
			if (flag2)
			{
				this.scrollBars = ScrollBars.Horizontal;
				return;
			}
			if (flag)
			{
				this.scrollBars = ScrollBars.Vertical;
				return;
			}
			this.scrollBars = ScrollBars.None;
		}

		public ListboxControl()
		{
			this.m_Text = DetailsTemplateControl.NoTextString;
		}

		internal override DetailsTemplateControl.MapiPrefix GetMapiPrefix()
		{
			return DetailsTemplateControl.MapiPrefix.Listbox;
		}

		public override string ToString()
		{
			return "List Box";
		}

		private ScrollBars scrollBars;
	}
}
