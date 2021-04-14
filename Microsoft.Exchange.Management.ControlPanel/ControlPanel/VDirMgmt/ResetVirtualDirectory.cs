using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel.VDirMgmt
{
	public class ResetVirtualDirectory : BaseForm
	{
		protected override void OnInit(EventArgs e)
		{
			string text = HttpContext.Current.Request.QueryString["schema"];
			if (string.Equals(text, "ResetOABVirtualDirectory") || string.Equals(text, "ResetOWAVirtualDirectory") || string.Equals(text, "ResetECPVirtualDirectory") || string.Equals(text, "ResetEASVirtualDirectory") || string.Equals(text, "ResetEWSVirtualDirectory") || string.Equals(text, "ResetAutoDiscVirtualDirectory") || string.Equals(text, "ResetPowerShellVirtualDirectory"))
			{
				this.resetVDir.ServiceUrl = new WebServiceReference(string.Format("{0}DDI/DDIService.svc?schema={1}", EcpUrl.EcpVDirForStaticResource, text));
				base.OnInit(e);
				return;
			}
			throw new BadQueryParameterException(string.Format("schema {0} is not reconized.", text));
		}

		protected PropertyPageSheet resetVDir;
	}
}
