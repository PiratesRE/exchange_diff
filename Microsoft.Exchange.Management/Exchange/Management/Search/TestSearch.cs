using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Search
{
	[Cmdlet("Test", "ExchangeSearch", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestSearch : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestExchangeSearch;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Alias(new string[]
		{
			"mailbox"
		})]
		[ValidateNotNullOrEmpty]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "server", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		[Parameter(Mandatory = false)]
		public int IndexingTimeoutInSeconds
		{
			get
			{
				return (int)(base.Fields["IndexingTimeOutInSeconds"] ?? 120);
			}
			set
			{
				base.Fields["IndexingTimeOutInSeconds"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "database", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public DatabaseIdParameter MailboxDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["MailboxDatabase"];
			}
			set
			{
				base.Fields["MailboxDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (SwitchParameter)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
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

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalValidate()
		{
			this.ResetForParametersFromPipeline();
			SearchTestResult searchTestResult = SearchTestResult.DefaultSearchTestResult;
			Exception ex = null;
			try
			{
				if (this.Identity != null)
				{
					base.InternalValidate();
					ADUser dataObject = this.DataObject;
					searchTestResult = this.GetTestResultFromMailbox(dataObject);
					this.CheckDatabaseRecoveryAndIndexEnabled(this.GetMailboxDatabase(DatabaseIdParameter.Parse(dataObject.Database.Name)), searchTestResult);
					this.WriteProgress(Strings.TestSearchCurrentMailbox(dataObject.DisplayName));
					this.monitor.AddMonitoringEvent(searchTestResult, Strings.TestSearchCurrentMailbox(dataObject.DisplayName));
					this.searchTestResults.Add(searchTestResult);
				}
				else
				{
					List<MailboxDatabase> list = new List<MailboxDatabase>(1);
					if (this.MailboxDatabase != null)
					{
						MailboxDatabase mailboxDatabase = this.GetMailboxDatabase(this.MailboxDatabase);
						list.Add(mailboxDatabase);
					}
					else
					{
						if (this.Server == null)
						{
							this.Server = ServerIdParameter.Parse(Environment.MachineName);
						}
						Server server = this.GetServer(this.Server);
						foreach (MailboxDatabase item in server.GetMailboxDatabases())
						{
							list.Add(item);
						}
						if (list.Count == 0)
						{
							throw new RecipientTaskException(Strings.TestSearchServerNoMdbs(server.Name));
						}
					}
					foreach (MailboxDatabase mailboxDatabase2 in list)
					{
						searchTestResult = this.GetTestResultFromMailboxDatabase(mailboxDatabase2);
						this.CheckDatabaseRecoveryAndIndexEnabled(mailboxDatabase2, searchTestResult);
						this.WriteProgress(Strings.TestSearchCurrentMDB(mailboxDatabase2.Name));
						this.monitor.AddMonitoringEvent(searchTestResult, Strings.TestSearchCurrentMDB(mailboxDatabase2.Name));
						this.searchTestResults.Add(searchTestResult);
					}
				}
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				ex = ex3;
			}
			catch (DatabaseNotFoundException ex4)
			{
				ex = ex4;
			}
			catch (ServerNotFoundException ex5)
			{
				ex = ex5;
			}
			catch (ObjectNotFoundException ex6)
			{
				ex = ex6;
			}
			catch (StorageTransientException ex7)
			{
				ex = ex7;
			}
			catch (StoragePermanentException ex8)
			{
				ex = ex8;
			}
			catch (RecipientTaskException ex9)
			{
				ex = ex9;
			}
			finally
			{
				if (ex is StorageTransientException || ex is StoragePermanentException)
				{
					searchTestResult.SetErrorTestResult(EventId.ActiveManagerError, new LocalizedString(ex.Message));
					this.WriteTestResult(searchTestResult);
					base.WriteError(ex, ErrorCategory.ResourceUnavailable, null);
				}
				else if (ex != null)
				{
					searchTestResult.SetErrorTestResult(EventId.ADError, new LocalizedString(ex.Message));
					this.WriteTestResult(searchTestResult);
					base.WriteError(ex, ErrorCategory.ResourceUnavailable, null);
				}
			}
		}

		private Server GetServer(ServerIdParameter server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			IEnumerable<Server> objects = server.GetObjects<Server>(null, base.GlobalConfigSession);
			using (IEnumerator<Server> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			throw new ServerNotFoundException("server not found", server.ToString());
		}

		private MailboxDatabase GetMailboxDatabase(DatabaseIdParameter databaseId)
		{
			if (databaseId == null)
			{
				throw new ArgumentNullException("databaseId");
			}
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseId, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseId.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseId.ToString())));
			if (mailboxDatabase == null)
			{
				throw new DatabaseNotFoundException(databaseId.ToString());
			}
			return mailboxDatabase;
		}

		protected override void InternalProcessRecord()
		{
			this.testThread = new Thread(new ThreadStart(this.CreateAndSearchMessages));
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = new TimeSpan(0, 0, 0);
			this.testThread.Start();
			while ((int)timeSpan.TotalSeconds < this.IndexingTimeoutInSeconds)
			{
				if (this.testThread.Join(TestSearch.ThreadCheckInterval))
				{
					this.isTestThreadFinished = true;
					break;
				}
				timeSpan = DateTime.UtcNow - utcNow;
				this.WriteMainProgress((int)(timeSpan.TotalSeconds * 100.0) / this.IndexingTimeoutInSeconds);
			}
			if (!this.isTestThreadFinished)
			{
				TestSearch.TestSearchTracer.TraceDebug((long)this.GetHashCode(), "Test exit due to IndexTimeout is reached");
				this.threadExit.SetStop();
				this.JoinTestThread();
			}
			Exception ex = null;
			lock (this.resultsLock)
			{
				foreach (SearchTestResult searchTestResult in this.searchTestResults)
				{
					try
					{
						if (searchTestResult.SearchTimeInSeconds == 0.0)
						{
							this.ProcessFailureResult(searchTestResult);
						}
					}
					catch (MapiExceptionNotFound mapiExceptionNotFound)
					{
						ex = mapiExceptionNotFound;
					}
					catch (MapiRetryableException ex2)
					{
						ex = ex2;
					}
					catch (MapiPermanentException ex3)
					{
						ex = ex3;
					}
					finally
					{
						if (ex != null)
						{
							this.HandleExceptionInTestThread(searchTestResult.DatabaseGuid, EventId.MapiError, searchTestResult, ex);
						}
						this.WriteTestResult(searchTestResult);
					}
				}
			}
		}

		private void JoinTestThread()
		{
			try
			{
				lock (this.threadLock)
				{
					if (this.testThread != null)
					{
						this.testThread.Join(TestSearch.ThreadCheckInterval);
					}
					this.testThread = null;
				}
			}
			catch (ThreadStateException)
			{
			}
		}

		private SearchTestResult GetTestResultFromMailboxDatabase(MailboxDatabase mdb)
		{
			if (mdb == null)
			{
				throw new ArgumentNullException("mdb");
			}
			SearchTestResult defaultSearchTestResult = SearchTestResult.DefaultSearchTestResult;
			defaultSearchTestResult.Database = mdb.ToString();
			defaultSearchTestResult.DatabaseGuid = mdb.Guid;
			Server activeServer = this.GetActiveServer(mdb.Guid);
			defaultSearchTestResult.Server = activeServer.Name;
			defaultSearchTestResult.ServerGuid = activeServer.Guid;
			ADUser monitoringMailbox = this.GetMonitoringMailbox(mdb, activeServer);
			if (monitoringMailbox == null)
			{
				throw new ManagementObjectNotFoundException(Strings.TestSearchMdbMonitorMbxIsNull(mdb.ToString()));
			}
			defaultSearchTestResult.UserLegacyExchangeDN = monitoringMailbox.LegacyExchangeDN;
			defaultSearchTestResult.Mailbox = monitoringMailbox.DisplayName;
			defaultSearchTestResult.MailboxGuid = monitoringMailbox.ExchangeGuid;
			return defaultSearchTestResult;
		}

		private IRecipientSession GetRecipientSessionForMonitoringMailbox()
		{
			if (Datacenter.GetExchangeSku() == Datacenter.ExchangeSku.ExchangeDatacenter)
			{
				if (this.monitoringTenantRecipientSession == null)
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromTenantCUName(MailboxTaskHelper.GetMonitoringTenantName("E15"));
					this.monitoringTenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 540, "GetRecipientSessionForMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ContentIndex\\TestSearch.cs");
				}
				return this.monitoringTenantRecipientSession;
			}
			return base.RootOrgGlobalCatalogSession;
		}

		private SearchTestResult GetTestResultFromMailbox(ADUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			SearchTestResult defaultSearchTestResult = SearchTestResult.DefaultSearchTestResult;
			if (this.Archive)
			{
				if (user.ArchiveGuid == Guid.Empty)
				{
					throw new RecipientTaskException(Strings.TestSearchMailboxNotArchived(user.Name));
				}
				defaultSearchTestResult.Database = user.Database.ToString();
				defaultSearchTestResult.MailboxGuid = user.ArchiveGuid;
				defaultSearchTestResult.Mailbox = user.ArchiveName[0];
				defaultSearchTestResult.DatabaseGuid = user.Database.ObjectGuid;
				defaultSearchTestResult.UserLegacyExchangeDN = user.LegacyExchangeDN;
			}
			else
			{
				defaultSearchTestResult.Database = user.Database.ToString();
				defaultSearchTestResult.MailboxGuid = user.ExchangeGuid;
				defaultSearchTestResult.Mailbox = user.DisplayName;
				defaultSearchTestResult.DatabaseGuid = user.Database.ObjectGuid;
				defaultSearchTestResult.UserLegacyExchangeDN = user.LegacyExchangeDN;
			}
			Server activeServer = this.GetActiveServer(defaultSearchTestResult.DatabaseGuid);
			defaultSearchTestResult.Server = activeServer.Name;
			defaultSearchTestResult.ServerGuid = activeServer.Guid;
			return defaultSearchTestResult;
		}

		private Server GetActiveServer(Guid dbGuid)
		{
			if (dbGuid == Guid.Empty)
			{
				throw new ArgumentNullException("dbGuid");
			}
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(dbGuid);
			Server server = base.GlobalConfigSession.FindServerByFqdn(serverForDatabase.ServerFqdn);
			if (server == null)
			{
				throw new RecipientTaskException(new LocalizedString(new ServerNotFoundException("couldn't find server", serverForDatabase.ServerFqdn).Message));
			}
			return server;
		}

		private ADUser GetMonitoringMailbox(MailboxDatabase mdb, Server activeServer)
		{
			string monitoringMailboxDisplayName = this.GetMonitoringMailboxDisplayName(mdb, activeServer);
			if (string.IsNullOrEmpty(monitoringMailboxDisplayName))
			{
				return null;
			}
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.DisplayName, monitoringMailboxDisplayName);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MonitoringMailbox);
			IRecipientSession recipientSessionForMonitoringMailbox = this.GetRecipientSessionForMonitoringMailbox();
			ADUser[] array = recipientSessionForMonitoringMailbox.FindADUser(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			}), null, 1000);
			if (array != null)
			{
				return array.FirstOrDefault<ADUser>();
			}
			return null;
		}

		private string GetMonitoringMailboxDisplayName(MailboxDatabase mdb, Server activeServer)
		{
			foreach (DatabaseCopy databaseCopy in mdb.DatabaseCopies)
			{
				if (activeServer.Id.Equals(databaseCopy.HostServer))
				{
					return string.Format(CultureInfo.InvariantCulture, "HealthMailbox-{0}-{1}", new object[]
					{
						databaseCopy.HostServerName,
						databaseCopy.DatabaseName.Replace(" ", "-")
					});
				}
			}
			return null;
		}

		private void CheckDatabaseRecoveryAndIndexEnabled(MailboxDatabase database, SearchTestResult result)
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			string mdb = database.ServerName + "\\\\" + database.Name;
			if (database.Recovery)
			{
				base.WriteVerbose(Strings.TestSearchRecoveryMdb(mdb));
				result.SetErrorTestResult(EventId.RecoveryMailboxDatabaseNotTested, Strings.TestSearchRecoveryMdb(mdb));
				return;
			}
			if (!database.IndexEnabled)
			{
				base.WriteVerbose(Strings.TestSearchCIIsDisabled(mdb));
				result.SetErrorTestResult(EventId.CIIsDisabled, Strings.TestSearchCIIsDisabled(mdb));
			}
		}

		private void HandleExceptionInTestThread(Guid mdbGuid, EventId id, SearchTestResult result, Exception e)
		{
			if (this.CheckForServices(result))
			{
				return;
			}
			TestSearch.TestSearchTracer.TraceDebug<Exception>((long)this.GetHashCode(), "got exception {0}", e);
			string error = SearchCommon.DiagnoseException(result.Server, mdbGuid, e);
			if (id == EventId.MapiError)
			{
				result.SetErrorTestResult(id, Strings.TestSearchMapiError(result.Database, error));
				return;
			}
			result.SetErrorTestResult(id, Strings.TestSearchActiveManager(result.Database, error));
		}

		internal void CheckForCatalogState(SearchTestResult result)
		{
			Guid databaseGuid = result.DatabaseGuid;
			Guid serverGuid = result.ServerGuid;
			Guid mailboxGuid = result.MailboxGuid;
			AmServerName amServer = new AmServerName(result.Server);
			this.monitor.AddMonitoringEvent(result, Strings.TestSearchGetMDBCatalogState(result.Database));
			base.WriteVerbose(Strings.TestSearchGetMDBCatalogState(result.Database));
			Exception ex;
			CopyStatusClientCachedEntry[] copyStatus = CopyStatusHelper.GetCopyStatus(amServer, RpcGetDatabaseCopyStatusFlags2.None, new Guid[]
			{
				databaseGuid
			}, 5000, null, out ex);
			if (ex != null || copyStatus == null || copyStatus.Length != 1)
			{
				this.monitor.AddMonitoringEvent(result, Strings.TestSearchGetCatalogStatusError(result.Database));
				return;
			}
			RpcDatabaseCopyStatus2 copyStatus2 = copyStatus[0].CopyStatus;
			base.WriteVerbose(Strings.TestSearchCatalogState(copyStatus2.ContentIndexStatus.ToString()));
			this.monitor.AddMonitoringEvent(result, Strings.TestSearchCatalogState(copyStatus2.ContentIndexStatus.ToString()));
			if (copyStatus2.ContentIndexStatus != ContentIndexStatusType.Healthy && copyStatus2.ContentIndexStatus != ContentIndexStatusType.HealthyAndUpgrading)
			{
				result.SetErrorTestResult(EventId.CatalogInUnhealthyState, Strings.TestSearchCatalogState(copyStatus2.ContentIndexStatus.ToString()));
				if (!string.IsNullOrWhiteSpace(copyStatus2.ContentIndexErrorMessage))
				{
					this.monitor.AddMonitoringEvent(result, Strings.TestSearchCatalogErrorMessage(copyStatus2.ContentIndexErrorMessage));
					return;
				}
			}
			else if (copyStatus2.ContentIndexBacklog != null)
			{
				int value = copyStatus2.ContentIndexBacklog.Value;
				string backlog = (value / 60).ToString();
				this.monitor.AddMonitoringEvent(result, Strings.TestSearchCatalogBacklog(backlog, copyStatus2.ContentIndexRetryQueueSize.ToString()));
				if (value > this.IndexingTimeoutInSeconds)
				{
					result.SetErrorTestResult(EventId.CatalogBacklog, Strings.TestSearchCatalogBacklog(backlog, copyStatus2.ContentIndexRetryQueueSize.ToString()));
				}
			}
		}

		private bool CheckForServices(SearchTestResult result)
		{
			bool flag = false;
			if (!SearchCommon.IsServiceRunning("MSExchangeIS", result.Server, out flag))
			{
				if (!flag)
				{
					result.SetErrorTestResult(EventId.ServiceNotRunning, Strings.TestSearchServiceNotRunning("MSExchangeIS"));
					return true;
				}
			}
			else if (!SearchCommon.IsServiceRunning("MSExchangeFASTSearch", result.Server, out flag))
			{
				if (!flag)
				{
					result.SetErrorTestResult(EventId.ServiceNotRunning, Strings.TestSearchServiceNotRunning("MSExchangeFASTSearch"));
					return true;
				}
			}
			else if (!SearchCommon.IsServiceRunning("HostControllerService", result.Server, out flag) && !flag)
			{
				result.SetErrorTestResult(EventId.ServiceNotRunning, Strings.TestSearchServiceNotRunning("HostControllerService"));
				return true;
			}
			return false;
		}

		private void WriteTestResult(SearchTestResult result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			base.WriteObject(result);
			if (this.MonitoringContext)
			{
				this.monitoringData.Events.Clear();
				foreach (MonitoringEvent monitoringEvent in result.DetailEvents)
				{
					if (monitoringEvent.EventIdentifier != 1002)
					{
						this.monitoringData.Events.Add(monitoringEvent);
					}
				}
				base.WriteObject(this.monitoringData, true);
			}
		}

		private void WriteProgress(LocalizedString strMsg)
		{
			base.WriteProgress(Strings.TestSearchStatus, strMsg, 0);
		}

		private void WriteMainProgress(int progress)
		{
			lock (this.monitor)
			{
				if (this.monitor.HasMessage())
				{
					LocalizedString statusDescription = this.monitor.PeekMessage();
					base.WriteProgress(Strings.TestSearchTask, statusDescription, Math.Min(progress, 100));
				}
			}
		}

		private void WriteVeboseFromMessageSatck()
		{
			lock (this.monitor)
			{
				while (this.monitor.HasMessage())
				{
					LocalizedString text = this.monitor.PopMessage();
					base.WriteVerbose(text);
				}
			}
		}

		private void CreateAndSearchMessages()
		{
			lock (this.resultsLock)
			{
				foreach (SearchTestResult result in this.searchTestResults)
				{
					using (MessageSearcher messageSearcher = this.CreateMessageSearcher(result))
					{
						if (messageSearcher != null)
						{
							this.SearchForMessage(messageSearcher, result);
						}
					}
				}
			}
		}

		private MessageSearcher CreateMessageSearcher(SearchTestResult result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			Exception ex = null;
			MessageSearcher result2 = null;
			try
			{
				this.monitor.AddMonitoringEvent(result, Strings.TestSearchGetMapiStore(result.Mailbox));
				MapiStore mapiStore = MapiStore.OpenMailbox(result.Server, result.UserLegacyExchangeDN, result.MailboxGuid, result.DatabaseGuid, null, null, null, ConnectFlag.UseAdminPrivilege | ConnectFlag.UseSeparateConnection, OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership | OpenStoreFlag.MailboxGuid, CultureInfo.InvariantCulture, null, "Client=Management;Action=Test-Search", null);
				if (mapiStore != null)
				{
					result2 = new MessageSearcher(mapiStore, result, this.monitor, this.threadExit);
				}
				else
				{
					result.SetErrorTestResult(EventId.MapiStoreError, Strings.TestSearchMapiStoreError(result.Mailbox, result.Database));
				}
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				ex = ex3;
			}
			catch (MapiExceptionNotFound mapiExceptionNotFound)
			{
				ex = mapiExceptionNotFound;
			}
			catch (MapiPermanentException ex4)
			{
				ex = ex4;
			}
			catch (MapiRetryableException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (ex is ADTransientException || ex is ADExternalException)
				{
					result.SetErrorTestResult(EventId.ADError, Strings.TestSearchADError(ex.Message));
				}
				if (ex is MapiExceptionNotFound || ex is MapiPermanentException || ex is MapiRetryableException)
				{
					this.HandleExceptionInTestThread(result.DatabaseGuid, EventId.MapiError, result, ex);
				}
			}
			return result2;
		}

		private void SearchForMessage(MessageSearcher messageSearcher, SearchTestResult result)
		{
			if (messageSearcher == null)
			{
				throw new ArgumentNullException("messageSearcher");
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			TimeSpan timeout = new TimeSpan(0, 0, 1);
			DateTime utcNow = DateTime.UtcNow;
			Exception ex = null;
			try
			{
				this.threadExit.CheckStop();
				messageSearcher.InitializeSearch();
				bool flag = false;
				int tickCount = Environment.TickCount;
				while (!flag)
				{
					this.threadExit.CheckStop();
					try
					{
						flag = messageSearcher.DoSearch();
						int tickCount2 = Environment.TickCount;
						if (flag)
						{
							result.SetTestResult(true, new TimeSpan((long)((tickCount2 - tickCount) * 10000)).TotalSeconds);
						}
					}
					catch (MapiRetryableException exception)
					{
						this.monitor.PushMessage(SearchCommon.DiagnoseException(result.Server, result.DatabaseGuid, exception));
					}
					this.threadExit.CheckStop();
					if (!flag)
					{
						Thread.Sleep(timeout);
					}
				}
			}
			catch (MapiExceptionNotFound mapiExceptionNotFound)
			{
				ex = mapiExceptionNotFound;
			}
			catch (MapiRetryableException ex2)
			{
				ex = ex2;
			}
			catch (MapiPermanentException ex3)
			{
				ex = ex3;
			}
			catch (TestSearchOperationAbortedException)
			{
				TestSearch.TestSearchTracer.TraceDebug((long)this.GetHashCode(), "Thread terminated on TestSearchOperationAbortedException");
				this.monitor.PushMessage(Strings.TestSearchTestThreadTimeOut);
				result.SetErrorTestResultWithTestThreadTimeOut();
			}
			finally
			{
				if (ex != null)
				{
					this.HandleExceptionInTestThread(result.DatabaseGuid, EventId.MapiError, result, ex);
				}
			}
		}

		private void ProcessFailureResult(SearchTestResult result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			if (this.CheckForServices(result))
			{
				return;
			}
			this.CheckForCatalogState(result);
		}

		private void ResetForParametersFromPipeline()
		{
			this.isTestThreadFinished = false;
			this.threadExit = new StopClass();
			lock (this.resultsLock)
			{
				this.searchTestResults.Clear();
			}
		}

		protected override void InternalStopProcessing()
		{
			TestSearch.TestSearchTracer.TraceDebug((long)this.GetHashCode(), "InternalStopProcessing is called");
			this.threadExit.SetStop();
			this.JoinTestThread();
			base.InternalStopProcessing();
		}

		protected override void Dispose(bool disposing)
		{
			TestSearch.TestSearchTracer.TraceDebug((long)this.GetHashCode(), "Dispose is called");
			this.threadExit.SetStop();
			this.JoinTestThread();
			base.Dispose(disposing);
		}

		private const string SearchService = "MSExchangeFASTSearch";

		private const string FASTSearchService = "HostControllerService";

		private const string StoreService = "MSExchangeIS";

		private const string ServerParam = "Server";

		private const string IndexingTimeoutInSecondsParam = "IndexingTimeOutInSeconds";

		private const string MailboxDatabaseIDParam = "MailboxDatabase";

		private const string ArchiveParam = "Archive";

		private const string MonitoringContextParam = "MonitoringContext";

		private const int DefaultIndexingTimeoutInSeconds = 120;

		private MonitoringData monitoringData = new MonitoringData();

		private List<SearchTestResult> searchTestResults = new List<SearchTestResult>(1);

		private MonitorHelper monitor = new MonitorHelper();

		private bool isTestThreadFinished;

		private Thread testThread;

		private StopClass threadExit = new StopClass();

		private object threadLock = new object();

		private object resultsLock = new object();

		internal static readonly Trace TestSearchTracer = ExTraceGlobals.TestExchangeSearchTracer;

		private static TimeSpan ThreadCheckInterval = new TimeSpan(0, 0, 1);

		private IRecipientSession monitoringTenantRecipientSession;
	}
}
