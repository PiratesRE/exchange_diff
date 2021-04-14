using System;
using System.Security.Permissions;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class Global : HttpApplication
	{
		public Global()
		{
			if (!string.IsNullOrEmpty(HttpRuntime.AppDomainAppId) && HttpRuntime.AppDomainAppId.EndsWith("calendar", StringComparison.CurrentCultureIgnoreCase))
			{
				this.owaApplication = new CalendarVDirApplication();
				return;
			}
			this.owaApplication = new OwaApplication();
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public override void Init()
		{
			base.Init();
		}

		internal void ExecuteApplicationStart(object sender, EventArgs e)
		{
			this.owaApplication.ExecuteApplicationStart(sender, e);
		}

		internal void ExecuteApplicationEnd(object sender, EventArgs e)
		{
			this.owaApplication.ExecuteApplicationEnd(sender, e);
		}

		private void Application_Start(object sender, EventArgs e)
		{
			this.ExecuteApplicationStart(sender, e);
		}

		private void Application_End(object sender, EventArgs e)
		{
			this.ExecuteApplicationEnd(sender, e);
		}

		private OwaApplicationBase owaApplication;
	}
}
