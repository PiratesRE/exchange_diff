using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Common
{
	internal class ExWebHealthHandler
	{
		internal ExWebHealthHandler(string appName)
		{
			this.appName = appName;
		}

		internal Func<ExWebHealthHandler.CustomHealthCheckResult> CustomHealthCallback { get; set; }

		internal void ProcessHealth(IExWebHealthResponseWrapper response)
		{
			ExTraceGlobals.WebHealthTracer.TraceDebug((long)this.GetHashCode(), "ExWebHealthHandler::ProcessHealth()");
			string value = "Passed";
			response.AddHeader("X-MSExchApplicationHealthHandler", this.appName);
			Func<ExWebHealthHandler.CustomHealthCheckResult> customHealthCallback = this.CustomHealthCallback;
			if (customHealthCallback != null)
			{
				ExTraceGlobals.WebHealthTracer.TraceDebug((long)this.GetHashCode(), "ExWebHealthHandler::ProcessHealth calling custom health check callback ");
				try
				{
					ExWebHealthHandler.CustomHealthCheckResult customHealthCheckResult = customHealthCallback();
					ExTraceGlobals.WebHealthTracer.TraceDebug<ExWebHealthHandler.CustomHealthCheckResult>((long)this.GetHashCode(), "ExWebHealthHandler::ProcessHealth Custom health check status = {0}", customHealthCheckResult);
					if (customHealthCheckResult == ExWebHealthHandler.CustomHealthCheckResult.Healthy)
					{
						value = "Passed";
					}
					else if (customHealthCheckResult == ExWebHealthHandler.CustomHealthCheckResult.NotHealthy)
					{
						value = "Failed";
					}
					else
					{
						value = "NonDeterministic";
					}
				}
				catch (Exception ex)
				{
					Exception ex2;
					Exception ex = ex2;
					ExTraceGlobals.WebHealthTracer.TraceError<Exception>((long)this.GetHashCode(), "ExWebHealthHandler::ProcessHealth Callback encountered error {0}", ex);
					if (ex is OutOfMemoryException)
					{
						value = "Failed";
					}
					else
					{
						value = "NonDeterministic";
						if (this.ShouldSubmitWatson(ex))
						{
							ThreadPool.QueueUserWorkItem(delegate(object o)
							{
								ExWatson.SendReport(ex, ReportOptions.DoNotCollectDumps, string.Empty);
							});
						}
					}
				}
			}
			response.AddHeader("X-MSExchApplicationHealthHandlerStatus", value);
			response.StatusCode = 200;
		}

		protected virtual bool ShouldSubmitWatson(Exception ex)
		{
			return true;
		}

		private readonly string appName;

		internal enum CustomHealthCheckResult
		{
			Healthy,
			NotHealthy,
			HealthCannotBeDetermined
		}

		internal static class Headers
		{
			public const string HealthHandler = "X-MSExchApplicationHealthHandler";

			public const string HealthHandlerStatus = "X-MSExchApplicationHealthHandlerStatus";

			public const string HealthHandlerStatusFailureValue = "Failed";

			public const string HealthHandlerStatusSuccessValue = "Passed";

			public const string HealthHandlerStatusCannotBeDetermined = "NonDeterministic";
		}
	}
}
