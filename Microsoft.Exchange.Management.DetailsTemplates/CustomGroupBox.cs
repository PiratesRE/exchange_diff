using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[Designer(typeof(CustomControlDesigner))]
	internal sealed class CustomGroupBox : GroupBox, IDetailsTemplateControlBound
	{
		public override string Text
		{
			get
			{
				return this.detailsTemplateControl.Text;
			}
			set
			{
				this.detailsTemplateControl.Text = value;
				base.Text = value;
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
				this.detailsTemplateControl = (value as GroupboxControl);
				base.Text = this.detailsTemplateControl.Text;
			}
		}

		private GroupboxControl detailsTemplateControl = new GroupboxControl();
	}
}
