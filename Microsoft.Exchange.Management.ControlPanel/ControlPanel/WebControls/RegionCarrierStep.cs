using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.SMSProperties.js")]
	public class RegionCarrierStep : WizardStep
	{
		public RegionCarrierStep()
		{
			base.ClientClassName = "RegionCarrierStep";
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.AddComponentProperty("Properties", base.FindPropertiesParent().ClientID);
			return scriptDescriptor;
		}
	}
}
