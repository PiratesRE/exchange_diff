using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[Designer(typeof(CustomControlDesigner))]
	internal sealed class CustomComboBox : ComboBox, IDetailsTemplateControlBound
	{
		[TypeConverter(typeof(MAPITypeConverter))]
		public string AttributeName
		{
			get
			{
				return this.detailsTemplateControl.AttributeName;
			}
			set
			{
				this.detailsTemplateControl.AttributeName = value;
			}
		}

		[Browsable(false)]
		public DetailsTemplateControl DetailsTemplateControl
		{
			get
			{
				return this.detailsTemplateControl;
			}
			set
			{
				this.detailsTemplateControl = (value as MultiValuedDropdownControl);
			}
		}

		private MultiValuedDropdownControl detailsTemplateControl = new MultiValuedDropdownControl();
	}
}
