using System;
using System.Security;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.DelegatedAuthentication;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ErrorHandlingModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			ExWatson.Register("E12");
			application.Error += ErrorHandlingModule.Application_Error;
			application.EndRequest += ErrorHandlingModule.Application_EndRequest;
			application.PostAcquireRequestState += ErrorHandlingModule.Application_PostAcquireRequestState;
		}

		private static void Application_EndRequest(object sender, EventArgs e)
		{
			HttpContext context = HttpContext.Current;
			if (context.IsUploadRequest())
			{
				HttpResponse response = HttpContext.Current.Response;
				if (response.StatusCode == 404 && response.SubStatusCode == 13)
				{
					HttpException e2 = new HttpException(Strings.FileExceedsIISLimit);
					ErrorHandlingModule.SendJsonErrorForUpload(context, e2);
				}
			}
		}

		private static void Application_PostAcquireRequestState(object sender, EventArgs e)
		{
			if (!VirtualDirectoryConfiguration.EcpVirtualDirectoryAnonymousAuthenticationEnabled)
			{
				ExTraceGlobals.RBACTracer.TraceInformation(0, 0L, "Anonymous authentication must be enabled in ECP.");
				throw new ExchangeConfigurationException(Strings.AnonymousAuthenticationDisabledErrorMessage);
			}
			HttpRequest request = HttpContext.Current.Request;
			HttpBrowserCapabilities browser = request.Browser;
			if (browser != null && browser.IsBrowser("IE") && browser.MajorVersion < 7)
			{
				ErrorHandlingUtil.TransferToErrorPage("browsernotsupported");
			}
		}

		private static void Application_Error(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			Exception ex = httpContext.GetError();
			ExTraceGlobals.EventLogTracer.TraceError<EcpTraceFormatter<Exception>>(0, 0L, "Application Error: {0}", ex.GetTraceFormatter());
			DDIHelper.Trace("Application Error: {0}", new object[]
			{
				ex.GetTraceFormatter()
			});
			EcpPerfCounters.AspNetErrors.Increment();
			EcpEventLogConstants.Tuple_RequestFailed.LogPeriodicFailure(EcpEventLogExtensions.GetUserNameToLog(), httpContext.GetRequestUrlForLog(), ex, EcpEventLogExtensions.GetFlightInfoForLog());
			RbacPrincipal current = RbacPrincipal.GetCurrent(false);
			string tenantName = string.Empty;
			if (current != null)
			{
				OrganizationId organizationId = current.RbacConfiguration.OrganizationId;
				if (organizationId != null && organizationId.OrganizationalUnit != null)
				{
					tenantName = organizationId.OrganizationalUnit.Name;
				}
			}
			ActivityContextLogger.Instance.LogEvent(new PeriodicFailureEvent(ActivityContext.ActivityId.FormatForLog(), tenantName, httpContext.GetRequestUrlForLog(), ex, EcpEventLogExtensions.GetFlightInfoForLog()));
			ActivityContextManager.CleanupActivityContext(httpContext);
			if (ex is DelegatedSecurityTokenExpiredException)
			{
				ErrorHandlingModule.HandleDelegatedSecurityTokenExpire(httpContext);
				return;
			}
			if (httpContext.IsWebServiceRequest())
			{
				string errorCause = DiagnosticsBehavior.GetErrorCause(ex);
				ErrorHandlingUtil.SendReportForCriticalException(httpContext, ex);
				ErrorHandlingModule.SendJsonError(httpContext, ex, errorCause);
				return;
			}
			if (httpContext.IsUploadRequest())
			{
				ErrorHandlingUtil.SendReportForCriticalException(httpContext, ex);
				ErrorHandlingModule.SendJsonErrorForUpload(httpContext, ex);
				return;
			}
			if (ex is HttpException && ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			httpContext.Request.ServerVariables["X-ECP-ERROR"] = ex.GetType().FullName;
			string text = null;
			string text2 = null;
			if (ex is OverBudgetException)
			{
				text = "overbudget";
			}
			else if (ex is IdentityNotMappedException || ex is TransientException)
			{
				text = "transientserviceerror";
			}
			else if (ex is ObjectNotFoundException)
			{
				if (ex.InnerException is NonUniqueRecipientException)
				{
					text = "nonuniquerecipient";
				}
				else
				{
					text = "nonmailbox";
				}
			}
			else if (ex is ServerNotInSiteException || ex is LowVersionUserDeniedException)
			{
				text = "lowversion";
			}
			else if (ex is CmdletAccessDeniedException || ex is DelegatedAccessDeniedException)
			{
				text = "noroles";
			}
			else if (ex is UrlNotFoundOrNoAccessException)
			{
				text = "urlnotfoundornoaccess";
			}
			else if (ex is BadRequestException)
			{
				text = "badrequest";
			}
			else if (ex is BadQueryParameterException)
			{
				text = "badqueryparameter";
			}
			else if (ex is ProxyFailureException)
			{
				text = "transientserviceerror";
			}
			else if (ex is ProxyCantFindCasServerException)
			{
				text = "proxy";
			}
			else if (ex is CasServerNotSupportEsoException)
			{
				text = "noeso";
			}
			else if (ex is RegionalSettingsNotConfiguredException)
			{
				text = "regionalsettingsnotconfigured";
			}
			else if (ex is SecurityException || (ErrorHandlingUtil.KnownReflectedExceptions.Value.ContainsKey("Microsoft.Exchange.Hygiene.Security.Authorization.NoValidRolesAssociatedToUserException, Microsoft.Exchange.Hygiene.Security.Authorization") && ex.GetType() == ErrorHandlingUtil.KnownReflectedExceptions.Value["Microsoft.Exchange.Hygiene.Security.Authorization.NoValidRolesAssociatedToUserException, Microsoft.Exchange.Hygiene.Security.Authorization"]))
			{
				text = "noroles";
			}
			else if (ex is ExchangeConfigurationException)
			{
				text = "anonymousauthenticationdisabled";
			}
			else if (ex is CannotAccessOptionsWithBEParamOrCookieException)
			{
				text = "cannotaccessoptionswithbeparamorcookie";
			}
			else if (ex.IsMaxRequestLengthExceededException())
			{
				EcpPerfCounters.RedirectToError.Increment();
				text2 = httpContext.Request.AppRelativeCurrentExecutionFilePath;
			}
			else
			{
				ErrorHandlingUtil.SendReportForCriticalException(httpContext, ex);
				if (!ErrorHandlingUtil.ShowIisNativeErrorPage && !ex.IsInterestingHttpException())
				{
					text = "unexpected";
				}
			}
			if (text2 != null)
			{
				httpContext.Server.Transfer(text2, true);
				return;
			}
			if (text != null)
			{
				ErrorHandlingModule.TransferToErrorPage(text, ErrorHandlingUtil.CanShowDebugInfo(ex));
			}
		}

		private static void SendJsonError(HttpContext context, Exception e, string cause)
		{
			context.ClearError();
			context.Response.Clear();
			context.Response.TrySkipIisCustomErrors = true;
			context.Response.Cache.SetCacheability(HttpCacheability.Private);
			context.Response.ContentType = "application/json; charset=utf-8";
			context.Response.Headers["jsonerror"] = "true";
			string value = (!string.IsNullOrEmpty(cause)) ? cause : ((e == null) ? "" : e.GetType().FullName);
			string s = (e == null) ? cause : new JsonFaultDetail(e).ToJsonString(null);
			context.Response.AddHeader("X-ECP-ERROR", value);
			context.Response.Write(s);
			context.Response.StatusCode = 500;
			context.Response.End();
		}

		private static void SendJsonErrorForUpload(HttpContext context, Exception e)
		{
			context.ClearError();
			context.Response.Clear();
			context.Response.ContentType = "text/html";
			context.Response.AddHeader("X-ECP-ERROR", e.GetType().FullName);
			string s = new PowerShellResults
			{
				ErrorRecords = new ErrorRecord[]
				{
					new ErrorRecord(e)
				}
			}.ToJsonString(null);
			context.Response.Write(HttpUtility.HtmlEncode(s));
			context.Response.StatusCode = 200;
			context.Response.End();
		}

		private static void HandleDelegatedSecurityTokenExpire(HttpContext context)
		{
			context.ClearError();
			HttpResponse response = context.Response;
			response.Clear();
			if (context.IsWebServiceRequest())
			{
				response.ContentType = "text/html";
				response.StatusCode = 443;
				string s = "<HTML><BODY>443 Delegated Security Token Expired.</BODY></HTML>";
				response.Write(s);
				response.End();
				return;
			}
			string url = EcpUrl.ProcessUrl("/ecp/error.aspx?cause=tokenrenewed&exsvurl=1", true);
			EcpPerfCounters.RedirectToError.Increment();
			response.Redirect(url, true);
		}

		internal static void TransferToErrorPage(string cause, bool showCallStack)
		{
			HttpContext httpContext = HttpContext.Current;
			if (httpContext.IsWebServiceRequest())
			{
				ErrorHandlingModule.SendJsonError(httpContext, httpContext.GetError(), cause);
				return;
			}
			if (httpContext.IsUploadRequest())
			{
				ErrorHandlingModule.SendJsonErrorForUpload(httpContext, httpContext.GetError());
				return;
			}
			EcpPerfCounters.RedirectToError.Increment();
			if (!showCallStack)
			{
				httpContext.ClearError();
			}
			httpContext.Response.Clear();
			string text = string.Format("~/error.aspx?cause={0}", cause);
			string text2 = httpContext.Request.QueryString["isNarrow"];
			if (text2 != null)
			{
				text = EcpUrl.AppendQueryParameter(text, "isNarrow", text2);
			}
			httpContext.Server.Transfer(text);
		}

		public void Dispose()
		{
		}

		internal const string NoValidRolesAssociatedToUserException = "Microsoft.Exchange.Hygiene.Security.Authorization.NoValidRolesAssociatedToUserException, Microsoft.Exchange.Hygiene.Security.Authorization";
	}
}
