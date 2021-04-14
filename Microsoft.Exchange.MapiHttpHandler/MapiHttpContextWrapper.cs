using System;
using System.Web;

namespace Microsoft.Exchange.MapiHttp
{
	public class MapiHttpContextWrapper : HttpContextWrapper
	{
		public MapiHttpContextWrapper(HttpContext context) : base(context)
		{
		}

		public override HttpRequestBase Request
		{
			get
			{
				if (this.request == null)
				{
					this.request = base.Request;
				}
				return this.request;
			}
		}

		public override HttpResponseBase Response
		{
			get
			{
				if (this.response == null)
				{
					this.response = base.Response;
				}
				return this.response;
			}
		}

		public static MapiHttpContextWrapper GetWrapper(HttpContext context)
		{
			MapiHttpContextWrapper mapiHttpContextWrapper = context.Items["MapiHttpContextWrapper"] as MapiHttpContextWrapper;
			if (mapiHttpContextWrapper == null)
			{
				mapiHttpContextWrapper = new MapiHttpContextWrapper(context);
				context.Items["MapiHttpContextWrapper"] = mapiHttpContextWrapper;
			}
			return mapiHttpContextWrapper;
		}

		private const string ContextWrapperName = "MapiHttpContextWrapper";

		private HttpRequestBase request;

		private HttpResponseBase response;
	}
}
