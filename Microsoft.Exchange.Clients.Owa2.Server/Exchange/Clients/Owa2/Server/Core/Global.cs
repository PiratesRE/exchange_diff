using System;
using System.Security.Permissions;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class Global : HttpApplication
	{
		public Global()
		{
			this.application = BaseApplication.CreateInstance();
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public override void Init()
		{
			base.Init();
		}

		private void Application_Start(object sender, EventArgs e)
		{
			this.application.ExecuteApplicationStart(sender, e);
		}

		private void Application_End(object sender, EventArgs e)
		{
			this.application.ExecuteApplicationEnd(sender, e);
		}

		private void Application_BeginRequest(object sender, EventArgs e)
		{
			this.application.ExecuteApplicationBeginRequest(sender, e);
		}

		private void Application_EndRequest(object sender, EventArgs e)
		{
			this.application.ExecuteApplicationEndRequest(sender, e);
		}

		private readonly BaseApplication application;
	}
}
