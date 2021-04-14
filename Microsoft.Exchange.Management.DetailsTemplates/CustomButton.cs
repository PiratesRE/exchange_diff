using System;
using System.ComponentModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[Designer(typeof(CustomControlDesigner))]
	internal sealed class CustomButton : ExchangeButton, IDetailsTemplateControlBound
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
				this.detailsTemplateControl = (value as ButtonControl);
				this.Text = this.detailsTemplateControl.Text;
			}
		}

		private ButtonControl detailsTemplateControl = new ButtonControl();
	}
}
