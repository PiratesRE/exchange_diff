using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.EventLogs;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class ConcurrencyGuards
	{
		public static ConcurrencyGuard TargetBackend
		{
			get
			{
				return ConcurrencyGuards.targetBackend;
			}
		}

		public static ConcurrencyGuard TargetDag
		{
			get
			{
				return ConcurrencyGuards.targetDag;
			}
		}

		public static ConcurrencyGuard TargetForest
		{
			get
			{
				return ConcurrencyGuards.targetForest;
			}
		}

		public static ConcurrencyGuard SharedCache
		{
			get
			{
				return ConcurrencyGuards.sharedCache;
			}
		}

		private static void LogTargetOustandingRequests(ConcurrencyGuard guard, string bucketName, object stateObject)
		{
			RequestDetailsLogger requestDetailsLogger = stateObject as RequestDetailsLogger;
			if (requestDetailsLogger == null)
			{
				return;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(requestDetailsLogger, HttpProxyMetadata.TargetOutstandingRequests, guard.GetCurrentValue(bucketName));
		}

		private static void LogEventOnRejectDelegate(ConcurrencyGuard guard, string bucketName, object stateObject, Exception ex)
		{
			string text = ConcurrencyGuard.FormatGuardBucketName(guard, bucketName);
			Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_TooManyOutstandingRequests, text, new object[]
			{
				HttpProxyGlobals.ProtocolType,
				text,
				guard.MaxConcurrency
			});
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ConcurrencyGuards()
		{
			string guardName = "TargetBackend";
			int value = ConcurrencyGuards.TargetBackendLimit.Value;
			Action<ConcurrencyGuard, string, object> onIncrementDelegate = new Action<ConcurrencyGuard, string, object>(ConcurrencyGuards.LogTargetOustandingRequests);
			Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException> onRejectDelegate = new Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException>(ConcurrencyGuards.LogEventOnRejectDelegate);
			ConcurrencyGuards.targetBackend = new ConcurrencyGuard(guardName, value, ConcurrencyGuards.UseTrainingMode, onIncrementDelegate, null, null, onRejectDelegate);
			string guardName2 = "TargetDag";
			int value2 = ConcurrencyGuards.TargetDagLimit.Value;
			Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException> onRejectDelegate2 = new Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException>(ConcurrencyGuards.LogEventOnRejectDelegate);
			ConcurrencyGuards.targetDag = new ConcurrencyGuard(guardName2, value2, ConcurrencyGuards.UseTrainingMode, null, null, null, onRejectDelegate2);
			string guardName3 = "TargetForest";
			int value3 = ConcurrencyGuards.TargetForestLimit.Value;
			Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException> onRejectDelegate3 = new Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException>(ConcurrencyGuards.LogEventOnRejectDelegate);
			ConcurrencyGuards.targetForest = new ConcurrencyGuard(guardName3, value3, ConcurrencyGuards.UseTrainingMode, null, null, null, onRejectDelegate3);
			string guardName4 = "SharedCache";
			int value4 = ConcurrencyGuards.SharedCacheLimit.Value;
			Action<ConcurrencyGuard, string, object> onIncrementDelegate2 = delegate(ConcurrencyGuard a, string b, object c)
			{
				PerfCounters.HttpProxyCountersInstance.OutstandingSharedCacheRequests.Increment();
			};
			Action<ConcurrencyGuard, string, object> onDecrementDelegate = delegate(ConcurrencyGuard a, string b, object c)
			{
				PerfCounters.HttpProxyCountersInstance.OutstandingSharedCacheRequests.Decrement();
			};
			ConcurrencyGuards.sharedCache = new ConcurrencyGuard(guardName4, value4, ConcurrencyGuards.UseTrainingMode, onIncrementDelegate2, onDecrementDelegate, null, null);
		}

		private static readonly IntAppSettingsEntry TargetBackendLimit = new IntAppSettingsEntry(HttpProxySettings.Prefix("ConcurrencyGuards.TargetBackendLimit"), 150, ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry TargetDagLimit = new IntAppSettingsEntry(HttpProxySettings.Prefix("ConcurrencyGuards.TargetDagLimit"), 5000, ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry TargetForestLimit = new IntAppSettingsEntry(HttpProxySettings.Prefix("ConcurrencyGuards.TargetForestLimit"), 15000, ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry SharedCacheLimit = new IntAppSettingsEntry(HttpProxySettings.Prefix("ConcurrencyGuards.SharedCacheLimit"), 100, ExTraceGlobals.VerboseTracer);

		private static readonly bool UseTrainingMode = !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.EnforceConcurrencyGuards.Enabled;

		private static ConcurrencyGuard targetBackend;

		private static ConcurrencyGuard targetDag;

		private static ConcurrencyGuard targetForest;

		private static ConcurrencyGuard sharedCache;
	}
}
