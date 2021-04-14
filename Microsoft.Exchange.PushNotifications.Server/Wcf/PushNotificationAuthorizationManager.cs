using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.PushNotifications.Server.Wcf
{
	public class PushNotificationAuthorizationManager : ServiceAuthorizationManager
	{
		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			if (HttpContext.Current != null)
			{
				if (!HttpContext.Current.Request.IsAuthenticated)
				{
					this.LogAccessDenied(HttpContext.Current.Request.Headers, "IsAuthenticated=false");
					return false;
				}
				GenericPrincipal genericPrincipal = HttpContext.Current.User as GenericPrincipal;
				if (genericPrincipal != null)
				{
					OAuthIdentity oauthIdentity = genericPrincipal.Identity as OAuthIdentity;
					if (oauthIdentity == null)
					{
						this.LogAccessDenied(HttpContext.Current.Request.Headers, "OAuthIdentity=false");
						return false;
					}
					if (!oauthIdentity.IsAppOnly)
					{
						this.LogAccessDenied(HttpContext.Current.Request.Headers, "IsAppOnly=false");
						return false;
					}
					this.LogOrganizationAuthN(oauthIdentity.OrganizationId);
					operationContext.SetPrincipal(genericPrincipal);
				}
				else
				{
					WindowsPrincipal windowsPrincipal = HttpContext.Current.User as WindowsPrincipal;
					if (windowsPrincipal == null)
					{
						this.LogAccessDenied(HttpContext.Current.Request.Headers, string.Format("User={0}", genericPrincipal.ToNullableString(null)));
						return false;
					}
					if (!new ServicePrincipal(windowsPrincipal.Identity, ExTraceGlobals.PushNotificationServiceTracer).IsInRole("LocalAdministrators"))
					{
						this.LogAccessDenied(HttpContext.Current.Request.Headers, "WindowsAdministrator=false");
						return false;
					}
					operationContext.SetPrincipal(windowsPrincipal);
				}
			}
			return base.CheckAccessCore(operationContext);
		}

		private void LogOrganizationAuthN(OrganizationId organiaztion)
		{
			if (PushNotificationsCrimsonEvents.PushNotificationAuthorizationManager.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.PushNotificationAuthorizationManager.Log<string>(organiaztion.ToString());
			}
			if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "[PushNotificationAuthorizationManager] A valid authenticated principal was added to the OperationContext for Organization: '{0}'.", organiaztion.ToString());
			}
		}

		private void LogAccessDenied(NameValueCollection headers, string reason)
		{
			if (PushNotificationsCrimsonEvents.PushNotificationAuthorizationManagerAccessDenied.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.PushNotificationAuthorizationManagerAccessDenied.Log<string, string>(headers.ToTraceString(null), reason);
			}
			if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceError<string, string>((long)this.GetHashCode(), "[PushNotificationAuthorizationManager] Access Denied for OperationContext '{0}', detail: '{1}'.", headers.ToTraceString(null), reason);
			}
		}
	}
}
