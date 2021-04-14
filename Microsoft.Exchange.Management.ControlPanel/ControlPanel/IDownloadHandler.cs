using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal interface IDownloadHandler
	{
		PowerShellResults ProcessRequest(HttpContext context);
	}
}
