using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class PswsAuthNModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			context.AuthenticateRequest += this.OnAuthenticate;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnAuthenticate(object source, EventArgs args)
		{
			ExTraceGlobals.AccessCheckTracer.TraceDebug((long)this.GetHashCode(), "[PswsAuthNModule.OnAuthenticate] Enter");
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			if (!request.IsAuthenticated)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug((long)this.GetHashCode(), "[PswsAuthNModule.OnAuthenticate] Request is not authenticated. Skip.");
				return;
			}
			UserToken userToken = context.CurrentUserToken();
			if (userToken == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceError((long)this.GetHashCode(), "[PswsAuthNModule.OnAuthenticate] user token is null. Skip.");
				throw new InvalidOperationException("Unexpected condition: userToken from HttpContext is null.");
			}
			context.Items["X-Psws-CurrentLogonUser"] = context.User.Identity;
			string pswsMembershipId = PswsAuthZHelper.GetPswsMembershipId(userToken, request.Headers);
			ExAssert.RetailAssert(!string.IsNullOrWhiteSpace(pswsMembershipId), "userIdWithMembershipId can't be null or blank.");
			string text = "PswsMembership-" + userToken.AuthenticationType;
			ExTraceGlobals.AccessCheckTracer.TraceDebug((long)this.GetHashCode(), string.Format("[PswsAuthNModule.OnAuthenticate] userId = \"{0}\", userType = \"{1}\".", pswsMembershipId, text));
			context.User = new GenericPrincipal(new GenericIdentity(pswsMembershipId, text), null);
			ExTraceGlobals.AccessCheckTracer.TraceDebug((long)this.GetHashCode(), "[PswsAuthNModule.OnAuthenticate] Leave");
		}

		private const string PswsAuthNTypePrefix = "PswsMembership-";
	}
}
