using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("UMEnableExtensionStep", "Microsoft.Exchange.Management.ControlPanel.Client.UnifiedMessaging.js")]
	public class UMEnableExtensionStep : WizardStep
	{
		public UMEnableExtensionStep()
		{
			base.ClientClassName = "UMEnableExtensionStep";
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.AddComponentProperty("PropertyPage", base.FindPropertiesParent().ClientID);
			return scriptDescriptor;
		}
	}
}
