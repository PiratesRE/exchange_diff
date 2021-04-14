using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class LoggerManager : DisposableBase, ILoggerFactory, IDisposable
	{
		private LoggerManager(IBinaryLogger longOperationLogger, IBinaryLogger ropSummaryLogger, IBinaryLogger fullTextIndexLogger, IBinaryLogger diagnosticQueryLogger, IBinaryLogger referenceDataLogger, IBinaryLogger heavyClientActivityLogger, IBinaryLogger breadCrumbsLogger, IBinaryLogger syntheticCountersLogger)
		{
			this.longOperationLogger = longOperationLogger;
			this.ropSummaryLogger = ropSummaryLogger;
			this.fullTextIndexLogger = fullTextIndexLogger;
			this.diagnosticQueryLogger = diagnosticQueryLogger;
			this.referenceDataLogger = referenceDataLogger;
			this.heavyClientActivityLogger = heavyClientActivityLogger;
			this.breadCrumbsLogger = breadCrumbsLogger;
			this.syntheticCountersLogger = syntheticCountersLogger;
		}

		private static Hookable<ILoggerFactory> LoggerFactory
		{
			get
			{
				if (LoggerManager.loggerFactory == null)
				{
					using (LockManager.Lock(LoggerManager.loggerFactoryLockObject))
					{
						if (LoggerManager.loggerFactory == null)
						{
							Interlocked.Exchange<Hookable<ILoggerFactory>>(ref LoggerManager.loggerFactory, Hookable<ILoggerFactory>.Create(true, LoggerManager.Create()));
						}
					}
				}
				return LoggerManager.loggerFactory;
			}
		}

		private static ILoggerFactory LoggerFactoryInstance
		{
			get
			{
				return LoggerManager.LoggerFactory.Value;
			}
		}

		private IBinaryLogger LongOperation
		{
			get
			{
				return this.longOperationLogger;
			}
		}

		private IBinaryLogger RopSummary
		{
			get
			{
				return this.ropSummaryLogger;
			}
		}

		private IBinaryLogger FullTextIndex
		{
			get
			{
				return this.fullTextIndexLogger;
			}
		}

		private IBinaryLogger DiagnosticQuery
		{
			get
			{
				return this.diagnosticQueryLogger;
			}
		}

		private IBinaryLogger ReferenceData
		{
			get
			{
				return this.referenceDataLogger;
			}
		}

		private IBinaryLogger HeavyClientActivity
		{
			get
			{
				return this.heavyClientActivityLogger;
			}
		}

		private IBinaryLogger BreadCrumbs
		{
			get
			{
				return this.breadCrumbsLogger;
			}
		}

		private IBinaryLogger SyntheticCounters
		{
			get
			{
				return this.syntheticCountersLogger;
			}
		}

		public static void Terminate()
		{
			LoggerManager.LoggerFactoryInstance.Dispose();
		}

		public static void DoTraceLogDirectoryMaintenance()
		{
			foreach (EtwLoggerDefinition etwLoggerDefinition in LoggerManager.LoggerDefinitions.All)
			{
				LoggerManager.InternalTraceLogDirectoryMaintenance(LoggerManager.GetLogPath(), etwLoggerDefinition.LogFilePrefixName, etwLoggerDefinition.MaximumTotalFilesSizeMB * 1024U * 1024U, etwLoggerDefinition.RetentionLimit);
			}
		}

		public static void StartAllTraceSessions()
		{
			foreach (EtwLoggerDefinition definition in LoggerManager.LoggerDefinitions.All)
			{
				if (LoggerManager.LoggerFactoryInstance.IsTracingEnabled(definition.LoggerType))
				{
					LoggerManager.StartTraceSession(LoggerManager.GetLogFileName(definition), definition.LogFilePrefixName, definition);
				}
			}
		}

		public static void StopAllTraceSessions()
		{
			foreach (EtwLoggerDefinition etwLoggerDefinition in LoggerManager.LoggerDefinitions.All)
			{
				LoggerManager.StopTraceSession(etwLoggerDefinition.LogFilePrefixName);
			}
		}

		public static IBinaryLogger GetLogger(LoggerType type)
		{
			switch (type)
			{
			case LoggerType.LongOperation:
			case LoggerType.RopSummary:
			case LoggerType.FullTextIndex:
			case LoggerType.DiagnosticQuery:
			case LoggerType.ReferenceData:
			case LoggerType.HeavyClientActivity:
			case LoggerType.BreadCrumbs:
			case LoggerType.SyntheticCounters:
				return LoggerManager.LoggerFactoryInstance.GetLoggerInstance(type);
			default:
				throw new StoreException((LID)40508U, ErrorCodeValue.CallFailed, "Invalid ETW logger type");
			}
		}

		internal static void InternalTraceLogDirectoryMaintenance(string logPath, string filePrefix, uint maximumSizeBytes, TimeSpan maximumRetention)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(logPath);
			List<LoggerManager.TraceFileInfo> list = null;
			try
			{
				list = new List<LoggerManager.TraceFileInfo>(from info in directoryInfo.GetFiles(filePrefix + "*")
				select LoggerManager.TraceFileInfo.Create(info));
			}
			catch (UnauthorizedAccessException ex)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TraceMaintenanceFailed, new object[]
				{
					DiagnosticsNativeMethods.GetCurrentProcessId(),
					filePrefix,
					ex
				});
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
			}
			if (list != null && list.Count > 0)
			{
				LoggerManager.InternalTraceLogDirectoryMaintenance(list, maximumSizeBytes, maximumRetention);
			}
		}

		internal static void InternalTraceLogDirectoryMaintenance(List<LoggerManager.TraceFileInfo> files, uint maximumSizeBytes, TimeSpan maximumRetention)
		{
			long num = 0L;
			DateTime dateTime = DateTime.MaxValue;
			DateTime t = DateTime.UtcNow - maximumRetention;
			foreach (LoggerManager.TraceFileInfo traceFileInfo in files)
			{
				num += traceFileInfo.Length;
				dateTime = ((traceFileInfo.LastWriteTimeUtc < dateTime) ? traceFileInfo.LastWriteTimeUtc : dateTime);
			}
			if (num <= (long)((ulong)maximumSizeBytes) && dateTime >= t)
			{
				return;
			}
			files.Sort((LoggerManager.TraceFileInfo first, LoggerManager.TraceFileInfo second) => DateTime.Compare(first.CreationTimeUtc, second.CreationTimeUtc));
			foreach (LoggerManager.TraceFileInfo traceFileInfo2 in files)
			{
				if (num <= (long)((ulong)maximumSizeBytes) && traceFileInfo2.LastWriteTimeUtc >= t)
				{
					break;
				}
				num -= traceFileInfo2.Length;
				try
				{
					File.Delete(traceFileInfo2.FullName);
				}
				catch (IOException exception)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
				}
				catch (UnauthorizedAccessException exception2)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(exception2);
				}
			}
		}

		internal static IDisposable SetTestHook(ILoggerFactory factory)
		{
			return LoggerManager.LoggerFactory.SetTestHook(factory);
		}

		private static LoggerManager Create()
		{
			return new LoggerManager(LoggerManager.CreateLogger(LoggerType.LongOperation), LoggerManager.CreateLogger(LoggerType.RopSummary), LoggerManager.CreateLogger(LoggerType.FullTextIndex), LoggerManager.CreateLogger(LoggerType.DiagnosticQuery), LoggerManager.CreateLogger(LoggerType.ReferenceData), LoggerManager.CreateLogger(LoggerType.HeavyClientActivity), LoggerManager.CreateLogger(LoggerType.BreadCrumbs), LoggerManager.CreateLogger(LoggerType.SyntheticCounters));
		}

		private static IBinaryLogger CreateLogger(LoggerType type)
		{
			IBinaryLogger binaryLogger = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				binaryLogger = disposeGuard.Add<IBinaryLogger>(LoggerManager.InternalCreate(type));
				if (binaryLogger != null)
				{
					binaryLogger.Start();
				}
				disposeGuard.Success();
			}
			return binaryLogger;
		}

		private static IBinaryLogger InternalCreate(LoggerType type)
		{
			switch (type)
			{
			case LoggerType.LongOperation:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.LongOperation);
			case LoggerType.RopSummary:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.RopSummary);
			case LoggerType.FullTextIndex:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.FullTextIndex);
			case LoggerType.DiagnosticQuery:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.DiagnosticQuery);
			case LoggerType.ReferenceData:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.ReferenceData);
			case LoggerType.HeavyClientActivity:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.HeavyClientActivity);
			case LoggerType.BreadCrumbs:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.BreadCrumbs);
			case LoggerType.SyntheticCounters:
				return LoggerManager.InternalCreate(LoggerManager.LoggerDefinitions.SyntheticCounters);
			default:
				throw new ArgumentException("type");
			}
		}

		private static IBinaryLogger InternalCreate(EtwLoggerDefinition definition)
		{
			return EtwBinaryLogger.Create(definition.LogFilePrefixName, definition.ProviderGuid);
		}

		private static string GetLogPath()
		{
			string oldValue = "%ExchangeInstallDir%";
			string value = ConfigurationSchema.LogPath.Value;
			string value2 = RegistryReader.Instance.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", string.Empty);
			string text = value.Replace(oldValue, value2);
			Directory.CreateDirectory(text);
			return text;
		}

		private static string GetLogFileName(EtwLoggerDefinition definition)
		{
			string str = definition.LogFilePrefixName + DateTime.UtcNow.ToString("_yyyyMMdd-HHmmss-fffffff");
			if (definition.FileModeCreateNew)
			{
				str += "_%d";
			}
			return Path.Combine(LoggerManager.GetLogPath(), str + ".etl");
		}

		private static DiagnosticsNativeMethods.EventTraceProperties CreateTraceProperties(string logFileName, string sessionName, EtwLoggerDefinition definition)
		{
			DiagnosticsNativeMethods.EventTraceProperties eventTraceProperties = default(DiagnosticsNativeMethods.EventTraceProperties);
			DiagnosticsNativeMethods.LogFileMode logFileMode = definition.FileModeCreateNew ? DiagnosticsNativeMethods.LogFileMode.EVENT_TRACE_FILE_MODE_NEWFILE : DiagnosticsNativeMethods.LogFileMode.EVENT_TRACE_FILE_MODE_CIRCULAR;
			eventTraceProperties.etp.wnode.bufferSize = (uint)Marshal.SizeOf(eventTraceProperties);
			eventTraceProperties.etp.wnode.guid = Guid.NewGuid();
			eventTraceProperties.etp.wnode.flags = 131072U;
			eventTraceProperties.etp.wnode.clientContext = 1U;
			eventTraceProperties.etp.bufferSize = definition.MemoryBufferSizeKB;
			eventTraceProperties.etp.minimumBuffers = definition.MinimumNumberOfMemoryBuffers;
			eventTraceProperties.etp.maximumBuffers = definition.NumberOfMemoryBuffers;
			eventTraceProperties.etp.maximumFileSize = definition.LogFileSizeMB;
			eventTraceProperties.etp.logFileMode = (uint)(logFileMode | DiagnosticsNativeMethods.LogFileMode.EVENT_TRACE_USE_PAGED_MEMORY | DiagnosticsNativeMethods.LogFileMode.EVENT_TRACE_USE_LOCAL_SEQUENCE);
			eventTraceProperties.etp.flushTimer = (uint)definition.FlushTimer.TotalSeconds;
			eventTraceProperties.etp.enableFlags = 0U;
			eventTraceProperties.etp.logFileNameOffset = (uint)((int)Marshal.OffsetOf(typeof(DiagnosticsNativeMethods.EventTraceProperties), "logFileName"));
			eventTraceProperties.etp.loggerNameOffset = (uint)((int)Marshal.OffsetOf(typeof(DiagnosticsNativeMethods.EventTraceProperties), "loggerName"));
			eventTraceProperties.logFileName = logFileName;
			eventTraceProperties.loggerName = sessionName;
			return eventTraceProperties;
		}

		public IBinaryLogger GetLoggerInstance(LoggerType type)
		{
			switch (type)
			{
			case LoggerType.LongOperation:
				return this.longOperationLogger;
			case LoggerType.RopSummary:
				return this.ropSummaryLogger;
			case LoggerType.FullTextIndex:
				return this.fullTextIndexLogger;
			case LoggerType.DiagnosticQuery:
				return this.diagnosticQueryLogger;
			case LoggerType.ReferenceData:
				return this.referenceDataLogger;
			case LoggerType.HeavyClientActivity:
				return this.heavyClientActivityLogger;
			case LoggerType.BreadCrumbs:
				return this.breadCrumbsLogger;
			case LoggerType.SyntheticCounters:
				return this.syntheticCountersLogger;
			default:
				throw new StoreException((LID)43728U, ErrorCodeValue.CallFailed, "Invalid ETW logger type");
			}
		}

		public bool IsTracingEnabled(LoggerType type)
		{
			switch (type)
			{
			case LoggerType.LongOperation:
				return ConfigurationSchema.EnableTraceLongOperation.Value;
			case LoggerType.RopSummary:
				return ConfigurationSchema.EnableTraceRopSummary.Value;
			case LoggerType.FullTextIndex:
				return ConfigurationSchema.EnableTraceFullTextIndexQuery.Value;
			case LoggerType.DiagnosticQuery:
				return ConfigurationSchema.EnableTraceDiagnosticQuery.Value;
			case LoggerType.ReferenceData:
				return ConfigurationSchema.EnableTraceReferenceData.Value;
			case LoggerType.HeavyClientActivity:
				return ConfigurationSchema.EnableTraceHeavyClientActivity.Value;
			case LoggerType.BreadCrumbs:
				return ConfigurationSchema.EnableTraceBreadCrumbs.Value;
			case LoggerType.SyntheticCounters:
				return ConfigurationSchema.EnableTraceSyntheticCounters.Value;
			default:
				throw new ArgumentException("type");
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.longOperationLogger != null)
				{
					this.longOperationLogger.Dispose();
					this.longOperationLogger = null;
				}
				if (this.ropSummaryLogger != null)
				{
					this.ropSummaryLogger.Dispose();
					this.ropSummaryLogger = null;
				}
				if (this.fullTextIndexLogger != null)
				{
					this.fullTextIndexLogger.Dispose();
					this.fullTextIndexLogger = null;
				}
				if (this.diagnosticQueryLogger != null)
				{
					this.diagnosticQueryLogger.Dispose();
					this.diagnosticQueryLogger = null;
				}
				if (this.referenceDataLogger != null)
				{
					this.referenceDataLogger.Dispose();
					this.referenceDataLogger = null;
				}
				if (this.heavyClientActivityLogger != null)
				{
					this.heavyClientActivityLogger.Dispose();
					this.heavyClientActivityLogger = null;
				}
				if (this.breadCrumbsLogger != null)
				{
					this.breadCrumbsLogger.Dispose();
					this.breadCrumbsLogger = null;
				}
				if (this.syntheticCountersLogger != null)
				{
					this.syntheticCountersLogger.Dispose();
					this.syntheticCountersLogger = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LoggerManager>(this);
		}

		internal static void StartTraceSession(string logFileName, string sessionName, EtwLoggerDefinition definition)
		{
			int currentProcessId = DiagnosticsNativeMethods.GetCurrentProcessId();
			DiagnosticsNativeMethods.EventTraceProperties eventTraceProperties = LoggerManager.CreateTraceProperties(logFileName, sessionName, definition);
			long sessionHandle;
			uint num = DiagnosticsNativeMethods.StartTrace(out sessionHandle, sessionName, ref eventTraceProperties);
			if (num != 0U)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_StartTraceSessionFailed, new object[]
				{
					"StartTrace",
					sessionName,
					definition.ProviderGuid,
					currentProcessId,
					num
				});
				return;
			}
			Guid providerGuid = definition.ProviderGuid;
			num = DiagnosticsNativeMethods.EnableTrace(1U, 0U, 5U, ref providerGuid, sessionHandle);
			if (num != 0U)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_StartTraceSessionFailed, new object[]
				{
					"EnableTrace",
					sessionName,
					definition.ProviderGuid,
					currentProcessId,
					num
				});
			}
		}

		internal static void StopTraceSession(string sessionName)
		{
			DiagnosticsNativeMethods.EventTraceProperties eventTraceProperties = LoggerManager.CreateTraceProperties(string.Empty, sessionName, default(EtwLoggerDefinition));
			DiagnosticsNativeMethods.ControlTrace(0L, sessionName, ref eventTraceProperties, 1U);
		}

		internal static void FlushSessionLog(string logFileName, string sessionName)
		{
			DiagnosticsNativeMethods.EventTraceProperties eventTraceProperties = LoggerManager.CreateTraceProperties(logFileName, sessionName, default(EtwLoggerDefinition));
			uint num = DiagnosticsNativeMethods.ControlTrace(0L, sessionName, ref eventTraceProperties, 3U);
			if (num != 0U)
			{
				throw new StoreException((LID)62584U, ErrorCodeValue.CallFailed, string.Format("ControlTrace(FLUSH) failed: {0}", num));
			}
		}

		private static object loggerFactoryLockObject = new object();

		private static Hookable<ILoggerFactory> loggerFactory;

		private IBinaryLogger longOperationLogger;

		private IBinaryLogger ropSummaryLogger;

		private IBinaryLogger fullTextIndexLogger;

		private IBinaryLogger diagnosticQueryLogger;

		private IBinaryLogger referenceDataLogger;

		private IBinaryLogger heavyClientActivityLogger;

		private IBinaryLogger breadCrumbsLogger;

		private IBinaryLogger syntheticCountersLogger;

		internal static class TraceGuids
		{
			public static readonly Guid LongOperationDetail = new Guid("{4edb6394-0bf6-4feb-ad88-5eb8a73ae5fe}");

			public static readonly Guid LongOperationSummary = new Guid("{a7386ff4-27f7-4546-9573-05eac353d033}");

			public static readonly Guid RopSummary = new Guid("{b91ae2e7-f3c7-4ad5-aa79-748b093b60ef}");

			public static readonly Guid DiagnosticQuery = new Guid("{a3bdbaa4-d7be-4103-8930-4ce1637e6ae7}");

			public static readonly Guid FullTextIndexQuery = new Guid("{969a0e45-d76e-4b62-9628-6ab4986ad6f0}");

			public static readonly Guid ClientType = new Guid("{5e419c98-31b0-421e-8ac7-724c4bc308a1}");

			public static readonly Guid DatabaseInfo = new Guid("{b3d00223-c4f4-4684-a7be-2d35125dbe7f}");

			public static readonly Guid ErrorCode = new Guid("{af8c813c-1ad2-49ce-9346-29a119cd4a58}");

			public static readonly Guid RopId = new Guid("{f829d186-4bc8-4a68-b324-baecac2484d5}");

			public static readonly Guid MailboxInfo = new Guid("{b3739cd6-30a8-43a7-a9d8-621d7852953c}");

			public static readonly Guid FullTextIndexDetail = new Guid("{b1c468c0-e08c-42dc-88ca-d4ee662582b9}");

			public static readonly Guid FullTextIndexSingleLine = new Guid("{260accff-993c-478a-be98-191c6a8a353a}");

			public static readonly Guid ActivityInfo = new Guid("{c9fe591e-0508-4431-ba5b-0f7e1460c9a2}");

			public static readonly Guid HeavyClientActivityDetail = new Guid("{415858e9-d31c-4bff-b7ca-96a35939d9a0}");

			public static readonly Guid HeavyClientActivitySummary = new Guid("{166c6b8f-e9b8-4084-b61e-3d44f1f66b6e}");

			public static readonly Guid BreadCrumbs = new Guid("{d644df02-3a11-45b2-9970-a16c39b7b222}");

			public static readonly Guid ServerInfo = new Guid("{29b93039-3837-4656-8496-2714244c9ed6}");

			public static readonly Guid OperationType = new Guid("{9bb5db73-7b59-46ae-bf7c-3988da83983d}");

			public static readonly Guid OperationDetail = new Guid("{afec1b6c-a307-4bd4-80e6-bc8d0190453f}");

			public static readonly Guid TaskType = new Guid("{852004a4-f4ea-45a0-99d3-4c264126ee55}");

			public static readonly Guid AdminMethod = new Guid("{5e6fa64a-892a-4877-8751-d829b3c6d35a}");

			public static readonly Guid SyntheticCounters = new Guid("{061016d0-2d4d-4a46-a0dc-b5061303b605}");

			public static readonly Guid MailboxStatus = new Guid("{9d28577c-6344-43bb-9457-329048432c2c}");

			public static readonly Guid MailboxType = new Guid("{48f9bb9e-764e-4e2b-825d-f8cd4e8f9448}");

			public static readonly Guid MailboxTypeDetail = new Guid("{626325ae-bd70-4026-bc95-c962dcf76e44}");

			public static readonly Guid BreadCrumbKind = new Guid("{36a346c1-792b-447d-8e5f-f5c219537487}");

			public static readonly Guid OperationSource = new Guid("{eb325a9f-ca07-4896-a117-c8326f95deac}");
		}

		private static class LoggerDefinitions
		{
			public static EtwLoggerDefinition LongOperation
			{
				get
				{
					return LoggerManager.LoggerDefinitions.longOperationDefinition;
				}
			}

			public static EtwLoggerDefinition RopSummary
			{
				get
				{
					return LoggerManager.LoggerDefinitions.ropSummaryDefinition;
				}
			}

			public static EtwLoggerDefinition FullTextIndex
			{
				get
				{
					return LoggerManager.LoggerDefinitions.fullTextIndexDefinition;
				}
			}

			public static EtwLoggerDefinition DiagnosticQuery
			{
				get
				{
					return LoggerManager.LoggerDefinitions.diagnosticQueryDefinition;
				}
			}

			public static EtwLoggerDefinition ReferenceData
			{
				get
				{
					return LoggerManager.LoggerDefinitions.referenceDataDefinition;
				}
			}

			public static EtwLoggerDefinition HeavyClientActivity
			{
				get
				{
					return LoggerManager.LoggerDefinitions.heavyClientActivityDefinition;
				}
			}

			public static EtwLoggerDefinition BreadCrumbs
			{
				get
				{
					return LoggerManager.LoggerDefinitions.breadCrumbsDefinition;
				}
			}

			public static EtwLoggerDefinition SyntheticCounters
			{
				get
				{
					return LoggerManager.LoggerDefinitions.syntheticCountersDefinition;
				}
			}

			public static EtwLoggerDefinition[] All
			{
				get
				{
					return LoggerManager.LoggerDefinitions.allDefinitions;
				}
			}

			private static EtwLoggerDefinition longOperationDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.LongOperation,
				LogFilePrefixName = "LongOperation",
				ProviderGuid = new Guid("{6551ea1e-9124-4e76-a971-50ef868272f1}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 1000U,
				FileModeCreateNew = true,
				FlushTimer = TimeSpan.FromHours(1.0),
				RetentionLimit = TimeSpan.FromDays(30.0)
			};

			private static EtwLoggerDefinition ropSummaryDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.RopSummary,
				LogFilePrefixName = "RopSummary",
				ProviderGuid = new Guid("{A6EAE9C9-5A1C-452d-A1B2-F65BE3918D74}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 2500U,
				FileModeCreateNew = true,
				RetentionLimit = TimeSpan.FromDays(14.0)
			};

			private static EtwLoggerDefinition fullTextIndexDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.FullTextIndex,
				LogFilePrefixName = "FullTextIndexQuery",
				ProviderGuid = new Guid("{7609B5F1-F8ED-4798-913C-1541723BFF60}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 1000U,
				FileModeCreateNew = true,
				FlushTimer = TimeSpan.FromHours(1.0),
				RetentionLimit = TimeSpan.FromDays(30.0)
			};

			private static EtwLoggerDefinition diagnosticQueryDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.DiagnosticQuery,
				LogFilePrefixName = "DiagnosticQuery",
				ProviderGuid = new Guid("{1B54734A-5DD9-44B5-A0F4-B613624C2AC9}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 1000U,
				FileModeCreateNew = true,
				FlushTimer = TimeSpan.FromHours(1.0),
				RetentionLimit = TimeSpan.FromDays(365.0)
			};

			private static EtwLoggerDefinition referenceDataDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.ReferenceData,
				LogFilePrefixName = "ReferenceData",
				ProviderGuid = new Guid("{E434A360-74A6-4A3F-957B-1F69D1006302}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 1000U,
				FileModeCreateNew = true,
				FlushTimer = TimeSpan.FromHours(6.0),
				RetentionLimit = TimeSpan.FromDays(30.0)
			};

			private static EtwLoggerDefinition heavyClientActivityDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.HeavyClientActivity,
				LogFilePrefixName = "HeavyClientActivity",
				ProviderGuid = new Guid("{d2371af6-80ff-4c1a-8e2e-e7f12bced4ec}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 1000U,
				FileModeCreateNew = true,
				FlushTimer = TimeSpan.FromHours(1.0),
				RetentionLimit = TimeSpan.FromDays(30.0)
			};

			private static EtwLoggerDefinition breadCrumbsDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.BreadCrumbs,
				LogFilePrefixName = "BreadCrumbs",
				ProviderGuid = new Guid("{ee60aded-233a-41d7-98d7-6f72e2b74f32}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 2500U,
				FileModeCreateNew = true,
				FlushTimer = TimeSpan.FromHours(1.0),
				RetentionLimit = TimeSpan.FromDays(14.0)
			};

			private static EtwLoggerDefinition syntheticCountersDefinition = new EtwLoggerDefinition
			{
				LoggerType = LoggerType.SyntheticCounters,
				LogFilePrefixName = "SyntheticCounters",
				ProviderGuid = new Guid("{55a1769d-5ac5-4f8e-b1c9-37d3a2bb1305}"),
				LogFileSizeMB = 10U,
				MemoryBufferSizeKB = 128U,
				MinimumNumberOfMemoryBuffers = 2U,
				NumberOfMemoryBuffers = 100U,
				MaximumTotalFilesSizeMB = 2500U,
				FileModeCreateNew = true,
				FlushTimer = TimeSpan.FromHours(1.0),
				RetentionLimit = TimeSpan.FromDays(14.0)
			};

			private static EtwLoggerDefinition[] allDefinitions = new EtwLoggerDefinition[]
			{
				LoggerManager.LoggerDefinitions.longOperationDefinition,
				LoggerManager.LoggerDefinitions.ropSummaryDefinition,
				LoggerManager.LoggerDefinitions.fullTextIndexDefinition,
				LoggerManager.LoggerDefinitions.diagnosticQueryDefinition,
				LoggerManager.LoggerDefinitions.referenceDataDefinition,
				LoggerManager.LoggerDefinitions.heavyClientActivityDefinition,
				LoggerManager.LoggerDefinitions.breadCrumbsDefinition,
				LoggerManager.LoggerDefinitions.syntheticCountersDefinition
			};
		}

		internal class TraceFileInfo
		{
			public DateTime CreationTimeUtc { get; internal set; }

			public string FullName { get; private set; }

			public DateTime LastWriteTimeUtc { get; internal set; }

			public long Length { get; private set; }

			public static LoggerManager.TraceFileInfo Create(FileInfo info)
			{
				return new LoggerManager.TraceFileInfo
				{
					CreationTimeUtc = info.CreationTimeUtc,
					FullName = info.FullName,
					LastWriteTimeUtc = info.LastWriteTimeUtc,
					Length = info.Length
				};
			}
		}
	}
}
