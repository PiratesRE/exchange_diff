using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class DownloadHandler : IHttpHandler
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
			this.SetCommonHeadersOnResponse(context.Response);
			ServerDownloadManager.Instance.Value.ProcessDownloadRequest(context);
			context.Response.End();
		}

		private void SetCommonHeadersOnResponse(HttpResponse response)
		{
			response.Clear();
			response.BufferOutput = false;
			response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1.0));
		}
	}
}
