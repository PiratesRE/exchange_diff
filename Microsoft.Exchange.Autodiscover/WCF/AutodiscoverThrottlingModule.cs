using System;
using System.Configuration;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class AutodiscoverThrottlingModule : IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.PostAuthenticateRequest += this.Application_PostAuthenticate;
			AutodiscoverThrottlingModule.InitializeCPUSlowdown();
		}

		private static void InitializeCPUSlowdown()
		{
			int appSettingAsInt = AutodiscoverThrottlingModule.GetAppSettingAsInt("WSSecuritySlowdownCpuThreshold", 25);
			AutodiscoverThrottlingModule.anonymousSlowdownCpuThreshold = (uint)(((long)appSettingAsInt <= 10L) ? 25 : appSettingAsInt);
			if (AutodiscoverThrottlingModule.anonymousSlowdownCpuThreshold > CPUBasedSleeper.ProcessCpuSlowDownThreshold)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug(0L, "[AutodiscoverThrottlingModule::InitializeCPUSlowdown] The Sharing CPU threshold is higher than the default threshold");
				AutodiscoverThrottlingModule.anonymousSlowdownCpuThreshold = CPUBasedSleeper.ProcessCpuSlowDownThreshold;
			}
		}

		private void Application_PostAuthenticate(object source, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			Uri url = context.Request.Url;
			if (!context.Request.IsAuthenticated && !ExternalAuthentication.GetCurrent().Enabled && (Common.IsWsSecurityAddress(url) || Common.IsWsSecuritySymmetricKeyAddress(url) || Common.IsWsSecurityX509CertAddress(url)))
			{
				context.Response.Close();
				httpApplication.CompleteRequest();
				return;
			}
			uint processCpuSlowDownThreshold = CPUBasedSleeper.ProcessCpuSlowDownThreshold;
			if (!context.Request.IsAuthenticated)
			{
				processCpuSlowDownThreshold = AutodiscoverThrottlingModule.anonymousSlowdownCpuThreshold;
			}
			int arg;
			float arg2;
			if (CPUBasedSleeper.SleepIfNecessary(processCpuSlowDownThreshold, out arg, out arg2))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<int, float>((long)this.GetHashCode(), "[AutodiscoverThrottlingModule::Application_PostAuthenticate] Slept request for {0} msec due to current process CPU percent of {1}%", arg, arg2);
			}
		}

		internal static int GetAppSettingAsInt(string key, int defaultValue)
		{
			string s = ConfigurationManager.AppSettings[key];
			int result;
			if (int.TryParse(s, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private const string AppSettingsAnonymousSlowdownCpuThreshold = "WSSecuritySlowdownCpuThreshold";

		private const uint MinimumAnonymousSlowdownCpuThreshold = 10U;

		private const uint DefaultAnonymousSlowdownCpuThreshold = 25U;

		private static uint anonymousSlowdownCpuThreshold;
	}
}
