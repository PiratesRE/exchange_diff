using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class UploadHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			PowerShellResults dataContract = ServerUploadManager.Instance.Value.ProcessFileUploadRequest(context);
			context.Response.Clear();
			context.Response.ContentType = "text/html";
			context.Response.Write(HttpUtility.HtmlEncode(dataContract.ToJsonString(null)));
			context.Response.End();
		}
	}
}
