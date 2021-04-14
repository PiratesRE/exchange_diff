using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.RedirectionModule.LocStrings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	public class EMCVersionBlockerModule : IHttpModule
	{
		static EMCVersionBlockerModule()
		{
			EMCVersionBlockerModule.perfCounter.PID.RawValue = (long)Process.GetCurrentProcess().Id;
			Globals.InitializeMultiPerfCounterInstance("RemotePS");
		}

		void IHttpModule.Init(HttpApplication application)
		{
			application.PostAuthenticateRequest += EMCVersionBlockerModule.OnPostAuthenticateRequestHandler;
		}

		void IHttpModule.Dispose()
		{
		}

		private static void OnPostAuthenticateRequest(object source, EventArgs args)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpRequest request = httpApplication.Context.Request;
			if (!request.IsAuthenticated)
			{
				Logger.LogWarning(EMCVersionBlockerModule.traceSrc, "[EMCVersionBlockerModule] OnPostAuthenticateRequest was called on a not Authenticated Request!");
				return;
			}
			if (!EMCVersionBlockerModule.ShouldBlockConnectionRequest(request))
			{
				Logger.LogVerbose(EMCVersionBlockerModule.traceSrc, "[EMCVersionBlockerModule] ShouldProcessClientRedirection == false, for user '{0}', Uri '{1}'.", new object[]
				{
					httpApplication.Context.User,
					request.Url
				});
				return;
			}
			EMCVersionBlockerModule.ReportError(httpApplication.Context.Response, HttpStatusCode.BadRequest, 100, Strings.ExchangeClientVersionBlocked(ExchangeSetupContext.InstalledVersion.ToString()));
		}

		private static bool ShouldBlockConnectionRequest(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			NameValueCollection urlProperties = RedirectionHelper.GetUrlProperties(request.Url);
			return "EMC".Equals(urlProperties["clientApplication"], StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(urlProperties["ExchClientVer"]);
		}

		private static void ReportError(HttpResponse response, HttpStatusCode status, int subStatus, string errorMessage)
		{
			Logger.LogVerbose(EMCVersionBlockerModule.traceSrc, "Reporting HTTP error: Status - {0}, SubStatus - {1}, Message - {2}", new object[]
			{
				status,
				subStatus,
				errorMessage
			});
			response.Clear();
			response.StatusCode = (int)status;
			response.SubStatusCode = subStatus;
			if (EMCVersionBlockerModule.IsExchangeCustomError(response.StatusCode, response.SubStatusCode))
			{
				response.TrySkipIisCustomErrors = true;
			}
			if (!string.IsNullOrEmpty(errorMessage))
			{
				Logger.LogVerbose(EMCVersionBlockerModule.traceSrc, "Set the content type to {0} so as to trigger the WSMan plug-in for further handling", new object[]
				{
					"application/soap+xml;charset=UTF-8"
				});
				response.ContentType = "application/soap+xml;charset=UTF-8";
				response.Write(errorMessage);
			}
			response.End();
		}

		private static bool IsExchangeCustomError(int status, int subStatus)
		{
			return (status / 100 == 4 && subStatus / 100 == 1) || (status / 100 == 5 && subStatus / 100 == 2);
		}

		internal const string ExchangeClientVersionUriProperty = "ExchClientVer";

		private const string WSManContentType = "application/soap+xml;charset=UTF-8";

		private const int ExchangeVersionBlockedSubStatusCode = 100;

		private const string W3wpPerfCounterInstanceName = "RemotePS";

		private const string ClientVersionItemKeyName = "ExchangeVersionRedirection.ClientVersionItemKeyName";

		private static readonly EventHandler OnPostAuthenticateRequestHandler = new EventHandler(EMCVersionBlockerModule.OnPostAuthenticateRequest);

		private static readonly TraceSource traceSrc = new TraceSource("EMCVersionBlockerModule");

		private static RemotePowershellPerformanceCountersInstance perfCounter = RemotePowershellPerformanceCounters.GetInstance("RemotePS");
	}
}
