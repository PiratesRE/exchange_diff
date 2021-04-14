using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Database)]
	[Serializable]
	public class Database : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return Database.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMDB";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, MailboxDatabase.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, PublicFolderDatabase.MostDerivedClass)
				});
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (this.IsExchange2009OrLater && this.MasterServerOrAvailabilityGroup == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorMasterServerInvalid(this.Name), DatabaseSchema.MasterServerOrAvailabilityGroup, this));
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!base.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				AsciiCharactersOnlyConstraint asciiCharactersOnlyConstraint = new AsciiCharactersOnlyConstraint();
				PropertyConstraintViolationError propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.Name, DatabaseSchema.Name, null);
				if (propertyConstraintViolationError != null)
				{
					errors.Add(propertyConstraintViolationError);
				}
				if (null != this.EdbFilePath)
				{
					propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.EdbFilePath, DatabaseSchema.EdbFilePath, null);
					if (propertyConstraintViolationError != null)
					{
						errors.Add(propertyConstraintViolationError);
					}
				}
				if (!base.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
				{
					if (null == this.LogFolderPath)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.PropertyRequired(DatabaseSchema.LogFolderPath.Name, base.GetType().ToString()), this.Identity, string.Empty));
					}
					else
					{
						propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.LogFolderPath, StorageGroupSchema.LogFolderPath, null);
						if (propertyConstraintViolationError != null)
						{
							errors.Add(propertyConstraintViolationError);
						}
					}
					if (null == this.SystemFolderPath)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.PropertyRequired(DatabaseSchema.SystemFolderPath.Name, base.GetType().ToString()), this.Identity, string.Empty));
					}
					else
					{
						propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.SystemFolderPath, StorageGroupSchema.SystemFolderPath, null);
						if (propertyConstraintViolationError != null)
						{
							errors.Add(propertyConstraintViolationError);
						}
					}
				}
			}
			if (null != this.EdbFilePath && this.EdbFilePath.IsPathInRootDirectory)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorEdbFilePathInRoot(this.EdbFilePath.PathName), this.Identity, string.Empty));
			}
			if (base.Id.DomainId != null && base.Id.Depth - base.Id.DomainId.Depth < 8)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorInvalidDNDepth, this.Identity, string.Empty));
			}
		}

		public ReplicationType ReplicationType
		{
			get
			{
				ReplicationType result;
				lock (this)
				{
					if (this.replicationType == ReplicationType.Unknown)
					{
						this.CompleteAllCalculatedProperties();
					}
					result = this.replicationType;
				}
				return result;
			}
		}

		internal bool IsValidDatabase(bool allowInvalid)
		{
			return allowInvalid || this.IsValid;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				ServerVersion serverVersion = (ServerVersion)this[DatabaseSchema.AdminDisplayVersion];
				if (serverVersion == null)
				{
					serverVersion = Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.GetServerVersion(this.ServerName);
					this.AdminDisplayVersion = serverVersion;
				}
				return serverVersion;
			}
			internal set
			{
				this[DatabaseSchema.AdminDisplayVersion] = value;
			}
		}

		public ADObjectId AdministrativeGroup
		{
			get
			{
				return (ADObjectId)this[DatabaseSchema.AdministrativeGroup];
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowFileRestore
		{
			get
			{
				return (bool)this[DatabaseSchema.AllowFileRestore];
			}
			set
			{
				this[DatabaseSchema.AllowFileRestore] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BackgroundDatabaseMaintenance
		{
			get
			{
				return (bool)this[DatabaseSchema.BackgroundDatabaseMaintenance];
			}
			set
			{
				this[DatabaseSchema.BackgroundDatabaseMaintenance] = value;
			}
		}

		public bool? ReplayBackgroundDatabaseMaintenance
		{
			get
			{
				return (bool?)this[DatabaseSchema.ReplayBackgroundDatabaseMaintenance];
			}
		}

		public bool? BackgroundDatabaseMaintenanceSerialization
		{
			get
			{
				return (bool?)this[DatabaseSchema.BackgroundDatabaseMaintenanceSerialization];
			}
		}

		public int? BackgroundDatabaseMaintenanceDelay
		{
			get
			{
				return (int?)this[DatabaseSchema.BackgroundDatabaseMaintenanceDelay];
			}
		}

		public int? ReplayBackgroundDatabaseMaintenanceDelay
		{
			get
			{
				return (int?)this[DatabaseSchema.ReplayBackgroundDatabaseMaintenanceDelay];
			}
		}

		public int? MimimumBackgroundDatabaseMaintenanceInterval
		{
			get
			{
				return (int?)this[DatabaseSchema.MimimumBackgroundDatabaseMaintenanceInterval];
			}
		}

		public int? MaximumBackgroundDatabaseMaintenanceInterval
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumBackgroundDatabaseMaintenanceInterval];
			}
		}

		public bool? BackupInProgress
		{
			get
			{
				return this.databaseBackupInProgress;
			}
			internal set
			{
				this.databaseBackupInProgress = value;
			}
		}

		public bool DatabaseCreated
		{
			get
			{
				return (bool)this[DatabaseSchema.DatabaseCreated];
			}
			internal set
			{
				this[DatabaseSchema.DatabaseCreated] = value;
			}
		}

		internal DeliveryMechanisms DeliveryMechanism
		{
			get
			{
				return (DeliveryMechanisms)this[DatabaseSchema.DeliveryMechanism];
			}
		}

		public string Description
		{
			get
			{
				return (string)this[DatabaseSchema.Description];
			}
		}

		public EdbFilePath EdbFilePath
		{
			get
			{
				return (EdbFilePath)this[DatabaseSchema.EdbFilePath];
			}
			internal set
			{
				this[DatabaseSchema.EdbFilePath] = value;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[DatabaseSchema.ExchangeLegacyDN];
			}
			internal set
			{
				this[DatabaseSchema.ExchangeLegacyDN] = value;
			}
		}

		internal bool FixedFont
		{
			get
			{
				return (bool)this[DatabaseSchema.FixedFont];
			}
			set
			{
				this[DatabaseSchema.FixedFont] = value;
			}
		}

		public DatabaseCopy[] DatabaseCopies
		{
			get
			{
				DatabaseCopy[] result;
				lock (this)
				{
					if (this.databaseCopies == null)
					{
						this.CompleteAllCalculatedProperties();
					}
					result = this.databaseCopies;
				}
				return result;
			}
		}

		public DatabaseCopy[] InvalidDatabaseCopies
		{
			get
			{
				DatabaseCopy[] result;
				lock (this)
				{
					if (this.invalidDatabaseCopies == null)
					{
						this.CompleteAllCalculatedProperties();
					}
					result = this.invalidDatabaseCopies;
				}
				return result;
			}
		}

		public DatabaseCopy[] AllDatabaseCopies
		{
			get
			{
				DatabaseCopy[] result;
				lock (this)
				{
					if (this.allDatabaseCopies == null)
					{
						this.CompleteAllCalculatedProperties();
					}
					result = this.allDatabaseCopies;
				}
				return result;
			}
		}

		public ADObjectId[] Servers
		{
			get
			{
				lock (this)
				{
					if (this.servers == null)
					{
						this.CompleteAllCalculatedProperties();
					}
				}
				return this.servers;
			}
		}

		public KeyValuePair<ADObjectId, int>[] ActivationPreference
		{
			get
			{
				KeyValuePair<ADObjectId, int>[] result;
				lock (this)
				{
					if (this.activationPreference == null)
					{
						this.CompleteAllCalculatedProperties();
					}
					result = this.activationPreference;
				}
				return result;
			}
		}

		public KeyValuePair<ADObjectId, EnhancedTimeSpan>[] ReplayLagTimes
		{
			get
			{
				KeyValuePair<ADObjectId, EnhancedTimeSpan>[] result;
				lock (this)
				{
					if (this.replayLagTimes == null)
					{
						this.CompleteAllCalculatedProperties();
					}
					result = this.replayLagTimes;
				}
				return result;
			}
		}

		public KeyValuePair<ADObjectId, EnhancedTimeSpan>[] TruncationLagTimes
		{
			get
			{
				KeyValuePair<ADObjectId, EnhancedTimeSpan>[] result;
				lock (this)
				{
					if (this.truncationLagTimes == null)
					{
						this.CompleteAllCalculatedProperties();
					}
					result = this.truncationLagTimes;
				}
				return result;
			}
		}

		internal string RpcClientAccessServerLegacyDN
		{
			get
			{
				return (string)this[DatabaseSchema.RpcClientAccessServerExchangeLegacyDN];
			}
			set
			{
				this[DatabaseSchema.RpcClientAccessServerExchangeLegacyDN] = value;
			}
		}

		public string RpcClientAccessServer
		{
			get
			{
				return this.rpcClientAccessServerFqdn;
			}
			internal set
			{
				this.rpcClientAccessServerFqdn = value;
			}
		}

		public string MountedOnServer
		{
			get
			{
				return this.mountedOnServer;
			}
			internal set
			{
				this.mountedOnServer = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan DeletedItemRetention
		{
			get
			{
				return (EnhancedTimeSpan)this[DatabaseSchema.DeletedItemRetention];
			}
			set
			{
				this[DatabaseSchema.DeletedItemRetention] = value;
			}
		}

		public bool? SnapshotLastFullBackup
		{
			get
			{
				return this.snapshotLastFullBackup;
			}
			internal set
			{
				this.snapshotLastFullBackup = value;
			}
		}

		public bool? SnapshotLastIncrementalBackup
		{
			get
			{
				return this.snapshotLastIncrementalBackup;
			}
			internal set
			{
				this.snapshotLastIncrementalBackup = value;
			}
		}

		public bool? SnapshotLastDifferentialBackup
		{
			get
			{
				return this.snapshotLastDifferentialBackup;
			}
			internal set
			{
				this.snapshotLastDifferentialBackup = value;
			}
		}

		public bool? SnapshotLastCopyBackup
		{
			get
			{
				return this.snapshotLastCopyBackup;
			}
			internal set
			{
				this.snapshotLastCopyBackup = value;
			}
		}

		public DateTime? LastFullBackup
		{
			get
			{
				return this.databaseLastFullBackup;
			}
			internal set
			{
				this.databaseLastFullBackup = value;
			}
		}

		public DateTime? LastIncrementalBackup
		{
			get
			{
				return this.databaseLastIncrementalBackup;
			}
			internal set
			{
				this.databaseLastIncrementalBackup = value;
			}
		}

		public DateTime? LastDifferentialBackup
		{
			get
			{
				return this.databaseLastDifferentialBackup;
			}
			internal set
			{
				this.databaseLastDifferentialBackup = value;
			}
		}

		public DateTime? LastCopyBackup
		{
			get
			{
				return this.databaseLastCopyBackup;
			}
			internal set
			{
				this.databaseLastCopyBackup = value;
			}
		}

		public ByteQuantifiedSize? DatabaseSize
		{
			get
			{
				return this.databaseSize;
			}
			internal set
			{
				this.databaseSize = value;
			}
		}

		public ByteQuantifiedSize? AvailableNewMailboxSpace
		{
			get
			{
				return this.availableNewMailboxSpace;
			}
			internal set
			{
				this.availableNewMailboxSpace = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Schedule MaintenanceSchedule
		{
			get
			{
				return (Schedule)this[DatabaseSchema.MaintenanceSchedule];
			}
			set
			{
				this[DatabaseSchema.MaintenanceSchedule] = value;
			}
		}

		internal ScheduleMode MaintenanceScheduleMode
		{
			get
			{
				return (ScheduleMode)this[DatabaseSchema.MaintenanceScheduleMode];
			}
		}

		internal int MaxCachedViews
		{
			get
			{
				return (int)this[DatabaseSchema.MaxCachedViews];
			}
		}

		[Parameter(Mandatory = false)]
		public bool MountAtStartup
		{
			get
			{
				return (bool)this[DatabaseSchema.MountAtStartup];
			}
			set
			{
				this[DatabaseSchema.MountAtStartup] = value;
			}
		}

		public bool? Mounted
		{
			get
			{
				return this.databaseMounted;
			}
			internal set
			{
				this.databaseMounted = value;
			}
		}

		internal bool? OnlineMaintenanceInProgress
		{
			get
			{
				return this.databaseOnlineMaintenanceInProgress;
			}
			set
			{
				this.databaseOnlineMaintenanceInProgress = value;
			}
		}

		public ADObjectId Organization
		{
			get
			{
				return (ADObjectId)this[DatabaseSchema.Organization];
			}
		}

		internal ScheduleMode QuotaNotificationMode
		{
			get
			{
				return (ScheduleMode)this[DatabaseSchema.QuotaNotificationMode];
			}
		}

		[Parameter(Mandatory = false)]
		public Schedule QuotaNotificationSchedule
		{
			get
			{
				return (Schedule)this[DatabaseSchema.QuotaNotificationSchedule];
			}
			set
			{
				this[DatabaseSchema.QuotaNotificationSchedule] = value;
			}
		}

		public bool Recovery
		{
			get
			{
				return (bool)this[DatabaseSchema.Recovery];
			}
			internal set
			{
				this[DatabaseSchema.Recovery] = value;
			}
		}

		internal bool RestoreInProgress
		{
			get
			{
				return (bool)this[DatabaseSchema.RestoreInProgress];
			}
			set
			{
				this[DatabaseSchema.RestoreInProgress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetainDeletedItemsUntilBackup
		{
			get
			{
				return (bool)this[DatabaseSchema.RetainDeletedItemsUntilBackup];
			}
			set
			{
				this[DatabaseSchema.RetainDeletedItemsUntilBackup] = value;
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[DatabaseSchema.Server];
			}
			internal set
			{
				this[DatabaseSchema.Server] = value;
			}
		}

		public ADObjectId MasterServerOrAvailabilityGroup
		{
			get
			{
				return (ADObjectId)this[DatabaseSchema.MasterServerOrAvailabilityGroup];
			}
			internal set
			{
				this[DatabaseSchema.MasterServerOrAvailabilityGroup] = value;
			}
		}

		public int? WorkerProcessId
		{
			get
			{
				return this.workerProcessId;
			}
			internal set
			{
				this.workerProcessId = value;
			}
		}

		public string CurrentSchemaVersion
		{
			get
			{
				return this.currentSchemaVersion;
			}
			internal set
			{
				this.currentSchemaVersion = value;
			}
		}

		public string RequestedSchemaVersion
		{
			get
			{
				return this.requestedSchemaVersion;
			}
			internal set
			{
				this.requestedSchemaVersion = value;
			}
		}

		internal DatabaseAutoDagFlags AutoDagFlags
		{
			get
			{
				return (DatabaseAutoDagFlags)this[DatabaseSchema.AutoDagFlags];
			}
			set
			{
				this[DatabaseSchema.AutoDagFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagExcludeFromMonitoring
		{
			get
			{
				return (bool)this[DatabaseSchema.AutoDagExcludeFromMonitoring];
			}
			set
			{
				this[DatabaseSchema.AutoDagExcludeFromMonitoring] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AutoDatabaseMountDial AutoDatabaseMountDial
		{
			get
			{
				return (AutoDatabaseMountDial)this[DatabaseSchema.AutoDatabaseMountDialType];
			}
			set
			{
				this[DatabaseSchema.AutoDatabaseMountDialType] = value;
			}
		}

		internal bool InvalidDatabaseCopiesAllowed
		{
			get
			{
				return (bool)this[DatabaseSchema.InvalidDatabaseCopiesAllowed];
			}
			set
			{
				this[DatabaseSchema.InvalidDatabaseCopiesAllowed] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DatabaseGroup
		{
			get
			{
				return (string)this[DatabaseSchema.DatabaseGroup];
			}
			set
			{
				this[DatabaseSchema.DatabaseGroup] = value;
			}
		}

		public MasterType MasterType
		{
			get
			{
				if (this.m_masterType == null)
				{
					this.CalculateMasterType();
				}
				return this.m_masterType.Value;
			}
		}

		internal void CalculateMasterType()
		{
			this.m_masterType = new MasterType?(Database.FindMasterType(this.MasterServerOrAvailabilityGroup));
		}

		public string ServerName
		{
			get
			{
				return (string)this[DatabaseSchema.ServerName];
			}
		}

		internal bool SMimeSignatureEnabled
		{
			get
			{
				return (bool)this[DatabaseSchema.SMimeSignatureEnabled];
			}
			set
			{
				this[DatabaseSchema.SMimeSignatureEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[DatabaseSchema.IssueWarningQuota];
			}
			set
			{
				this[DatabaseSchema.IssueWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan EventHistoryRetentionPeriod
		{
			get
			{
				return (EnhancedTimeSpan)this[DatabaseSchema.EventHistoryRetentionPeriod];
			}
			set
			{
				this[DatabaseSchema.EventHistoryRetentionPeriod] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public new string Name
		{
			get
			{
				return (string)this[DatabaseSchema.Name];
			}
			set
			{
				this[DatabaseSchema.Name] = value;
				this[ADConfigurationObjectSchema.AdminDisplayName] = value;
				this[DatabaseSchema.DisplayName] = value;
			}
		}

		public NonRootLocalLongFullPath LogFolderPath
		{
			get
			{
				return (NonRootLocalLongFullPath)this[DatabaseSchema.LogFolderPath];
			}
			internal set
			{
				this[DatabaseSchema.LogFolderPath] = value;
			}
		}

		internal NonRootLocalLongFullPath SystemFolderPath
		{
			get
			{
				return (NonRootLocalLongFullPath)this[DatabaseSchema.SystemFolderPath];
			}
			set
			{
				this[DatabaseSchema.SystemFolderPath] = value;
			}
		}

		public NonRootLocalLongFullPath TemporaryDataFolderPath
		{
			get
			{
				return (NonRootLocalLongFullPath)this[DatabaseSchema.TemporaryDataFolderPath];
			}
		}

		[Parameter(Mandatory = false)]
		public bool CircularLoggingEnabled
		{
			get
			{
				return (bool)this[DatabaseSchema.CircularLoggingEnabled];
			}
			set
			{
				this[DatabaseSchema.CircularLoggingEnabled] = value;
			}
		}

		public string LogFilePrefix
		{
			get
			{
				return (string)this[DatabaseSchema.LogFilePrefix];
			}
			internal set
			{
				this[DatabaseSchema.LogFilePrefix] = value;
			}
		}

		public int LogFileSize
		{
			get
			{
				return (int)this[DatabaseSchema.LogFileSize];
			}
		}

		public int? LogBuffers
		{
			get
			{
				return (int?)this[DatabaseSchema.LogBuffers];
			}
		}

		public int? MaximumOpenTables
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumOpenTables];
			}
		}

		public int? MaximumTemporaryTables
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumTemporaryTables];
			}
		}

		public int? MaximumCursors
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumCursors];
			}
		}

		public int? MaximumSessions
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumSessions];
			}
		}

		public int? MaximumVersionStorePages
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumVersionStorePages];
			}
		}

		public int? PreferredVersionStorePages
		{
			get
			{
				return (int?)this[DatabaseSchema.PreferredVersionStorePages];
			}
		}

		public int? DatabaseExtensionSize
		{
			get
			{
				return (int?)this[DatabaseSchema.DatabaseExtensionSize];
			}
		}

		public int? LogCheckpointDepth
		{
			get
			{
				return (int?)this[DatabaseSchema.LogCheckpointDepth];
			}
		}

		public int? ReplayCheckpointDepth
		{
			get
			{
				return (int?)this[DatabaseSchema.ReplayCheckpointDepth];
			}
		}

		public int? CachedClosedTables
		{
			get
			{
				return (int?)this[DatabaseSchema.CachedClosedTables];
			}
		}

		public int? CachePriority
		{
			get
			{
				return (int?)this[DatabaseSchema.CachePriority];
			}
		}

		public int? ReplayCachePriority
		{
			get
			{
				return (int?)this[DatabaseSchema.ReplayCachePriority];
			}
		}

		public int? MaximumPreReadPages
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumPreReadPages];
			}
		}

		public int? MaximumReplayPreReadPages
		{
			get
			{
				return (int?)this[DatabaseSchema.MaximumReplayPreReadPages];
			}
		}

		[Parameter(Mandatory = false)]
		public DataMoveReplicationConstraintParameter DataMoveReplicationConstraint
		{
			get
			{
				return (DataMoveReplicationConstraintParameter)this[DatabaseSchema.DataMoveReplicationConstraintDefinition];
			}
			set
			{
				this[DatabaseSchema.DataMoveReplicationConstraintDefinition] = value;
			}
		}

		public bool IsMailboxDatabase
		{
			get
			{
				return base.ObjectClass.Contains(MailboxDatabase.MostDerivedClass);
			}
		}

		public bool IsPublicFolderDatabase
		{
			get
			{
				return base.ObjectClass.Contains(PublicFolderDatabase.MostDerivedClass);
			}
		}

		public MailboxProvisioningAttributes MailboxProvisioningAttributes
		{
			get
			{
				return this[DatabaseSchema.MailboxProvisioningAttributes] as MailboxProvisioningAttributes;
			}
			set
			{
				this[DatabaseSchema.MailboxProvisioningAttributes] = value;
			}
		}

		internal Server GetServer()
		{
			return base.Session.Read<Server>(this.Server);
		}

		internal DatabaseCopy[] GetDatabaseCopies()
		{
			DatabaseCopy[] result;
			lock (this)
			{
				if (this.databaseCopies == null)
				{
					this.CompleteAllCalculatedProperties();
				}
				result = this.databaseCopies;
			}
			return result;
		}

		internal void ExcludeDatabaseCopyFromProperties(string hostServerToExclude)
		{
			lock (this)
			{
				if (this.allDatabaseCopies == null)
				{
					this.CompleteAllCalculatedProperties();
				}
				if (this.allDatabaseCopies != null && this.allDatabaseCopies.Length > 1)
				{
					DatabaseCopy[] array = (from dbCopy in this.allDatabaseCopies
					where !string.Equals(dbCopy.Name, hostServerToExclude, StringComparison.OrdinalIgnoreCase)
					select dbCopy).ToArray<DatabaseCopy>();
					if (array.Length != this.allDatabaseCopies.Length)
					{
						this.CompletePropertiesFromDbCopies(array);
					}
				}
			}
		}

		internal void CompleteAllCalculatedProperties()
		{
			IConfigurationSession configurationSession = this.CreateCustomConfigSessionIfNecessary();
			if (configurationSession == null)
			{
				return;
			}
			this.AdminDisplayVersion = this.AdminDisplayVersion;
			DatabaseCopy[] knownCopies = configurationSession.Find<DatabaseCopy>((ADObjectId)this.Identity, QueryScope.SubTree, null, null, 0);
			this.CompletePropertiesFromDbCopies(knownCopies);
			if (this.rpcClientAccessServerFqdn == null)
			{
				this.CalculateRpcClientAccessServer();
			}
			if (this.m_masterType == null)
			{
				this.CalculateMasterType();
			}
		}

		private void CompletePropertiesFromDbCopies(DatabaseCopy[] knownCopies)
		{
			if (knownCopies == null || knownCopies.Length == 0)
			{
				this.databaseCopies = new DatabaseCopy[0];
				this.invalidDatabaseCopies = new DatabaseCopy[0];
				this.allDatabaseCopies = new DatabaseCopy[0];
				this.servers = new ADObjectId[0];
				this.activationPreference = new KeyValuePair<ADObjectId, int>[0];
				this.replayLagTimes = new KeyValuePair<ADObjectId, EnhancedTimeSpan>[0];
				this.truncationLagTimes = new KeyValuePair<ADObjectId, EnhancedTimeSpan>[0];
				this.replicationType = ReplicationType.None;
				return;
			}
			Array.Sort<DatabaseCopy>(knownCopies);
			int num = 1;
			List<DatabaseCopy> list = new List<DatabaseCopy>(knownCopies.Length);
			List<DatabaseCopy> list2 = new List<DatabaseCopy>(knownCopies.Length);
			List<DatabaseCopy> list3 = new List<DatabaseCopy>(knownCopies.Length);
			List<ADObjectId> list4 = new List<ADObjectId>(knownCopies.Length);
			List<KeyValuePair<ADObjectId, int>> list5 = new List<KeyValuePair<ADObjectId, int>>(knownCopies.Length);
			List<KeyValuePair<ADObjectId, EnhancedTimeSpan>> list6 = new List<KeyValuePair<ADObjectId, EnhancedTimeSpan>>(knownCopies.Length);
			List<KeyValuePair<ADObjectId, EnhancedTimeSpan>> list7 = new List<KeyValuePair<ADObjectId, EnhancedTimeSpan>>(knownCopies.Length);
			foreach (DatabaseCopy databaseCopy in knownCopies)
			{
				if (databaseCopy.IsValidForRead && databaseCopy.IsHostServerPresent)
				{
					databaseCopy.ActivationPreference = num++;
					list.Add(databaseCopy);
					list3.Add(databaseCopy);
					list4.Add(databaseCopy.HostServer);
					list5.Add(new KeyValuePair<ADObjectId, int>(databaseCopy.HostServer, databaseCopy.ActivationPreference));
					list6.Add(new KeyValuePair<ADObjectId, EnhancedTimeSpan>(databaseCopy.HostServer, databaseCopy.ReplayLagTime));
					list7.Add(new KeyValuePair<ADObjectId, EnhancedTimeSpan>(databaseCopy.HostServer, databaseCopy.TruncationLagTime));
				}
				else
				{
					databaseCopy.ActivationPreference = num++;
					list2.Add(databaseCopy);
					list3.Add(databaseCopy);
				}
			}
			this.databaseCopies = list.ToArray();
			this.invalidDatabaseCopies = list2.ToArray();
			this.allDatabaseCopies = list3.ToArray();
			this.servers = list4.ToArray();
			this.activationPreference = list5.ToArray();
			this.replayLagTimes = list6.ToArray();
			this.truncationLagTimes = list7.ToArray();
			if (this.allDatabaseCopies.Length > 1)
			{
				this.replicationType = ReplicationType.Remote;
				return;
			}
			this.replicationType = ReplicationType.None;
		}

		private IConfigurationSession CreateCustomConfigSessionIfNecessary()
		{
			IConfigurationSession configurationSession = base.Session;
			if (configurationSession != null && configurationSession.ConsistencyMode != ConsistencyMode.PartiallyConsistent)
			{
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(configurationSession.DomainController, configurationSession.ReadOnly, ConsistencyMode.PartiallyConsistent, configurationSession.NetworkCredential, configurationSession.SessionSettings, 2385, "CreateCustomConfigSessionIfNecessary", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\database.cs");
			}
			return configurationSession;
		}

		private void CalculateRpcClientAccessServer()
		{
			MiniClientAccessServerOrArray miniClientAccessServerOrArray;
			if (base.Session != null && ((ITopologyConfigurationSession)base.Session).TryFindByExchangeLegacyDN(this.RpcClientAccessServerLegacyDN, Database.s_propertiesNeededFromServer, out miniClientAccessServerOrArray))
			{
				this.rpcClientAccessServerFqdn = miniClientAccessServerOrArray.Fqdn;
				return;
			}
			this.rpcClientAccessServerFqdn = string.Empty;
		}

		internal DatabaseCopy GetDatabaseCopy(ADObjectId server)
		{
			return this.GetDatabaseCopy(server.Name);
		}

		internal DatabaseCopy GetDatabaseCopy(string serverShortName)
		{
			DatabaseCopy result = null;
			lock (this)
			{
				if (this.databaseCopies == null)
				{
					this.CompleteAllCalculatedProperties();
				}
				foreach (DatabaseCopy databaseCopy in this.databaseCopies)
				{
					if (MachineName.Comparer.Equals(databaseCopy.HostServerName, serverShortName))
					{
						result = databaseCopy;
						break;
					}
				}
			}
			return result;
		}

		internal bool IsExchange2009OrLater
		{
			get
			{
				return (bool)this[DatabaseSchema.IsExchange2009OrLater];
			}
		}

		internal Database CreateDetachedWriteableClone<TDatabase>() where TDatabase : Database, new()
		{
			TDatabase tdatabase = (TDatabase)((object)this.Clone());
			tdatabase.SetIsReadOnly(false);
			return tdatabase;
		}

		private static MasterType FindMasterType(ADObjectId masterId)
		{
			if (masterId != null)
			{
				ADObjectId parent = masterId.Parent;
				if (parent != null)
				{
					if (string.Compare(parent.Name, DatabaseAvailabilityGroupContainer.DefaultName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return MasterType.DatabaseAvailabilityGroup;
					}
					if (string.Compare(parent.Name, ServersContainer.DefaultName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return MasterType.Server;
					}
				}
			}
			return MasterType.Unknown;
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(DatabaseSchema.MaintenanceSchedule))
			{
				this.MaintenanceSchedule = Schedule.DailyFrom1AMTo5AM;
			}
			if (!base.IsModified(DatabaseSchema.QuotaNotificationSchedule))
			{
				this.QuotaNotificationSchedule = Schedule.Daily1AM;
			}
			if (!base.IsModified(DatabaseSchema.IssueWarningQuota))
			{
				this.IssueWarningQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromMB(1945UL));
			}
			base.StampPersistableDefaultValues();
		}

		internal static ValidationError[] ValidateAscendingQuotas(PropertyBag propertyBag, ProviderPropertyDefinition[] ascendingQuotaDefinitions, ObjectId identity)
		{
			List<ValidationError> list = new List<ValidationError>(ascendingQuotaDefinitions.Length);
			ProviderPropertyDefinition providerPropertyDefinition = null;
			Unlimited<ByteQuantifiedSize> unlimited = default(Unlimited<ByteQuantifiedSize>);
			int num = 0;
			while (ascendingQuotaDefinitions.Length > num)
			{
				ProviderPropertyDefinition providerPropertyDefinition2 = ascendingQuotaDefinitions[num];
				Unlimited<ByteQuantifiedSize> unlimited2 = (Unlimited<ByteQuantifiedSize>)propertyBag[providerPropertyDefinition2];
				if (!unlimited2.IsUnlimited)
				{
					if (providerPropertyDefinition != null && 0 < unlimited.CompareTo(unlimited2))
					{
						if (propertyBag.IsChanged(providerPropertyDefinition))
						{
							list.Add(new ObjectValidationError(DirectoryStrings.ErrorProperty1GtProperty2(providerPropertyDefinition.Name, unlimited.ToString(), providerPropertyDefinition2.Name, unlimited2.ToString()), identity, string.Empty));
						}
						else
						{
							list.Add(new ObjectValidationError(DirectoryStrings.ErrorProperty1LtProperty2(providerPropertyDefinition2.Name, unlimited2.ToString(), providerPropertyDefinition.Name, unlimited.ToString()), identity, string.Empty));
						}
					}
					providerPropertyDefinition = providerPropertyDefinition2;
					unlimited = unlimited2;
				}
				num++;
			}
			return list.ToArray();
		}

		internal static void InternalAssertComparisonFilter(SinglePropertyFilter filter, PropertyDefinition propertyDefinition)
		{
			string name = propertyDefinition.Name;
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(name, filter.GetType(), typeof(ComparisonFilter)));
			}
			object propertyValue = comparisonFilter.PropertyValue;
			if (propertyValue == null)
			{
				throw new ArgumentNullException("filter.PropertyValue");
			}
			Type type = propertyValue.GetType();
			if (type != propertyDefinition.Type)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValueType(name, type, propertyDefinition.Type));
			}
		}

		internal static object AdministrativeGroupGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				result = ((ADObjectId)propertyBag[ADObjectSchema.Id]).DescendantDN(6);
			}
			catch (NullReferenceException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdministrativeGroup", ex.Message), DatabaseSchema.AdministrativeGroup, propertyBag[ADObjectSchema.Id]), ex);
			}
			catch (InvalidOperationException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdministrativeGroup", ex2.Message), DatabaseSchema.AdministrativeGroup, propertyBag[ADObjectSchema.Id]), ex2);
			}
			return result;
		}

		internal static object MaintenanceScheduleGetter(IPropertyBag propertyBag)
		{
			return propertyBag[DatabaseSchema.MaintenanceScheduleBitmaps];
		}

		internal static void MaintenanceScheduleSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[DatabaseSchema.MaintenanceScheduleBitmaps] = value;
			if (value == null)
			{
				propertyBag[DatabaseSchema.MaintenanceScheduleMode] = ScheduleMode.Never;
				return;
			}
			propertyBag[DatabaseSchema.MaintenanceScheduleMode] = ((Schedule)value).Mode;
		}

		internal static QueryFilter MountAtStartupFilterBuilder(SinglePropertyFilter filter)
		{
			Database.InternalAssertComparisonFilter(filter, DatabaseSchema.MountAtStartup);
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, DatabaseSchema.EdbOfflineAtStartup, !(bool)comparisonFilter.PropertyValue);
		}

		internal static object OrganizationGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				result = ((ADObjectId)propertyBag[ADObjectSchema.Id]).DescendantDN(4);
			}
			catch (NullReferenceException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Organization", ex.Message), DatabaseSchema.Organization, propertyBag[ADObjectSchema.Id]), ex);
			}
			catch (InvalidOperationException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Organization", ex2.Message), DatabaseSchema.Organization, propertyBag[ADObjectSchema.Id]), ex2);
			}
			return result;
		}

		internal static object QuotaNotificationScheduleGetter(IPropertyBag propertyBag)
		{
			return propertyBag[DatabaseSchema.QuotaNotificationScheduleBitmaps];
		}

		internal static void QuotaNotificationScheduleSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[DatabaseSchema.QuotaNotificationScheduleBitmaps] = value;
			if (value == null)
			{
				propertyBag[DatabaseSchema.QuotaNotificationMode] = ScheduleMode.Never;
				return;
			}
			propertyBag[DatabaseSchema.QuotaNotificationMode] = ((Schedule)value).Mode;
		}

		internal static QueryFilter RetainDeletedItemsUntilBackupFilterBuilder(SinglePropertyFilter filter)
		{
			Database.InternalAssertComparisonFilter(filter, DatabaseSchema.RetainDeletedItemsUntilBackup);
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, DatabaseSchema.DelItemAfterBackupEnum, ((bool)comparisonFilter.PropertyValue) ? Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainUntilBackupOrCustomPeriod : Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainForCustomPeriod);
		}

		internal static object RetainDeletedItemsUntilBackupGetter(IPropertyBag propertyBag)
		{
			return Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainUntilBackupOrCustomPeriod == (DeletedItemRetention)propertyBag[DatabaseSchema.DelItemAfterBackupEnum];
		}

		internal static void RetainDeletedItemsUntilBackupSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[DatabaseSchema.DelItemAfterBackupEnum] = (((bool)value) ? Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainUntilBackupOrCustomPeriod : Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainForCustomPeriod);
		}

		internal static object ServerNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[DatabaseSchema.Server];
			if (adobjectId == null)
			{
				return DatabaseSchema.ServerName.DefaultValue;
			}
			return adobjectId.Name;
		}

		internal static object IsExchange2009OrLaterGetter(IPropertyBag propertyBag)
		{
			ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)propertyBag[ADObjectSchema.ExchangeVersion];
			return !exchangeObjectVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010);
		}

		internal static object MasterServerOrAvailabilityGroupNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[DatabaseSchema.MasterServerOrAvailabilityGroup];
			if (adobjectId == null)
			{
				return DatabaseSchema.MasterServerOrAvailabilityGroupName.DefaultValue;
			}
			return adobjectId.Name;
		}

		internal static object DatabaseNameGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADObjectSchema.RawName];
		}

		internal static void DatabaseNameSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADObjectSchema.RawName] = value;
		}

		internal static LegacyDN GetRcaLegacyDNFromDatabaseLegacyDN(LegacyDN database)
		{
			return database.GetParentLegacyDN();
		}

		internal static LegacyDN GetDatabaseLegacyDNFromRcaLegacyDN(LegacyDN rpcClientAccessServer, bool isPublic)
		{
			return new LegacyDN(rpcClientAccessServer, "cn", isPublic ? "Microsoft Public MDB" : "Microsoft Private MDB");
		}

		internal static object RpcClientAccessServerExchangeLegacyDNGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[DatabaseSchema.ExchangeLegacyDN];
			LegacyDN database;
			if (LegacyDN.TryParse(text, out database))
			{
				return Database.GetRcaLegacyDNFromDatabaseLegacyDN(database).ToString();
			}
			throw new DataValidationException(new ValidLegacyDNConstraint().Validate(text, DatabaseSchema.ExchangeLegacyDN, propertyBag));
		}

		internal static void RpcClientAccessServerExchangeLegacyDNSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[DatabaseSchema.ExchangeLegacyDN] = Database.GetDatabaseLegacyDNFromRcaLegacyDN(LegacyDN.Parse((string)value), ((MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass]).Contains(PublicFolderDatabase.MostDerivedClass)).ToString();
		}

		internal static object MailboxProvisioningAttributesGetter(IPropertyBag propertyBag)
		{
			DatabaseConfigXml databaseConfigXml = (DatabaseConfigXml)DatabaseSchema.ConfigurationXML.GetterDelegate(propertyBag);
			List<MailboxProvisioningAttribute> list;
			if (databaseConfigXml != null && databaseConfigXml.MailboxProvisioningAttributes != null)
			{
				list = new List<MailboxProvisioningAttribute>(databaseConfigXml.MailboxProvisioningAttributes.Attributes);
			}
			else
			{
				list = new List<MailboxProvisioningAttribute>();
			}
			MailboxProvisioningAttribute item = new MailboxProvisioningAttribute
			{
				Key = MailboxProvisioningAttributesSchema.DatabaseName.Name,
				Value = (string)Database.DatabaseNameGetter(propertyBag)
			};
			list.Add(item);
			MailboxProvisioningAttribute item2 = new MailboxProvisioningAttribute
			{
				Key = MailboxProvisioningAttributesSchema.ServerName.Name,
				Value = (string)Database.ServerNameGetter(propertyBag)
			};
			list.Add(item2);
			ADObjectId adobjectId = (ADObjectId)propertyBag[DatabaseSchema.MasterServerOrAvailabilityGroup];
			if (Database.FindMasterType(adobjectId) == MasterType.DatabaseAvailabilityGroup)
			{
				MailboxProvisioningAttribute item3 = new MailboxProvisioningAttribute
				{
					Key = MailboxProvisioningAttributesSchema.DagName.Name,
					Value = adobjectId.Name
				};
				list.Add(item3);
			}
			return new MailboxProvisioningAttributes(list.ToArray());
		}

		internal static void MailboxProvisioningAttributesSetter(object value, IPropertyBag propertyBag)
		{
			MailboxProvisioningAttributes mailboxProvisioningAttributes = (MailboxProvisioningAttributes)value;
			DatabaseConfigXml databaseConfigXml = (DatabaseConfigXml)DatabaseSchema.ConfigurationXML.GetterDelegate(propertyBag);
			if (databaseConfigXml == null)
			{
				databaseConfigXml = new DatabaseConfigXml();
			}
			databaseConfigXml.MailboxProvisioningAttributes = mailboxProvisioningAttributes;
			DatabaseSchema.ConfigurationXML.SetterDelegate(databaseConfigXml, propertyBag);
		}

		private const string MostDerivedClassInternal = "msExchMDB";

		private const string PrivateMdbCnInternal = "Microsoft Private MDB";

		private const string PublicMdbCnInternal = "Microsoft Public MDB";

		private static readonly DatabaseSchema schema = ObjectSchema.GetInstance<DatabaseSchema>();

		private static readonly PropertyDefinition[] s_propertiesNeededFromServer = new PropertyDefinition[]
		{
			ServerSchema.Fqdn
		};

		private bool? databaseMounted;

		private bool? databaseOnlineMaintenanceInProgress;

		private bool? databaseBackupInProgress;

		private bool? snapshotLastFullBackup;

		private bool? snapshotLastIncrementalBackup;

		private bool? snapshotLastDifferentialBackup;

		private bool? snapshotLastCopyBackup;

		private DateTime? databaseLastFullBackup;

		private DateTime? databaseLastIncrementalBackup;

		private DateTime? databaseLastDifferentialBackup;

		private DateTime? databaseLastCopyBackup;

		private ByteQuantifiedSize? databaseSize;

		private ByteQuantifiedSize? availableNewMailboxSpace;

		private DatabaseCopy[] databaseCopies;

		private DatabaseCopy[] invalidDatabaseCopies;

		private DatabaseCopy[] allDatabaseCopies;

		private ADObjectId[] servers;

		private KeyValuePair<ADObjectId, int>[] activationPreference;

		private KeyValuePair<ADObjectId, EnhancedTimeSpan>[] replayLagTimes;

		private KeyValuePair<ADObjectId, EnhancedTimeSpan>[] truncationLagTimes;

		private ReplicationType replicationType = ReplicationType.Unknown;

		private string mountedOnServer;

		private string rpcClientAccessServerFqdn;

		private int? workerProcessId;

		private string currentSchemaVersion;

		private string requestedSchemaVersion;

		private MasterType? m_masterType = null;
	}
}
