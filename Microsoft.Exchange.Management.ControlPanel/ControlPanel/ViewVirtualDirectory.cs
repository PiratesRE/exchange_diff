using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ViewVirtualDirectory : EcpContentPage
	{
		protected override void OnInit(EventArgs e)
		{
			string text = HttpContext.Current.Request.QueryString["schema"];
			if (string.Equals(text, "OWAVirtualDirectory") || string.Equals(text, "ECPVirtualDirectory") || string.Equals(text, "AutoDiscVirtualDirectory") || string.Equals(text, "EASVirtualDirectory") || string.Equals(text, "EWSVirtualDirectory") || string.Equals(text, "OABVirtualDirectory") || string.Equals(text, "PowershellVirtualDirectory"))
			{
				this.vdirSDO.ServiceUrl = new WebServiceReference(string.Format("{0}DDI/DDIService.svc?schema={1}&workflow=GetForSDO", EcpUrl.EcpVDirForStaticResource, text));
				base.OnInit(e);
				return;
			}
			throw new BadQueryParameterException(string.Format("schema {0} is not reconized.", text));
		}

		protected PropertyPageSheet vdirSDO;
	}
}
