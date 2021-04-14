using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class EventLogConstants
	{
		internal static bool IsPowerShellWebService
		{
			get
			{
				if (EventLogConstants.isPowerShellWebService == null)
				{
					EventLogConstants.isPowerShellWebService = new bool?(EventLogConstants.CalculateIsPswsFlag());
				}
				return EventLogConstants.isPowerShellWebService.Value;
			}
		}

		internal static ExEventLog NetEventLogger
		{
			get
			{
				return EventLogConstants.netEventLogger.Value;
			}
		}

		private static bool CalculateIsPswsFlag()
		{
			try
			{
				if (HttpContext.Current == null || HttpContext.Current.Request == null || HttpContext.Current.Request.Url == null)
				{
					return false;
				}
			}
			catch (HttpException)
			{
				return false;
			}
			bool result;
			try
			{
				Uri url = HttpContext.Current.Request.Url;
				if (url.AbsolutePath == null)
				{
					result = false;
				}
				else
				{
					result = url.AbsolutePath.StartsWith("/psws", StringComparison.OrdinalIgnoreCase);
				}
			}
			catch (InvalidOperationException)
			{
				result = false;
			}
			return result;
		}

		private static readonly Lazy<ExEventLog> netEventLogger = new Lazy<ExEventLog>(() => new ExEventLog(ExTraceGlobals.AppSettingsTracer.Category, "MSExchange Net"));

		private static bool? isPowerShellWebService;
	}
}
