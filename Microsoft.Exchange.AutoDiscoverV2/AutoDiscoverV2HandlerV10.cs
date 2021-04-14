using System;
using System.Net;
using System.Web;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	internal class AutoDiscoverV2HandlerV10 : AutoDiscoverV2HandlerBase, IHttpHandler
	{
		public override bool Validate(HttpContextBase context)
		{
			if (context.Request.Url.AbsolutePath.EndsWith("v1.0", StringComparison.OrdinalIgnoreCase))
			{
				throw AutoDiscoverResponseException.BadRequest(HttpStatusCode.BadRequest.ToString(), "Id segment is missing in the URL", null);
			}
			return true;
		}

		public override string GetEmailAddressFromUrl(HttpContextBase context)
		{
			int num = context.Request.Url.AbsolutePath.LastIndexOf('/');
			return context.Request.Url.AbsolutePath.Substring(num + 1);
		}
	}
}
