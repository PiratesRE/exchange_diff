using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(WizardForm))]
	[ClientScriptResource("EditPolicy", "Microsoft.Exchange.Management.ControlPanel.Client.DLPPolicy.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class EditPolicy : BaseForm
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			PropertyPageSheet propertyPageSheet = (PropertyPageSheet)base.ContentPanel.FindControl("policyPropertyPage");
			ListView listView = (ListView)propertyPageSheet.Sections["rules"].FindControl("rulesListView");
			descriptor.AddComponentProperty("RulesListView", listView.ClientID, true);
			descriptor.AddComponentProperty("RulesDataSourceRefreshMethod", this.rulesdatasource.RefreshWebServiceMethod.ClientID);
			descriptor.AddProperty("DLPPolicyParameterName", "DlpPolicy");
			base.BuildScriptDescriptor(descriptor);
		}

		protected WebServiceListSource rulesdatasource;
	}
}
