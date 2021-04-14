using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADDatabaseWrapper : ADObjectWrapperBase, IADDatabase, IADObjectCommon
	{
		private void FinishConstructionCommon(ADObject sourceObj)
		{
			this.MasterServerOrAvailabilityGroup = (ADObjectId)sourceObj[DatabaseSchema.MasterServerOrAvailabilityGroup];
			this.EdbFilePath = (EdbFilePath)sourceObj[DatabaseSchema.EdbFilePath];
			this.LogFolderPath = (NonRootLocalLongFullPath)sourceObj[DatabaseSchema.LogFolderPath];
			this.SystemFolderPath = (NonRootLocalLongFullPath)sourceObj[DatabaseSchema.SystemFolderPath];
			this.LogFilePrefix = (string)sourceObj[DatabaseSchema.LogFilePrefix];
			this.Recovery = (bool)sourceObj[DatabaseSchema.Recovery];
			this.AutoDagExcludeFromMonitoring = (bool)sourceObj[DatabaseSchema.AutoDagExcludeFromMonitoring];
			this.Server = (ADObjectId)sourceObj[DatabaseSchema.Server];
			this.MountAtStartup = (bool)sourceObj[DatabaseSchema.MountAtStartup];
			this.DatabaseCreated = (bool)sourceObj[DatabaseSchema.DatabaseCreated];
			this.AllowFileRestore = (bool)sourceObj[DatabaseSchema.AllowFileRestore];
			this.CircularLoggingEnabled = (bool)sourceObj[DatabaseSchema.CircularLoggingEnabled];
			this.ExchangeLegacyDN = (string)sourceObj[DatabaseSchema.ExchangeLegacyDN];
			this.RpcClientAccessServerLegacyDN = (string)sourceObj[DatabaseSchema.RpcClientAccessServerExchangeLegacyDN];
			this.DistinguishedName = sourceObj.DistinguishedName;
			this.MailboxPublicFolderDatabase = (ADObjectId)sourceObj[DatabaseSchema.MailboxPublicFolderDatabase];
			this.IsExchange2009OrLater = (bool)sourceObj[DatabaseSchema.IsExchange2009OrLater];
			this.DatabaseGroup = (string)sourceObj[DatabaseSchema.DatabaseGroup];
			this.IsPublicFolderDatabase = sourceObj.ObjectClass.Contains(PublicFolderDatabase.MostDerivedClass);
			this.IsMailboxDatabase = sourceObj.ObjectClass.Contains(MailboxDatabase.MostDerivedClass);
		}

		internal ADDatabaseWrapper(IADDatabase database) : base(database)
		{
			this.MasterServerOrAvailabilityGroup = database.MasterServerOrAvailabilityGroup;
			this.EdbFilePath = database.EdbFilePath;
			this.LogFolderPath = database.LogFolderPath;
			this.SystemFolderPath = database.SystemFolderPath;
			this.LogFilePrefix = database.LogFilePrefix;
			this.Recovery = database.Recovery;
			this.AutoDagExcludeFromMonitoring = database.AutoDagExcludeFromMonitoring;
			this.Server = database.Server;
			this.MountAtStartup = database.MountAtStartup;
			this.DatabaseCreated = database.DatabaseCreated;
			this.AllowFileRestore = database.AllowFileRestore;
			this.CircularLoggingEnabled = database.CircularLoggingEnabled;
			this.ExchangeLegacyDN = database.ExchangeLegacyDN;
			this.RpcClientAccessServerLegacyDN = database.RpcClientAccessServerLegacyDN;
			this.DistinguishedName = database.DistinguishedName;
			this.MailboxPublicFolderDatabase = database.MailboxPublicFolderDatabase;
			this.IsExchange2009OrLater = database.IsExchange2009OrLater;
			this.DatabaseGroup = database.DatabaseGroup;
			this.IsPublicFolderDatabase = database.IsPublicFolderDatabase;
			this.IsMailboxDatabase = database.IsMailboxDatabase;
		}

		private ADDatabaseWrapper(Database database) : base(database)
		{
			this.FinishConstructionCommon(database);
			this.CompletePropertiesFromDbCopies(database.AllDatabaseCopies);
		}

		private ADDatabaseWrapper(MiniDatabase database) : base(database)
		{
			this.FinishConstructionCommon(database);
		}

		private IConfigurationSession CreateCustomConfigSessionIfNecessary(IConfigurationSession existingSession)
		{
			IConfigurationSession configurationSession = existingSession;
			if (configurationSession != null && configurationSession.ConsistencyMode != ConsistencyMode.PartiallyConsistent)
			{
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(configurationSession.DomainController, configurationSession.ReadOnly, ConsistencyMode.PartiallyConsistent, configurationSession.NetworkCredential, configurationSession.SessionSettings, 567, "CreateCustomConfigSessionIfNecessary", "f:\\15.00.1497\\sources\\dev\\data\\src\\HA\\DirectoryServices\\ADObjectWrappers.cs");
			}
			return configurationSession;
		}

		public void FinishConstructionFromMiniDatabase(IConfigurationSession session)
		{
			session = this.CreateCustomConfigSessionIfNecessary(session);
			DatabaseCopy[] allCopies = session.Find<DatabaseCopy>((ADObjectId)base.Identity, QueryScope.SubTree, null, null, 0);
			this.CompletePropertiesFromDbCopies(allCopies);
		}

		private void CompletePropertiesFromDbCopies(DatabaseCopy[] allCopies)
		{
			if (allCopies == null || allCopies.Length == 0)
			{
				this.AssignCopies(null);
				return;
			}
			ADDatabaseCopyWrapper[] array = new ADDatabaseCopyWrapper[allCopies.Length];
			for (int i = 0; i < allCopies.Length; i++)
			{
				array[i] = ADObjectWrapperFactory.CreateWrapper(allCopies[i]);
			}
			this.AssignCopies(array);
		}

		private void AssignCopies(ADDatabaseCopyWrapper[] knownCopies)
		{
			if (knownCopies == null || knownCopies.Length == 0)
			{
				this.validDbCopies = new ADDatabaseCopyWrapper[0];
				this.allDbCopies = new ADDatabaseCopyWrapper[0];
				this.servers = new ADObjectId[0];
				this.ReplicationType = ReplicationType.None;
				return;
			}
			Array.Sort<ADDatabaseCopyWrapper>(knownCopies);
			int num = 1;
			List<ADDatabaseCopyWrapper> list = new List<ADDatabaseCopyWrapper>(knownCopies.Length);
			List<ADObjectId> list2 = new List<ADObjectId>(knownCopies.Length);
			foreach (ADDatabaseCopyWrapper addatabaseCopyWrapper in knownCopies)
			{
				addatabaseCopyWrapper.ActivationPreference = num++;
				if (addatabaseCopyWrapper.IsValidForRead && addatabaseCopyWrapper.IsHostServerPresent)
				{
					list.Add(addatabaseCopyWrapper);
					list2.Add(addatabaseCopyWrapper.HostServer);
				}
			}
			this.allDbCopies = knownCopies;
			this.validDbCopies = list.ToArray();
			this.servers = list2.ToArray();
			this.HostServerForPreference1 = knownCopies[0].HostServer;
			if (this.allDbCopies.Length > 1)
			{
				this.replicationType = new ReplicationType?(ReplicationType.Remote);
				return;
			}
			this.replicationType = new ReplicationType?(ReplicationType.None);
		}

		public void ExcludeDatabaseCopyFromProperties(string hostServerToExclude)
		{
			if (this.allDbCopies == null || this.allDbCopies.Length <= 1)
			{
				return;
			}
			ADDatabaseCopyWrapper[] knownCopies = (from dbCopy in this.allDbCopies
			where !string.Equals(dbCopy.Name, hostServerToExclude, StringComparison.OrdinalIgnoreCase)
			select dbCopy).ToArray<ADDatabaseCopyWrapper>();
			this.AssignCopies(knownCopies);
		}

		public static ADDatabaseWrapper CreateWrapper(Database database)
		{
			if (database == null)
			{
				return null;
			}
			return new ADDatabaseWrapper(database);
		}

		public static ADDatabaseWrapper CreateWrapper(MiniDatabase database)
		{
			if (database == null)
			{
				return null;
			}
			return new ADDatabaseWrapper(database);
		}

		public ReplicationType ReplicationType
		{
			get
			{
				return this.replicationType.Value;
			}
			private set
			{
				this.replicationType = new ReplicationType?(value);
			}
		}

		public EdbFilePath EdbFilePath
		{
			get
			{
				base.CheckMinimizedProperty("EdbFilePath");
				return this._edbFilePath;
			}
			private set
			{
				this._edbFilePath = value;
			}
		}

		public NonRootLocalLongFullPath LogFolderPath
		{
			get
			{
				base.CheckMinimizedProperty("LogFolderPath");
				return this._logFolderPath;
			}
			private set
			{
				this._logFolderPath = value;
			}
		}

		public NonRootLocalLongFullPath SystemFolderPath
		{
			get
			{
				base.CheckMinimizedProperty("SystemFolderPath");
				return this._systemFolderPath;
			}
			private set
			{
				this._systemFolderPath = value;
			}
		}

		public ADObjectId HostServerForPreference1 { get; private set; }

		public bool Recovery { get; private set; }

		public bool AutoDagExcludeFromMonitoring { get; private set; }

		public IADDatabaseCopy[] DatabaseCopies
		{
			get
			{
				base.CheckMinimizedProperty("DatabaseCopies");
				return this.validDbCopies;
			}
		}

		public IADDatabaseCopy[] AllDatabaseCopies
		{
			get
			{
				base.CheckMinimizedProperty("AllDatabaseCopies");
				return this.allDbCopies;
			}
		}

		private IADDatabaseCopy FindCopy(string serverShortName, IADDatabaseCopy[] copies)
		{
			if (copies != null)
			{
				foreach (IADDatabaseCopy iaddatabaseCopy in copies)
				{
					if (MachineName.Comparer.Equals(iaddatabaseCopy.HostServerName, serverShortName))
					{
						return iaddatabaseCopy;
					}
				}
			}
			return null;
		}

		public IADDatabaseCopy GetDatabaseCopy(string serverShortName)
		{
			return this.FindCopy(serverShortName, this.AllDatabaseCopies);
		}

		public ADObjectId Server
		{
			get
			{
				return this._owningServer;
			}
			private set
			{
				this._owningServer = value;
			}
		}

		public ADObjectId[] Servers
		{
			get
			{
				base.CheckMinimizedProperty("ADDatabaseWrapper.Servers");
				return this.servers;
			}
		}

		public bool MountAtStartup { get; private set; }

		public bool DatabaseCreated { get; private set; }

		public bool AllowFileRestore { get; private set; }

		public string DistinguishedName
		{
			get
			{
				base.CheckMinimizedProperty("ADDatabaseWrapper.DistinguishedName");
				return this._distinguishedName;
			}
			private set
			{
				this._distinguishedName = value;
			}
		}

		public string LogFilePrefix { get; private set; }

		public bool IsPublicFolderDatabase { get; private set; }

		public bool IsMailboxDatabase { get; private set; }

		public bool CircularLoggingEnabled { get; private set; }

		public string ExchangeLegacyDN { get; private set; }

		public string RpcClientAccessServerLegacyDN { get; private set; }

		public ADObjectId MailboxPublicFolderDatabase { get; private set; }

		public bool IsExchange2009OrLater { get; private set; }

		public ADObjectId MasterServerOrAvailabilityGroup { get; private set; }

		public string DatabaseGroup { get; private set; }

		public override void Minimize()
		{
			this.allDbCopies = null;
			this.validDbCopies = null;
			this.EdbFilePath = null;
			this.LogFolderPath = null;
			this.SystemFolderPath = null;
			this.servers = null;
			this._distinguishedName = null;
			base.Minimize();
		}

		public static readonly ADPropertyDefinition[] PropertiesNeededForDatabase = new ADPropertyDefinition[]
		{
			DatabaseSchema.MasterServerOrAvailabilityGroup,
			DatabaseSchema.EdbFilePath,
			DatabaseSchema.LogFolderPath,
			DatabaseSchema.SystemFolderPath,
			DatabaseSchema.LogFilePrefix,
			DatabaseSchema.Recovery,
			DatabaseSchema.AutoDagExcludeFromMonitoring,
			DatabaseSchema.Server,
			DatabaseSchema.MountAtStartup,
			DatabaseSchema.DatabaseCreated,
			DatabaseSchema.AllowFileRestore,
			DatabaseSchema.CircularLoggingEnabled,
			DatabaseSchema.ExchangeLegacyDN,
			DatabaseSchema.RpcClientAccessServerExchangeLegacyDN,
			DatabaseSchema.MailboxPublicFolderDatabase,
			DatabaseSchema.IsExchange2009OrLater,
			DatabaseSchema.DatabaseGroup,
			ADObjectSchema.ObjectClass
		};

		private ADDatabaseCopyWrapper[] validDbCopies;

		private ADDatabaseCopyWrapper[] allDbCopies;

		private ADObjectId[] servers;

		private ReplicationType? replicationType;

		private EdbFilePath _edbFilePath;

		private NonRootLocalLongFullPath _logFolderPath;

		private NonRootLocalLongFullPath _systemFolderPath;

		private ADObjectId _owningServer;

		private string _distinguishedName;
	}
}
