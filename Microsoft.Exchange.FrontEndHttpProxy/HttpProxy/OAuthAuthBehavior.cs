using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OAuthAuthBehavior : DefaultAuthBehavior
	{
		public OAuthAuthBehavior(HttpContext httpContext, int serverVersion) : base(httpContext, serverVersion)
		{
		}

		public override bool ShouldDoFullAuthOnUnresolvedAnchorMailbox
		{
			get
			{
				return true;
			}
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
				return HttpProxySettings.EnableOAuthBEAuthVersion.Value;
			}
		}

		public static bool IsOAuth(HttpContext httpContext)
		{
			IPrincipal user = httpContext.User;
			return user != null && user.Identity != null && user.Identity is GenericIdentity && (string.Equals(user.Identity.AuthenticationType, Constants.BearerAuthenticationType, StringComparison.OrdinalIgnoreCase) || string.Equals(user.Identity.AuthenticationType, Constants.BearerPreAuthenticationType, StringComparison.OrdinalIgnoreCase));
		}

		public override AnchorMailbox CreateAuthModuleSpecificAnchorMailbox(IRequestContext requestContext)
		{
			HttpContext httpContext = requestContext.HttpContext;
			IPrincipal user = httpContext.User;
			IIdentity identity = httpContext.User.Identity;
			OAuthPreAuthIdentity oauthPreAuthIdentity = identity as OAuthPreAuthIdentity;
			if (oauthPreAuthIdentity == null)
			{
				return null;
			}
			string text = httpContext.Request.Headers[Constants.ExternalDirectoryObjectIdHeaderName];
			if (!string.IsNullOrEmpty(text))
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "PreAuth-ExternalDirectoryObjectId-Header");
				return new ExternalDirectoryObjectIdAnchorMailbox(text, oauthPreAuthIdentity.OrganizationId, requestContext);
			}
			switch (oauthPreAuthIdentity.PreAuthType)
			{
			case OAuthPreAuthType.OrganizationOnly:
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "PreAuth-AppOrganization");
				return new OrganizationAnchorMailbox(oauthPreAuthIdentity.OrganizationId, requestContext);
			case OAuthPreAuthType.Smtp:
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "PreAuth-Smtp");
				return new SmtpAnchorMailbox(oauthPreAuthIdentity.LookupValue, requestContext);
			case OAuthPreAuthType.WindowsLiveID:
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "PreAuth-LiveID");
				return new LiveIdMemberNameAnchorMailbox(oauthPreAuthIdentity.LookupValue, null, requestContext);
			case OAuthPreAuthType.ExternalDirectoryObjectId:
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "PreAuth-ExternalDirectoryObjectId");
				return new ExternalDirectoryObjectIdAnchorMailbox(oauthPreAuthIdentity.LookupValue, oauthPreAuthIdentity.OrganizationId, requestContext);
			case OAuthPreAuthType.Puid:
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "PreAuth-Puid");
				return new PuidAnchorMailbox(oauthPreAuthIdentity.LookupValue, "fake@outlook.com", requestContext);
			default:
				throw new InvalidOperationException("unknown preauth type");
			}
		}

		public override bool IsFullyAuthenticated()
		{
			OAuthIdentity oauthIdentity = base.HttpContext.User.Identity as OAuthIdentity;
			return oauthIdentity != null;
		}

		public override void ContinueOnAuthenticate(HttpApplication app, AsyncCallback callback)
		{
			OAuthHttpModule.ContinueOnAuthenticate(app, callback);
		}

		public override void SetFailureStatus()
		{
		}
	}
}
