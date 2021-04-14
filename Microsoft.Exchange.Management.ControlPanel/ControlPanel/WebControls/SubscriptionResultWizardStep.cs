using System;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class SubscriptionResultWizardStep : ShowResultWizardStep
	{
		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "SubscriptionResultWizardStep";
			scriptDescriptor.AddProperty("FailedHelpLink", HelpUtil.BuildEhcHref(OptionsHelpId.AutoProvisionFailed.ToString()));
			scriptDescriptor.AddProperty("NewPopAccessible", LoginUtil.CheckUrlAccess("NewPopSubscription.aspx"));
			scriptDescriptor.AddProperty("NewImapAccessible", LoginUtil.CheckUrlAccess("NewImapSubscription.aspx"));
			return scriptDescriptor;
		}
	}
}
