using System;
using System.Web;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class EacHttpContext : IEacHttpContext
	{
		private EacHttpContext()
		{
		}

		public static IEacHttpContext Instance { get; internal set; } = new EacHttpContext();

		public ShouldContinueContext ShouldContinueContext
		{
			get
			{
				return HttpContext.Current.Items["ShouldContinueContext"] as ShouldContinueContext;
			}
			set
			{
				HttpContext.Current.Items["ShouldContinueContext"] = value;
			}
		}

		public bool PostHydrationActionPresent
		{
			get
			{
				HttpCookie httpCookie = HttpContext.Current.Request.Cookies["PostHydrationAction"];
				return httpCookie != null && httpCookie.Value == "1";
			}
		}
	}
}
