using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	[Serializable]
	public class SafetyNetInfo
	{
		[XmlIgnore]
		public Version Version
		{
			get
			{
				return this.m_version;
			}
		}

		public string VersionString
		{
			get
			{
				return this.m_versionString;
			}
			set
			{
				this.SetModifiedIfNecessary<string>(this.m_versionString, value);
				this.m_versionString = value;
			}
		}

		public string SourceServerName
		{
			get
			{
				return this.m_sourceServerName;
			}
			set
			{
				this.SetModifiedIfNecessary<string>(this.m_sourceServerName, value);
				this.m_sourceServerName = value;
			}
		}

		public bool RedeliveryRequired
		{
			get
			{
				return this.m_redeliveryRequired;
			}
			set
			{
				this.SetModifiedIfNecessary<bool>(this.m_redeliveryRequired, value);
				this.m_redeliveryRequired = value;
			}
		}

		public long LastLogGenBeforeActivation
		{
			get
			{
				return this.m_lastLogGenBeforeActivation;
			}
			set
			{
				this.SetModifiedIfNecessary<long>(this.m_lastLogGenBeforeActivation, value);
				this.m_lastLogGenBeforeActivation = value;
			}
		}

		public long NumberOfLogsLost
		{
			get
			{
				return this.m_numberOfLogsLost;
			}
			set
			{
				this.SetModifiedIfNecessary<long>(this.m_numberOfLogsLost, value);
				this.m_numberOfLogsLost = value;
			}
		}

		public DateTime ShadowRequestCreateTimeUtc
		{
			get
			{
				return this.m_shadowRequestCreateTimeUtc;
			}
			set
			{
				this.SetModifiedIfNecessary<DateTime>(this.m_shadowRequestCreateTimeUtc, value);
				this.m_shadowRequestCreateTimeUtc = value;
			}
		}

		public DateTime RequestLastAttemptedTimeUtc
		{
			get
			{
				return this.m_requestLastAttemptedTimeUtc;
			}
			set
			{
				this.SetModifiedIfNecessary<DateTime>(this.m_requestLastAttemptedTimeUtc, value);
				this.m_requestLastAttemptedTimeUtc = value;
			}
		}

		public DateTime RequestNextDueTimeUtc
		{
			get
			{
				return this.m_requestNextDueTimeUtc;
			}
			set
			{
				this.SetModifiedIfNecessary<DateTime>(this.m_requestNextDueTimeUtc, value);
				this.m_requestNextDueTimeUtc = value;
			}
		}

		public DateTime RequestCompletedTimeUtc
		{
			get
			{
				return this.m_requestCompletedTimeUtc;
			}
			set
			{
				this.SetModifiedIfNecessary<DateTime>(this.m_requestCompletedTimeUtc, value);
				this.m_requestCompletedTimeUtc = value;
			}
		}

		public DateTime FailoverTimeUtc
		{
			get
			{
				return this.m_failoverTimeUtc;
			}
			set
			{
				this.SetModifiedIfNecessary<DateTime>(this.m_failoverTimeUtc, value);
				this.m_failoverTimeUtc = value;
			}
		}

		public DateTime StartTimeUtc
		{
			get
			{
				return this.m_startTimeUtc;
			}
			set
			{
				this.SetModifiedIfNecessary<DateTime>(this.m_startTimeUtc, value);
				this.m_startTimeUtc = value;
			}
		}

		public DateTime EndTimeUtc
		{
			get
			{
				return this.m_endTimeUtc;
			}
			set
			{
				this.SetModifiedIfNecessary<DateTime>(this.m_endTimeUtc, value);
				this.m_endTimeUtc = value;
			}
		}

		public List<string> HubServers
		{
			get
			{
				return this.m_hubServers;
			}
			set
			{
				this.SetModifiedIfNecessary<List<string>>(this.m_hubServers, value);
				this.m_hubServers = value;
			}
		}

		public List<string> PrimaryHubServers
		{
			get
			{
				return this.m_primaryHubServers;
			}
			set
			{
				this.SetModifiedIfNecessary<List<string>>(this.m_primaryHubServers, value);
				this.m_primaryHubServers = value;
			}
		}

		public List<string> ShadowHubServers
		{
			get
			{
				return this.m_shadowHubServers;
			}
			set
			{
				this.SetModifiedIfNecessary<List<string>>(this.m_shadowHubServers, value);
				this.m_shadowHubServers = value;
			}
		}

		public string UniqueStr
		{
			get
			{
				return this.m_uniqueStr;
			}
			set
			{
				this.SetModifiedIfNecessary<string>(this.m_uniqueStr, value);
				this.m_uniqueStr = value;
			}
		}

		public static SafetyNetInfo Deserialize(string dbName, string blob, Trace tracer, bool throwOnError)
		{
			Exception ex = null;
			SafetyNetInfo safetyNetInfo = null;
			object obj = SerializationUtil.XmlToObject(blob, typeof(SafetyNetInfo), out ex);
			if (ex == null)
			{
				safetyNetInfo = (obj as SafetyNetInfo);
				if (safetyNetInfo == null && tracer != null)
				{
					tracer.TraceError<string, string>(0L, "Deserialized object {0} was not compatible with expected type {1}.", (obj != null) ? obj.GetType().Name : "(null)", typeof(SafetyNetInfo).Name);
				}
				else
				{
					safetyNetInfo.m_version = new Version(safetyNetInfo.m_versionString);
					safetyNetInfo.m_fModified = false;
					safetyNetInfo.m_serializedForm = blob;
				}
			}
			if (ex != null && tracer != null)
			{
				tracer.TraceError<string, string>(0L, "Deserialization of object {0} failed:\n{1}", typeof(SafetyNetInfo).Name, ex.ToString());
			}
			if (safetyNetInfo == null && throwOnError)
			{
				throw new FailedToDeserializeDumpsterRequestStrException(dbName, blob, typeof(SafetyNetInfo).Name, AmExceptionHelper.GetExceptionMessageOrNoneString(ex), ex);
			}
			return safetyNetInfo;
		}

		public string Serialize()
		{
			this.m_serializedForm = SerializationUtil.ObjectToXml(this);
			return this.m_serializedForm;
		}

		public SafetyNetInfo()
		{
			this.m_fModified = true;
			this.m_version = SafetyNetInfo.VersionNumber;
			this.m_versionString = this.m_version.ToString();
			this.RedeliveryRequired = false;
			this.StartTimeUtc = DateTime.MaxValue;
			this.EndTimeUtc = DateTime.MinValue;
			this.FailoverTimeUtc = DateTime.MinValue;
			this.ShadowRequestCreateTimeUtc = DateTime.MinValue;
			this.RequestLastAttemptedTimeUtc = DateTime.MinValue;
			this.RequestNextDueTimeUtc = DateTime.MinValue;
			this.RequestCompletedTimeUtc = DateTime.MinValue;
			this.HubServers = new List<string>(0);
			this.PrimaryHubServers = new List<string>(0);
			this.ShadowHubServers = new List<string>(0);
			this.SourceServerName = string.Empty;
			this.LastLogGenBeforeActivation = 0L;
			this.NumberOfLogsLost = 0L;
			this.UniqueStr = Guid.NewGuid().ToString().Substring(0, 8);
		}

		public SafetyNetInfo(string sourceServerName, long lastLogGenBeforeActivation, long numLogsLost, DateTime failoverTime, DateTime startTime, DateTime endTime)
		{
			this.m_fModified = true;
			this.m_version = SafetyNetInfo.VersionNumber;
			this.m_versionString = this.m_version.ToString();
			this.SourceServerName = sourceServerName;
			this.LastLogGenBeforeActivation = lastLogGenBeforeActivation;
			this.NumberOfLogsLost = numLogsLost;
			this.ShadowRequestCreateTimeUtc = DateTime.MinValue;
			this.RequestLastAttemptedTimeUtc = DateTime.MinValue;
			this.RequestNextDueTimeUtc = DateTime.MinValue;
			this.RequestCompletedTimeUtc = DateTime.MinValue;
			this.FailoverTimeUtc = failoverTime;
			this.StartTimeUtc = startTime;
			this.EndTimeUtc = endTime;
			this.RedeliveryRequired = true;
			this.HubServers = new List<string>(0);
			this.PrimaryHubServers = new List<string>(0);
			this.ShadowHubServers = new List<string>(0);
			this.UniqueStr = Guid.NewGuid().ToString().Substring(0, 8);
		}

		public override string ToString()
		{
			this.Serialize();
			return this.GetSerializedForm();
		}

		public string GetSerializedForm()
		{
			return this.m_serializedForm;
		}

		public bool IsModified()
		{
			return this.m_fModified;
		}

		public void ClearModified()
		{
			this.m_fModified = false;
		}

		public bool IsVersionCompatible()
		{
			return this.IsVersionCompatibleImpl(SafetyNetInfo.VersionNumber);
		}

		internal bool TestIsVersionCompatible(Version fakeServerVersion)
		{
			return this.IsVersionCompatibleImpl(fakeServerVersion);
		}

		internal void TestSetVersion(Version fakeVersion)
		{
			this.m_version = fakeVersion;
			this.m_versionString = this.m_version.ToString();
		}

		private bool IsVersionCompatibleImpl(Version serverVersion)
		{
			return this.Version.Major == serverVersion.Major && this.Version.Minor <= serverVersion.Minor;
		}

		private bool IsPropertyChanged<T>(T oldValue, T newValue)
		{
			if (oldValue is string)
			{
				return !SharedHelper.StringIEquals(oldValue as string, newValue as string);
			}
			if (oldValue is List<string>)
			{
				List<string> first = oldValue as List<string>;
				List<string> second = newValue as List<string>;
				return !first.SequenceEqual(second, StringComparer.OrdinalIgnoreCase);
			}
			return !oldValue.Equals(newValue);
		}

		private void SetModifiedIfNecessary<T>(T oldValue, T newValue)
		{
			this.m_fModified = (this.m_fModified || this.IsPropertyChanged<T>(oldValue, newValue));
		}

		public static readonly Version VersionNumber = new Version(1, 0);

		private Version m_version;

		private string m_versionString;

		private string m_sourceServerName;

		private bool m_redeliveryRequired;

		private long m_lastLogGenBeforeActivation;

		private long m_numberOfLogsLost;

		private DateTime m_shadowRequestCreateTimeUtc;

		private DateTime m_requestLastAttemptedTimeUtc;

		private DateTime m_requestNextDueTimeUtc;

		private DateTime m_requestCompletedTimeUtc;

		private DateTime m_failoverTimeUtc;

		private DateTime m_startTimeUtc;

		private DateTime m_endTimeUtc;

		private List<string> m_hubServers;

		private List<string> m_primaryHubServers;

		private List<string> m_shadowHubServers;

		private string m_uniqueStr;

		[NonSerialized]
		private bool m_fModified;

		[NonSerialized]
		private string m_serializedForm;
	}
}
