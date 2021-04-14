using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Clients.Security;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.HttpProxy
{
	internal class LiveIdCookieAuthBehavior : DefaultAuthBehavior
	{
		public LiveIdCookieAuthBehavior(HttpContext httpContext, int serverVersion) : base(httpContext, serverVersion)
		{
		}

		public override bool ShouldCopyAuthenticationHeaderToClientResponse
		{
			get
			{
				return base.AuthState == AuthState.BackEndFullAuth;
			}
		}

		protected override string VersionSupportsBackEndFullAuthString
		{
			get
			{
				return HttpProxySettings.EnableLiveIdCookieBEAuthVersion.Value;
			}
		}

		public static bool IsLiveIdCookieAuth(HttpContext httpContext)
		{
			IPrincipal user = httpContext.User;
			return user != null && user.Identity != null && user.Identity is GenericIdentity && string.Equals(user.Identity.AuthenticationType, "OrgId", StringComparison.OrdinalIgnoreCase);
		}

		public override bool IsFullyAuthenticated()
		{
			return base.HttpContext.Items["Item-CommonAccessToken"] is CommonAccessToken || (base.HttpContext.Items["LiveIdSkippedAuthForAnonResource"] != null && (bool)base.HttpContext.Items["LiveIdSkippedAuthForAnonResource"]);
		}

		public override AnchorMailbox CreateAuthModuleSpecificAnchorMailbox(IRequestContext requestContext)
		{
			HttpCookie httpCookie = requestContext.HttpContext.Request.Cookies["DefaultAnchorMailbox"];
			AnchorMailbox result;
			if (httpCookie != null && !string.IsNullOrWhiteSpace(httpCookie.Value))
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "DefaultAnchorMailboxCookie");
				result = new LiveIdMemberNameAnchorMailbox(httpCookie.Value, null, requestContext);
			}
			else
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "UnauthenticatedAnonymous");
				result = new AnonymousAnchorMailbox(requestContext);
			}
			return result;
		}

		public override void ContinueOnAuthenticate(HttpApplication app, AsyncCallback callback)
		{
			LiveIdAuthenticationModule.ContinueOnAuthenticate(app.Context);
			callback(null);
		}

		public override void SetFailureStatus()
		{
		}
	}
}
