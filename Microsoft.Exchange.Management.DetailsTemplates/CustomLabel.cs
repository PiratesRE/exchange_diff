using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[Designer(typeof(CustomControlDesigner))]
	internal sealed class CustomLabel : Label, IDetailsTemplateControlBound
	{
		public override string Text
		{
			get
			{
				return this.detailsTemplateControl.Text;
			}
			set
			{
				base.Text = value;
				this.detailsTemplateControl.Text = value;
				this.Refresh();
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
				this.detailsTemplateControl = (value as LabelControl);
				base.Text = this.detailsTemplateControl.Text;
			}
		}

		private LabelControl detailsTemplateControl = new LabelControl();
	}
}
