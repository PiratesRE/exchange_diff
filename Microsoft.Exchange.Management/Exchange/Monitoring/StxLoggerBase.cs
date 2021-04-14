using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class StxLoggerBase
	{
		internal StxLoggerBase()
		{
			this.extendedFields = new List<FieldInfo>();
		}

		internal static StxLoggerBase GetLoggerInstance(StxLogType type)
		{
			StxLoggerBase stxLoggerBase;
			if (!StxLoggerBase.LogDictionary.TryGetValue(type, out stxLoggerBase))
			{
				switch (type)
				{
				case StxLogType.TestLiveIdAuthentication:
					stxLoggerBase = new LiveIdAuthenticationStxLogger();
					break;
				case StxLogType.TestNtlmConnectivity:
					stxLoggerBase = new NtlmConnectivityStxLogger();
					break;
				case StxLogType.TestActiveDirectoryConnectivity:
					stxLoggerBase = new ActiveDirectoryConnectivityStxLogger();
					break;
				case StxLogType.TestTopologyService:
					stxLoggerBase = new TopologyServiceStxLogger();
					break;
				case StxLogType.TestGlobalLocatorService:
					stxLoggerBase = new GlobalLocatorServiceStxLogger();
					break;
				case StxLogType.TestForwardFullSync:
					stxLoggerBase = new ForwardFullSyncStxLogger();
					break;
				case StxLogType.TestForwardSyncCookie:
					stxLoggerBase = new ForwardSyncCookieStxLogger();
					break;
				case StxLogType.TestForwardSyncCookieResponder:
					stxLoggerBase = new ForwardSyncCookieResponderStxLogger();
					break;
				case StxLogType.TestForwardSyncCompanyProbe:
					stxLoggerBase = new ForwardSyncCompanyProbeStxLogger();
					break;
				case StxLogType.TestForwardSyncCompanyResponder:
					stxLoggerBase = new ForwardSyncCompanyResponderStxLogger();
					break;
				case StxLogType.DatabaseAvailability:
					stxLoggerBase = new DatabaseAvailabilityStxLogger();
					break;
				case StxLogType.TestRidMonitor:
					stxLoggerBase = new RidMonitorLogger();
					break;
				case StxLogType.TestRidSetMonitor:
					stxLoggerBase = new RidSetMonitorLogger();
					break;
				case StxLogType.TestActiveDirectorySelfCheck:
					stxLoggerBase = new ActiveDirectorySelfCheckStxLogger();
					break;
				case StxLogType.TenantRelocationErrorMonitor:
					stxLoggerBase = new TenantRelocationErrorLogger();
					break;
				case StxLogType.SharedConfigurationTenantMonitor:
					stxLoggerBase = new SharedConfigurationTenantMonitorLogger();
					break;
				case StxLogType.TestActivedirectoryConnectivityForConfigDC:
					stxLoggerBase = new ActiveDirectoryConnectivityConfigDCStxLogger();
					break;
				case StxLogType.SyntheticReplicationTransaction:
					stxLoggerBase = new SyntheticReplicationTransactionLogger();
					break;
				case StxLogType.SyntheticReplicationMonitor:
					stxLoggerBase = new SyntheticReplicationMonitorLogger();
					break;
				case StxLogType.PassiveReplicationMonitor:
					stxLoggerBase = new PassiveReplicationMonitorLogger();
					break;
				case StxLogType.PassiveADReplicationMonitor:
					stxLoggerBase = new PassiveADReplicationMonitorLogger();
					break;
				case StxLogType.PassiveReplicationPerfCounterProbe:
					stxLoggerBase = new PassiveReplicationPerfCounterProbeLogger();
					break;
				case StxLogType.RemoteDomainControllerStateProbe:
					stxLoggerBase = new RemoteDomainControllerStateProbeLogger();
					break;
				case StxLogType.TrustMonitorProbe:
					stxLoggerBase = new TrustMonitorProbeLogger();
					break;
				case StxLogType.TestKDCService:
					stxLoggerBase = new TestKDCServiceStxLogger();
					break;
				case StxLogType.TestDoMTConnectivity:
					stxLoggerBase = new DoMTConnectivityStxLogger();
					break;
				case StxLogType.TestOfflineGLS:
					stxLoggerBase = new OfflineGLSStxLogger();
					break;
				}
				stxLoggerBase = StxLoggerBase.LogDictionary.GetOrAdd(type, stxLoggerBase);
			}
			return stxLoggerBase;
		}

		internal Log Log
		{
			get
			{
				if (this.log == null)
				{
					this.log = new Log(this.LogFilePrefix, new LogHeaderFormatter(this.Schema, this.HeaderCsvOption), this.LogComponent);
				}
				return this.log;
			}
			private set
			{
				this.log = value;
			}
		}

		internal void BeginAppend(LogRowFormatter row)
		{
			StxLogger.BeginAppend(this, row);
		}

		internal void BeginAppend(string target, bool status, TimeSpan latency, int error, string errorString)
		{
			this.BeginAppend(target, status, latency, error, errorString, null, null, null, null);
		}

		internal void BeginAppend(string target, bool status, TimeSpan latency, int error, string errorString, string stateAttribute1, string stateAttribute2, string stateAttribute3, string stateAttribute4)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.Schema);
			logRowFormatter[1] = target;
			logRowFormatter[2] = (status ? 0 : 1);
			logRowFormatter[3] = latency.TotalSeconds;
			logRowFormatter[4] = error.ToString();
			logRowFormatter[5] = ((errorString != null) ? errorString : string.Empty);
			logRowFormatter[6] = ((stateAttribute1 != null) ? stateAttribute1 : string.Empty);
			logRowFormatter[7] = ((stateAttribute2 != null) ? stateAttribute2 : string.Empty);
			logRowFormatter[8] = ((stateAttribute3 != null) ? stateAttribute3 : string.Empty);
			logRowFormatter[9] = ((stateAttribute4 != null) ? stateAttribute4 : string.Empty);
			this.BeginAppend(logRowFormatter);
		}

		internal void BeginAppend(string target, bool status, TimeSpan latency, int error, string errorString, string stateAttribute1, string stateAttribute2, string stateAttribute3, string stateAttribute4, List<string> extendedAttributes)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.Schema);
			logRowFormatter[1] = target;
			logRowFormatter[2] = (status ? 0 : 1);
			logRowFormatter[3] = latency.TotalSeconds;
			logRowFormatter[4] = error.ToString();
			logRowFormatter[5] = ((errorString != null) ? errorString : string.Empty);
			logRowFormatter[6] = ((stateAttribute1 != null) ? stateAttribute1 : string.Empty);
			logRowFormatter[7] = ((stateAttribute2 != null) ? stateAttribute2 : string.Empty);
			logRowFormatter[8] = ((stateAttribute3 != null) ? stateAttribute3 : string.Empty);
			logRowFormatter[9] = ((stateAttribute4 != null) ? stateAttribute4 : string.Empty);
			if (this.ExtendedFields.Count != extendedAttributes.Count)
			{
				throw new Exception(string.Format("the count of extended attribute values does not match with extended fields count. expected {0}, actual {1}", this.ExtendedFields.Count, extendedAttributes.Count));
			}
			for (int i = 0; i < this.ExtendedFields.Count; i++)
			{
				logRowFormatter[StxLoggerBase.MandatoryFields.Count + i] = ((extendedAttributes[i] != null) ? extendedAttributes[i] : string.Empty);
			}
			this.BeginAppend(logRowFormatter);
		}

		internal virtual LogSchema Schema
		{
			get
			{
				if (this.schema == null)
				{
					this.schema = new LogSchema("Microsoft Exchange", "15.00.1497.015", this.LogTypeName, this.GetColumnArray());
				}
				return this.schema;
			}
		}

		internal virtual int DateTimeField
		{
			get
			{
				return 0;
			}
		}

		internal List<FieldInfo> ExtendedFields
		{
			get
			{
				return this.extendedFields;
			}
		}

		internal virtual LogHeaderCsvOption HeaderCsvOption
		{
			get
			{
				return LogHeaderCsvOption.CsvCompatible;
			}
		}

		internal bool Initialized { get; set; }

		internal abstract string LogTypeName { get; }

		internal abstract string LogComponent { get; }

		internal abstract string LogFilePrefix { get; }

		private string[] GetColumnArray()
		{
			string[] array = new string[StxLoggerBase.MandatoryFields.Count + this.ExtendedFields.Count];
			for (int i = 0; i < StxLoggerBase.MandatoryFields.Count; i++)
			{
				array[i] = StxLoggerBase.MandatoryFields[i].ColumnName;
			}
			int num = 0;
			for (int j = StxLoggerBase.MandatoryFields.Count; j < this.ExtendedFields.Count + StxLoggerBase.MandatoryFields.Count; j++)
			{
				array[j] = this.ExtendedFields[num].ColumnName;
				num++;
			}
			return array;
		}

		internal static readonly List<FieldInfo> MandatoryFields = new List<FieldInfo>
		{
			new FieldInfo(0, "Timestamp"),
			new FieldInfo(1, "Target Entity"),
			new FieldInfo(2, "Status"),
			new FieldInfo(3, "Latency"),
			new FieldInfo(4, "Error"),
			new FieldInfo(5, "Exception"),
			new FieldInfo(6, "StateAttribute1"),
			new FieldInfo(7, "StateAttribute2"),
			new FieldInfo(8, "StateAttribute3"),
			new FieldInfo(9, "StateAttribute4")
		};

		private Log log;

		private LogSchema schema;

		private List<FieldInfo> extendedFields;

		internal static readonly ConcurrentDictionary<StxLogType, StxLoggerBase> LogDictionary = new ConcurrentDictionary<StxLogType, StxLoggerBase>();

		internal enum MandatoryField
		{
			DateTime,
			Target,
			Status,
			Latency,
			Error,
			Exception,
			StateAttribute1,
			StateAttribute2,
			StateAttribute3,
			StateAttribute4
		}
	}
}
