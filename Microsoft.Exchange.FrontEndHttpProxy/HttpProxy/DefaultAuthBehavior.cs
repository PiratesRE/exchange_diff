using System;
using System.Web;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DefaultAuthBehavior : IAuthBehavior
	{
		protected DefaultAuthBehavior(HttpContext httpContext, int serverVersion)
		{
			this.HttpContext = httpContext;
			this.VersionSupportsBackEndFullAuth = Utilities.ConvertToServerVersion(this.VersionSupportsBackEndFullAuthString);
			this.SetState(serverVersion);
		}

		public AuthState AuthState { get; private set; }

		public virtual bool ShouldDoFullAuthOnUnresolvedAnchorMailbox
		{
			get
			{
				return false;
			}
		}

		public virtual bool ShouldCopyAuthenticationHeaderToClientResponse
		{
			get
			{
				return false;
			}
		}

		private protected HttpContext HttpContext { protected get; private set; }

		protected virtual string VersionSupportsBackEndFullAuthString
		{
			get
			{
				return string.Empty;
			}
		}

		private ServerVersion VersionSupportsBackEndFullAuth { get; set; }

		private bool BackEndFullAuthEnabled
		{
			get
			{
				return this.VersionSupportsBackEndFullAuth != null;
			}
		}

		public static IAuthBehavior CreateAuthBehavior(HttpContext httpContext)
		{
			return DefaultAuthBehavior.CreateAuthBehavior(httpContext, 0);
		}

		public static IAuthBehavior CreateAuthBehavior(HttpContext httpContext, int serverVersion)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (LiveIdBasicAuthBehavior.IsLiveIdBasicAuth(httpContext))
			{
				return new LiveIdBasicAuthBehavior(httpContext, serverVersion);
			}
			if (OAuthAuthBehavior.IsOAuth(httpContext))
			{
				return new OAuthAuthBehavior(httpContext, serverVersion);
			}
			if (LiveIdCookieAuthBehavior.IsLiveIdCookieAuth(httpContext))
			{
				return new LiveIdCookieAuthBehavior(httpContext, serverVersion);
			}
			if (ConsumerEasAuthBehavior.IsConsumerEasAuth(httpContext))
			{
				return new ConsumerEasAuthBehavior(httpContext, serverVersion);
			}
			return new DefaultAuthBehavior(httpContext, serverVersion);
		}

		public void SetState(int serverVersion)
		{
			this.AuthState = AuthState.FrontEndFullAuth;
			if (this.BackEndFullAuthEnabled)
			{
				this.AuthState = ((serverVersion >= this.VersionSupportsBackEndFullAuth.ToInt()) ? AuthState.BackEndFullAuth : AuthState.FrontEndContinueAuth);
			}
		}

		public void ResetState()
		{
			this.SetState(0);
		}

		public virtual AnchorMailbox CreateAuthModuleSpecificAnchorMailbox(IRequestContext requestContext)
		{
			return null;
		}

		public virtual string GetExecutingUserOrganization()
		{
			return string.Empty;
		}

		public virtual bool IsFullyAuthenticated()
		{
			return true;
		}

		public virtual void ContinueOnAuthenticate(HttpApplication app, AsyncCallback callback)
		{
			throw new NotSupportedException();
		}

		public virtual void SetFailureStatus()
		{
			this.HttpContext.Response.StatusCode = 401;
		}
	}
}
