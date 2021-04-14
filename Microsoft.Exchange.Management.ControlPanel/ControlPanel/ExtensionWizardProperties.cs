using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Extension.js")]
	[RequiredScript(typeof(Properties))]
	public class ExtensionWizardProperties : Properties
	{
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			base.UseSetObject = false;
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "ExtensionWizardProperties";
			return scriptDescriptor;
		}
	}
}
