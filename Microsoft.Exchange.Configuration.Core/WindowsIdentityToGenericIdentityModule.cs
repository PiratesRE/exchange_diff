using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Configuration.Core
{
	public class WindowsIdentityToGenericIdentityModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.AuthenticateRequest += this.OnAuthenticateRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnAuthenticateRequest(object source, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			if (!(httpContext.User.Identity is WindowsIdentity))
			{
				return;
			}
			WindowsIdentity windowsIdentity = (WindowsIdentity)httpContext.User.Identity;
			if (windowsIdentity.User == null)
			{
				string value = string.Format("UnExpected: WindowsIdentity.User = null, AuthNType = {0}, Name = {1}", windowsIdentity.AuthenticationType, windowsIdentity.Name);
				HttpLogger.SafeAppendGenericError("WindowsIdentityToGenericIdentityModule", value, true);
				HttpApplication httpApplication = (HttpApplication)source;
				httpContext.Response.StatusCode = 401;
				httpApplication.CompleteRequest();
				return;
			}
			ExAssert.RetailAssert(httpContext.User != null && httpContext.User.Identity != null && httpContext.User.Identity.Name != null, "HttpContext.User.Identity.Name should not be null.");
			GenericIdentity identity = new GenericIdentity(httpContext.User.Identity.Name.ToLower(), "Converted-" + httpContext.User.Identity.AuthenticationType);
			httpContext.User = new GenericPrincipal(identity, new string[0]);
			using (WinRMDataSender winRMDataSender = new WinRMDataSender(httpContext, null))
			{
				WinRMInfo winRMInfo = httpContext.Items["X-RemotePS-WinRMInfo"] as WinRMInfo;
				if (winRMInfo != null)
				{
					winRMDataSender.SessionId = winRMInfo.FomattedSessionId;
				}
				winRMDataSender.RequestId = HttpLogger.ActivityScope.ActivityId.ToString();
				winRMDataSender.UserToken = new UserToken(AuthenticationType.Kerberos, null, null, windowsIdentity.Name, windowsIdentity.User, null, OrganizationId.ForestWideOrgId, null, false, new CommonAccessToken(windowsIdentity));
				winRMDataSender.Send();
			}
		}
	}
}
