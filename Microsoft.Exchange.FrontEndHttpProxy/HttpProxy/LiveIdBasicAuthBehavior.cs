using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.HttpProxy
{
	internal class LiveIdBasicAuthBehavior : DefaultAuthBehavior
	{
		public LiveIdBasicAuthBehavior(HttpContext httpContext, int serverVersion) : base(httpContext, serverVersion)
		{
		}

		public override bool ShouldDoFullAuthOnUnresolvedAnchorMailbox
		{
			get
			{
				return true;
			}
		}

		protected override string VersionSupportsBackEndFullAuthString
		{
			get
			{
				return HttpProxySettings.EnableLiveIdBasicBEAuthVersion.Value;
			}
		}

		public static bool IsLiveIdBasicAuth(HttpContext httpContext)
		{
			IPrincipal user = httpContext.User;
			return user != null && user.Identity != null && string.Equals(user.Identity.AuthenticationType, "LiveIdBasic", StringComparison.OrdinalIgnoreCase);
		}

		public override AnchorMailbox CreateAuthModuleSpecificAnchorMailbox(IRequestContext requestContext)
		{
			string text = base.HttpContext.Items[Constants.WLIDMemberName] as string;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			string organizationContext = base.HttpContext.Items[Constants.WLIDOrganizationContextHeaderName] as string;
			requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "LiveIdBasic-LiveIdMemberName");
			return new LiveIdMemberNameAnchorMailbox(text, organizationContext, requestContext);
		}

		public override string GetExecutingUserOrganization()
		{
			string text = base.HttpContext.Items[Constants.WLIDMemberName] as string;
			if (!string.IsNullOrEmpty(text))
			{
				SmtpAddress smtpAddress = new SmtpAddress(text);
				return smtpAddress.Domain;
			}
			return base.GetExecutingUserOrganization();
		}

		public override bool IsFullyAuthenticated()
		{
			return base.HttpContext.Items["Item-CommonAccessToken"] is CommonAccessToken;
		}

		public override void ContinueOnAuthenticate(HttpApplication app, AsyncCallback callback)
		{
			LiveIdBasicAuthModule.ContinueOnAuthenticate(app, null, callback, null);
		}

		public override void SetFailureStatus()
		{
			if (base.HttpContext.Response.StatusCode == 200)
			{
				base.HttpContext.Response.StatusCode = 401;
			}
		}
	}
}
