using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.HttpProxy
{
	internal class ConsumerEasAuthBehavior : DefaultAuthBehavior
	{
		public ConsumerEasAuthBehavior(HttpContext httpContext, int serverVersion) : base(httpContext, serverVersion)
		{
		}

		public override bool ShouldDoFullAuthOnUnresolvedAnchorMailbox
		{
			get
			{
				return false;
			}
		}

		protected override string VersionSupportsBackEndFullAuthString
		{
			get
			{
				return HttpProxySettings.EnableRpsTokenBEAuthVersion.Value;
			}
		}

		public static bool IsConsumerEasAuth(HttpContext httpContext)
		{
			IPrincipal user = httpContext.User;
			return user != null && user.Identity != null && string.Equals(user.Identity.AuthenticationType, "RpsTokenAuth", StringComparison.OrdinalIgnoreCase);
		}

		public override AnchorMailbox CreateAuthModuleSpecificAnchorMailbox(IRequestContext requestContext)
		{
			HttpCookie httpCookie = requestContext.HttpContext.Request.Cookies["DefaultAnchorMailbox"];
			AnchorMailbox result;
			if (httpCookie != null && !string.IsNullOrWhiteSpace(httpCookie.Value) && SmtpAddress.IsValidSmtpAddress(httpCookie.Value))
			{
				string organizationContext = base.HttpContext.Items[Constants.WLIDOrganizationContextHeaderName] as string;
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "DefaultAnchorMailboxCookie");
				result = new LiveIdMemberNameAnchorMailbox(httpCookie.Value, organizationContext, requestContext);
			}
			else
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "UnauthenticatedAnonymous");
				result = new AnonymousAnchorMailbox(requestContext);
			}
			return result;
		}

		public override bool IsFullyAuthenticated()
		{
			return false;
		}

		public override void ContinueOnAuthenticate(HttpApplication app, AsyncCallback callback)
		{
			throw new InvalidOperationException("Full authentication for RPS token on front end is not supported.");
		}

		public override void SetFailureStatus()
		{
			throw new InvalidOperationException("Full authentication for RPS token on front end is not supported.");
		}
	}
}
