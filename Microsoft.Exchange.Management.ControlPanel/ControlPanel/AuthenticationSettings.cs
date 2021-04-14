using System;
using System.Web;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal struct AuthenticationSettings
	{
		public AuthenticationSettings(HttpContext context)
		{
			if (!context.Request.IsAuthenticated || Utility.IsResourceRequest(context.Request.Path))
			{
				this.Session = (context.IsLogoffRequest() ? LogoffSession.AnonymousSession : AnonymousSession.Instance);
				return;
			}
			RbacSettings rbacSettings = new RbacSettings(context);
			if (context.IsLogoffRequest())
			{
				this.Session = new LogoffSession(rbacSettings, context.User.Identity);
				return;
			}
			this.Session = rbacSettings.Session;
		}

		public readonly IRbacSession Session;
	}
}
