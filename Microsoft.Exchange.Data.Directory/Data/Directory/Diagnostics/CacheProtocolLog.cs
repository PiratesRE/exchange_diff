using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Diagnostics
{
	internal sealed class CacheProtocolLog : BaseDirectoryProtocolLog
	{
		protected override LogSchema Schema
		{
			get
			{
				return CacheProtocolLog.schema;
			}
		}

		internal static void BeginAppend(string operation, string dn, DateTime whenReadUTC, long totalProcessingTime, long wcfGetProcessingTime, long wcfRemoveProcessingTime, long wcfPutProcessingTime, long adProcessingTime, bool isNewProxyObject, int retryCount, string objectType, string cachePerformanceTracker, Guid activityId, string callerInfo, string error = null)
		{
			if (CacheProtocolLog.instance == null)
			{
				CacheProtocolLog value = new CacheProtocolLog();
				Interlocked.CompareExchange<CacheProtocolLog>(ref CacheProtocolLog.instance, value, null);
			}
			CacheProtocolLog.AppendDelegate appendDelegate = new CacheProtocolLog.AppendDelegate(CacheProtocolLog.instance.AppendInstance);
			appendDelegate.BeginInvoke(operation, dn, whenReadUTC, totalProcessingTime, wcfGetProcessingTime, wcfRemoveProcessingTime, wcfPutProcessingTime, adProcessingTime, isNewProxyObject, retryCount, objectType, cachePerformanceTracker, activityId, callerInfo, error, null, null);
		}

		private void AppendInstance(string operation, string dn, DateTime whenReadUTC, long totalProcessingTime, long wcfGetProcessingTime, long wcfRemoveProcessingTime, long wcfPutProcessingTime, long adProcessingTime, bool isNewProxy, int retryCount, string objectType, string cachePerformanceTracker, Guid activityId, string callerInfo, string error)
		{
			if (!base.Initialized)
			{
				this.Initialize();
			}
			if (BaseDirectoryProtocolLog.LoggingEnabled && !this.protocolLogDisabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(CacheProtocolLog.schema);
				logRowFormatter[1] = CacheProtocolLog.instance.GetNextSequenceNumber();
				logRowFormatter[2] = Globals.ProcessName;
				logRowFormatter[4] = Globals.ProcessAppName;
				logRowFormatter[3] = Globals.ProcessId;
				logRowFormatter[5] = operation;
				logRowFormatter[6] = totalProcessingTime;
				logRowFormatter[7] = wcfGetProcessingTime;
				logRowFormatter[8] = wcfPutProcessingTime;
				logRowFormatter[9] = wcfRemoveProcessingTime;
				logRowFormatter[10] = adProcessingTime;
				logRowFormatter[11] = isNewProxy;
				logRowFormatter[12] = retryCount;
				logRowFormatter[13] = objectType;
				logRowFormatter[15] = whenReadUTC;
				logRowFormatter[14] = dn;
				logRowFormatter[16] = cachePerformanceTracker;
				logRowFormatter[17] = activityId;
				logRowFormatter[18] = callerInfo;
				logRowFormatter[19] = error;
				base.Logger.Append(logRowFormatter, 0);
			}
		}

		private void Initialize()
		{
			lock (this.logLock)
			{
				if (!base.Initialized)
				{
					this.ReadConfigData();
					base.Initialize(ExDateTime.UtcNow, Path.Combine(BaseDirectoryProtocolLog.GetExchangeInstallPath(), "Logging\\DirCache\\"), BaseDirectoryProtocolLog.DefaultMaxRetentionPeriod, BaseDirectoryProtocolLog.DefaultDirectorySizeQuota, BaseDirectoryProtocolLog.DefaultPerFileSizeQuota, true, "DirCacheLogs");
				}
			}
		}

		protected override void UpdateConfigIfChanged(object state)
		{
			base.UpdateConfigIfChanged(state);
			this.ReadConfigData();
		}

		private void ReadConfigData()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
			{
				this.protocolLogDisabled = BaseDirectoryProtocolLog.GetRegistryBool(registryKey, "CacheProtocolLogDisabled", false);
			}
		}

		private const string CacheProtocolLogDisabled = "CacheProtocolLogDisabled";

		private const string LogTypeName = "DirCache Logs";

		private const string LogComponent = "DirCacheLogs";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\DirCache\\";

		private static CacheProtocolLog instance = null;

		internal static readonly BaseDirectoryProtocolLog.FieldInfo[] Fields = new BaseDirectoryProtocolLog.FieldInfo[]
		{
			new BaseDirectoryProtocolLog.FieldInfo(0, "date-time"),
			new BaseDirectoryProtocolLog.FieldInfo(1, "seq-number"),
			new BaseDirectoryProtocolLog.FieldInfo(2, "process-name"),
			new BaseDirectoryProtocolLog.FieldInfo(3, "process-id"),
			new BaseDirectoryProtocolLog.FieldInfo(4, "application-name"),
			new BaseDirectoryProtocolLog.FieldInfo(5, "operation"),
			new BaseDirectoryProtocolLog.FieldInfo(6, "processing-time"),
			new BaseDirectoryProtocolLog.FieldInfo(7, "wcfgetprocessing-time"),
			new BaseDirectoryProtocolLog.FieldInfo(8, "wcfputprocessing-time"),
			new BaseDirectoryProtocolLog.FieldInfo(9, "wcfremoveprocessing-time"),
			new BaseDirectoryProtocolLog.FieldInfo(10, "adprocessing-time"),
			new BaseDirectoryProtocolLog.FieldInfo(11, "is-new-proxy-object"),
			new BaseDirectoryProtocolLog.FieldInfo(12, "retry-count"),
			new BaseDirectoryProtocolLog.FieldInfo(13, "objecttype"),
			new BaseDirectoryProtocolLog.FieldInfo(14, "distinguished-name"),
			new BaseDirectoryProtocolLog.FieldInfo(15, "whenread-utc"),
			new BaseDirectoryProtocolLog.FieldInfo(16, "cacheperf-details"),
			new BaseDirectoryProtocolLog.FieldInfo(17, "activity-id"),
			new BaseDirectoryProtocolLog.FieldInfo(18, "caller-info"),
			new BaseDirectoryProtocolLog.FieldInfo(19, "error")
		};

		private static readonly LogSchema schema = new LogSchema("Microsoft Exchange", "15.00.1497.012", "DirCache Logs", BaseDirectoryProtocolLog.GetColumnArray(CacheProtocolLog.Fields));

		private object logLock = new object();

		private bool protocolLogDisabled;

		private enum Field : byte
		{
			DateTime,
			SequenceNumber,
			ClientName,
			Pid,
			AppName,
			Operation,
			TotalProcessingTime,
			WCFGetProcessingTime,
			WCFPutProcessingTime,
			WCFRemoveProcessingTime,
			ADProcessingTime,
			IsNewProxyObject,
			RetryCount,
			ObjectType,
			DN,
			WhenReadUTC,
			CachePerformanceDetails,
			ActivityId,
			CallerInfo,
			Error
		}

		internal delegate void AppendDelegate(string operation, string dn, DateTime whenReadUTC, long totalProcessingTime, long wcfGetProcessingTime, long wcfRemoveProcessingTime, long wcfPutProcessingTime, long adProcessingTime, bool isNewProxyObject, int retryCount, string objectType, string cachePerformanceTracker, Guid requestId, string callerInfo, string error);
	}
}
