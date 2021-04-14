using System;
using System.Web;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.MapiHttp
{
	public class MapiHttpBackendRehydrationModule : BackendRehydrationModule
	{
		protected override bool UseAuthIdentifierCache
		{
			get
			{
				return true;
			}
		}

		protected override bool NeedTokenRehydration(HttpContext context)
		{
			if (string.Compare(context.Request.RequestType, "POST", true) == 0)
			{
				string contentType = context.Request.ContentType;
				if (!string.IsNullOrEmpty(contentType) && (string.Compare(contentType, "application/mapi-http", true) == 0 || string.Compare(contentType, "application/octet-stream", true) == 0))
				{
					string[] values = context.Request.Headers.GetValues("X-RequestType");
					if (values != null && values.Length == 1)
					{
						return MapiHttpHandler.NeedTokenRehydration(values[0].Trim());
					}
				}
			}
			return true;
		}
	}
}
