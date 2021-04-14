using System;
using System.Configuration;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal static class MServe
	{
		static MServe()
		{
			if (!int.TryParse(ConfigurationManager.AppSettings["PodSiteStartRange"], out MServe.podSiteStartRange))
			{
				MServe.podSiteStartRange = -1;
			}
			if (!int.TryParse(ConfigurationManager.AppSettings["PodSiteEndRange"], out MServe.podSiteEndRange))
			{
				MServe.podSiteEndRange = int.MaxValue;
			}
		}

		internal static string GetRedirectServer(string smtpAddress)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "MServe.GetRedirectServer. Entry. smtpAddress = {0}.", smtpAddress);
			IGlobalDirectorySession globalDirectorySession = new MServDirectorySession(MServe.podRedirectTemplate);
			string empty;
			if (!globalDirectorySession.TryGetRedirectServer(smtpAddress, out empty))
			{
				empty = string.Empty;
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>(0L, "MServe.GetRedirectServer. Exit. smtpAddress = {0}, redirectServer = {1}.", smtpAddress, empty);
			return empty;
		}

		private static string podRedirectTemplate = ConfigurationManager.AppSettings["PodRedirectTemplate"];

		private static int podSiteStartRange;

		private static int podSiteEndRange;
	}
}
