using System;
using System.Web;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal class HttpCallContext : IContextPlugin
	{
		public static IContextPlugin Singleton
		{
			get
			{
				return HttpCallContext.singleton;
			}
		}

		public Guid? LocalId
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null && httpContext.Items != null)
				{
					return (Guid?)httpContext.Items["MSExchangeLocalId"];
				}
				return null;
			}
			set
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null && httpContext.Items != null)
				{
					if (value != null)
					{
						httpContext.Items["MSExchangeLocalId"] = value;
						return;
					}
					httpContext.Items.Remove("MSExchangeLocalId");
				}
			}
		}

		public bool IsContextPresent
		{
			get
			{
				return HttpCallContext.IsHttpContextValid(HttpContext.Current);
			}
		}

		public void SetId()
		{
		}

		public bool CheckId()
		{
			return HttpCallContext.IsHttpContextValid(HttpContext.Current);
		}

		public void Clear()
		{
			HttpContext httpContext = HttpContext.Current;
			if (HttpCallContext.IsHttpContextValid(httpContext))
			{
				httpContext.Items.Remove("MSExchangeLocalId");
				httpContext.Items.Remove("SingleContextIdKey");
			}
		}

		private static bool IsHttpContextValid(HttpContext current)
		{
			return current != null && current.Items != null;
		}

		private static IContextPlugin singleton = new HttpCallContext();
	}
}
