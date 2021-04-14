using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.MapiTasks.Presentation;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.MapiTasks
{
	public abstract class GetStatisticsBase<TIdentity, TDataObject, TPresentationObject> : GetTenantADObjectWithIdentityTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : MapiObject, new() where TPresentationObject : MapiObject, new()
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = false, ParameterSetName = "AuditLog")]
		public override TIdentity Identity
		{
			get
			{
				return (TIdentity)((object)base.Fields["Identity"]);
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Database", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Server", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Database")]
		public ServerIdParameter CopyOnServer
		{
			get
			{
				return (ServerIdParameter)base.Fields["CopyOnServer"];
			}
			set
			{
				base.Fields["CopyOnServer"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Server")]
		public SwitchParameter IncludePassive
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludePassive"] ?? false);
			}
			set
			{
				base.Fields["IncludePassive"] = value;
			}
		}

		protected virtual StoreMailboxIdParameter StoreMailboxId
		{
			get
			{
				return null;
			}
		}

		protected virtual bool NoADLookupForMailboxStatistics
		{
			get
			{
				return false;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is ParsingException || e is InvalidCastException || e is StoragePermanentException || base.IsKnownException(e);
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			this.readOnlyRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 159, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\Mailbox\\GetStatisticsBase.cs");
			if (base.DomainController == null)
			{
				this.readOnlyRecipientSession.UseGlobalCatalog = true;
			}
			this.isRunningMailboxStatisticsTask = (typeof(TDataObject) == typeof(Microsoft.Exchange.Data.Mapi.MailboxStatistics));
			this.isRunningLogonStatisticsTask = (typeof(TDataObject) == typeof(LogonStatistics));
			this.isRunningResourceMonitorDigestTask = (typeof(TDataObject) == typeof(MailboxResourceMonitor));
			if (this.Database != null && this.isRunningMailboxStatisticsTask && this.StoreMailboxId != null)
			{
				Guid mailboxGuid;
				if (Guid.TryParse(this.StoreMailboxId.ToString(), out mailboxGuid))
				{
					this.identity = new MailboxId(null, mailboxGuid);
				}
				else
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidGuidFormat), ErrorCategory.InvalidArgument, this.StoreMailboxId.ToString());
				}
			}
			else
			{
				this.identity = new MailboxId();
			}
			this.databases.Clear();
			this.server = null;
			this.ResolveDatabaseAndServer();
			if (this.mapiSession == null)
			{
				this.mapiSession = new MapiAdministrationSession(this.server.ExchangeLegacyDN, Fqdn.Parse(this.server.Fqdn));
			}
			else
			{
				this.mapiSession.RedirectServer(this.server.ExchangeLegacyDN, Fqdn.Parse(this.server.Fqdn));
			}
			TaskLogger.LogExit();
			return this.mapiSession;
		}

		protected void ResolveDatabaseAndServer()
		{
			DatabaseIdParameter databaseIdParameter = this.Database;
			ServerIdParameter serverIdParameter = this.Server ?? new ServerIdParameter();
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			if (this.Identity != null)
			{
				bool flag = false;
				if (this.isRunningLogonStatisticsTask)
				{
					TIdentity tidentity = this.Identity;
					IEnumerable<Database> objects = tidentity.GetObjects<Database>(null, base.GlobalConfigSession);
					foreach (Database item in objects)
					{
						this.databases.Add(item);
					}
					if (this.databases.Count > 0)
					{
						if (this.databases[0].Server == null)
						{
							string name = typeof(Database).Name;
							TIdentity tidentity2 = this.Identity;
							base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(name, tidentity2.ToString(), DatabaseSchema.Server.Name)), ErrorCategory.InvalidArgument, this.Identity);
						}
						DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(this.databases[0].Guid);
						serverIdParameter = ServerIdParameter.Parse(serverForDatabase.ServerFqdn);
						if (string.IsNullOrEmpty(this.databases[0].ExchangeLegacyDN))
						{
							string name2 = typeof(Database).Name;
							TIdentity tidentity3 = this.Identity;
							base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(name2, tidentity3.ToString(), DatabaseSchema.ExchangeLegacyDN.Name)), ErrorCategory.InvalidArgument, this.Identity);
						}
						this.identity = new MailboxId(this.databases[0].ExchangeLegacyDN);
						flag = true;
					}
				}
				if (!flag)
				{
					IIdentityParameter id = this.Identity;
					IConfigDataProvider session = this.readOnlyRecipientSession;
					ObjectId rootID = null;
					TIdentity tidentity4 = this.Identity;
					LocalizedString? notFoundError = new LocalizedString?(Strings.ErrorMailboxNotFound(tidentity4.ToString()));
					TIdentity tidentity5 = this.Identity;
					ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(id, session, rootID, notFoundError, new LocalizedString?(Strings.ErrorMailboxNotUnique(tidentity5.ToString())));
					Guid guid = Guid.Empty;
					string mailboxExchangeLegacyDn = null;
					ADObjectId adobjectId = null;
					if (string.IsNullOrEmpty(adrecipient.LegacyExchangeDN))
					{
						string name3 = typeof(ADRecipient).Name;
						TIdentity tidentity6 = this.Identity;
						base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(name3, tidentity6.ToString(), ADRecipientSchema.LegacyExchangeDN.Name)), ErrorCategory.InvalidArgument, this.Identity);
					}
					ADUser aduser = adrecipient as ADUser;
					ADSystemMailbox adsystemMailbox = adrecipient as ADSystemMailbox;
					ADSystemAttendantMailbox adsystemAttendantMailbox = adrecipient as ADSystemAttendantMailbox;
					ADPublicDatabase adpublicDatabase = adrecipient as ADPublicDatabase;
					if (aduser != null)
					{
						if (this.isRunningMailboxStatisticsTask && aduser.RecipientTypeDetails == RecipientTypeDetails.AuditLogMailbox && !this.GetAuditLogMailboxStatistics())
						{
							TIdentity tidentity7 = this.Identity;
							base.WriteError(new MdbAdminTaskException(Strings.RecipientNotFoundException(tidentity7.ToString())), ErrorCategory.InvalidArgument, null);
						}
						bool archiveMailboxStatistics = this.GetArchiveMailboxStatistics();
						if (aduser.RecipientType == RecipientType.MailUser && !archiveMailboxStatistics)
						{
							base.WriteError(new MdbAdminTaskException(Strings.RecipientTypeNotValid(aduser.ToString())), (ErrorCategory)1003, this.Identity);
						}
						RecipientIdParameter recipientIdParameter = this.Identity as RecipientIdParameter;
						if (this.isRunningMailboxStatisticsTask && recipientIdParameter != null && recipientIdParameter.RawMailboxGuidInvolvedInSearch != Guid.Empty && aduser.MailboxLocations != null)
						{
							IMailboxLocationInfo mailboxLocation = aduser.MailboxLocations.GetMailboxLocation(recipientIdParameter.RawMailboxGuidInvolvedInSearch);
							if (mailboxLocation != null)
							{
								guid = mailboxLocation.MailboxGuid;
								adobjectId = mailboxLocation.DatabaseLocation;
							}
						}
						if (guid == Guid.Empty || adobjectId == null)
						{
							if (archiveMailboxStatistics)
							{
								if (aduser.ArchiveGuid != Guid.Empty)
								{
									if (aduser.ArchiveDomain != null)
									{
										base.WriteError(new MdbAdminTaskException(Strings.ErrorRemoteArchiveNoStats(aduser.ToString())), (ErrorCategory)1003, this.Identity);
									}
									else
									{
										guid = aduser.ArchiveGuid;
										adobjectId = (aduser.ArchiveDatabase ?? aduser.Database);
									}
								}
								else
								{
									base.WriteError(new MdbAdminTaskException(Strings.ErrorArchiveNotEnabled(aduser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
								}
							}
							else
							{
								guid = aduser.ExchangeGuid;
								adobjectId = aduser.Database;
							}
						}
						mailboxExchangeLegacyDn = aduser.LegacyExchangeDN;
					}
					else if (adsystemMailbox != null)
					{
						guid = adsystemMailbox.ExchangeGuid;
						mailboxExchangeLegacyDn = adsystemMailbox.LegacyExchangeDN;
						adobjectId = adsystemMailbox.Database;
					}
					else if (adsystemAttendantMailbox != null)
					{
						guid = adsystemAttendantMailbox.Guid;
						mailboxExchangeLegacyDn = adsystemAttendantMailbox.LegacyExchangeDN;
						adobjectId = adsystemAttendantMailbox.Database;
					}
					else if (adpublicDatabase != null)
					{
						mailboxExchangeLegacyDn = adpublicDatabase.LegacyExchangeDN;
						adobjectId = (ADObjectId)adpublicDatabase.Identity;
					}
					if (adobjectId == null)
					{
						string name4 = adrecipient.GetType().Name;
						TIdentity tidentity8 = this.Identity;
						base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(name4, tidentity8.ToString(), IADMailStorageSchema.Database.Name)), ErrorCategory.InvalidArgument, adrecipient);
					}
					databaseIdParameter = new DatabaseIdParameter(adobjectId);
					if (this.isRunningLogonStatisticsTask)
					{
						this.identity = new MailboxId(mailboxExchangeLegacyDn);
					}
					else
					{
						this.identity = new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectIdToDatabaseId(adobjectId), guid);
					}
				}
			}
			if (databaseIdParameter != null)
			{
				databaseIdParameter.AllowLegacy = true;
				LocalizedString empty = LocalizedString.Empty;
				LocalizedString empty2 = LocalizedString.Empty;
				Database database;
				if (this.isRunningLogonStatisticsTask)
				{
					database = (Database)base.GetDataObject<Database>(databaseIdParameter, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseIdParameter.ToString())));
				}
				else
				{
					database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseIdParameter, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(databaseIdParameter.ToString())));
				}
				if (database.Server == null)
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(this.isRunningLogonStatisticsTask ? typeof(Database).Name : typeof(MailboxDatabase).Name, databaseIdParameter.ToString(), DatabaseSchema.Server.Name)), ErrorCategory.InvalidArgument, database);
				}
				this.databases = new List<Database>(new Database[]
				{
					database
				});
				if (this.CopyOnServer != null)
				{
					serverIdParameter = this.CopyOnServer;
				}
				else
				{
					DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(database.Guid);
					serverIdParameter = ServerIdParameter.Parse(serverForDatabase.ServerFqdn);
				}
			}
			if (this.Server != null)
			{
				serverIdParameter = this.Server;
				this.server = MapiTaskHelper.GetMailboxServer(this.Server, base.GlobalConfigSession, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			else
			{
				this.server = (Server)base.GetDataObject<Server>(serverIdParameter, base.GlobalConfigSession, null, new LocalizedString?((this.Identity == null && this.Database == null) ? Strings.ErrorLocalMachineIsNotExchangeServer : Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				if (!this.server.IsExchange2007OrLater || !this.server.IsMailboxServer)
				{
					if (this.Identity != null)
					{
						TIdentity tidentity9 = this.Identity;
						base.WriteError(new MdbAdminTaskException(Strings.ErrorMailboxInNonMailboxServer(tidentity9.ToString())), ErrorCategory.InvalidArgument, this.server);
					}
					if (this.Database != null)
					{
						base.WriteError(new MdbAdminTaskException(Strings.ErrorDatabaseInNonMailboxServer(this.Database.ToString())), ErrorCategory.InvalidArgument, this.server);
					}
					base.WriteError(new MdbAdminTaskException(Strings.ErrorLocalServerIsNotMailboxServer), ErrorCategory.InvalidArgument, this.server);
				}
			}
			if (string.IsNullOrEmpty(this.server.ExchangeLegacyDN))
			{
				base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Server).Name, serverIdParameter.ToString(), ServerSchema.ExchangeLegacyDN.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.databases.Count == 0)
			{
				if (this.isRunningLogonStatisticsTask)
				{
					this.FilterActiveDatabases(activeManagerInstance, this.server.GetDatabases());
					return;
				}
				this.FilterActiveDatabases(activeManagerInstance, this.server.GetMailboxDatabases());
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			foreach (Database database in this.databases)
			{
				try
				{
					Guid mailboxGuid = Guid.Empty;
					MailboxTableFlags mailboxTableFlags = MailboxTableFlags.MailboxTableFlagsNone;
					if (null != this.identity)
					{
						mailboxGuid = this.identity.MailboxGuid;
					}
					if (this.isRunningMailboxStatisticsTask)
					{
						mailboxTableFlags |= MailboxTableFlags.IncludeSoftDeletedMailbox;
					}
					QueryFilter filter = new MailboxContextFilter(mailboxGuid, (ulong)((long)mailboxTableFlags), this.NoADLookupForMailboxStatistics);
					IEnumerable<TPresentationObject> enumerable = this.mapiSession.FindPaged<TDataObject, TPresentationObject>(filter, MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(database), QueryScope.SubTree, null, 0, 0);
					foreach (TPresentationObject tpresentationObject in enumerable)
					{
						try
						{
							MailboxId mailboxId = tpresentationObject.Identity as MailboxId;
							if (this.Identity == null || (null != mailboxId && ((Guid.Empty != this.identity.MailboxGuid && this.identity.MailboxGuid == mailboxId.MailboxGuid) || (!string.IsNullOrEmpty(this.identity.MailboxExchangeLegacyDn) && string.Equals(this.identity.MailboxExchangeLegacyDn, mailboxId.MailboxExchangeLegacyDn, StringComparison.OrdinalIgnoreCase)))))
							{
								if (this.isRunningLogonStatisticsTask)
								{
									((LogonStatistics)((object)tpresentationObject)).ServerName = database.ServerName;
									((LogonStatistics)((object)tpresentationObject)).DatabaseName = database.Name;
								}
								else if (this.isRunningResourceMonitorDigestTask)
								{
									((MailboxResourceMonitor)((object)tpresentationObject)).ServerName = this.server.Name;
									((MailboxResourceMonitor)((object)tpresentationObject)).DatabaseName = database.Name;
									((MailboxResourceMonitor)((object)tpresentationObject)).IsDatabaseCopyActive = (database.Server.ObjectGuid == this.server.Guid);
								}
								else if (this.isRunningMailboxStatisticsTask)
								{
									Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics mailboxStatistics = (Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics)((object)tpresentationObject);
									mailboxStatistics.ServerName = this.server.Name;
									mailboxStatistics.DatabaseName = database.Name;
									mailboxStatistics.Database = database.Identity;
									mailboxStatistics.IsDatabaseCopyActive = (database.Server.ObjectGuid == this.server.Guid);
									MailboxDatabase mailboxDatabase = database as MailboxDatabase;
									if (mailboxDatabase != null)
									{
										mailboxStatistics.DatabaseIssueWarningQuota = mailboxDatabase.IssueWarningQuota;
										mailboxStatistics.DatabaseProhibitSendQuota = mailboxDatabase.ProhibitSendQuota;
										mailboxStatistics.DatabaseProhibitSendReceiveQuota = mailboxDatabase.ProhibitSendReceiveQuota;
									}
									if (mailboxId != null && this.GetMoveHistoryOption() != MoveHistoryOption.None)
									{
										UserMailboxFlags userMailboxFlags = UserMailboxFlags.None;
										if (database.Recovery)
										{
											userMailboxFlags |= UserMailboxFlags.RecoveryMDB;
										}
										if (mailboxStatistics.IsMoveDestination ?? false)
										{
											userMailboxFlags |= UserMailboxFlags.MoveDestination;
										}
										else if (mailboxStatistics.DisconnectReason != null)
										{
											if (mailboxStatistics.DisconnectReason.Value == MailboxState.SoftDeleted)
											{
												userMailboxFlags |= UserMailboxFlags.SoftDeleted;
											}
											else
											{
												userMailboxFlags |= UserMailboxFlags.Disconnected;
											}
										}
										try
										{
											mailboxStatistics.MoveHistory = MoveHistoryEntry.LoadMoveHistory(mailboxId.MailboxGuid, database.Id.ObjectGuid, this.GetMoveHistoryOption() == MoveHistoryOption.IncludeMoveHistoryAndReport, userMailboxFlags);
										}
										catch (LocalizedException exception)
										{
											base.WriteError(exception, ErrorCategory.ResourceUnavailable, mailboxId);
										}
									}
								}
								bool flag = true;
								if (this.isRunningMailboxStatisticsTask)
								{
									Exception ex = null;
									try
									{
										QueryFilter internalFilter = this.InternalFilter;
										if (internalFilter != null && !OpathFilterEvaluator.FilterMatches(internalFilter, (Microsoft.Exchange.Data.Mapi.MailboxStatistics)((object)tpresentationObject)))
										{
											flag = false;
										}
									}
									catch (InvalidCastException ex2)
									{
										ex = ex2;
									}
									catch (ParsingException ex3)
									{
										ex = ex3;
									}
									catch (ArgumentOutOfRangeException ex4)
									{
										ex = ex4;
									}
									if (ex != null)
									{
										base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidMailboxStatisticsFilter(this.InternalFilter.ToString())), ErrorCategory.InvalidArgument, this.InternalFilter);
									}
								}
								if (flag)
								{
									this.WriteResult(tpresentationObject);
								}
								if (this.Identity != null && this.isRunningMailboxStatisticsTask)
								{
									TaskLogger.LogExit();
									return;
								}
							}
						}
						finally
						{
							if (tpresentationObject != null)
							{
								tpresentationObject.Dispose();
							}
						}
					}
				}
				catch (DatabaseUnavailableException ex5)
				{
					if (this.Identity == null && this.Database == null)
					{
						base.WriteWarning(ex5.Message);
					}
					else
					{
						base.WriteError(ex5, ErrorCategory.ResourceUnavailable, database);
					}
				}
				catch (MapiObjectNotFoundException exception2)
				{
					if (this.Identity == null || !this.isRunningMailboxStatisticsTask)
					{
						base.WriteError(exception2, ErrorCategory.ObjectNotFound, this.Identity);
					}
				}
			}
			if (this.Identity != null && this.isRunningMailboxStatisticsTask)
			{
				TIdentity tidentity = this.Identity;
				this.WriteWarning(Strings.WarningMailboxNeverBeenLoggedOn(tidentity.ToString(), this.identity.ToString()));
			}
			TaskLogger.LogExit();
		}

		internal virtual MoveHistoryOption GetMoveHistoryOption()
		{
			return MoveHistoryOption.None;
		}

		internal virtual bool GetArchiveMailboxStatistics()
		{
			return false;
		}

		internal virtual bool GetAuditLogMailboxStatistics()
		{
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.mapiSession != null)
			{
				this.mapiSession.Dispose();
				this.mapiSession = null;
			}
			base.Dispose(disposing);
		}

		private void FilterActiveDatabases(ActiveManager activeManager, IEnumerable<Database> databases)
		{
			AmServerName amServerName = new AmServerName(this.server);
			foreach (Database database in databases)
			{
				if (this.IncludePassive)
				{
					this.databases.Add(database);
				}
				else
				{
					DatabaseLocationInfo serverForDatabase = activeManager.GetServerForDatabase(database.Guid);
					AmServerName dst = new AmServerName(serverForDatabase.ServerFqdn);
					if (amServerName.Equals(dst))
					{
						this.databases.Add(database);
					}
				}
			}
		}

		internal const string ParameterSetDatabase = "Database";

		internal const string ParameterSetServer = "Server";

		private const string ParameterDatabase = "Database";

		private MapiAdministrationSession mapiSession;

		private IRecipientSession readOnlyRecipientSession;

		private List<Database> databases = new List<Database>();

		private Server server;

		private MailboxId identity;

		private bool isRunningMailboxStatisticsTask;

		private bool isRunningLogonStatisticsTask;

		private bool isRunningResourceMonitorDigestTask;
	}
}
