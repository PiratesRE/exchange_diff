using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewDatabaseTask<TDataObject> : NewFixedNameSystemConfigurationObjectTask<TDataObject> where TDataObject : Database, new()
	{
		protected string Name
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.Name;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.Name = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter]
		public EdbFilePath EdbFilePath
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.EdbFilePath;
			}
			set
			{
				if (null != value)
				{
					value.ValidateEdbFileExtension();
					TDataObject dataObject = this.DataObject;
					dataObject.EdbFilePath = value;
				}
			}
		}

		[Parameter]
		public NonRootLocalLongFullPath LogFolderPath
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.LogFolderPath;
			}
			set
			{
				if (null != value)
				{
					TDataObject dataObject = this.DataObject;
					dataObject.LogFolderPath = value;
				}
			}
		}

		[Parameter]
		public SwitchParameter SkipDatabaseLogFolderCreation
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipDatabaseLogFolderCreation"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipDatabaseLogFolderCreation"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || SystemConfigurationTasksHelper.IsKnownWmiException(exception) || SystemConfigurationTasksHelper.IsKnownClusterUpdateDatabaseResourceException(exception);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			Database database = (Database)base.PrepareDataObject();
			database.InvalidDatabaseCopiesAllowed = true;
			if (this.preExistingDatabase != null)
			{
				database = this.preExistingDatabase;
				database.InvalidDatabaseCopiesAllowed = true;
				TaskLogger.LogExit();
				return database;
			}
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.GlobalConfigSession, typeof(PublicFolderDatabase), null, this.OwnerServer.Identity, true));
			this.ownerServerPublicFolderDatabases = this.OwnerServer.GetPublicFolderDatabases();
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.GlobalConfigSession, typeof(Database), null, this.OwnerServer.Identity, true));
			this.ownerServerDatabases = this.OwnerServer.GetDatabases();
			string logFilePrefix = this.CalculateLogFilePrefix();
			database.LogFilePrefix = logFilePrefix;
			if (null != this.LogFolderPath)
			{
				try
				{
					this.ValidateLogFolderPath();
				}
				catch (WmiException ex)
				{
					Exception exception = new InvalidOperationException(Strings.ErrorFailedToConnectToServer(this.OwnerServer.Name, ex.Message));
					ErrorCategory category = ErrorCategory.InvalidOperation;
					TDataObject dataObject = this.DataObject;
					base.WriteError(exception, category, dataObject.Identity);
				}
				catch (UnauthorizedAccessException ex2)
				{
					Exception exception2 = new InvalidOperationException(Strings.ErrorFailedToConnectToServer(this.OwnerServer.Name, ex2.Message));
					ErrorCategory category2 = ErrorCategory.InvalidOperation;
					TDataObject dataObject2 = this.DataObject;
					base.WriteError(exception2, category2, dataObject2.Identity);
				}
			}
			database.SetId(base.GlobalConfigSession.GetDatabasesContainerId().GetChildId(this.Name));
			database.Name = this.Name;
			database.AdminDisplayName = this.Name;
			database.Server = (ADObjectId)this.OwnerServer.Identity;
			TDataObject dataObject3 = this.DataObject;
			dataObject3.DataMoveReplicationConstraint = DataMoveReplicationConstraintParameter.None;
			if (this.OwnerServer.DatabaseAvailabilityGroup != null)
			{
				database.MasterServerOrAvailabilityGroup = this.OwnerServer.DatabaseAvailabilityGroup;
			}
			else
			{
				database.MasterServerOrAvailabilityGroup = (ADObjectId)this.OwnerServer.Identity;
			}
			string text = new ClientAccessArrayTaskHelper(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError)).FindRpcClientAccessArrayOrServer((ITopologyConfigurationSession)this.ConfigurationSession, database.Server);
			if (text != null)
			{
				database.RpcClientAccessServerLegacyDN = text;
			}
			else
			{
				database.RpcClientAccessServerLegacyDN = this.OwnerServer.ExchangeLegacyDN;
			}
			TaskLogger.LogExit();
			return database;
		}

		private string CalculateLogFilePrefix()
		{
			string text = null;
			for (int i = 0; i < 256; i++)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "E{0:X2}", new object[]
				{
					i
				});
				bool flag = false;
				foreach (Database database in this.OwnerServerDatabases)
				{
					if (database.LogFilePrefix.Equals(text2, StringComparison.InvariantCultureIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					text = text2;
					break;
				}
			}
			if (text == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorExceededMaxiumNumberOfDatabasesPerServer), ErrorCategory.InvalidOperation, this.Server);
			}
			return text;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Name, this.Name);
			Database[] array = base.GlobalConfigSession.Find<Database>(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length == 1)
			{
				DatabaseCopy[] databaseCopies = array[0].GetDatabaseCopies();
				if (databaseCopies != null && databaseCopies.Length > 0)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorDatabaseNotUnique(this.Name)), ErrorCategory.InvalidOperation, this.Name);
				}
				else
				{
					Database database = array[0];
					this.preExistingDatabase = (TDataObject)((object)base.DataSession.Read<TDataObject>(database.Id));
				}
			}
			else if (array != null && array.Length > 1)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorDatabaseNotUnique(this.Name)), ErrorCategory.InvalidOperation, this.Name);
			}
			this.ownerServer = (Server)base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			MapiTaskHelper.VerifyIsWithinConfigWriteScope(base.SessionSettings, this.ownerServer, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			if (!this.ownerServer.IsE14OrLater)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorModifyE12ServerNotAllowed), ErrorCategory.InvalidOperation, this.ownerServer.Identity);
			}
			if (!this.ownerServer.IsMailboxServer)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorOperationOnlyOnMailboxServer(this.ownerServer.Name)), ErrorCategory.InvalidOperation, this.ownerServer.Identity);
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (!this.SkipDatabaseLogFolderCreation)
			{
				string directoryName = Path.GetDirectoryName(this.EdbFilePath.PathName);
				SystemConfigurationTasksHelper.TryCreateDirectory(this.ownerServer.Fqdn, directoryName, Database_Directory.GetDomainWidePermissions(), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				SystemConfigurationTasksHelper.TryCreateDirectory(this.ownerServer.Fqdn, this.LogFolderPath.PathName, Database_Directory.GetDomainWidePermissions(), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			TaskLogger.LogExit();
		}

		protected DatabaseCopy SaveDBCopy()
		{
			TaskLogger.LogEnter();
			DatabaseCopy databaseCopy = null;
			if (this.preExistingDatabase != null)
			{
				foreach (DatabaseCopy databaseCopy2 in this.preExistingDatabase.InvalidDatabaseCopies)
				{
					if (databaseCopy2.Name.Equals(this.OwnerServer.Name, StringComparison.OrdinalIgnoreCase))
					{
						databaseCopy = databaseCopy2;
						break;
					}
				}
			}
			DatabaseCopy databaseCopy3 = databaseCopy ?? new DatabaseCopy();
			databaseCopy3.HostServer = (ADObjectId)this.OwnerServer.Identity;
			if (databaseCopy == null)
			{
				databaseCopy3.ActivationPreference = 1;
			}
			else
			{
				databaseCopy3.ActivationPreference = databaseCopy3.ActivationPreference;
			}
			ADRawEntry adrawEntry = databaseCopy3;
			TDataObject dataObject = this.DataObject;
			adrawEntry.SetId(dataObject.Id.GetChildId(this.OwnerServer.Name));
			databaseCopy3.ParentObjectClass = ((this.DatabaseType == NewDatabaseTask<TDataObject>.ExchangeDatabaseType.Public) ? PublicFolderDatabase.MostDerivedClass : MailboxDatabase.MostDerivedClass);
			TDataObject dataObject2 = this.DataObject;
			ActivationPreferenceSetter<DatabaseCopy> activationPreferenceSetter = new ActivationPreferenceSetter<DatabaseCopy>(dataObject2.AllDatabaseCopies, databaseCopy3, (databaseCopy == null) ? EntryAction.Insert : EntryAction.Modify);
			UpdateResult updateResult = activationPreferenceSetter.UpdateCachedValues();
			if (updateResult == UpdateResult.AllChanged)
			{
				activationPreferenceSetter.SaveAllUpdatedValues(base.DataSession);
			}
			base.DataSession.Save(databaseCopy3);
			this.forcedReplicationSites = DagTaskHelper.DetermineRemoteSites(base.GlobalConfigSession, databaseCopy3.OriginatingServer, this.OwnerServer);
			if (this.forcedReplicationSites != null)
			{
				ITopologyConfigurationSession session = (ITopologyConfigurationSession)base.DataSession;
				TDataObject dataObject3 = this.DataObject;
				string objectIdentity = dataObject3.Identity.ToString();
				if (DagTaskHelper.ForceReplication(session, this.DataObject, this.forcedReplicationSites, objectIdentity, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)))
				{
					DagTaskHelper.ForceReplication(session, databaseCopy3, this.forcedReplicationSites, objectIdentity, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
			}
			TaskLogger.LogExit();
			return databaseCopy3;
		}

		protected Server OwnerServer
		{
			get
			{
				return this.ownerServer;
			}
		}

		protected Database[] OwnerServerPublicFolderDatabases
		{
			get
			{
				return this.ownerServerPublicFolderDatabases;
			}
		}

		protected Database[] OwnerServerDatabases
		{
			get
			{
				return this.ownerServerDatabases;
			}
		}

		internal virtual IRecipientSession RecipientSessionForSystemMailbox
		{
			get
			{
				if (this.recipientSessionForSystemMailbox == null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 647, "RecipientSessionForSystemMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\NewDatabase.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
					this.recipientSessionForSystemMailbox = tenantOrRootOrgRecipientSession;
				}
				return this.recipientSessionForSystemMailbox;
			}
		}

		protected void ValidateFilePaths(bool recovery)
		{
			try
			{
				this.ValidateEdbFile(recovery);
				this.ValidateLogFolderPath();
			}
			catch (WmiException ex)
			{
				Exception exception = new InvalidOperationException(Strings.ErrorFailedToConnectToServer(this.OwnerServer.Name, ex.Message));
				ErrorCategory category = ErrorCategory.InvalidOperation;
				TDataObject dataObject = this.DataObject;
				base.WriteError(exception, category, dataObject.Identity);
			}
			catch (UnauthorizedAccessException ex2)
			{
				Exception exception2 = new InvalidOperationException(Strings.ErrorFailedToConnectToServer(this.OwnerServer.Name, ex2.Message));
				ErrorCategory category2 = ErrorCategory.InvalidOperation;
				TDataObject dataObject2 = this.DataObject;
				base.WriteError(exception2, category2, dataObject2.Identity);
			}
		}

		protected void PrepareFilePaths(string databaseName, bool recovery, Database dataObject)
		{
			if (null == dataObject.LogFolderPath)
			{
				dataObject.LogFolderPath = NewDatabaseTask<TDataObject>.GetDefaultLogFolderPath(databaseName, this.ownerServer.DataPath.PathName, (ADObjectId)this.ownerServer.Identity, this.ownerServerDatabases, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (null == dataObject.SystemFolderPath)
			{
				dataObject.SystemFolderPath = dataObject.LogFolderPath;
			}
			if (null == dataObject.EdbFilePath)
			{
				string fileName = dataObject.Name + ".edb";
				try
				{
					dataObject.EdbFilePath = EdbFilePath.Parse(Path.Combine(LocalLongFullPath.ConvertInvalidCharactersInPathName(dataObject.LogFolderPath.PathName), LocalLongFullPath.ConvertInvalidCharactersInFileName(fileName)));
				}
				catch (ArgumentException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidOperation, dataObject);
				}
			}
		}

		private void ValidateLogFolderPath()
		{
			base.WriteVerbose(Strings.VerboseLogFolderPathUniqueUnderDAGCondition(this.LogFolderPath.PathName));
			if (!new DbLogLocationUniqueUnderDAGCondition(this.LogFolderPath.PathName, (ADObjectId)this.OwnerServer.Identity, new ADObjectId[]
			{
				(ADObjectId)this.OwnerServer.Identity
			}, this.ownerServerDatabases).Verify())
			{
				Exception exception = new ArgumentException(Strings.ErrorLogFolderPathNotUniqueUnderSameDAG(this.LogFolderPath.PathName), "LogFolderPath");
				ErrorCategory category = ErrorCategory.InvalidArgument;
				TDataObject dataObject = this.DataObject;
				base.WriteError(exception, category, dataObject.Identity);
			}
			if (!new PathOnFixedOrNetworkDriveCondition(this.OwnerServer.Fqdn, this.LogFolderPath.PathName).Verify())
			{
				Exception exception2 = new ArgumentException(Strings.ErrorPathIsNotOnFixedDrive("LogFolderPath"));
				ErrorCategory category2 = ErrorCategory.InvalidArgument;
				TDataObject dataObject2 = this.DataObject;
				base.WriteError(exception2, category2, dataObject2.Identity);
			}
			string fqdn = this.OwnerServer.Fqdn;
			string pathName = this.LogFolderPath.PathName;
			TDataObject dataObject3 = this.DataObject;
			if (!new LogLocationAvailableCondition(fqdn, pathName, dataObject3.LogFilePrefix).Verify())
			{
				Exception exception3 = new ArgumentException(Strings.ErrorLogFolderPathNotAvailable, "LogFolderPath");
				ErrorCategory category3 = ErrorCategory.InvalidArgument;
				TDataObject dataObject4 = this.DataObject;
				base.WriteError(exception3, category3, dataObject4.Identity);
			}
		}

		private void ValidateEdbFile(bool recovery)
		{
			base.WriteVerbose(Strings.VerboseEdbFileLocationUniqueUnderDAGCondition(this.EdbFilePath.PathName));
			if (!new EdbFileLocationUniqueUnderDAGCondition(this.EdbFilePath.PathName, (ADObjectId)this.OwnerServer.Identity, new ADObjectId[]
			{
				(ADObjectId)this.OwnerServer.Identity
			}, this.ownerServerDatabases).Verify())
			{
				Exception exception = new ArgumentException(Strings.ErrorEdbFileLocationNotUniqueUnderSameDAG(this.EdbFilePath.PathName), "EdbFilePath");
				ErrorCategory category = ErrorCategory.InvalidArgument;
				TDataObject dataObject = this.DataObject;
				base.WriteError(exception, category, dataObject.Identity);
			}
			base.WriteVerbose(Strings.VerbosePathOnFixedOrNetworkDriveCondition(this.OwnerServer.Fqdn, this.EdbFilePath.PathName));
			if (!new PathOnFixedOrNetworkDriveCondition(this.OwnerServer.Fqdn, this.EdbFilePath.PathName).Verify())
			{
				Exception exception2 = new ArgumentException(Strings.ErrorEdbFileLocationNotOnFixedDrive(this.EdbFilePath.PathName), "EdbFilePath");
				ErrorCategory category2 = ErrorCategory.InvalidArgument;
				TDataObject dataObject2 = this.DataObject;
				base.WriteError(exception2, category2, dataObject2.Identity);
			}
			base.WriteVerbose(Strings.VerboseCheckDirectoryExistenceCondition(this.OwnerServer.Name, this.EdbFilePath.PathName));
			if (!new DirectoryNotExistCondition(this.OwnerServer.Fqdn, this.EdbFilePath.PathName).Verify())
			{
				Exception exception3 = new ArgumentException(Strings.ErrorEdbFilePathOccupiedByDirectory(this.EdbFilePath.PathName, this.OwnerServer.Name), "EdbFilePath");
				ErrorCategory category3 = ErrorCategory.InvalidArgument;
				TDataObject dataObject3 = this.DataObject;
				base.WriteError(exception3, category3, dataObject3.Identity);
			}
			base.WriteVerbose(Strings.VerboseCheckFileExistenceCondition(this.OwnerServer.Name, this.EdbFilePath.PathName));
			if (!new FileNotExistCondition(this.OwnerServer.Fqdn, this.EdbFilePath.PathName).Verify())
			{
				if (!recovery)
				{
					Exception exception4 = new ArgumentException(Strings.ErrorEdbFilePathOccupiedByFile(this.EdbFilePath.PathName, this.OwnerServer.Name), "EdbFilePath");
					ErrorCategory category4 = ErrorCategory.InvalidArgument;
					TDataObject dataObject4 = this.DataObject;
					base.WriteError(exception4, category4, dataObject4.Identity);
					return;
				}
				this.WriteWarning(Strings.RestoreUsingExistingFile(this.Name, this.EdbFilePath.PathName));
			}
		}

		internal static string GetDefaultEdbFolderPath(ExchangeServer ownerServer, string databaseName)
		{
			string text = Path.Combine(ownerServer.DataPath.PathName, LocalLongFullPath.ConvertInvalidCharactersInFileName(databaseName));
			string path = string.Format("{0}{1}{2}", databaseName, "0000", ".edb");
			EdbFilePath edbFilePath = null;
			if (!EdbFilePath.TryParse(Path.Combine(text, path), out edbFilePath))
			{
				text = ownerServer.DataPath.PathName;
				if (!EdbFilePath.TryParse(Path.Combine(text, path), out edbFilePath))
				{
					text = EdbFilePath.DefaultEdbFilePath;
				}
			}
			return text;
		}

		internal static NonRootLocalLongFullPath GetDefaultLogFolderPath(string databaseName, string serverDataPath, ADObjectId OwnerServerId, IEnumerable<Database> existingDatabases, Task.TaskErrorLoggingDelegate logError)
		{
			string value = LocalLongFullPath.ConvertInvalidCharactersInFileName(databaseName);
			StringBuilder stringBuilder = new StringBuilder(serverDataPath);
			stringBuilder.Append(Path.DirectorySeparatorChar.ToString()).Append(value);
			string text = null;
			NonRootLocalLongFullPath nonRootLocalLongFullPath = null;
			Exception ex = null;
			try
			{
				text = stringBuilder.ToString();
				nonRootLocalLongFullPath = NonRootLocalLongFullPath.Parse(text);
				nonRootLocalLongFullPath.ValidateDirectoryPathLength();
				for (int i = 0; i < 200; i++)
				{
					bool flag = false;
					foreach (Database database in existingDatabases)
					{
						if (nonRootLocalLongFullPath == database.LogFolderPath)
						{
							if (database.Servers != null && database.Servers.Length != 0)
							{
								foreach (ADObjectId adobjectId in database.Servers)
								{
									if (OwnerServerId == adobjectId)
									{
										flag = true;
										break;
									}
								}
							}
							else if (OwnerServerId == database.Server)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						break;
					}
					text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
					{
						stringBuilder,
						i + 1
					});
					nonRootLocalLongFullPath = NonRootLocalLongFullPath.Parse(text);
					nonRootLocalLongFullPath.ValidateDirectoryPathLength();
				}
			}
			catch (ArgumentException ex2)
			{
				ex = ex2;
			}
			catch (FormatException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				nonRootLocalLongFullPath = null;
				if (logError != null)
				{
					logError(new InvalidOperationException(Strings.ErrorFailToParseLocalLongFullPath(databaseName, DatabaseSchema.LogFolderPath.Name, text, ex.Message)), ErrorCategory.InvalidOperation, databaseName);
				}
			}
			return nonRootLocalLongFullPath;
		}

		internal static string GetDefaultEdbFileName(string databaseName)
		{
			return LocalLongFullPath.ConvertInvalidCharactersInFileName(databaseName + ".edb");
		}

		protected abstract NewDatabaseTask<TDataObject>.ExchangeDatabaseType DatabaseType { get; }

		internal const string paramServer = "Server";

		internal const string paramName = "Name";

		internal const string paramEdbFilePath = "EdbFilePath";

		internal const string paramLogFolderPath = "LogFolderPath";

		protected ADObjectId[] forcedReplicationSites;

		protected TDataObject preExistingDatabase;

		protected DatabaseCopy dbCopy;

		private Server ownerServer;

		private Database[] ownerServerPublicFolderDatabases;

		private Database[] ownerServerDatabases;

		private IRecipientSession recipientSessionForSystemMailbox;

		protected enum ExchangeDatabaseType
		{
			Private,
			Public
		}
	}
}
