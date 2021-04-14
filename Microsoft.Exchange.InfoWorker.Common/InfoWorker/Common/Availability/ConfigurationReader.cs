using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class ConfigurationReader
	{
		internal static void Start(RequestLogger requestLogger)
		{
			if (ConfigurationReader.timer == null)
			{
				lock (ConfigurationReader.locker)
				{
					if (ConfigurationReader.timer == null)
					{
						ConfigurationReader.Initialize(requestLogger);
						using (ActivityContext.SuppressThreadScope())
						{
							ConfigurationReader.timer = new Timer(new TimerCallback(ConfigurationReader.RefreshTimer), null, ConfigurationReader.dueTime, ConfigurationReader.refreshInterval);
						}
						ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "Timer object for refreshing configuration has been created successfully.");
					}
				}
			}
		}

		private static void RefreshTimer(object notUsed)
		{
			ThreadContext.SetWithExceptionHandling("ConfigurationReader.RefreshTimer", DummyApplication.Instance.Worker, null, null, new ThreadContext.ExecuteDelegate(ConfigurationReader.Refresh));
		}

		internal static void Refresh()
		{
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is now refreshing TargetForestConfigurationCache.");
			DateTime populateDeadline = DateTime.UtcNow + ConfigurationReader.refreshTimeout;
			TargetForestConfigurationCache.Populate(populateDeadline);
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is done refreshing TargetForestConfigurationCache.");
		}

		private static void Initialize(RequestLogger requestLogger)
		{
			requestLogger.CaptureRequestStage("CRInit");
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is now initializing NetworkServiceImpersonator.");
			NetworkServiceImpersonator.Initialize();
			requestLogger.CaptureRequestStage("CRNSInit");
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is done initializing NetworkServiceImpersonator.");
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is now initializing Dns for AS discovery.");
			AutoDiscoverDnsReader.Initialize();
			requestLogger.CaptureRequestStage("CRAD");
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is done initializing Dns for AS discovery.");
			DateTime populateDeadline = DateTime.UtcNow + ConfigurationReader.initializeTimeInterval;
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is now refreshing TargetForestConfigurationCache.");
			TargetForestConfigurationCache.Populate(populateDeadline);
			ConfigurationReader.ConfigurationTracer.TraceDebug(0L, "ConfigurationReader is done refreshing TargetForestConfigurationCache.");
			requestLogger.CaptureRequestStage("CRTC");
			ConfigurationReader.ASFaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjection.Callback));
			ConfigurationReader.RequestDispatchFaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjection.Callback));
		}

		internal static void HandleException(Exception e)
		{
			ConfigurationReader.ConfigurationTracer.TraceError<Exception>(0L, "Exception occurred while reading AD configuration: {0}", e);
		}

		private static readonly TimeSpan refreshInterval = TimeSpan.FromMinutes((double)Configuration.ADRefreshIntervalInMinutes);

		private static readonly TimeSpan initializeTimeInterval = TimeSpan.FromSeconds(50.0);

		private static readonly TimeSpan refreshTimeout = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan dueTime = ConfigurationReader.refreshInterval;

		private static readonly Trace ConfigurationTracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability.ExTraceGlobals.ConfigurationTracer;

		private static readonly FaultInjectionTrace ASFaultInjectionTracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability.ExTraceGlobals.FaultInjectionTracer;

		private static readonly FaultInjectionTrace RequestDispatchFaultInjectionTracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch.ExTraceGlobals.FaultInjectionTracer;

		private static object locker = new object();

		private static Timer timer;
	}
}
