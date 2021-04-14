using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class Context : DisposableBase, ICriticalBlockFailureHandler, IDatabaseExecutionContext, IExecutionContext, IContextProvider, IConnectionProvider
	{
		internal static void Initialize()
		{
			Context.maximumMailboxLockWaitingCount = Hookable<int>.Create(false, ConfigurationSchema.MailboxLockMaximumWaitCount.Value);
		}

		protected Context()
		{
			this.executionDiagnostics = null;
			this.securityContext = null;
			this.ownSecurityContext = false;
			this.clientType = ClientType.System;
			this.culture = CultureHelper.DefaultCultureInfo;
			this.PerfInstance = null;
			this.testCaseId = TestCaseId.GetInProcessTestCaseId();
		}

		protected Context(ExecutionDiagnostics executionDiagnostics) : this(executionDiagnostics, Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext, false, ClientType.System, CultureHelper.DefaultCultureInfo)
		{
		}

		internal Context(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture) : this(executionDiagnostics, securityContext, false, clientType, culture)
		{
		}

		internal Context(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, bool ownSecurityContext, ClientType clientType, CultureInfo culture)
		{
			this.executionDiagnostics = executionDiagnostics;
			this.securityContext = securityContext;
			this.ownSecurityContext = ownSecurityContext;
			this.UpdateClientType(clientType);
			this.culture = culture;
			this.PerfInstance = null;
			this.testCaseId = TestCaseId.GetInProcessTestCaseId();
			if (ExTraceGlobals.ContextTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ContextTracer.TraceDebug(0L, "Context:Context(): Context created");
			}
		}

		public Context CurrentContext
		{
			get
			{
				return this;
			}
		}

		public Guid UserIdentity { get; internal set; }

		internal static int ThreadIdCriticalDatabaseFailure
		{
			get
			{
				return Context.threadIdCriticalDatabaseFailure;
			}
			set
			{
				Context.threadIdCriticalDatabaseFailure = value;
			}
		}

		public bool TransactionStarted
		{
			get
			{
				return this.DatabaseTransactionStarted || (this.affectedObjects != null && this.affectedObjects.Count != 0) || (this.uncommittedNotifications != null && this.uncommittedNotifications.Count != 0) || (this.uncommittedTimedEvents != null && this.uncommittedTimedEvents.Count != 0);
			}
		}

		public bool DatabaseTransactionStarted
		{
			get
			{
				return this.connection != null && this.connection.TransactionStarted;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.database != null;
			}
		}

		Database IConnectionProvider.Database
		{
			get
			{
				return this.database.PhysicalDatabase;
			}
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public virtual ClientSecurityContext SecurityContext
		{
			get
			{
				return this.securityContext;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
			}
		}

		public virtual ClientType ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public TestCaseId TestCaseId
		{
			get
			{
				return this.testCaseId;
			}
		}

		public bool IsMailboxOperationStarted
		{
			get
			{
				return this.isMailboxOperationStarted;
			}
		}

		public bool CriticalDatabaseFailureDetected
		{
			get
			{
				return this.criticalDatabaseFailureDetected;
			}
		}

		public bool DatabaseFailureDetected
		{
			get
			{
				return this.databaseFailureDetected;
			}
		}

		public object EventHistoryUncommittedTransactionLink { get; set; }

		public MailboxState LockedMailboxState
		{
			get
			{
				return this.lockedMailboxState;
			}
		}

		public virtual IMailboxContext PrimaryMailboxContext
		{
			get
			{
				return null;
			}
		}

		public DatabaseType DatabaseType
		{
			get
			{
				return this.Database.PhysicalDatabase.DatabaseType;
			}
		}

		public bool IsAnyCriticalBlockFailed
		{
			get
			{
				return this.highestFailedCriticalBlockScope != CriticalBlockScope.None;
			}
		}

		public CriticalBlockScope HighestFailedCriticalBlockScope
		{
			get
			{
				return this.highestFailedCriticalBlockScope;
			}
		}

		public ExecutionDiagnostics Diagnostics
		{
			get
			{
				return this.executionDiagnostics;
			}
		}

		IExecutionDiagnostics IExecutionContext.Diagnostics
		{
			get
			{
				return this.executionDiagnostics;
			}
		}

		public bool SkipDatabaseLogsFlush
		{
			get
			{
				return this.skipDatabaseLogsFlush && this.connectionLevel == 0;
			}
			set
			{
				this.skipDatabaseLogsFlush = value;
			}
		}

		public bool ForceNotificationPublish
		{
			get
			{
				return this.forceNotificationPublish;
			}
			set
			{
				this.forceNotificationPublish = value;
			}
		}

		internal StorePerDatabasePerformanceCountersInstance PerfInstance { get; set; }

		public bool IsSharedMailboxOperation
		{
			get
			{
				return this.isSharedMailboxOperation;
			}
		}

		public bool IsSharedUserOperation
		{
			get
			{
				return this.isSharedUserOperation;
			}
		}

		protected bool PartitionFullAccessGranted
		{
			get
			{
				return this.partitionFullAccessGranted;
			}
		}

		private Stack<LockableMailboxComponent> CurrentLockableMailboxComponents
		{
			get
			{
				if (this.currentLockableMailboxComponents == null)
				{
					this.currentLockableMailboxComponents = new Stack<LockableMailboxComponent>(5);
				}
				return this.currentLockableMailboxComponents;
			}
		}

		public static IDisposable SetStaticCommitTestHook(Action action)
		{
			return Context.staticCommitTestHook.SetTestHook(action);
		}

		public static IDisposable SetStaticAbortTestHook(Action action)
		{
			return Context.staticAbortTestHook.SetTestHook(action);
		}

		public static IDisposable SetRiseNotificationTestHook(Action<NotificationEvent> action)
		{
			return Context.riseNotificationTestHook.SetTestHook(action);
		}

		public static IDisposable SetStartMailboxOperationTestHook(Action<Context> action)
		{
			return Context.startMailboxOperationTestHook.SetTestHook(action);
		}

		public static IDisposable SetEndMailboxOperationTestHook(Action<Context, bool> action)
		{
			return Context.endMailboxOperationTestHook.SetTestHook(action);
		}

		public static IDisposable SetPulseOperationTestHook(Action<bool> action)
		{
			return Context.pulseOperationTestHook.SetTestHook(action);
		}

		public static Context CreateForSystem()
		{
			return Context.CreateForSystem(new ExecutionDiagnostics());
		}

		public static Context CreateForSystem(ExecutionDiagnostics executionDiagnostics)
		{
			return Context.Create(executionDiagnostics, Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext, ClientType.System, CultureHelper.DefaultCultureInfo);
		}

		internal static Context Create(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture)
		{
			return new Context(executionDiagnostics, securityContext, clientType, culture);
		}

		internal static TimeSpan GetMailboxLockTimeout(MailboxState mailboxState, TimeSpan desiredTimeout)
		{
			if (mailboxState == null || desiredTimeout == LockManager.InfiniteTimeout || desiredTimeout == TimeSpan.Zero)
			{
				return desiredTimeout;
			}
			int waitingCount = LockManager.GetWaitingCount(mailboxState);
			if (waitingCount >= Context.maximumMailboxLockWaitingCount.Value)
			{
				DiagnosticContext.TraceDword((LID)53152U, (uint)waitingCount);
				return TimeSpan.Zero;
			}
			return desiredTimeout;
		}

		internal static IDisposable SetMaximumMailboxLockWaitingCount(int waitCount)
		{
			return Context.maximumMailboxLockWaitingCount.SetTestHook(waitCount);
		}

		[Conditional("DEBUG")]
		public void AssertUserLocked()
		{
		}

		[Conditional("DEBUG")]
		public void AssertUserLocked(Context.UserLockCheckFrame.Scope scope)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertUserLocked(Guid? userIdentity, IMailboxLockName mailboxLockName)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertUserLocked(ILockName userLockName, IMailboxLockName mailboxLockName)
		{
		}

		[Conditional("DEBUG")]
		public void AssertUserExclusiveLocked()
		{
		}

		[Conditional("DEBUG")]
		public void AssertUserExclusiveLocked(Context.UserLockCheckFrame.Scope scope)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertUserExclusiveLocked(Guid? userIdentity, IMailboxLockName mailboxLockName)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertUserExclusiveLocked(ILockName userLockName, IMailboxLockName mailboxLockName)
		{
		}

		public bool IsUserLocked()
		{
			return Context.IsUserLocked(new Guid?(this.UserIdentity), this.lockedMailboxState);
		}

		public bool IsUserLocked(Context.UserLockCheckFrame.Scope scope)
		{
			return this.userLockChecker != null && this.userLockChecker.Value.IsScope(scope) && this.userLockChecker.Value.HasAnyLock();
		}

		public static bool IsUserLocked(Guid? userIdentity, IMailboxLockName mailboxLockName)
		{
			ILockName lockName = null;
			if (mailboxLockName != null && userIdentity != null)
			{
				lockName = new Context.UserLockName(mailboxLockName, userIdentity.Value);
			}
			return Context.IsUserLocked(lockName, mailboxLockName);
		}

		public static bool IsUserLocked(ILockName userLockName, IMailboxLockName mailboxLockName)
		{
			return (userLockName != null && (LockManager.TestLock(userLockName, LockManager.LockType.UserShared) || LockManager.TestLock(userLockName, LockManager.LockType.UserExclusive))) || (mailboxLockName != null && LockManager.TestLock(mailboxLockName, LockManager.LockType.MailboxExclusive));
		}

		public bool IsUserExclusiveLocked()
		{
			return Context.IsUserExclusiveLocked(new Guid?(this.UserIdentity), this.lockedMailboxState);
		}

		public bool IsUserExclusiveLocked(Context.UserLockCheckFrame.Scope scope)
		{
			return this.userLockChecker != null && this.userLockChecker.Value.IsScope(scope) && this.userLockChecker.Value.HasExclusiveLock();
		}

		public static bool IsUserExclusiveLocked(Guid? userIdentity, IMailboxLockName mailboxLockName)
		{
			ILockName lockName = null;
			if (mailboxLockName != null && userIdentity != null)
			{
				lockName = new Context.UserLockName(mailboxLockName, userIdentity.Value);
			}
			return Context.IsUserExclusiveLocked(lockName, mailboxLockName);
		}

		public static bool IsUserExclusiveLocked(ILockName userLockName, IMailboxLockName mailboxLockName)
		{
			return (userLockName != null && LockManager.TestLock(userLockName, LockManager.LockType.UserExclusive)) || (mailboxLockName != null && LockManager.TestLock(mailboxLockName, LockManager.LockType.MailboxExclusive));
		}

		internal void SetUserInfo(ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture)
		{
			this.securityContext = securityContext;
			this.UpdateClientType(clientType);
			this.culture = culture;
		}

		public Context.DatabaseAssociationBlockFrame AssociateWithDatabase(StoreDatabase database)
		{
			return new Context.DatabaseAssociationBlockFrame(this, database, Context.LockKind.Shared);
		}

		public Context.DatabaseAssociationBlockFrame AssociateWithDatabaseExclusive(StoreDatabase database)
		{
			return new Context.DatabaseAssociationBlockFrame(this, database, Context.LockKind.Exclusive);
		}

		public Context.DatabaseAssociationBlockFrame AssociateWithDatabaseNoLock(StoreDatabase database)
		{
			return new Context.DatabaseAssociationBlockFrame(this, database, Context.LockKind.None);
		}

		public Context.MailboxContextDisAssociationBlockFrameForTest TemporarilyDisassociateMailboxContextForTest()
		{
			return new Context.MailboxContextDisAssociationBlockFrameForTest(this);
		}

		public Context.DatabaseDisAssociationBlockFrameForTest TemporarilyDisassociateDatabaseForTest()
		{
			return new Context.DatabaseDisAssociationBlockFrameForTest(this);
		}

		public void Connect(StoreDatabase database)
		{
			this.Connect(database, Context.LockKind.Shared);
		}

		public void ConnectNoLock(StoreDatabase database)
		{
			this.Connect(database, Context.LockKind.None);
		}

		public void Disconnect()
		{
			this.Disconnect(Context.LockKind.Shared);
		}

		public void DisconnectNoLock()
		{
			this.Disconnect(Context.LockKind.None);
		}

		private void Connect(StoreDatabase database, Context.LockKind lockKind)
		{
			switch (lockKind)
			{
			case Context.LockKind.Shared:
				database.GetSharedLock(this.Diagnostics);
				break;
			case Context.LockKind.Exclusive:
				database.GetExclusiveLock();
				break;
			}
			this.database = database;
			if (database != null)
			{
				this.PerfInstance = PerformanceCounterFactory.GetDatabaseInstance(database);
				if (database.DatabaseHeaderInfo != null)
				{
					this.Diagnostics.DatabaseRepaired = new bool?(database.DatabaseHeaderInfo.DatabaseRepaired);
				}
			}
		}

		private void Disconnect(Context.LockKind lockKind)
		{
			try
			{
				this.Abort();
			}
			finally
			{
				try
				{
					if (this.connection != null && !this.criticalDatabaseFailureDetected)
					{
						using (this.CriticalBlock((LID)39392U, CriticalBlockScope.Database))
						{
							this.connection.FlushDatabaseLogs(false);
							this.EndCriticalBlock();
						}
					}
				}
				finally
				{
					if (this.database != null)
					{
						if (this.connection != null)
						{
							this.connection.Dispose();
							this.connection = null;
						}
						switch (lockKind)
						{
						case Context.LockKind.Shared:
							this.database.ReleaseSharedLock();
							break;
						case Context.LockKind.Exclusive:
							this.database.ReleaseExclusiveLock();
							break;
						}
						this.database = null;
					}
					this.PerfInstance = null;
				}
			}
		}

		public virtual void DismountOnCriticalFailure()
		{
			if (!this.criticalDatabaseFailureDetected)
			{
				return;
			}
			this.SystemCriticalOperation(new TryDelegate(this, (UIntPtr)ldftn(<DismountOnCriticalFailure>b__0)));
		}

		public virtual Connection GetConnection()
		{
			if (this.connection == null && this.database != null)
			{
				this.connection = Factory.CreateConnection(this, this.database.PhysicalDatabase, string.Empty);
			}
			return this.connection;
		}

		public void PushConnection()
		{
			if (this.connectionLevel == 0)
			{
				if (this.connection != null)
				{
					this.connection.Suspend();
				}
				this.savedConnection = this.connection;
				this.savedAffectedObjects = this.affectedObjects;
				this.connection = null;
				this.affectedObjects = null;
			}
			this.connectionLevel++;
		}

		public void PopConnection()
		{
			this.connectionLevel--;
			if (this.connectionLevel == 0)
			{
				if (this.connection != null)
				{
					if (this.connection != null && !this.criticalDatabaseFailureDetected)
					{
						using (this.CriticalBlock((LID)43488U, CriticalBlockScope.Database))
						{
							this.connection.FlushDatabaseLogs(false);
							this.EndCriticalBlock();
						}
					}
					this.connection.Dispose();
					this.connection = null;
				}
				this.connection = this.savedConnection;
				this.affectedObjects = this.savedAffectedObjects;
				this.savedConnection = null;
				this.savedAffectedObjects = null;
				if (this.connection != null)
				{
					this.connection.Resume();
				}
			}
		}

		public void BeginTransactionIfNeeded()
		{
			this.GetConnection().BeginTransactionIfNeeded();
		}

		public void Commit()
		{
			if (this.TransactionStarted)
			{
				List<NotificationEvent> list = null;
				FaultInjection.InjectFault(Context.staticCommitTestHook);
				if (this.connectionLevel == 0)
				{
					bool flag = false;
					try
					{
						if (this.affectedObjects != null)
						{
							foreach (IStateObject stateObject in this.affectedObjects)
							{
								ExTraceGlobals.FaultInjectionTracer.TraceTest(3435539773U);
								stateObject.OnBeforeCommit(this);
							}
						}
						flag = true;
					}
					finally
					{
						if (!flag)
						{
							if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.NotificationTracer.TraceDebug(58137L, "Dump uncommitted notifications because OnBeforeCommit failed");
							}
							this.uncommittedNotifications = null;
							this.uncommittedTimedEvents = null;
						}
					}
					flag = false;
					try
					{
						list = this.uncommittedNotifications;
						this.uncommittedNotifications = null;
						if (list != null)
						{
							Statistics.ContextNotifications.Max.Bump(list.Count);
							this.PublishNotificationsPreCommit(list);
						}
						List<TimedEventEntry> list2 = this.uncommittedTimedEvents;
						this.uncommittedTimedEvents = null;
						if (list2 != null)
						{
							this.PublishTimedEventsPreCommit(list2);
						}
						flag = true;
					}
					finally
					{
					}
				}
				if (this.DatabaseTransactionStarted)
				{
					using (this.CriticalBlock((LID)59872U, CriticalBlockScope.Database))
					{
						this.AddLogTransactionInformationCommon();
						this.connection.Commit(this.SerializeLogTransactionInformation());
						this.EndCriticalBlock();
					}
				}
				if (this.connectionLevel == 0)
				{
					if (this.affectedObjects != null)
					{
						ICollection<IStateObject> collection = this.affectedObjects;
						this.affectedObjects = null;
						using (this.CriticalBlock((LID)35296U, CriticalBlockScope.MailboxSession))
						{
							foreach (IStateObject stateObject2 in collection)
							{
								stateObject2.OnCommit(this);
							}
							this.EndCriticalBlock();
						}
					}
					if (this.mailboxOperationNotifications == null)
					{
						this.mailboxOperationNotifications = list;
					}
					else if (list != null)
					{
						this.mailboxOperationNotifications.AddRange(list);
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.connection == null || this.connection.NumberOfDirtyObjects == 0, "We still have dirty objects");
				}
			}
		}

		public void Abort()
		{
			if (this.TransactionStarted)
			{
				this.Diagnostics.OnTransactionAbort();
				FaultInjection.InjectFault(Context.staticAbortTestHook);
				ICollection<IStateObject> collection = this.affectedObjects;
				this.affectedObjects = null;
				if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.NotificationTracer.TraceDebug(31804L, "Dump uncommited notifications because tx is being aborted");
				}
				this.uncommittedNotifications = null;
				this.uncommittedTimedEvents = null;
				if (this.DatabaseTransactionStarted)
				{
					using (this.CriticalBlock((LID)53728U, CriticalBlockScope.Database))
					{
						this.AddLogTransactionInformationCommon();
						this.connection.Abort(this.SerializeLogTransactionInformation());
						this.EndCriticalBlock();
					}
				}
				if (this.connectionLevel == 0 && collection != null)
				{
					using (this.CriticalBlock((LID)37344U, CriticalBlockScope.MailboxSession))
					{
						foreach (IStateObject stateObject in collection)
						{
							stateObject.OnAbort(this);
						}
						this.EndCriticalBlock();
					}
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.connection == null || this.connection.NumberOfDirtyObjects == 0, "We still have dirty objects");
			}
		}

		public bool IsStateObjectRegistered(IStateObject stateObject)
		{
			if (this.connectionLevel != 0)
			{
				return this.savedAffectedObjects != null && this.savedAffectedObjects.Contains(stateObject);
			}
			return this.affectedObjects != null && this.affectedObjects.Contains(stateObject);
		}

		public void RegisterStateObject(IStateObject stateObject)
		{
			if (this.affectedObjects == null)
			{
				this.affectedObjects = new List<IStateObject>(10);
			}
			else if (this.affectedObjects is List<IStateObject> && this.affectedObjects.Count > 100)
			{
				this.affectedObjects = new HashSet<IStateObject>(this.affectedObjects);
			}
			this.affectedObjects.Add(stateObject);
		}

		public IStateObject RegisterStateAction(Action<Context> commitAction, Action<Context> abortAction)
		{
			Context.SimpleStateObject simpleStateObject = new Context.SimpleStateObject(commitAction, abortAction);
			this.RegisterStateObject(simpleStateObject);
			return simpleStateObject;
		}

		public void UnregisterStateObject(IStateObject stateObject)
		{
			if (this.affectedObjects != null)
			{
				this.affectedObjects.Remove(stateObject);
			}
		}

		public void ResetFailureHistory()
		{
			this.ResetHighestFailedCriticalBlockScope();
			this.Diagnostics.ClearExceptionHistory();
		}

		public void SystemCriticalOperation(TryDelegate operation)
		{
			WatsonOnUnhandledException.Guard(this.Diagnostics, operation);
		}

		public Context.CriticalBlockFrame CriticalBlock(LID lid, ICriticalBlockFailureHandler criticalBlockHandler, CriticalBlockScope scope)
		{
			return new Context.CriticalBlockFrame(lid, this, criticalBlockHandler, scope);
		}

		public Context.CriticalBlockFrame CriticalBlock(LID lid, CriticalBlockScope scope)
		{
			return new Context.CriticalBlockFrame(lid, this, this, scope);
		}

		public void EndCriticalBlock()
		{
			this.criticalBlockHandler = null;
		}

		public virtual void OnCriticalBlockFailed(LID lid, CriticalBlockScope criticalBlockScope)
		{
			if (criticalBlockScope > this.highestFailedCriticalBlockScope)
			{
				this.highestFailedCriticalBlockScope = criticalBlockScope;
			}
			if (criticalBlockScope > CriticalBlockScope.MailboxSession)
			{
				this.reportExceptionCaught = true;
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_CriticalBlockFailure, new object[]
				{
					lid,
					criticalBlockScope,
					new StackTrace(true).ToString()
				});
			}
			if (criticalBlockScope == CriticalBlockScope.Database)
			{
				this.Database.PublishHaFailure(FailureTag.UnexpectedDismount);
				this.OnDatabaseFailure(true, lid);
			}
		}

		protected internal void ResetHighestFailedCriticalBlockScope()
		{
			this.highestFailedCriticalBlockScope = CriticalBlockScope.None;
		}

		protected void PostConstructionInitialize(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture)
		{
			this.executionDiagnostics = executionDiagnostics;
			this.SetUserInfo(securityContext, clientType, culture);
		}

		void ICriticalBlockFailureHandler.OnCriticalBlockFailed(LID lid, Context context, CriticalBlockScope criticalBlockScope)
		{
			this.OnCriticalBlockFailed(lid, criticalBlockScope);
		}

		public void RiseNotificationEvent(NotificationEvent nev)
		{
			if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.NotificationTracer.TraceDebug<NotificationEvent>(46809L, "Queue Notification: {0}", nev);
			}
			if (this.Database.IsRecovery)
			{
				return;
			}
			Statistics.ContextNotifications.Total.Bump();
			if (this.uncommittedNotifications == null)
			{
				this.uncommittedNotifications = new List<NotificationEvent>(8);
			}
			this.uncommittedNotifications.Add(nev);
			if (Context.riseNotificationTestHook.Value != null)
			{
				Context.riseNotificationTestHook.Value(nev);
			}
		}

		public void RaiseTimedEvent(TimedEventEntry timedEvent)
		{
			if (this.uncommittedTimedEvents == null)
			{
				this.uncommittedTimedEvents = new List<TimedEventEntry>();
			}
			this.uncommittedTimedEvents.Add(timedEvent);
		}

		public TOperationData RecordOperation<TOperationData>(IOperationExecutionTrackable operation) where TOperationData : class, IExecutionTrackingData<TOperationData>, new()
		{
			return this.executionDiagnostics.RecordOperation<TOperationData>(operation);
		}

		public void OnDatabaseFailure(bool isCriticalFailure, LID lid)
		{
			if (!this.IsConnected)
			{
				return;
			}
			this.databaseFailureDetected = true;
			if (this.criticalDatabaseFailureDetected)
			{
				return;
			}
			if (isCriticalFailure)
			{
				this.criticalDatabaseFailureDetected = true;
				this.databaseWithCriticalFailure = this.database;
			}
			if (this.criticalDatabaseFailureDetected && Interlocked.CompareExchange(ref Context.threadIdCriticalDatabaseFailure, Environment.CurrentManagedThreadId, 0) == 0)
			{
				string condensedCallStack = ErrorHelper.GetCondensedCallStack(new StackTrace(1, false).ToString());
				string reason = string.Format("LID {0}: {1}", lid, condensedCallStack);
				this.executionDiagnostics.TryPrequarantineMailbox(reason);
			}
		}

		public void OnExceptionCatch(Exception exception)
		{
			this.OnExceptionCatch(exception, null);
		}

		public void OnExceptionCatch(Exception exception, object diagnosticData)
		{
			if (this.reportExceptionCaught)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_UnhandledException, new object[]
				{
					exception
				});
				this.reportExceptionCaught = false;
			}
			this.Diagnostics.OnExceptionCatch(exception, diagnosticData);
		}

		public virtual void OnBeforeTableAccess(Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			if (!this.IsSharedMailboxOperation)
			{
				return;
			}
			if (operationType == Connection.OperationType.Query && (table.Equals(DatabaseSchema.GlobalsTable(this.Database).Table) || table.Equals(DatabaseSchema.MailboxTable(this.Database).Table) || table.Equals(DatabaseSchema.TimedEventsTable(this.Database).Table)))
			{
				return;
			}
			foreach (LockableMailboxComponent lockableMailboxComponent in this.CurrentLockableMailboxComponents)
			{
				if (lockableMailboxComponent.IsValidTableOperation(this, operationType, table, partitionValues))
				{
					return;
				}
			}
			throw new InvalidOperationException(string.Format("Invalid table operation. operationType = {0}, table = {1}, partitionValues = {2}", operationType, table, (partitionValues == null) ? "null" : string.Join<object>(", ", partitionValues)));
		}

		protected virtual void ConnectDatabase()
		{
		}

		protected virtual void DisconnectDatabase()
		{
		}

		protected virtual void ConnectMailboxes()
		{
		}

		protected virtual void DisconnectMailboxes(bool pulseOnly)
		{
			Mailbox[] array = this.mailboxContexts.Values.ToArray<Mailbox>();
			foreach (Mailbox mailbox in array)
			{
				if (mailbox.IsConnected)
				{
					mailbox.Disconnect();
				}
			}
			this.RemoveInternalMailboxes();
		}

		private void SaveMailboxChanges()
		{
			foreach (KeyValuePair<int, Mailbox> keyValuePair in this.mailboxContexts)
			{
				Mailbox value = keyValuePair.Value;
				if (!value.IsDead)
				{
					value.Save(this);
				}
			}
		}

		public void InitializeMailboxExclusiveOperation(MailboxState mailboxState, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout)
		{
			this.InitializeMailboxOperation(mailboxState, operationSource, lockTimeout, false, true);
		}

		public void InitializeMailboxExclusiveOperation(int mailboxNumber, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout)
		{
			this.InitializeMailboxOperation(mailboxNumber, operationSource, lockTimeout, false, true);
		}

		public void InitializeMailboxExclusiveOperation(Guid mailboxGuid, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout)
		{
			this.InitializeMailboxOperation(mailboxGuid, operationSource, lockTimeout, false, true);
		}

		public void InitializeMailboxOperation(MailboxState mailboxState, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout, bool isSharedMailboxOperation, bool isSharedUserOperation)
		{
			this.Diagnostics.MailboxNumber = mailboxState.MailboxNumber;
			this.Diagnostics.MailboxGuid = mailboxState.MailboxGuid;
			this.mailboxOperationParameters = new Context.MailboxOperationParameters(mailboxState, operationSource, lockTimeout, isSharedMailboxOperation, isSharedUserOperation);
		}

		public void InitializeMailboxOperation(int mailboxNumber, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout, bool isSharedMailboxOperation, bool isSharedUserOperation)
		{
			this.Diagnostics.MailboxNumber = mailboxNumber;
			this.mailboxOperationParameters = new Context.MailboxOperationParameters(mailboxNumber, operationSource, lockTimeout, isSharedMailboxOperation, isSharedUserOperation);
		}

		public void InitializeMailboxOperation(Guid mailboxGuid, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout, bool isSharedMailboxOperation, bool isSharedUserOperation)
		{
			this.Diagnostics.MailboxGuid = mailboxGuid;
			this.mailboxOperationParameters = new Context.MailboxOperationParameters(mailboxGuid, operationSource, lockTimeout, isSharedMailboxOperation, isSharedUserOperation);
		}

		public ErrorCode StartMailboxOperationForFailureHandling()
		{
			ErrorCode result;
			try
			{
				result = this.StartMailboxOperation(MailboxCreation.DontAllow, false, true);
			}
			catch (StoreException ex)
			{
				this.OnExceptionCatch(ex);
				result = ErrorCode.CreateWithLid((LID)37628U, ex.Error);
			}
			return result;
		}

		public ErrorCode StartMailboxOperation()
		{
			return this.StartMailboxOperation(MailboxCreation.DontAllow, false, false);
		}

		public ErrorCode StartMailboxOperation(bool allowMailboxCreation)
		{
			return this.StartMailboxOperation(allowMailboxCreation, false, false);
		}

		public ErrorCode StartMailboxOperation(bool allowMailboxCreation, bool findRemovedMailbox)
		{
			return this.StartMailboxOperation(allowMailboxCreation, findRemovedMailbox, false);
		}

		public ErrorCode StartMailboxOperation(bool allowMailboxCreation, bool findRemovedMailbox, bool skipQuarantineCheck)
		{
			return this.StartMailboxOperation(allowMailboxCreation ? MailboxCreation.Allow(null) : MailboxCreation.DontAllow, findRemovedMailbox, skipQuarantineCheck);
		}

		public virtual ErrorCode StartMailboxOperation(MailboxCreation mailboxCreation, bool findRemovedMailbox, bool skipQuarantineCheck)
		{
			return this.StartMailboxOperation(mailboxCreation, findRemovedMailbox, skipQuarantineCheck, false);
		}

		public virtual ErrorCode StartMailboxOperation(MailboxCreation mailboxCreation, bool findRemovedMailbox, bool skipQuarantineCheck, bool takeDababaseConnectionOwnership)
		{
			this.ResetHighestFailedCriticalBlockScope();
			bool operationSignalledAsStarted = false;
			ErrorCode noError;
			try
			{
				this.mailboxOperationOwnsDatabaseConnection = (takeDababaseConnectionOwnership || !this.IsConnected);
				if (Context.startMailboxOperationTestHook.Value != null)
				{
					Context.startMailboxOperationTestHook.Value(this);
				}
				this.ConnectDatabase();
				if (this.mailboxOperationParameters.MailboxState != null)
				{
					if (!this.TryLockMailboxForOperation(this.mailboxOperationParameters.MailboxState, this.mailboxOperationParameters.IsSharedMailboxOperation, this.mailboxOperationParameters.LockTimeout))
					{
						return ErrorCode.CreateTimeout((LID)43632U);
					}
					if (this.mailboxOperationParameters.IsSharedMailboxOperation && this.IsSpecialCacheCleanupRequired(this.mailboxOperationParameters.MailboxState))
					{
						this.UnlockMailboxForOperation(this.mailboxOperationParameters.MailboxState);
						if (!this.TryLockMailboxForOperation(this.mailboxOperationParameters.MailboxState, false, this.mailboxOperationParameters.LockTimeout))
						{
							return ErrorCode.CreateTimeout((LID)36768U);
						}
					}
					this.lockedMailboxState = this.mailboxOperationParameters.MailboxState;
				}
				else if (this.mailboxOperationParameters.MailboxNumber != null)
				{
					bool flag;
					if (!this.TryLockMailboxForOperation(this.mailboxOperationParameters.MailboxNumber.Value, this.mailboxOperationParameters.IsSharedMailboxOperation, this.mailboxOperationParameters.LockTimeout, out flag, out this.lockedMailboxState))
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.lockedMailboxState == null, "Got unlocked MailboxState?");
						if (flag)
						{
							return ErrorCode.CreateTimeout((LID)60016U);
						}
						return ErrorCode.CreateNotFound((LID)51640U);
					}
				}
				else if (this.mailboxOperationParameters.MailboxGuid != null)
				{
					ErrorCode first = this.LockMailboxByGuidForOperation(this.mailboxOperationParameters.MailboxGuid.Value, mailboxCreation, findRemovedMailbox, this.mailboxOperationParameters.IsSharedMailboxOperation, this.mailboxOperationParameters.LockTimeout, out this.lockedMailboxState);
					if (first != ErrorCode.NoError)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.lockedMailboxState == null, "Got unlocked MailboxState?");
						return first.Propagate((LID)57721U);
					}
				}
				else
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "No MailboxState, no mailboxGuid, no mailboxNumber?");
				}
				if (!this.lockedMailboxState.IsValid)
				{
					using (this.CriticalBlock((LID)49632U, CriticalBlockScope.MailboxSession))
					{
						throw new StoreException((LID)63288U, ErrorCodeValue.MdbNotInitialized);
					}
				}
				if (this.isSharedMailboxOperation && !this.TryLockCurrentUser(this.lockedMailboxState, this.mailboxOperationParameters.IsSharedUserOperation, this.mailboxOperationParameters.LockTimeout, this.Diagnostics))
				{
					throw new StoreException((LID)63388U, ErrorCodeValue.Timeout);
				}
				this.SignalStartMailboxOperation(this.lockedMailboxState, this.mailboxOperationParameters.OperationSource);
				operationSignalledAsStarted = true;
				if (!skipQuarantineCheck && this.LockedMailboxState.Quarantined)
				{
					bool flag2 = false;
					DiagnosticContext.TraceDword((LID)32848U, (uint)this.ClientType);
					if (this.ClientType == ClientType.Migration || this.ClientType == ClientType.PublicFolderSystem)
					{
						flag2 = MailboxQuarantineProvider.Instance.IsMigrationAccessAllowed(this.LockedMailboxState.DatabaseGuid, this.LockedMailboxState.MailboxGuid);
					}
					if (!flag2)
					{
						throw new StoreException((LID)61416U, ErrorCodeValue.MailboxQuarantined);
					}
				}
				this.ConnectMailboxes();
				this.isMailboxOperationStarted = true;
				Mailbox storeMailbox;
				if (!this.IsSharedMailboxOperation && this.mailboxContexts.TryGetValue(this.lockedMailboxState.MailboxNumber, out storeMailbox))
				{
					bool flag3 = false;
					try
					{
						LazyMailboxActionList.ExecuteMailboxActions(this, storeMailbox);
						flag3 = true;
					}
					finally
					{
						this.isMailboxOperationStarted = flag3;
					}
				}
				noError = ErrorCode.NoError;
			}
			finally
			{
				if (!this.isMailboxOperationStarted)
				{
					this.UnwindMailboxOperation(true, operationSignalledAsStarted, false, false);
				}
			}
			return noError;
		}

		public void EndMailboxOperation(bool commit)
		{
			this.EndMailboxOperation(commit, false, false);
		}

		public void EndMailboxOperation(bool commit, bool skipDisconnectingDatabase)
		{
			this.EndMailboxOperation(commit, skipDisconnectingDatabase, false);
		}

		public virtual void EndMailboxOperation(bool commit, bool skipDisconnectingDatabase, bool pulseOnly)
		{
			bool flag = false;
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2361797949U);
				if (Context.endMailboxOperationTestHook.Value != null)
				{
					Context.endMailboxOperationTestHook.Value(this, commit);
				}
				if (commit)
				{
					if (this.IsAnyCriticalBlockFailed)
					{
						throw new InvalidOperationException("We should not be committing a transaction if any critical block has failed.");
					}
					this.SaveMailboxChanges();
					this.Commit();
					flag = true;
				}
			}
			finally
			{
				this.UnwindMailboxOperation(!flag, true, skipDisconnectingDatabase, pulseOnly);
			}
		}

		private void UnwindMailboxOperation(bool doAbort, bool operationSignalledAsStarted, bool skipDisconnectingDatabase, bool pulseOnly)
		{
			try
			{
				if (this.IsConnected)
				{
					try
					{
						if (doAbort)
						{
							using (this.CriticalBlock((LID)45024U, CriticalBlockScope.MailboxSession))
							{
								this.Abort();
								this.EndCriticalBlock();
							}
						}
					}
					finally
					{
						try
						{
							this.PublishAllPostCommitNotifications();
						}
						finally
						{
							this.DisconnectMailboxes(pulseOnly);
						}
					}
				}
			}
			finally
			{
				using (this.CriticalBlock((LID)49404U, CriticalBlockScope.MailboxSession))
				{
					try
					{
						try
						{
							if (this.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxShared)
							{
								MailboxStateCache.ResetMailboxState(this, this.lockedMailboxState, this.lockedMailboxState.Status);
							}
						}
						finally
						{
							if (this.lockedMailboxState != null)
							{
								try
								{
									if (operationSignalledAsStarted)
									{
										this.SignalBeforeEndMailboxOperation(this.lockedMailboxState.MailboxNumber);
									}
								}
								finally
								{
									try
									{
										try
										{
											this.UnlockCurrentUser();
										}
										finally
										{
											this.UnlockMailboxForOperation(this.lockedMailboxState);
											this.lockedMailboxState = null;
										}
									}
									finally
									{
										if (operationSignalledAsStarted)
										{
											this.SignalAfterEndMailboxOperation();
										}
									}
								}
							}
						}
					}
					finally
					{
						try
						{
							if (this.mailboxOperationOwnsDatabaseConnection && (!skipDisconnectingDatabase || this.Database.HasExclusiveLockContention()))
							{
								this.DisconnectDatabase();
								this.mailboxOperationOwnsDatabaseConnection = false;
							}
						}
						finally
						{
							this.mailboxOperationOwnsDatabaseConnection = false;
							this.isMailboxOperationStarted = false;
						}
					}
					this.EndCriticalBlock();
				}
			}
		}

		private void PublishAllPostCommitNotifications()
		{
			bool flag = false;
			bool flag2 = this.skipDatabaseLogsFlush;
			this.skipDatabaseLogsFlush = true;
			try
			{
				List<NotificationEvent> list = this.mailboxOperationNotifications;
				this.mailboxOperationNotifications = null;
				if (list != null)
				{
					using (this.CriticalBlock((LID)51680U, CriticalBlockScope.MailboxSession))
					{
						this.PublishNotificationsPostCommit(list);
						this.EndCriticalBlock();
					}
				}
				flag = true;
			}
			finally
			{
				try
				{
					if (this.TransactionStarted)
					{
						ICollection<IStateObject> collection = this.affectedObjects;
						this.affectedObjects = null;
						if (this.DatabaseTransactionStarted)
						{
							Statistics.MiscelaneousNotifications.NotificationHandlingRestartedTransaction.Bump();
							using (this.CriticalBlock((LID)45536U, CriticalBlockScope.Database))
							{
								if (!flag)
								{
									this.AddLogTransactionInformationCommon();
									this.connection.Abort(this.SerializeLogTransactionInformation());
								}
								else
								{
									this.AddLogTransactionInformationCommon();
									this.connection.Commit(this.SerializeLogTransactionInformation());
								}
								this.EndCriticalBlock();
							}
						}
						if (collection != null)
						{
							using (this.CriticalBlock((LID)61920U, CriticalBlockScope.MailboxSession))
							{
								foreach (IStateObject stateObject in collection)
								{
									if (!flag)
									{
										stateObject.OnAbort(this);
									}
									else
									{
										stateObject.OnCommit(this);
									}
								}
								this.EndCriticalBlock();
							}
						}
					}
				}
				finally
				{
					this.skipDatabaseLogsFlush = flag2;
				}
			}
		}

		public ErrorCode PulseMailboxOperation()
		{
			return this.PulseMailboxOperation(MailboxCreation.DontAllow, false, null);
		}

		public ErrorCode PulseMailboxOperation(Action actionOutsideMailboxLock)
		{
			return this.PulseMailboxOperation(MailboxCreation.DontAllow, false, actionOutsideMailboxLock);
		}

		public ErrorCode PulseMailboxOperation(MailboxCreation mailboxCreation, bool findRemovedMailbox, Action actionOutsideMailboxLock)
		{
			return this.PulseMailboxOperation(mailboxCreation, findRemovedMailbox, false, actionOutsideMailboxLock);
		}

		public ErrorCode PulseMailboxOperation(MailboxCreation mailboxCreation, bool findRemovedMailbox, bool skipQuarantineCheck, Action actionOutsideMailboxLock)
		{
			bool flag = 0 != LockManager.GetWaitingCount(this.lockedMailboxState);
			bool flag2 = this.Database.HasExclusiveLockContention();
			bool flag3 = this.mailboxOperationOwnsDatabaseConnection;
			this.EndMailboxOperation(true, true, true);
			if (flag2)
			{
				return ErrorCode.CreateMdbNotInitialized((LID)46028U);
			}
			if (actionOutsideMailboxLock != null)
			{
				actionOutsideMailboxLock();
			}
			if (Context.pulseOperationTestHook.Value != null)
			{
				Context.pulseOperationTestHook.Value(flag);
			}
			if (flag)
			{
				Thread.Sleep(1);
			}
			ErrorCode result = this.StartMailboxOperation(mailboxCreation, findRemovedMailbox, skipQuarantineCheck);
			this.mailboxOperationOwnsDatabaseConnection = flag3;
			return result;
		}

		public bool HasMailboxLockContention
		{
			get
			{
				return LockManager.HasContention(this.lockedMailboxState);
			}
		}

		internal void SetMailboxStateForTest(MailboxState mailboxState)
		{
			this.lockedMailboxState = mailboxState;
		}

		private ErrorCode LockMailboxByGuidForOperation(Guid mailboxGuid, MailboxCreation mailboxCreation, bool findRemovedMailbox, bool sharedLock, TimeSpan timeout, out MailboxState lockedMailboxState)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.TransactionStarted, "Transaction leaked.");
			bool flag;
			if (!MailboxStateCache.TryGetByGuidLocked(this, mailboxGuid, mailboxCreation, findRemovedMailbox, sharedLock, (MailboxState state) => Context.GetMailboxLockTimeout(state, timeout), this.Diagnostics, out flag, out lockedMailboxState) && flag)
			{
				return ErrorCode.CreateTimeout((LID)35440U);
			}
			if (lockedMailboxState == null)
			{
				return ErrorCode.CreateNotFound((LID)61208U);
			}
			this.isSharedMailboxOperation = sharedLock;
			return ErrorCode.NoError;
		}

		private bool TryLockMailboxForOperation(int mailboxNumber, bool sharedLock, TimeSpan timeout, out bool timeoutReached, out MailboxState lockedMailboxState)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.TransactionStarted, "Transaction leaked.");
			bool flag = MailboxStateCache.TryGetLocked(this, mailboxNumber, sharedLock, (MailboxState state) => Context.GetMailboxLockTimeout(state, timeout), this.Diagnostics, out timeoutReached, out lockedMailboxState);
			if (flag)
			{
				this.isSharedMailboxOperation = sharedLock;
			}
			return flag;
		}

		protected void LockMailboxForOperation(MailboxState mailboxState, bool sharedLock)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.TransactionStarted, "Transaction leaked.");
			this.TryLockMailboxForOperation(mailboxState, sharedLock, LockManager.InfiniteTimeout);
		}

		private bool TryLockMailboxForOperation(MailboxState mailboxState, bool sharedLock, TimeSpan timeout)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.TransactionStarted, "Transaction leaked.");
			bool flag = mailboxState.TryGetMailboxLock(sharedLock, Context.GetMailboxLockTimeout(mailboxState, timeout), this.Diagnostics);
			if (flag)
			{
				this.isSharedMailboxOperation = sharedLock;
			}
			return flag;
		}

		protected void UnlockMailboxForOperation(MailboxState mailboxState)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.TransactionStarted, "Transaction leaked.");
			mailboxState.ReleaseMailboxLock(this.IsSharedMailboxOperation);
		}

		public MailboxComponentOperationFrame MailboxComponentReadOperation(LockableMailboxComponent component)
		{
			return MailboxComponentOperationFrame.CreateRead(this, component);
		}

		public MailboxComponentOperationFrame MailboxComponentWriteOperation(LockableMailboxComponent component)
		{
			return MailboxComponentOperationFrame.CreateWrite(this, component);
		}

		public void StartMailboxComponentReadOperation(LockableMailboxComponent component)
		{
			component.LockShared(this.Diagnostics);
			this.CurrentLockableMailboxComponents.Push(component);
		}

		public void EndMailboxComponentReadOperation(LockableMailboxComponent component)
		{
			this.CurrentLockableMailboxComponents.Pop();
			component.ReleaseShared();
		}

		public void StartMailboxComponentWriteOperation(LockableMailboxComponent component)
		{
			component.LockExclusive(this.Diagnostics);
			this.CurrentLockableMailboxComponents.Push(component);
		}

		public void EndMailboxComponentWriteOperation(LockableMailboxComponent component, bool success)
		{
			bool flag = false;
			try
			{
				if (this.IsSharedMailboxOperation && component.Committable && success)
				{
					this.Commit();
					flag = true;
				}
			}
			finally
			{
				if (this.IsSharedMailboxOperation && component.Committable && (!success || !flag))
				{
					this.Abort();
				}
				this.CurrentLockableMailboxComponents.Pop();
				component.ReleaseExclusive();
			}
		}

		public Context.PartitionFullAccessFrame GrantPartitionFullAccess()
		{
			return new Context.PartitionFullAccessFrame(this);
		}

		public void UpdateClientType(ClientType clientType)
		{
			this.clientType = clientType;
			this.executionDiagnostics.UpdateClientType(clientType);
		}

		public void UpdateTestCaseId(TestCaseId testCaseId)
		{
			if (testCaseId.IsNull)
			{
				return;
			}
			this.testCaseId = testCaseId;
			if (this.executionDiagnostics != null)
			{
				this.executionDiagnostics.UpdateTestCaseId(testCaseId);
			}
		}

		public Context.UserIdentityFrame CreateUserIdentityFrame(Guid newUserIdentity)
		{
			return new Context.UserIdentityFrame(this, newUserIdentity);
		}

		private bool TryLockCurrentUser(IMailboxLockName mailboxLockName, bool shared, TimeSpan timeout, ILockStatistics lockStats)
		{
			if (!(this.UserIdentity != Guid.Empty))
			{
				return true;
			}
			this.userLockName = new Context.UserLockName(mailboxLockName, this.UserIdentity);
			LockManager.LockType lockType = shared ? LockManager.LockType.UserShared : LockManager.LockType.UserExclusive;
			if (LockManager.TryGetLock(this.userLockName, lockType, timeout, lockStats))
			{
				this.isSharedUserOperation = shared;
				return true;
			}
			this.userLockName = null;
			return false;
		}

		private void UnlockCurrentUser()
		{
			if (this.userLockName != null)
			{
				LockManager.LockType lockType = this.isSharedUserOperation ? LockManager.LockType.UserShared : LockManager.LockType.UserExclusive;
				LockManager.ReleaseLock(this.userLockName, lockType);
				this.userLockName = null;
			}
		}

		internal void RegisterMailboxContext(Mailbox mailbox)
		{
			this.mailboxContexts.Add(mailbox.MailboxNumber, mailbox);
		}

		internal void UnregisterMailboxContext(Mailbox mailbox)
		{
			this.mailboxContexts.Remove(mailbox.MailboxNumber);
		}

		public IMailboxContext GetMailboxContext(int mailboxNumber)
		{
			Mailbox result;
			if (this.mailboxContexts.TryGetValue(mailboxNumber, out result))
			{
				return result;
			}
			IMailboxContext mailboxContext = this.CreateMailboxContext(mailboxNumber);
			Mailbox mailbox = mailboxContext as Mailbox;
			return mailboxContext;
		}

		protected virtual IMailboxContext CreateMailboxContext(int mailboxNumber)
		{
			if (this.internalMailboxes == null)
			{
				this.internalMailboxes = new List<Mailbox>();
			}
			MailboxState mailboxState = MailboxStateCache.Get(this, mailboxNumber);
			if (mailboxState.Quarantined)
			{
				throw new StoreException((LID)63740U, ErrorCodeValue.MailboxQuarantined);
			}
			if (!mailboxState.IsAccessible)
			{
				throw new StoreException((LID)51452U, ErrorCodeValue.UnexpectedMailboxState);
			}
			Mailbox mailbox = Mailbox.OpenMailbox(this, mailboxState);
			if (mailbox == null)
			{
				throw new StoreException((LID)41212U, ErrorCodeValue.UnexpectedMailboxState);
			}
			this.internalMailboxes.Add(mailbox);
			return mailbox;
		}

		private void RemoveInternalMailboxes()
		{
			if (this.internalMailboxes == null)
			{
				return;
			}
			List<Mailbox> list = this.internalMailboxes;
			this.internalMailboxes = null;
			foreach (Mailbox mailbox in list)
			{
				mailbox.Dispose();
			}
		}

		private bool IsSpecialCacheCleanupRequired(MailboxState mailboxState)
		{
			SharedObjectPropertyBagDataCache cacheForMailbox = SharedObjectPropertyBagDataCache.GetCacheForMailbox(mailboxState);
			return cacheForMailbox != null && cacheForMailbox.IsCleanupRequired();
		}

		private void SignalStartMailboxOperation(MailboxState mailboxState, ExecutionDiagnostics.OperationSource operationSource)
		{
			if (operationSource != ExecutionDiagnostics.OperationSource.MailboxMaintenance && operationSource != ExecutionDiagnostics.OperationSource.MailboxCleanup && operationSource != ExecutionDiagnostics.OperationSource.LogicalIndexCleanup && operationSource != ExecutionDiagnostics.OperationSource.LogicalIndexMaintenanceTableTask && operationSource != ExecutionDiagnostics.OperationSource.MailboxTask)
			{
				MailboxStateCache.OnMailboxActivity(mailboxState);
			}
			if (this.connection != null)
			{
				this.connection.DumpRowStats();
			}
			this.executionDiagnostics.OnStartMailboxOperation(this.Database.MdbGuid, mailboxState.MailboxNumber, mailboxState.MailboxGuid, operationSource, mailboxState.ActivityDigestCollector, RopSummaryCollector.GetRopSummaryCollector(this), this.IsSharedMailboxOperation);
			if (ExTraceGlobals.ContextTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.ContextTracer.TraceInformation(0, 0L, "SignalStartMailboxOperation(mailboxNumber={0}, operationSource={1}), ResultState(MailboxGuid={2}, MailboxStatus={3})", new object[]
				{
					mailboxState.MailboxNumber,
					operationSource,
					mailboxState.MailboxGuid,
					mailboxState.Status
				});
			}
		}

		private void SignalBeforeEndMailboxOperation(int mailboxNumber)
		{
			if (this.connection != null)
			{
				this.connection.DumpRowStats();
			}
			try
			{
				this.executionDiagnostics.OnBeforeEndMailboxOperation();
			}
			finally
			{
				if (ExTraceGlobals.ContextTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ContextTracer.TraceInformation<int>(0, 0L, "SignalBeforeEndMailboxOperation(mailboxNumber={0})", mailboxNumber);
				}
			}
		}

		private void SignalAfterEndMailboxOperation()
		{
			this.executionDiagnostics.OnAfterEndMailboxOperation();
		}

		private void PublishNotificationsPreCommit(IList<NotificationEvent> notifications)
		{
			for (int i = 0; i < notifications.Count; i++)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2965777725U);
				NotificationSubscription.EnumerateSubscriptionsForEvent(NotificationPublishPhase.PreCommit, this, notifications[i], NotificationSubscription.PreCommitPublishCallback);
			}
		}

		private void PublishNotificationsPostCommit(IList<NotificationEvent> notifications)
		{
			for (int i = 0; i < notifications.Count; i++)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(4039519549U);
				NotificationSubscription.EnumerateSubscriptionsForEvent(NotificationPublishPhase.PostCommit, this, notifications[i], NotificationSubscription.PostCommitPublishCallback);
			}
		}

		private void PublishTimedEventsPreCommit(IList<TimedEventEntry> timedEvents)
		{
			if (timedEvents == null || timedEvents.Count == 0)
			{
				return;
			}
			TimedEventsQueue timedEventsQueue = (TimedEventsQueue)this.Database.ComponentData[TimedEventsQueue.TimedEventsQueueSlot];
			for (int i = 0; i < timedEvents.Count; i++)
			{
				timedEventsQueue.InsertTimedEventEntry(this, timedEvents[i]);
			}
		}

		private void AddLogTransactionInformationCommon()
		{
			ILogTransactionInformation logTransactionInformationBlock = new LogTransactionInformationIdentity(this.executionDiagnostics.MailboxNumber, this.executionDiagnostics.ClientType);
			this.executionDiagnostics.LogTransactionInformationCollector.AddLogTransactionInformationBlock(logTransactionInformationBlock);
		}

		private byte[] SerializeLogTransactionInformation()
		{
			int num = 0;
			byte[] array = null;
			num = this.Diagnostics.LogTransactionInformationCollector.Serialize(array, num);
			if (num > 0)
			{
				array = new byte[num];
				num = 0;
				num = this.Diagnostics.LogTransactionInformationCollector.Serialize(array, num);
			}
			this.Diagnostics.ResetLogTransactionInformationCollector();
			return array;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Context>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.database != null)
				{
					this.DisconnectNoLock();
				}
				if (this.ownSecurityContext && this.securityContext != null)
				{
					this.securityContext.Dispose();
				}
			}
			if (ExTraceGlobals.ContextTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ContextTracer.TraceDebug(0L, "Context:Dispose(): Context disposed");
			}
		}

		private const int AvgNotificationsPerTransaction = 8;

		private const int AvgAffectedObjectsPerTransaction = 10;

		private static Hookable<Action> staticCommitTestHook = Hookable<Action>.Create(true, null);

		private static Hookable<Action> staticAbortTestHook = Hookable<Action>.Create(true, null);

		private static Hookable<Action<NotificationEvent>> riseNotificationTestHook = Hookable<Action<NotificationEvent>>.Create(false, null);

		private static Hookable<Action<bool>> pulseOperationTestHook = Hookable<Action<bool>>.Create(true, null);

		private static Hookable<int> maximumMailboxLockWaitingCount;

		private static Hookable<Action<Context>> startMailboxOperationTestHook = Hookable<Action<Context>>.Create(false, null);

		private static Hookable<Action<Context, bool>> endMailboxOperationTestHook = Hookable<Action<Context, bool>>.Create(false, null);

		private static int threadIdCriticalDatabaseFailure = 0;

		private Context.UserLockCheckFrame? userLockChecker;

		private ClientType clientType;

		private TestCaseId testCaseId;

		private ClientSecurityContext securityContext;

		private bool ownSecurityContext;

		private CultureInfo culture;

		private StoreDatabase database;

		private Connection connection;

		private int connectionLevel;

		private Connection savedConnection;

		private List<NotificationEvent> uncommittedNotifications;

		private List<NotificationEvent> mailboxOperationNotifications;

		private ICollection<IStateObject> affectedObjects;

		private ICollection<IStateObject> savedAffectedObjects;

		private ICriticalBlockFailureHandler criticalBlockHandler;

		private CriticalBlockScope criticalBlockScope;

		private CriticalBlockScope highestFailedCriticalBlockScope;

		private ExecutionDiagnostics executionDiagnostics;

		private List<TimedEventEntry> uncommittedTimedEvents;

		private bool criticalDatabaseFailureDetected;

		private bool databaseFailureDetected;

		protected StoreDatabase databaseWithCriticalFailure;

		private bool skipDatabaseLogsFlush;

		private bool forceNotificationPublish;

		private Dictionary<int, Mailbox> mailboxContexts = new Dictionary<int, Mailbox>();

		private List<Mailbox> internalMailboxes;

		private Stack<LockableMailboxComponent> currentLockableMailboxComponents;

		private bool reportExceptionCaught;

		private Context.MailboxOperationParameters mailboxOperationParameters;

		private MailboxState lockedMailboxState;

		private bool isMailboxOperationStarted;

		private bool isSharedMailboxOperation;

		private bool isSharedUserOperation;

		private bool mailboxOperationOwnsDatabaseConnection;

		private bool partitionFullAccessGranted;

		private ILockName userLockName;

		public delegate void EndTransactionHandler(Context context, bool committed);

		public struct UserIdentityFrame : IDisposable
		{
			public UserIdentityFrame(Context context, Guid newUserIdentity)
			{
				this.context = context;
				this.originalUserIdentity = context.UserIdentity;
				context.UserIdentity = newUserIdentity;
			}

			public void Dispose()
			{
				this.context.UserIdentity = this.originalUserIdentity;
			}

			private Context context;

			private Guid originalUserIdentity;
		}

		public struct UserLockCheckFrame : IDisposable
		{
			public UserLockCheckFrame(Context context, Context.UserLockCheckFrame.Scope scope, Guid? userIdentityContext, IMailboxLockName mailboxLockName)
			{
				this.context = context;
				this.scope = scope;
				this.userIdentityContext = userIdentityContext;
				this.mailboxLockName = mailboxLockName;
				context.userLockChecker = new Context.UserLockCheckFrame?(this);
			}

			public void Dispose()
			{
				this.context.userLockChecker = null;
			}

			public bool IsScope(Context.UserLockCheckFrame.Scope scope)
			{
				return this.scope == scope;
			}

			public bool HasAnyLock()
			{
				return Context.IsUserLocked(this.userIdentityContext, this.mailboxLockName);
			}

			public bool HasExclusiveLock()
			{
				return Context.IsUserExclusiveLocked(this.userIdentityContext, this.mailboxLockName);
			}

			private Context.UserLockCheckFrame.Scope scope;

			private Guid? userIdentityContext;

			private IMailboxLockName mailboxLockName;

			private Context context;

			public enum Scope
			{
				Folder,
				SearchFolder,
				LogicalIndex
			}
		}

		public struct CriticalBlockFrame : IDisposable
		{
			internal CriticalBlockFrame(LID lid, Context context, ICriticalBlockFailureHandler criticalBlockHandler, CriticalBlockScope scope)
			{
				this.lid = lid;
				this.outerCriticalBlockHandler = context.criticalBlockHandler;
				this.outerCriticalBlockScope = context.criticalBlockScope;
				context.criticalBlockHandler = criticalBlockHandler;
				context.criticalBlockScope = scope;
				this.context = context;
			}

			public void Dispose()
			{
				if (this.context != null)
				{
					if (this.context.criticalBlockHandler != null)
					{
						if (ExTraceGlobals.CriticalBlockTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.CriticalBlockTracer.TraceError<string>(0L, "Critical Block failure, callstack:{0}", new StackTrace(true).ToString());
						}
						DiagnosticContext.TraceDword((LID)63456U, this.lid.Value);
						this.context.criticalBlockHandler.OnCriticalBlockFailed(this.lid, this.context, this.context.criticalBlockScope);
					}
					this.context.criticalBlockHandler = this.outerCriticalBlockHandler;
					this.context.criticalBlockScope = this.outerCriticalBlockScope;
					this.context = null;
				}
			}

			private LID lid;

			private Context context;

			private ICriticalBlockFailureHandler outerCriticalBlockHandler;

			private CriticalBlockScope outerCriticalBlockScope;
		}

		internal enum LockKind
		{
			None,
			Shared,
			Exclusive
		}

		public struct DatabaseAssociationBlockFrame : IDisposable
		{
			internal DatabaseAssociationBlockFrame(Context context, StoreDatabase database, Context.LockKind lockKind)
			{
				context.Connect(database, lockKind);
				this.context = context;
				this.lockKind = lockKind;
			}

			public void Dispose()
			{
				if (this.context != null)
				{
					this.context.Disconnect(this.lockKind);
					this.context = null;
				}
			}

			private Context context;

			private Context.LockKind lockKind;
		}

		public struct MailboxContextDisAssociationBlockFrameForTest : IDisposable
		{
			internal MailboxContextDisAssociationBlockFrameForTest(Context context)
			{
				this.context = context;
				this.savedMailboxContexts = context.mailboxContexts;
				context.mailboxContexts = new Dictionary<int, Mailbox>();
			}

			public void Dispose()
			{
				if (this.context != null)
				{
					this.context.mailboxContexts = this.savedMailboxContexts;
					this.context = null;
				}
			}

			private Context context;

			private Dictionary<int, Mailbox> savedMailboxContexts;
		}

		public struct DatabaseDisAssociationBlockFrameForTest : IDisposable
		{
			internal DatabaseDisAssociationBlockFrameForTest(Context context)
			{
				this.context = context;
				this.database = context.Database;
				if (context.Database.IsExclusiveLockHeld())
				{
					this.lockKind = Context.LockKind.Exclusive;
				}
				else if (context.Database.IsSharedLockHeld())
				{
					this.lockKind = Context.LockKind.Shared;
				}
				else
				{
					this.lockKind = Context.LockKind.None;
				}
				context.Disconnect(this.lockKind);
			}

			public void Dispose()
			{
				if (this.context != null)
				{
					this.context.Connect(this.database, this.lockKind);
					this.context = null;
				}
			}

			private Context context;

			private Context.LockKind lockKind;

			private StoreDatabase database;
		}

		public struct PartitionFullAccessFrame : IDisposable
		{
			internal PartitionFullAccessFrame(Context context)
			{
				this.previousPartitionFullAccessGranted = context.partitionFullAccessGranted;
				context.partitionFullAccessGranted = true;
				this.context = context;
			}

			public void Dispose()
			{
				if (this.context != null)
				{
					this.context.partitionFullAccessGranted = this.previousPartitionFullAccessGranted;
					this.context = null;
				}
			}

			private Context context;

			private bool previousPartitionFullAccessGranted;
		}

		public struct MailboxUnlockFrameForTest : IDisposable
		{
			internal MailboxUnlockFrameForTest(MailboxState mailboxState, Context context, bool sharedMailboxLock, bool sharedUserLock, ILockStatistics lockStats)
			{
				context.UnlockCurrentUser();
				mailboxState.ReleaseMailboxLock(sharedMailboxLock);
				this.mailboxState = mailboxState;
				this.sharedMailboxLock = sharedMailboxLock;
				this.sharedUserLock = sharedUserLock;
				this.lockStats = lockStats;
				this.context = context;
			}

			public void Dispose()
			{
				this.mailboxState.GetMailboxLock(this.sharedMailboxLock, this.lockStats);
				this.context.TryLockCurrentUser(this.mailboxState, this.sharedUserLock, LockManager.InfiniteTimeout, this.lockStats);
			}

			private readonly Context context;

			private readonly MailboxState mailboxState;

			private readonly bool sharedMailboxLock;

			private readonly bool sharedUserLock;

			private readonly ILockStatistics lockStats;
		}

		private class UserLockName : ILockName, IEquatable<ILockName>, IComparable<ILockName>
		{
			public UserLockName(IMailboxLockName mailboxLockName, Guid userGuid) : this(mailboxLockName.DatabaseGuid, mailboxLockName.MailboxPartitionNumber, userGuid)
			{
			}

			public UserLockName(Guid databaseGuid, int mailboxPartitionNumber, Guid userGuid)
			{
				this.databaseGuid = databaseGuid;
				this.mailboxPartitionNumber = mailboxPartitionNumber;
				this.userGuid = userGuid;
				this.hashCode = (databaseGuid.GetHashCode() ^ mailboxPartitionNumber ^ userGuid.GetHashCode());
			}

			public int MailboxPartitionNumber
			{
				get
				{
					return this.mailboxPartitionNumber;
				}
			}

			public Guid DatabaseGuid
			{
				get
				{
					return this.databaseGuid;
				}
			}

			public Guid UserGuid
			{
				get
				{
					return this.userGuid;
				}
			}

			public LockManager.LockLevel LockLevel
			{
				get
				{
					return LockManager.LockLevel.User;
				}
			}

			public LockManager.NamedLockObject CachedLockObject { get; set; }

			public ILockName GetLockNameToCache()
			{
				return this;
			}

			public override int GetHashCode()
			{
				return this.hashCode;
			}

			public override bool Equals(object other)
			{
				return this.Equals(other as Context.UserLockName);
			}

			public bool Equals(ILockName other)
			{
				return other != null && this.CompareTo(other) == 0;
			}

			public int CompareTo(ILockName other)
			{
				int num = this.LockLevel.CompareTo(other.LockLevel);
				if (num == 0)
				{
					Context.UserLockName userLockName = other as Context.UserLockName;
					num = this.databaseGuid.CompareTo(userLockName.databaseGuid);
					if (num == 0)
					{
						num = this.mailboxPartitionNumber.CompareTo(userLockName.mailboxPartitionNumber);
						if (num == 0)
						{
							num = this.userGuid.CompareTo(userLockName.userGuid);
						}
					}
				}
				return num;
			}

			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"DB ",
					this.DatabaseGuid.ToString(),
					"/MBX ",
					this.MailboxPartitionNumber.ToString(),
					"/User ",
					this.userGuid.ToString()
				});
			}

			private readonly Guid databaseGuid;

			private readonly int mailboxPartitionNumber;

			private readonly Guid userGuid;

			private readonly int hashCode;
		}

		private struct MailboxOperationParameters
		{
			internal MailboxOperationParameters(MailboxState mailboxState, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout, bool isSharedMailboxOperation, bool isSharedUserOperation)
			{
				this = new Context.MailboxOperationParameters(mailboxState, null, null, operationSource, lockTimeout, isSharedMailboxOperation, isSharedUserOperation);
			}

			internal MailboxOperationParameters(int mailboxNumber, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout, bool isSharedMailboxOperation, bool isSharedUserOperation)
			{
				this = new Context.MailboxOperationParameters(null, new int?(mailboxNumber), null, operationSource, lockTimeout, isSharedMailboxOperation, isSharedUserOperation);
			}

			internal MailboxOperationParameters(Guid mailboxGuid, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout, bool isSharedMailboxOperation, bool isSharedUserOperation)
			{
				this = new Context.MailboxOperationParameters(null, null, new Guid?(mailboxGuid), operationSource, lockTimeout, isSharedMailboxOperation, isSharedUserOperation);
			}

			private MailboxOperationParameters(MailboxState mailboxState, int? mailboxNumber, Guid? mailboxGuid, ExecutionDiagnostics.OperationSource operationSource, TimeSpan lockTimeout, bool isSharedMailboxOperation, bool isSharedUserOperation)
			{
				this.mailboxState = mailboxState;
				this.mailboxNumber = mailboxNumber;
				this.mailboxGuid = mailboxGuid;
				this.operationSource = operationSource;
				this.lockTimeout = lockTimeout;
				this.isSharedMailboxOperation = isSharedMailboxOperation;
				this.isSharedUserOperation = (isSharedUserOperation || !isSharedMailboxOperation);
			}

			internal MailboxState MailboxState
			{
				get
				{
					return this.mailboxState;
				}
			}

			internal int? MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			internal Guid? MailboxGuid
			{
				get
				{
					return this.mailboxGuid;
				}
			}

			internal ExecutionDiagnostics.OperationSource OperationSource
			{
				get
				{
					return this.operationSource;
				}
			}

			internal TimeSpan LockTimeout
			{
				get
				{
					return this.lockTimeout;
				}
			}

			internal bool IsSharedMailboxOperation
			{
				get
				{
					return this.isSharedMailboxOperation;
				}
			}

			internal bool IsSharedUserOperation
			{
				get
				{
					return this.isSharedUserOperation;
				}
			}

			private readonly MailboxState mailboxState;

			private readonly int? mailboxNumber;

			private readonly Guid? mailboxGuid;

			private readonly ExecutionDiagnostics.OperationSource operationSource;

			private readonly TimeSpan lockTimeout;

			private readonly bool isSharedMailboxOperation;

			private readonly bool isSharedUserOperation;
		}

		public class SimpleStateObject : IStateObject
		{
			public SimpleStateObject(Action<Context> commitAction, Action<Context> abortAction)
			{
				this.commitAction = commitAction;
				this.abortAction = abortAction;
			}

			public void OnBeforeCommit(Context context)
			{
			}

			public void OnCommit(Context context)
			{
				if (this.commitAction != null)
				{
					this.commitAction(context);
				}
			}

			public void OnAbort(Context context)
			{
				if (this.abortAction != null)
				{
					this.abortAction(context);
				}
			}

			private Action<Context> commitAction;

			private Action<Context> abortAction;
		}
	}
}
