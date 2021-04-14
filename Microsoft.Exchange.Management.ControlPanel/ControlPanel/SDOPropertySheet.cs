using System;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("SDOPropertySheet", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(DetailsPane))]
	public class SDOPropertySheet : PropertyPageSheet
	{
		public SDOPropertySheet()
		{
			base.UseWarningPanel = true;
			base.HasSaveMethod = false;
			this.CssClass = "DetailsProperties";
			base.ViewModel = "SDOViewModel";
		}
	}
}
