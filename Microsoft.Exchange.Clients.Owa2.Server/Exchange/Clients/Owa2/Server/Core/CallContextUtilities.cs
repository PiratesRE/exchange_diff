using System;
using System.Collections.Specialized;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class CallContextUtilities
	{
		internal static CallContext CreateAndSetCallContext(Message request, WorkloadType workloadType, bool duplicatedActionDetectionEnabled, string owaUserContextKey = "")
		{
			CallContext callContext = CallContextUtilities.CreateCallContext(request, null, duplicatedActionDetectionEnabled, owaUserContextKey);
			callContext.OwaExplicitLogonUser = UserContextUtilities.GetExplicitLogonUser(HttpContext.Current);
			callContext.WorkloadType = workloadType;
			CallContext.SetCurrent(callContext);
			return callContext;
		}

		internal static CallContext CreateCallContext(Message request, MessageHeaderProcessor headerProcessor, bool duplicatedActionDetectionEnabled, string owaUserContextKey = "")
		{
			ExAssert.RetailAssert(request != null, "request should not be null");
			ExAssert.RetailAssert(HttpContext.Current != null, "HttpContext.Current should not be null");
			MessageHeaderProcessor headerProcessor2 = headerProcessor ?? new JsonMessageHeaderProcessor();
			RequestContext requestContext = RequestContext.Get(HttpContext.Current);
			UserContext userContext = requestContext.UserContext as UserContext;
			bool flag = false;
			CallContext callContext;
			if (userContext != null && userContext.FeaturesManager.ClientServerSettings.OWAPLTPerf.Enabled && HttpContext.Current.User.Identity is SidBasedIdentity && !userContext.IsExplicitLogon)
			{
				Trace coreCallTracer = ExTraceGlobals.CoreCallTracer;
				long id = 0L;
				string formatString = "[CallContextUtilities::CreateAndSetCallContext] Using CreateFromInternalRequestContext to create CallContext. UserContextKey:{0}, primarySmtpAddress:{1}";
				string arg = (userContext.Key != null) ? userContext.Key.ToString() : "<null>";
				string arg2;
				if (userContext.LogonIdentity != null)
				{
					SmtpAddress primarySmtpAddress = userContext.LogonIdentity.PrimarySmtpAddress;
					arg2 = userContext.LogonIdentity.PrimarySmtpAddress.ToString();
				}
				else
				{
					arg2 = "<null>";
				}
				coreCallTracer.TraceDebug<string, string>(id, formatString, arg, arg2);
				callContext = CallContext.CreateFromInternalRequestContext(headerProcessor2, request, duplicatedActionDetectionEnabled, (IEWSPartnerRequestContext)requestContext.UserContext);
				flag = true;
			}
			else
			{
				IMailboxContext userContext2 = requestContext.UserContext;
				Trace coreCallTracer2 = ExTraceGlobals.CoreCallTracer;
				long id2 = 0L;
				string formatString2 = "[CallContextUtilities::CreateAndSetCallContext] Using CreateFromRequest to create CallContext.  UserContextKey:{0}, primarySmtpAddress:{1}";
				string arg3 = (userContext2 != null && userContext2.Key != null) ? userContext2.Key.ToString() : "<null>";
				string arg4;
				if (userContext2 != null && userContext2.LogonIdentity != null)
				{
					SmtpAddress primarySmtpAddress2 = userContext2.LogonIdentity.PrimarySmtpAddress;
					arg4 = userContext2.LogonIdentity.PrimarySmtpAddress.ToString();
				}
				else
				{
					arg4 = "<null>";
				}
				coreCallTracer2.TraceDebug<string, string>(id2, formatString2, arg3, arg4);
				callContext = CallContext.CreateFromRequest(headerProcessor2, request, duplicatedActionDetectionEnabled);
			}
			callContext.IsOwa = true;
			callContext.OwaUserContextKey = owaUserContextKey;
			try
			{
				if (callContext.AccessingADUser != null && callContext.AccessingPrincipal != null && !callContext.AccessingADUser.ObjectId.Equals(callContext.AccessingPrincipal.ObjectId))
				{
					throw new InvalidOperationException(string.Format("Invalid CallContext created from {0} due to mismatch user. AccessingADUser.LegDN = {1} and AccessingPrincipal.LegDN = {2}", flag ? "Internal Request" : "Request", callContext.AccessingADUser.LegacyExchangeDN, callContext.AccessingPrincipal.LegacyDn));
				}
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceError(0L, ex.Message);
				callContext.ProtocolLog.AppendGenericError("InvalidCC", ex.Message);
				StringBuilder stringBuilder = new StringBuilder(500);
				NameValueCollection headers = HttpContext.Current.Request.Headers;
				if (headers != null)
				{
					foreach (string text in headers.AllKeys)
					{
						string arg5 = headers[text] ?? string.Empty;
						stringBuilder.AppendFormat("{0}={1};", text, arg5);
					}
				}
				stringBuilder.Append("ADUser=" + callContext.AccessingADUser.LegacyExchangeDN + ";");
				stringBuilder.Append("ExchangePrincipal=" + callContext.AccessingPrincipal.LegacyDn + ";");
				ExWatson.SendReport(ex, ReportOptions.None, stringBuilder.ToString());
			}
			callContext.ADRecipientSessionContext.ExcludeInactiveMailboxInADRecipientSession();
			return callContext;
		}

		internal static AuthZClientInfo GetCallerClientInfo()
		{
			return CallContext.GetCallerClientInfo();
		}
	}
}
