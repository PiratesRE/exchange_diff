using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[RequiredScript(typeof(WizardForm))]
	[ClientScriptResource("NewPolicyFromTemplate", "Microsoft.Exchange.Management.ControlPanel.Client.DLPPolicy.js")]
	public class NewPolicyFromTemplate : BaseForm
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddComponentProperty("ListView", this.complianceProgramListView.ClientID, true);
			descriptor.AddElementProperty("DistributionGroupPanel", this.pnlDistributionGroup.ClientID);
			descriptor.AddElementProperty("IncidentManagementPanel", this.pnlIncidentMailbox.ClientID);
			EcpSingleSelect ecpSingleSelect = (EcpSingleSelect)this.pnlIncidentMailbox.FindControl("chooseIncidentManagementBox");
			EcpSingleSelect ecpSingleSelect2 = (EcpSingleSelect)this.pnlDistributionGroup.FindControl("chooseDistributionGroup");
			descriptor.AddComponentProperty("IncidentMailboxPicker", ecpSingleSelect.ClientID, true);
			descriptor.AddComponentProperty("DistibutionGroupPicker", ecpSingleSelect2.ClientID, true);
			base.BuildScriptDescriptor(descriptor);
		}

		protected Microsoft.Exchange.Management.ControlPanel.WebControls.ListView complianceProgramListView;

		protected TextBox templateId;

		protected Panel pnlDistributionGroup;

		protected Panel pnlIncidentMailbox;
	}
}
