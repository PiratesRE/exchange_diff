using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.DagManagement;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal class DbCopyHealthInfoTable
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DatabaseHealthTrackerTracer;
			}
		}

		public DbCopyHealthInfoTable(string persistedFilePath)
		{
			this.m_fileFullPath = persistedFilePath;
		}

		public void Initialize()
		{
			Exception ex = null;
			lock (this.m_locker)
			{
				if (this.m_healthTable == null)
				{
					DbCopyHealthInfoTable.Tracer.TraceDebug((long)this.GetHashCode(), "Initializing the health info table...");
					this.m_healthTable = new DbCopyHealthInfoInternalTable(this.m_fileFullPath);
					ex = this.m_healthTable.InitializeHealthInfoFromXML();
					this.m_fInitialized = true;
				}
			}
			if (ex != null)
			{
				DbCopyHealthInfoTable.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Failed to read health info from XML file. Error: {0}", ex);
				ReplayCrimsonEvents.DHTInitFromFileFailed.LogPeriodic<string, Exception>(Environment.MachineName, DateTimeHelper.OneHour, ex.Message, ex);
			}
		}

		public bool UpdateHealthInfo(HealthInfoPersisted healthInfo, bool isPrimary)
		{
			DbCopyHealthInfoTable.Tracer.TraceDebug<bool>((long)this.GetHashCode(), "UpdateHealthInfo() called with isPrimary = '{0}'", isPrimary);
			DateTime lastUpdateTimeUtc = healthInfo.GetLastUpdateTimeUtc();
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				if (this.m_fInitialized)
				{
					DateTimeHelper.SafeSubtract(lastUpdateTimeUtc, this.m_healthTable.LastUpdateTimeUtc);
					if (this.m_healthTable.LastUpdateTimeUtc > lastUpdateTimeUtc)
					{
						string text = DateTimeHelper.ToStringInvariantCulture(lastUpdateTimeUtc);
						string text2 = DateTimeHelper.ToStringInvariantCulture(this.m_healthTable.LastUpdateTimeUtc);
						if (isPrimary)
						{
							DbCopyHealthInfoTable.Tracer.TraceError<string, string>((long)this.GetHashCode(), "UpdateHealthInfo(): Primary node is ignoring older table with update time '{0}'. Local update time: {1}", text, text2);
							ReplayCrimsonEvents.DHTPrimaryStartupIgnoringOlderTable.LogPeriodic<string, string>(text, DateTimeHelper.OneHour, text, text2);
							return false;
						}
						DbCopyHealthInfoTable.Tracer.TraceError<TimeSpan>((long)this.GetHashCode(), "UpdateHealthInfo(): The health table is being replaced with an older one. Age difference: {0}", this.m_healthTable.LastUpdateTimeUtc.Subtract(lastUpdateTimeUtc));
						ReplayCrimsonEvents.DHTSecondaryOlderTable.LogPeriodic<string, string>(text, DateTimeHelper.OneHour, text, text2);
					}
				}
				DbCopyHealthInfoTable.Tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "UpdateHealthInfo(): The health table is being updated/replaced with a table of update timestamp: {0}", lastUpdateTimeUtc);
				this.m_fInitialized = false;
				this.m_healthTable = new DbCopyHealthInfoInternalTable(this.m_fileFullPath);
				this.m_healthTable.InitializeFromHealthInfoPersisted(healthInfo);
				this.m_fInitialized = true;
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			Exception ex = dbCopyHealthInfoInternalTable.PersistHealthInfoToXml();
			if (ex != null)
			{
				DbCopyHealthInfoTable.Tracer.TraceError<Exception>((long)this.GetHashCode(), "UpdateHealthInfo(): Failed to persist health table to XML file. Error: {0}", ex);
				if (isPrimary)
				{
					ReplayCrimsonEvents.DHTPrimaryPersistFailed.LogPeriodic<string, Exception>(Environment.MachineName, DateTimeHelper.OneHour, ex.Message, ex);
				}
				else
				{
					ReplayCrimsonEvents.DHTSecondaryPersistFailed.LogPeriodic<string, Exception>(Environment.MachineName, DateTimeHelper.OneHour, ex.Message, ex);
				}
			}
			return true;
		}

		public DateTime GetLastUpdateTime()
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			DateTime dateTime = DateTime.MinValue;
			lock (this.m_locker)
			{
				if (this.m_fInitialized)
				{
					dbCopyHealthInfoInternalTable = this.m_healthTable;
				}
				else
				{
					DbCopyHealthInfoTable.Tracer.TraceError<DateTime>((long)this.GetHashCode(), "GetLastUpdateTime(): The health table is not yet initialized. Returning '{0}'", dateTime);
				}
			}
			if (dbCopyHealthInfoInternalTable != null)
			{
				dateTime = dbCopyHealthInfoInternalTable.LastUpdateTimeUtc;
			}
			return dateTime;
		}

		public HealthInfoPersisted GetHealthInfo()
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			HealthInfoPersisted result = null;
			lock (this.m_locker)
			{
				if (this.m_fInitialized)
				{
					dbCopyHealthInfoInternalTable = this.m_healthTable;
				}
				else
				{
					DbCopyHealthInfoTable.Tracer.TraceError((long)this.GetHashCode(), "GetHealthInfo(): The health table is not yet initialized. Returning <NULL>");
				}
			}
			if (dbCopyHealthInfoInternalTable != null)
			{
				result = dbCopyHealthInfoInternalTable.ConvertToHealthInfoPersisted();
			}
			return result;
		}

		public void SetCreateTimeIfNecessary(DateTime createTimeUtc)
		{
			lock (this.m_locker)
			{
				if (this.m_healthTable.IsFileNotReadAtInitialization)
				{
					DbCopyHealthInfoTable.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SetCreateTime() is changing to '{0}'", DateTimeHelper.ToStringInvariantCulture(createTimeUtc));
					this.m_healthTable.CreateTimeUtc = createTimeUtc;
				}
			}
		}

		public void SetLastUpdateTime(DateTime lastUpdateTimeUtc)
		{
			lock (this.m_locker)
			{
				DbCopyHealthInfoTable.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SetLastUpdateTime() is changing to '{0}'", DateTimeHelper.ToStringInvariantCulture(lastUpdateTimeUtc));
				this.m_healthTable.LastUpdateTimeUtc = lastUpdateTimeUtc;
			}
		}

		public void ReportServerFoundInAD(AmServerName serverName)
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			dbCopyHealthInfoInternalTable.ReportServerFoundInAD(serverName);
		}

		public void ReportDbCopiesFoundInAD(IEnumerable<IADDatabase> databases, AmServerName serverName)
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			foreach (IADDatabase iaddatabase in databases)
			{
				DbCopyHealthInfoTable.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Reporting that database copy '{0}\\{1}' was found in the AD.", iaddatabase.Name, serverName.NetbiosName);
				dbCopyHealthInfoInternalTable.ReportDbCopyFoundInAD(iaddatabase, serverName);
			}
		}

		public void ReportDbCopyStatusesFound(IMonitoringADConfig adConfig, AmServerName serverName, IEnumerable<CopyStatusClientCachedEntry> statuses)
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in statuses)
			{
				IADDatabase iaddatabase;
				string text = adConfig.DatabaseByGuidMap.TryGetValue(copyStatusClientCachedEntry.DbGuid, out iaddatabase) ? iaddatabase.Name : copyStatusClientCachedEntry.DbGuid.ToString();
				DbCopyHealthInfoTable.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Reporting that a copy status for database copy '{0}\\{1}' was possibly retrieved.", text, serverName.NetbiosName);
				dbCopyHealthInfoInternalTable.ReportDbCopyStatusFound(copyStatusClientCachedEntry.DbGuid, text, serverName, copyStatusClientCachedEntry);
			}
		}

		public void PossiblyReportObjectsNotFoundInAD(IMonitoringADConfig adConfig)
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			dbCopyHealthInfoInternalTable.PossiblyReportObjectsNotFoundInAD(adConfig);
		}

		public void UpdateAvailabilityRedundancyStates(IMonitoringADConfig adConfig)
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			dbCopyHealthInfoInternalTable.UpdateAvailabilityRedundancyStates(adConfig);
		}

		public HealthInfoPersisted ConvertToHealthInfoPersisted()
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			return dbCopyHealthInfoInternalTable.ConvertToHealthInfoPersisted();
		}

		public Exception PersistHealthInfoToXml()
		{
			DbCopyHealthInfoInternalTable dbCopyHealthInfoInternalTable = null;
			lock (this.m_locker)
			{
				dbCopyHealthInfoInternalTable = this.m_healthTable;
			}
			return dbCopyHealthInfoInternalTable.PersistHealthInfoToXml();
		}

		private object m_locker = new object();

		private bool m_fInitialized;

		private readonly string m_fileFullPath;

		private DbCopyHealthInfoInternalTable m_healthTable;
	}
}
