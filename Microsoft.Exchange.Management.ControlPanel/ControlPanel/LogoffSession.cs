using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class LogoffSession : IRbacSession, IPrincipal, IIdentity
	{
		public LogoffSession(RbacSettings rbacSettings, IIdentity identity)
		{
			this.rbacSettings = rbacSettings;
			this.identity = identity;
		}

		IIdentity IPrincipal.Identity
		{
			get
			{
				return this;
			}
		}

		bool IPrincipal.IsInRole(string role)
		{
			return false;
		}

		string IIdentity.AuthenticationType
		{
			get
			{
				return this.identity.AuthenticationType;
			}
		}

		bool IIdentity.IsAuthenticated
		{
			get
			{
				return this.identity.IsAuthenticated;
			}
		}

		string IIdentity.Name
		{
			get
			{
				return this.identity.GetSafeName(true);
			}
		}

		public IIdentity OriginalIdentity
		{
			get
			{
				return this.identity;
			}
		}

		void IRbacSession.RequestReceived()
		{
			HttpContext httpContext = HttpContext.Current;
			if (!((IIdentity)this).IsAuthenticated)
			{
				httpContext.Response.Redirect(EcpUrl.EcpVDir, true);
				return;
			}
			this.rbacSettings.ExpireSession();
			string text = httpContext.Request.QueryString["url"];
			string str = string.IsNullOrEmpty(text) ? string.Empty : ("?url=" + HttpUtility.UrlEncode(text));
			httpContext.Response.Redirect(EcpUrl.OwaVDir + "logoff.owa" + str, true);
		}

		void IRbacSession.RequestCompleted()
		{
		}

		void IRbacSession.SetCurrentThreadPrincipal()
		{
			Thread.CurrentPrincipal = this;
		}

		private const string OwaLogoffUrl = "logoff.owa";

		public static readonly IRbacSession AnonymousSession = new LogoffSession(null, WindowsIdentity.GetAnonymous());

		private RbacSettings rbacSettings;

		private IIdentity identity;
	}
}
