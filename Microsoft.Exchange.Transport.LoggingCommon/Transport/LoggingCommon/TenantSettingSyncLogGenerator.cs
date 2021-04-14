using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.FfoSyncLog;
using Microsoft.Exchange.FfoSyncLog;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	public class TenantSettingSyncLogGenerator : IDisposable
	{
		protected TenantSettingSyncLogGenerator()
		{
			try
			{
				this.isSupportedOnCurrentSku = (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled || DatacenterRegistry.IsForefrontForOffice());
			}
			catch (Exception)
			{
			}
			if (!this.isSupportedOnCurrentSku)
			{
				return;
			}
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs\\FfoTenantSettingsSyncConfig"))
				{
					if (registryKey != null)
					{
						string text = registryKey.GetValue("RootLogPath") as string;
						if (!string.IsNullOrWhiteSpace(text))
						{
							this.LogDirectoryPath = text;
						}
						else
						{
							this.EventLogger.LogEvent(FfoSyncLogEventLogConstants.Tuple_FfoSyncLogLogPathNotConfigured, null, new object[0]);
						}
					}
				}
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs\\FfoTenantSettingsSyncConfig"))
				{
					if (registryKey2 != null)
					{
						this.MaxFileSizeMB = (int)registryKey2.GetValue("LogFileMaxSizeMB", this.MaxFileSizeMB);
						this.MaxDirSizeMB = (int)registryKey2.GetValue("LogDirectoryMaxSizeMB", this.MaxDirSizeMB);
						this.BufferSizeBytes = (int)registryKey2.GetValue("LogBufferCacheSizeBytes", this.BufferSizeBytes);
						this.MaxAgeDays = (int)registryKey2.GetValue("LogFileMaxAgeDays", this.MaxAgeDays);
						this.FlushIntervalSeconds = (int)registryKey2.GetValue("LogFlushIntervalSeconds", this.FlushIntervalSeconds);
					}
				}
			}
			catch (SecurityException ex)
			{
				this.EventLogger.LogEvent(FfoSyncLogEventLogConstants.Tuple_FfoSyncLogConfigRegistryReadAccessException, "SOFTWARE\\Microsoft\\ExchangeLabs\\FfoTenantSettingsSyncConfig", new object[]
				{
					"SOFTWARE\\Microsoft\\ExchangeLabs\\FfoTenantSettingsSyncConfig",
					ex.ToString()
				});
			}
			catch (UnauthorizedAccessException ex2)
			{
				this.EventLogger.LogEvent(FfoSyncLogEventLogConstants.Tuple_FfoSyncLogConfigRegistryReadAccessException, "SOFTWARE\\Microsoft\\ExchangeLabs\\FfoTenantSettingsSyncConfig", new object[]
				{
					"SOFTWARE\\Microsoft\\ExchangeLabs\\FfoTenantSettingsSyncConfig",
					ex2.ToString()
				});
			}
			this.schema = new LogSchema("Microsoft Exchange/FFO Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "TenantSettingSyncLog", Enum.GetNames(typeof(TenantSettingSchemaFields)));
			this.mapLogs = new Dictionary<TenantSettingSyncLogType, Log>();
			foreach (object obj in Enum.GetValues(typeof(TenantSettingSyncLogType)))
			{
				TenantSettingSyncLogType key = (TenantSettingSyncLogType)obj;
				this.mapLogs.Add(key, null);
			}
		}

		public static TenantSettingSyncLogGenerator Instance
		{
			get
			{
				if (TenantSettingSyncLogGenerator.instance == null)
				{
					lock (TenantSettingSyncLogGenerator.syncObject)
					{
						if (TenantSettingSyncLogGenerator.instance == null)
						{
							TenantSettingSyncLogGenerator.instance = new TenantSettingSyncLogGenerator();
						}
					}
				}
				return TenantSettingSyncLogGenerator.instance;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.isSupportedOnCurrentSku && this.LogDirectoryPath != null;
			}
		}

		private ExEventLog EventLogger
		{
			get
			{
				if (this.eventLogger == null)
				{
					this.eventLogger = new ExEventLog(ExTraceGlobals.LogGenTracer.Category, "FfoSyncLog");
				}
				return this.eventLogger;
			}
		}

		public bool LogChangesForSave(ADObject savedObject, Guid? externalDirectoryOrgId = null, Guid? immutableObjectId = null, List<KeyValuePair<string, object>> customProperties = null)
		{
			return this.LogChangesForSave(savedObject, this.GetLogType(savedObject), externalDirectoryOrgId, immutableObjectId, customProperties);
		}

		public bool LogChangesForSave(ADObject savedObject, TenantSettingSyncLogType logType, Guid? externalDirectoryOrgId = null, Guid? immutableObjectId = null, List<KeyValuePair<string, object>> customProperties = null)
		{
			if (!this.Enabled)
			{
				return false;
			}
			if (savedObject != null && savedObject.OrganizationId != OrganizationId.ForestWideOrgId && savedObject.OrganizationalUnitRoot != null)
			{
				Guid organizationUnitRootId = (externalDirectoryOrgId != null) ? externalDirectoryOrgId.Value : savedObject.OrganizationalUnitRoot.ObjectGuid;
				Guid value = (immutableObjectId != null) ? immutableObjectId.Value : savedObject.Guid;
				return this.AddLogLine(logType, TenantSettingChangeType.Save, organizationUnitRootId, savedObject.Name, new Guid?(value), savedObject.WhenChangedUTC, customProperties);
			}
			return false;
		}

		public bool LogChangesForDelete(ADObject deletedobject, Guid? externalDirectoryOrgId = null)
		{
			return this.LogChangesForDelete(deletedobject, this.GetLogType(deletedobject), externalDirectoryOrgId);
		}

		public bool LogChangesForDelete(ADObject deletedobject, TenantSettingSyncLogType logType, Guid? externalDirectoryOrgId = null)
		{
			if (!this.Enabled)
			{
				return false;
			}
			if (deletedobject != null && deletedobject.OrganizationId != OrganizationId.ForestWideOrgId && deletedobject.OrganizationalUnitRoot != null)
			{
				Guid organizationUnitRootId = (externalDirectoryOrgId != null) ? externalDirectoryOrgId.Value : deletedobject.OrganizationalUnitRoot.ObjectGuid;
				return this.AddLogLine(logType, TenantSettingChangeType.Delete, organizationUnitRootId, deletedobject.Name, null, null, null);
			}
			return false;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public TenantSettingSyncLogType GetLogType(ADObject objectInstance)
		{
			Type type = objectInstance.GetType();
			if (objectInstance is TransportRule)
			{
				return TenantSettingSyncLogType.SYNCTR;
			}
			if (objectInstance is ADComplianceProgram)
			{
				return TenantSettingSyncLogType.SYNCADCP;
			}
			if (objectInstance is HostedConnectionFilterPolicy)
			{
				return TenantSettingSyncLogType.SYNCCONNPOL;
			}
			if (objectInstance is HostedOutboundSpamFilterPolicy)
			{
				return TenantSettingSyncLogType.SYNCOBSPAMPOL;
			}
			if (objectInstance is TenantInboundConnector)
			{
				return TenantSettingSyncLogType.SYNCICONN;
			}
			if (objectInstance is DomainContentConfig)
			{
				return TenantSettingSyncLogType.SYNCDOMCON;
			}
			if (objectInstance is HostedContentFilterPolicy)
			{
				return TenantSettingSyncLogType.DUALSYNCCONTPOL;
			}
			if (objectInstance is AcceptedDomain)
			{
				return TenantSettingSyncLogType.SYNCACCEPTEDDOM;
			}
			string message = string.Format(CultureInfo.InvariantCulture, "TenantSettingSync is not supported for this type: {0}", new object[]
			{
				type
			});
			throw new InvalidOperationException(message);
		}

		internal void AddEventLogOnADException(ADOperationResult opResult)
		{
			if (this.Enabled && opResult != null && !opResult.Succeeded)
			{
				this.EventLogger.LogEvent(FfoSyncLogEventLogConstants.Tuple_FfoSyncLogADOperationException, opResult.Exception.Message, new object[]
				{
					opResult.Exception.ToString()
				});
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.mapLogs != null)
			{
				List<TenantSettingSyncLogType> list = this.mapLogs.Keys.ToList<TenantSettingSyncLogType>();
				foreach (TenantSettingSyncLogType key in list)
				{
					if (this.mapLogs[key] != null)
					{
						this.mapLogs[key].Close();
						this.mapLogs[key] = null;
					}
				}
			}
		}

		protected virtual string GetLogPrefix(TenantSettingSyncLogType logType)
		{
			return logType.ToString();
		}

		private void ConfigureLog(TenantSettingSyncLogType logType)
		{
			if (this.mapLogs[logType] == null)
			{
				lock (TenantSettingSyncLogGenerator.syncObject)
				{
					if (this.mapLogs[logType] == null)
					{
						Log log = new Log(this.GetLogPrefix(logType), new LogHeaderFormatter(this.schema), "TenantSettingSyncLog");
						log.Configure(this.LogDirectoryPath, TimeSpan.FromDays((double)this.MaxAgeDays), (long)this.MaxDirSizeMB * 1024L * 1024L, (long)this.MaxFileSizeMB * 1024L * 1024L, this.BufferSizeBytes, TimeSpan.FromSeconds((double)this.FlushIntervalSeconds), true);
						this.mapLogs[logType] = log;
						this.EventLogger.LogEvent(FfoSyncLogEventLogConstants.Tuple_FfoSyncLogConfigured, null, new object[]
						{
							logType,
							this.LogDirectoryPath
						});
					}
				}
			}
		}

		private bool AddLogLine(TenantSettingSyncLogType logType, TenantSettingChangeType changeType, Guid organizationUnitRootId, string name, Guid? id = null, DateTime? changedTimeUTC = null, List<KeyValuePair<string, object>> customProperties = null)
		{
			this.ConfigureLog(logType);
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[0] = ((changedTimeUTC != null) ? changedTimeUTC.Value : DateTime.UtcNow);
			logRowFormatter[1] = (int)changeType;
			logRowFormatter[2] = organizationUnitRootId;
			logRowFormatter[4] = name;
			if (id != null)
			{
				logRowFormatter[3] = id.Value;
			}
			try
			{
				logRowFormatter[5] = customProperties;
			}
			catch (ArgumentException ex)
			{
				this.EventLogger.LogEvent(FfoSyncLogEventLogConstants.Tuple_FfoSyncLogFormatException, logType.ToString(), new object[]
				{
					ex.ToString()
				});
				return false;
			}
			this.mapLogs[logType].Append(logRowFormatter, -1);
			return true;
		}

		private const string ConfigRegistryKey = "SOFTWARE\\Microsoft\\ExchangeLabs\\FfoTenantSettingsSyncConfig";

		private const string LogPathEntry = "RootLogPath";

		private const string MaxFileSizeEntry = "LogFileMaxSizeMB";

		private const string MaxDirSizeEntry = "LogDirectoryMaxSizeMB";

		private const string BufferSizeEntry = "LogBufferCacheSizeBytes";

		private const string MaxAgeEntry = "LogFileMaxAgeDays";

		private const string FlushIntervalEntry = "LogFlushIntervalSeconds";

		private const string LogSoftwareName = "Microsoft Exchange/FFO Server";

		private const string LogComponentName = "TenantSettingSyncLog";

		private readonly int MaxFileSizeMB = 10;

		private readonly int MaxDirSizeMB;

		private readonly int BufferSizeBytes = 4096;

		private readonly int MaxAgeDays = 30;

		private readonly int FlushIntervalSeconds = 30;

		private readonly LogSchema schema;

		private readonly string LogDirectoryPath;

		private readonly bool isSupportedOnCurrentSku;

		private static volatile TenantSettingSyncLogGenerator instance;

		private static object syncObject = new object();

		private Dictionary<TenantSettingSyncLogType, Log> mapLogs;

		private ExEventLog eventLogger;
	}
}
