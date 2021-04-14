using System;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class Return401RequestHandler : IHttpHandler
	{
		internal Return401RequestHandler()
		{
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.StatusCode = 401;
		}
	}
}
