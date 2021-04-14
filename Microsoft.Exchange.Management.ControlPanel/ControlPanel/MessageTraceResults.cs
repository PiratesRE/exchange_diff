using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(WizardForm))]
	[ClientScriptResource("MessageTraceResults", "Microsoft.Exchange.Management.ControlPanel.Client.MessageTrace.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class MessageTraceResults : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ViewDetailsCommand viewDetailsCommand = (ViewDetailsCommand)this.listViewSearchResults.Commands.FindCommandByName("viewdetails");
			if (viewDetailsCommand != null)
			{
				viewDetailsCommand.NavigateUrl = EcpUrl.ProcessUrl(EcpUrl.EcpVDirForStaticResource + "MessageTrace/MessageTraceDetails.aspx");
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddComponentProperty("DataSource", this.messageTraceDataSource);
			descriptor.AddComponentProperty("RefreshMethod", this.messageTraceDataSource.RefreshWebServiceMethod.ClientID, true);
			base.BuildScriptDescriptor(descriptor);
		}

		protected WebServiceListSource messageTraceDataSource;

		protected ListView listViewSearchResults;
	}
}
