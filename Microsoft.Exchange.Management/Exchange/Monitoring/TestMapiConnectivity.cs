using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "MAPIConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "Server")]
	public sealed class TestMapiConnectivity : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Server", ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Database", ValueFromPipeline = true)]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter EnableSoftDeletedRecipientLogon
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnableSoftDeletedRecipientLogon"] ?? false);
			}
			set
			{
				base.Fields["EnableSoftDeletedRecipientLogon"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(1, 2147483647)]
		public int PerConnectionTimeout
		{
			get
			{
				return (int)(base.Fields["PerConnectionTimeout"] ?? 60);
			}
			set
			{
				base.Fields["PerConnectionTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(1, 2147483647)]
		public int AllConnectionsTimeout
		{
			get
			{
				return (int)(base.Fields["AllConnectionsTimeout"] ?? 90);
			}
			set
			{
				base.Fields["AllConnectionsTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(1, 2147483647)]
		public int ActiveDirectoryTimeout
		{
			get
			{
				return (int)(base.Fields["ActiveDirectoryTimeout"] ?? 15);
			}
			set
			{
				base.Fields["ActiveDirectoryTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		private void EventForNonSuccessTransactionResultList(List<MapiTransactionOutcome> transactions, int emptyListEventId, EventTypeEnumeration emptyListEventType, string emptyListEventMsg, int nonEmptyListEventId, EventTypeEnumeration nonEmptyListEventType, string nonEmptyListEventMsgPrefix)
		{
			if (transactions.Count == 0)
			{
				this.monitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MAPIConnectivity", emptyListEventId, emptyListEventType, emptyListEventMsg));
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(nonEmptyListEventMsgPrefix);
			foreach (MapiTransactionOutcome mapiTransactionOutcome in transactions)
			{
				stringBuilder.Append(Strings.MapiTransactionFailedSummary(this.wasTargetMailboxSpecified ? mapiTransactionOutcome.LongTargetString() : Strings.SystemMailboxTarget(mapiTransactionOutcome.ShortTargetString()), mapiTransactionOutcome.Error));
			}
			this.monitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MAPIConnectivity", nonEmptyListEventId, nonEmptyListEventType, stringBuilder.ToString()));
		}

		private void WriteErrorAndMonitoringEvent(Exception exception, ErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, exception.Message));
			base.WriteError(exception, errorCategory, target);
		}

		private DatabaseLocationInfo GetServerForDatabase(ActiveManager activeManager, Guid dbGuid)
		{
			DatabaseLocationInfo result = null;
			try
			{
				result = activeManager.GetServerForDatabase(dbGuid);
			}
			catch (DatabaseNotFoundException)
			{
				base.WriteWarning(string.Format("Caught DatabaseNotFoundException when trying to get the server for database {0} from activemanager; will use the server info from AD.", dbGuid));
			}
			catch (ObjectNotFoundException)
			{
				base.WriteWarning(string.Format("Caught ObjectNotFoundException when trying to get the server for database {0} from activemanager; will use the server info from AD.", dbGuid));
			}
			catch (StoragePermanentException)
			{
				base.WriteWarning(string.Format("Caught StoragePermanentException when trying to get the server for database {0} from activemanager; will use the server info from AD.", dbGuid));
			}
			catch (StorageTransientException)
			{
				base.WriteWarning(string.Format("Caught StorageTransientException when trying to get the server for database {0} from activemanager; will use the server info from AD.", dbGuid));
			}
			return result;
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Database" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageTestMAPIConnectivityDatabase(this.Database.ToString());
				}
				if ("Server" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageTestMAPIConnectivityServer(this.Server.ToString());
				}
				return Strings.ConfirmationMessageTestMAPIConnectivityIdentity(this.Identity.ToString());
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalValidate()
		{
			bool flag = false;
			try
			{
				ADRecipient adrecipient = null;
				List<MailboxDatabase> list = new List<MailboxDatabase>();
				bool flag2 = false;
				ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
				base.TenantGlobalCatalogSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds((double)this.ActiveDirectoryTimeout));
				this.ConfigurationSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds((double)this.ActiveDirectoryTimeout));
				this.transactionTargets = new List<MapiTransaction>();
				if (this.Identity != null)
				{
					if (this.EnableSoftDeletedRecipientLogon)
					{
						IDirectorySession directorySession = base.DataSession as IDirectorySession;
						directorySession.SessionSettings.IncludeSoftDeletedObjects = true;
					}
					ADUser aduser = (ADUser)RecipientTaskHelper.ResolveDataObject<ADUser>(base.DataSession, base.TenantGlobalCatalogSession, base.ServerSettings, this.Identity, null, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
					if (this.Archive)
					{
						if (aduser.ArchiveDatabase == null)
						{
							base.WriteError(new MdbAdminTaskException(Strings.ErrorArchiveNotEnabled(aduser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
						}
						else
						{
							this.Database = new DatabaseIdParameter(aduser.ArchiveDatabase);
						}
					}
					else
					{
						this.Database = new DatabaseIdParameter(aduser.Database);
					}
					flag = true;
					adrecipient = aduser;
				}
				if (this.Database != null)
				{
					MailboxDatabase mailboxDatabase = null;
					MailboxDatabase mailboxDatabase2 = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())));
					if (mailboxDatabase2.Recovery)
					{
						string name = mailboxDatabase2.Name;
						RecoveryMailboxDatabaseNotMonitoredException exception = new RecoveryMailboxDatabaseNotMonitoredException(name);
						this.WriteErrorAndMonitoringEvent(exception, ErrorCategory.InvalidOperation, null, 1006, "MSExchange Monitoring MAPIConnectivity");
						return;
					}
					mailboxDatabase = mailboxDatabase2;
					if (!flag)
					{
						try
						{
							MapiTaskHelper.VerifyDatabaseAndItsOwningServerInScope(base.SessionSettings, mailboxDatabase, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
							flag = true;
						}
						catch (InvalidOperationException exception2)
						{
							this.WriteErrorAndMonitoringEvent(exception2, ErrorCategory.InvalidOperation, null, 1005, "MSExchange Monitoring MAPIConnectivity");
							return;
						}
					}
					list.Add(mailboxDatabase);
					if (this.CopyOnServer != null)
					{
						this.Server = this.CopyOnServer;
					}
					else
					{
						DatabaseLocationInfo serverForDatabase = this.GetServerForDatabase(activeManagerInstance, mailboxDatabase.Guid);
						if (serverForDatabase != null)
						{
							this.Server = ServerIdParameter.Parse(serverForDatabase.ServerFqdn);
						}
						else
						{
							this.Server = ServerIdParameter.Parse(mailboxDatabase.Server.DistinguishedName);
						}
					}
				}
				if (this.Server == null)
				{
					string machineName = Environment.MachineName;
					this.Server = ServerIdParameter.Parse(machineName);
				}
				this.targetServer = (Server)base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				LocalizedException ex = null;
				if (!this.targetServer.IsExchange2007OrLater)
				{
					ex = new OperationOnOldServerException(this.targetServer.Name);
				}
				else if (!this.targetServer.IsMailboxServer)
				{
					ex = new OperationOnlyOnMailboxServerException(this.targetServer.Name);
				}
				if (ex != null)
				{
					this.WriteErrorAndMonitoringEvent(ex, ErrorCategory.InvalidArgument, null, 1005, "MSExchange Monitoring MAPIConnectivity");
				}
				else
				{
					if (!flag)
					{
						try
						{
							MapiTaskHelper.VerifyIsWithinConfigWriteScope(base.SessionSettings, this.targetServer, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
						}
						catch (InvalidOperationException exception3)
						{
							this.WriteErrorAndMonitoringEvent(exception3, ErrorCategory.InvalidOperation, null, 1005, "MSExchange Monitoring MAPIConnectivity");
							return;
						}
					}
					if (list.Count == 0)
					{
						MailboxDatabase[] mailboxDatabases = this.targetServer.GetMailboxDatabases();
						if (mailboxDatabases.Length > 0)
						{
							flag2 = true;
						}
						foreach (MailboxDatabase mailboxDatabase3 in mailboxDatabases)
						{
							if (!mailboxDatabase3.AutoDagExcludeFromMonitoring)
							{
								if (this.IncludePassive)
								{
									list.Add(mailboxDatabase3);
								}
								else
								{
									DatabaseLocationInfo serverForDatabase = this.GetServerForDatabase(activeManagerInstance, mailboxDatabase3.Guid);
									if ((serverForDatabase != null && serverForDatabase.ServerGuid == this.targetServer.Guid) || (serverForDatabase == null && mailboxDatabase3.Server.ObjectGuid == this.targetServer.Guid))
									{
										list.Add(mailboxDatabase3);
									}
								}
							}
						}
					}
					if (adrecipient != null)
					{
						this.wasTargetMailboxSpecified = true;
						this.transactionTargets.Add(new MapiTransaction(this.targetServer, list[0], adrecipient, this.Archive, list[0].Server.ObjectGuid == this.targetServer.Guid));
					}
					else
					{
						foreach (MailboxDatabase mailboxDatabase4 in list)
						{
							if (!mailboxDatabase4.Recovery)
							{
								GeneralMailboxIdParameter id = GeneralMailboxIdParameter.Parse(string.Format(CultureInfo.InvariantCulture, "SystemMailbox{{{0}}}", new object[]
								{
									mailboxDatabase4.Guid.ToString()
								}));
								IEnumerable<ADSystemMailbox> dataObjects = base.GetDataObjects<ADSystemMailbox>(id, base.RootOrgGlobalCatalogSession, null);
								using (IEnumerator<ADSystemMailbox> enumerator2 = dataObjects.GetEnumerator())
								{
									adrecipient = (enumerator2.MoveNext() ? enumerator2.Current : null);
									this.transactionTargets.Add(new MapiTransaction(this.targetServer, mailboxDatabase4, adrecipient, false, mailboxDatabase4.Server.ObjectGuid == this.targetServer.Guid));
								}
							}
						}
					}
					this.transactionTargets.Sort();
					if (this.transactionTargets.Count < 1)
					{
						if (flag2)
						{
							this.WriteWarning(Strings.MapiTransactionServerWithoutMdbs(this.targetServer.Name));
							this.onlyPassives = true;
						}
						else
						{
							this.WriteErrorAndMonitoringEvent(new NoMdbForOperationException(this.targetServer.Name), ErrorCategory.ReadError, null, 1010, "MSExchange Monitoring MAPIConnectivity");
						}
					}
				}
			}
			finally
			{
				if (base.HasErrors && this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				List<MapiTransactionOutcome> list = new List<MapiTransactionOutcome>();
				List<MapiTransactionOutcome> list2 = new List<MapiTransactionOutcome>();
				List<MapiTransactionOutcome> list3 = new List<MapiTransactionOutcome>();
				List<MapiTransactionOutcome> list4 = new List<MapiTransactionOutcome>();
				int num = this.PerConnectionTimeout * 1000;
				if (this.transactionTargets.Count > 0 && TestMapiConnectivity.needExtraTransaction)
				{
					this.transactionTargets[0].TimedExecute((num < 10000) ? 10000 : num);
					TestMapiConnectivity.needExtraTransaction = false;
				}
				Stopwatch stopwatch = Stopwatch.StartNew();
				foreach (MapiTransaction mapiTransaction in this.transactionTargets)
				{
					MapiTransactionOutcome mapiTransactionOutcome;
					if (stopwatch.Elapsed.TotalSeconds < (double)this.AllConnectionsTimeout)
					{
						mapiTransactionOutcome = mapiTransaction.TimedExecute(num);
					}
					else
					{
						mapiTransactionOutcome = new MapiTransactionOutcome(mapiTransaction.TargetServer, mapiTransaction.Database, mapiTransaction.ADRecipient);
						mapiTransactionOutcome.Update(MapiTransactionResultEnum.Failure, TimeSpan.Zero, Strings.MapiTranscationErrorMsgNoTimeLeft(this.AllConnectionsTimeout), null, null, mapiTransaction.Database.Server.ObjectGuid == mapiTransaction.TargetServer.Guid);
					}
					base.WriteObject(mapiTransactionOutcome);
					string performanceInstance = this.wasTargetMailboxSpecified ? mapiTransactionOutcome.LongTargetString() : mapiTransactionOutcome.ShortTargetString();
					double performanceValue;
					if (mapiTransactionOutcome.Result.Value == MapiTransactionResultEnum.Success)
					{
						performanceValue = mapiTransactionOutcome.Latency.TotalMilliseconds;
					}
					else if (mapiTransactionOutcome.Result.Value == MapiTransactionResultEnum.MdbMoved)
					{
						list3.Add(mapiTransactionOutcome);
						performanceValue = 0.0;
					}
					else if (mapiTransactionOutcome.Result.Value == MapiTransactionResultEnum.StoreNotRunning)
					{
						list2.Add(mapiTransactionOutcome);
						performanceValue = -1.0;
					}
					else
					{
						if (this.onlyPassives)
						{
							list4.Add(mapiTransactionOutcome);
						}
						else
						{
							list.Add(mapiTransactionOutcome);
						}
						performanceValue = -1.0;
					}
					this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter("MSExchange Monitoring MAPIConnectivity", "Logon Latency", performanceInstance, performanceValue));
				}
				this.EventForNonSuccessTransactionResultList(list4, 1011, EventTypeEnumeration.Information, Strings.AllMapiTransactionsSucceeded, 1009, EventTypeEnumeration.Error, Strings.MapiTransactionFailedAgainstServerPrefix);
				this.EventForNonSuccessTransactionResultList(list2, 1008, EventTypeEnumeration.Information, Strings.AllMapiTransactionsSucceeded, 1004, EventTypeEnumeration.Error, Strings.SomeMapiTransactionsFailedPrefix);
				this.EventForNonSuccessTransactionResultList(list, 1000, EventTypeEnumeration.Information, Strings.AllMapiTransactionsSucceeded, 1001, EventTypeEnumeration.Error, Strings.SomeMapiTransactionsFailedPrefix);
				this.EventForNonSuccessTransactionResultList(list3, 1002, EventTypeEnumeration.Information, Strings.NoMdbWasMovedWhileRunning, 1003, EventTypeEnumeration.Warning, Strings.SomeMdbsWereMovedWhileRunningPrefix);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.monitoringData = new MonitoringData();
		}

		private const string CmdletNoun = "MAPIConnectivity";

		private const string CmdletMonitoringEventSource = "MSExchange Monitoring MAPIConnectivity";

		private const int DefaultPerConnectionTimeoutInSeconds = 60;

		private const int DefaultAllConnectionsTimeoutInSeconds = 90;

		private const int DefaultADOperationsTimeoutInSeconds = 15;

		private const string MonitoringPerformanceObject = "MSExchange Monitoring MAPIConnectivity";

		private const string MonitoringLatencyPerfCounter = "Logon Latency";

		private const double LatencyPerformanceInCaseOfError = -1.0;

		private const double LatencyPerformanceInCaseOfMdbMoved = 0.0;

		private const int MinimalTimeoutMSecForExtraTransaction = 10000;

		private MonitoringData monitoringData;

		private List<MapiTransaction> transactionTargets;

		private Server targetServer;

		private bool wasTargetMailboxSpecified;

		private bool onlyPassives;

		private static bool needExtraTransaction = true;

		private static class EventId
		{
			internal const int AllMapiTransactionsSucceeded = 1000;

			internal const int SomeMapiTransactionsFailed = 1001;

			internal const int NoMbdWasMovedWhileRunning = 1002;

			internal const int SomeMdbsWereMovedWhileRunning = 1003;

			internal const int MapiTransactionsFailedStoreNotRunning = 1004;

			public const int OperationOnInvalidServer = 1005;

			internal const int RecoveryMailboxDatabaseNotMonitored = 1006;

			internal const int NodeDoesNotOwnExchangeVirtualServer = 1007;

			internal const int MapiTransactionsSucceededStoreRunning = 1008;

			internal const int MapiTransactionsFailedAgainstServer = 1009;

			internal const int DomainControllerNotAccessible = 1010;

			internal const int MapiTransactionsSucceededAgainstServer = 1011;
		}
	}
}
