using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Compliance;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Diagnostics
{
	internal sealed class ADScenarioLog : BaseDirectoryProtocolLog
	{
		protected override LogSchema Schema
		{
			get
			{
				return ADScenarioLog.schema;
			}
		}

		internal static T InvokeGetObjectAPIAndLog<T>(DateTime whenUTC, string name, Guid activityId, string implementation, string caller, Func<T> action, Func<string> getDcFunc) where T : ADRawEntry
		{
			T t = default(T);
			Exception ex = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				t = action();
			}
			catch (Exception ex2)
			{
				ex = ex2;
				throw;
			}
			finally
			{
				stopwatch.Stop();
				try
				{
					string server;
					if (t != null && t.IsCached)
					{
						server = string.Empty;
					}
					else
					{
						server = getDcFunc();
					}
					ADScenarioLog.BeginAppend(whenUTC, name, implementation, stopwatch.ElapsedMilliseconds, activityId, caller, (ex == null) ? "" : ex.ToString(), server);
				}
				catch (Exception arg)
				{
					ExTraceGlobals.GeneralTracer.TraceError<Exception>((long)default(Guid).GetHashCode(), "Failed to create API logging with exception {0}", arg);
				}
			}
			return t;
		}

		internal static T InvokeWithAPILog<T>(DateTime whenUTC, string name, Guid activityId, string implementation, string caller, Func<T> action, Func<string> getDcFunc)
		{
			T result = default(T);
			Exception ex = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				result = action();
			}
			catch (Exception ex2)
			{
				ex = ex2;
				throw;
			}
			finally
			{
				stopwatch.Stop();
				try
				{
					ADScenarioLog.BeginAppend(whenUTC, name, implementation, stopwatch.ElapsedMilliseconds, activityId, caller, (ex == null) ? "" : ex.ToString(), getDcFunc());
				}
				catch (Exception arg)
				{
					ExTraceGlobals.GeneralTracer.TraceError<Exception>((long)default(Guid).GetHashCode(), "Failed to create API logging with exception {0}", arg);
				}
			}
			return result;
		}

		private void AppendInstance(DateTime whenUTC, string name, string implementor, long processingTime, Guid activityId, string callerInfo, string error, string server)
		{
			if (!base.Initialized)
			{
				this.Initialize();
			}
			if (BaseDirectoryProtocolLog.LoggingEnabled && !this.protocolLogDisabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(ADScenarioLog.schema);
				logRowFormatter[1] = ADScenarioLog.instance.GetNextSequenceNumber();
				logRowFormatter[2] = Globals.ProcessName;
				logRowFormatter[4] = Globals.ProcessAppName;
				logRowFormatter[3] = Globals.ProcessId;
				logRowFormatter[5] = name;
				logRowFormatter[6] = implementor;
				logRowFormatter[7] = processingTime;
				logRowFormatter[8] = callerInfo;
				logRowFormatter[9] = error;
				logRowFormatter[10] = server;
				base.Logger.Append(logRowFormatter, 0);
			}
		}

		internal static void BeginAppend(DateTime whenUTC, string name, string implementor, long processingTime, Guid activityId, string callerInfo, string error = null, string server = null)
		{
			if (ADScenarioLog.instance == null)
			{
				ADScenarioLog value = new ADScenarioLog();
				Interlocked.CompareExchange<ADScenarioLog>(ref ADScenarioLog.instance, value, null);
			}
			ADScenarioLog.AppendDelegate appendDelegate = new ADScenarioLog.AppendDelegate(ADScenarioLog.instance.AppendInstance);
			appendDelegate.BeginInvoke(whenUTC, name, implementor, processingTime, activityId, callerInfo, error, server, null, null);
		}

		private void Initialize()
		{
			lock (this.logLock)
			{
				if (!base.Initialized)
				{
					base.Initialize(ExDateTime.UtcNow, Path.Combine(BaseDirectoryProtocolLog.GetExchangeInstallPath(), "Logging\\ADScenario\\"), BaseDirectoryProtocolLog.DefaultMaxRetentionPeriod, BaseDirectoryProtocolLog.DefaultDirectorySizeQuota, BaseDirectoryProtocolLog.DefaultPerFileSizeQuota, true, "ADScenarioLogs");
				}
			}
			this.protocolLogDisabled = true;
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
				this.protocolLogDisabled = BaseDirectoryProtocolLog.GetRegistryBool(registryKey, "ADScenarioLogDisabled", true);
			}
		}

		private const string ADScenarioLogDisabled = "ADScenarioLogDisabled";

		private const string LogTypeName = "ADScenario Logs";

		private const string LogComponent = "ADScenarioLogs";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\ADScenario\\";

		private static ADScenarioLog instance = null;

		internal static readonly BaseDirectoryProtocolLog.FieldInfo[] Fields = new BaseDirectoryProtocolLog.FieldInfo[]
		{
			new BaseDirectoryProtocolLog.FieldInfo(0, "date-time"),
			new BaseDirectoryProtocolLog.FieldInfo(1, "seq-number"),
			new BaseDirectoryProtocolLog.FieldInfo(2, "process-name"),
			new BaseDirectoryProtocolLog.FieldInfo(3, "process-id"),
			new BaseDirectoryProtocolLog.FieldInfo(4, "application-name"),
			new BaseDirectoryProtocolLog.FieldInfo(5, "api-name"),
			new BaseDirectoryProtocolLog.FieldInfo(6, "implementor"),
			new BaseDirectoryProtocolLog.FieldInfo(7, "processing-time"),
			new BaseDirectoryProtocolLog.FieldInfo(8, "caller-info"),
			new BaseDirectoryProtocolLog.FieldInfo(9, "error"),
			new BaseDirectoryProtocolLog.FieldInfo(10, "server")
		};

		private static readonly LogSchema schema = new LogSchema("Microsoft Exchange", "15.00.1497.012", "ADScenario Logs", BaseDirectoryProtocolLog.GetColumnArray(ADScenarioLog.Fields));

		private object logLock = new object();

		private bool protocolLogDisabled;

		private enum Field : byte
		{
			DateTime,
			SequenceNumber,
			ClientName,
			Pid,
			AppName,
			ApiName,
			Implementor,
			ProcessingTime,
			CallerInfo,
			Error,
			Server
		}

		internal delegate void AppendDelegate(DateTime whenUTC, string name, string implementor, long processingTime, Guid activityId, string callerInfo, string error, string server);
	}
}
