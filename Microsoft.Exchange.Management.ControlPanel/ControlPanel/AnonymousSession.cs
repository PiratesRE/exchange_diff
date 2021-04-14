using System;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class AnonymousSession : IRbacSession, IPrincipal, IIdentity
	{
		private AnonymousSession()
		{
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
				return null;
			}
		}

		bool IIdentity.IsAuthenticated
		{
			get
			{
				return false;
			}
		}

		string IIdentity.Name
		{
			get
			{
				return null;
			}
		}

		void IRbacSession.RequestReceived()
		{
		}

		void IRbacSession.RequestCompleted()
		{
		}

		void IRbacSession.SetCurrentThreadPrincipal()
		{
			Thread.CurrentPrincipal = this;
		}

		public static readonly IRbacSession Instance = new AnonymousSession();
	}
}
