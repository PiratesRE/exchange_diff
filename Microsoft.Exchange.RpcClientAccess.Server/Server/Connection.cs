using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class Connection : BaseObject, IConnection, IConnectionInformation
	{
		public Connection(int connectionId, ConnectionInfo connectionInfo, ClientInfo clientInfo, IUser user, IBudget budget, ClientSecurityContext clientSecurityContext, HandlerFactory handlerFactory, IDriverFactory driverFactory)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.usageCount = 0;
				this.removeAction = null;
				this.connectionId = connectionId;
				this.connectionInfo = connectionInfo;
				this.clientInfo = clientInfo;
				this.budget = budget;
				this.clientSecurityContext = clientSecurityContext;
				this.user = user;
				this.user.AddReference();
				this.cultureInfo = StoreSession.CreateMapiCultureInfo(this.connectionInfo.LocaleInfo.StringLocaleId, this.connectionInfo.LocaleInfo.SortLocaleId, this.connectionInfo.LocaleInfo.CodePageId);
				if (ExMonHandler.IsEnabled)
				{
					this.handler = new ExMonHandler(Configuration.ServiceConfiguration.EnableExMonTestMode, connectionId, user.LegacyDistinguishedName, connectionInfo.ClientIpAddress.ToString(), connectionInfo.ClientVersion, handlerFactory(this), "MSExchangeRPC");
				}
				else
				{
					this.handler = handlerFactory(this);
				}
				this.ropDriver = driverFactory.CreateIRopDriver(this.handler, this);
				RopDriver ropDriver = this.ropDriver as RopDriver;
				if (ropDriver != null)
				{
					ropDriver.OnBeforeRopExecuted = new RopProcessingCallbackDelegate(Connection.OnBeforeRopExecuted);
					ropDriver.OnAfterRopExecuted = new RopProcessingCallbackDelegate(Connection.OnAfterRopExecuted);
					ropDriver.IsMimumResponseSizeEnforcementEnabled = new Func<bool>(Connection.IsMimumResponseSizeEnforcementEnabled);
				}
				ExTraceGlobals.ConnectRpcTracer.TraceInformation<string, int>(0, Activity.TraceId, "Connected user '{0}'. ConnectionId = {1:X}", user.LegacyDistinguishedName, this.connectionId);
				this.lastLogTimeBinary = ExDateTime.UtcNow.ToBinary();
				this.RegisterActivity();
				ReferencedActivityScope referencedActivityScope = ReferencedActivityScope.Current;
				if (referencedActivityScope != null)
				{
					referencedActivityScope.AddRef();
					this.referencedActivityScope = referencedActivityScope;
				}
				else
				{
					this.referencedActivityScope = ReferencedActivityScope.Create(null);
				}
				disposeGuard.Success();
			}
		}

		public IBudget Budget
		{
			get
			{
				return this.budget;
			}
		}

		internal DispatchOptions DispatchOptions
		{
			get
			{
				return this.connectionInfo.DispatchOptions;
			}
		}

		ushort IConnectionInformation.SessionId
		{
			get
			{
				base.CheckDisposed();
				return this.InternalSessionId;
			}
		}

		bool IConnectionInformation.ClientSupportsBackoffResult
		{
			get
			{
				return this.clientInfo.Mode != ClientMode.ExchangeServer && this.connectionInfo.ClientVersion >= MapiVersion.Outlook12;
			}
		}

		bool IConnectionInformation.ClientSupportsBufferTooSmallBreakup
		{
			get
			{
				return this.clientInfo.Mode != ClientMode.ExchangeServer && this.connectionInfo.ClientVersion >= MapiVersion.Outlook12;
			}
		}

		Encoding IConnectionInformation.String8Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		public ClientInfo ClientInformation
		{
			get
			{
				return this.clientInfo;
			}
		}

		public IPAddress ServerIpAddress
		{
			get
			{
				return this.connectionInfo.ServerIpAddress;
			}
		}

		public string ProtocolSequence
		{
			get
			{
				return this.connectionInfo.ProtocolSequence;
			}
		}

		public ConnectionFlags ConnectionFlags
		{
			get
			{
				return this.connectionInfo.ConnectionFlags;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.connectionInfo.OrganizationId ?? this.User.OrganizationId;
			}
		}

		public bool IsWebService
		{
			get
			{
				return this.connectionInfo.ProtocolSequence == "xrop";
			}
		}

		public string ActAsLegacyDN
		{
			get
			{
				return this.connectionInfo.UserDn;
			}
		}

		public bool IsFederatedSystemAttendant
		{
			get
			{
				return this.user.IsFederatedSystemAttendant;
			}
		}

		public ClientSecurityContext AccessingClientSecurityContext
		{
			get
			{
				base.CheckDisposed();
				return this.clientSecurityContext;
			}
		}

		public MiniRecipient MiniRecipient
		{
			get
			{
				return this.User.MiniRecipient;
			}
		}

		public void BackoffConnect(Exception reason)
		{
			base.CheckDisposed();
			ExTraceGlobals.ClientThrottledTracer.TraceInformation<string, int>(0, Activity.TraceId, "Backing off future connect requests for user {0}. ConnectionId = {1:X}", this.User.LegacyDistinguishedName, this.connectionId);
			this.User.BackoffConnect(reason);
		}

		public ExchangePrincipal FindExchangePrincipalByLegacyDN(string legacyDN)
		{
			base.CheckDisposed();
			return this.User.GetExchangePrincipal(legacyDN);
		}

		public void InvalidateCachedUserInfo()
		{
			base.CheckDisposed();
			this.User.InvalidatePrincipalCache();
		}

		public void MarkAsDeadAndDropAllAsyncCalls()
		{
			ExTraceGlobals.ConnectRpcTracer.TraceDebug<Connection>(0, Activity.TraceId, "MarkAsDeadAndDropAllAsyncCalls. Connection = {0}.", this);
			this.MarkAsDead();
			this.CompleteAction("backend connection is dead", true, RpcErrorCode.None);
		}

		public void ExecuteInContext<T>(T input, Action<T> action)
		{
			Activity.Guard guard = null;
			try
			{
				if (Activity.Current == null)
				{
					guard = new Activity.Guard();
					guard.AssociateWithCurrentThread(this.Activity, false);
				}
				bool flag = false;
				if (ExUserTracingAdaptor.Instance.IsTracingEnabledUser(this.ActAsLegacyDN))
				{
					flag = true;
					BaseTrace.CurrentThreadSettings.EnableTracing();
				}
				try
				{
					action(input);
				}
				finally
				{
					if (flag)
					{
						BaseTrace.CurrentThreadSettings.DisableTracing();
					}
				}
			}
			finally
			{
				if (guard != null)
				{
					guard.Dispose();
				}
			}
		}

		public Fqdn TargetServer { private get; set; }

		public bool IsEncrypted
		{
			get
			{
				return this.connectionInfo.IsEncrypted;
			}
		}

		public CultureInfo CultureInfo
		{
			get
			{
				base.CheckDisposed();
				return this.cultureInfo;
			}
		}

		public int CodePageId
		{
			get
			{
				base.CheckDisposed();
				return this.connectionInfo.LocaleInfo.CodePageId;
			}
		}

		public string RpcServerTarget
		{
			get
			{
				base.CheckDisposed();
				return this.connectionInfo.RpcServerTarget;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Connection>(this);
		}

		internal Activity Activity
		{
			get
			{
				return this.connectionInfo.Activity;
			}
		}

		internal ReferencedActivityScope ReferencedActivityScope
		{
			get
			{
				return this.referencedActivityScope;
			}
		}

		internal IRopDriver RopDriver
		{
			get
			{
				base.CheckDisposed();
				return this.ropDriver;
			}
		}

		internal IConnectionHandler Handler
		{
			get
			{
				base.CheckDisposed();
				return this.handler;
			}
		}

		internal IUser User
		{
			get
			{
				base.CheckDisposed();
				return this.user;
			}
		}

		internal Action<bool, int> CompletionAction
		{
			set
			{
				this.completionAction = value;
			}
		}

		internal bool CompletionActionAssigned
		{
			get
			{
				return this.completionAction != null;
			}
		}

		internal bool IsDead
		{
			get
			{
				return this.isDead;
			}
		}

		internal void MarkAsDead()
		{
			this.isDead = true;
		}

		internal void CompleteAction(string reason, bool eventsPending, RpcErrorCode storeError)
		{
			Action<bool, int> action = Interlocked.Exchange<Action<bool, int>>(ref this.completionAction, null);
			if (ExTraceGlobals.AsyncRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AsyncRpcTracer.TraceDebug(Activity.TraceId, "CompleteAction ({0}). Connection = {1}, eventsPending = {2}, storeError = {3}, completionActionAssigned = {4}.", new object[]
				{
					reason,
					this,
					eventsPending,
					storeError,
					action != null
				});
			}
			if (action != null)
			{
				action(eventsPending, (int)storeError);
			}
		}

		internal static int GetConnectionId(Connection connection)
		{
			if (connection == null || connection.IsDisposed)
			{
				return 0;
			}
			return connection.connectionId;
		}

		internal void RegisterActivity()
		{
			this.LastAccessTime = ExDateTime.UtcNow;
		}

		internal ExDateTime LastAccessTime
		{
			get
			{
				return ExDateTime.FromBinary(this.lastAccessTimeBinary);
			}
			private set
			{
				this.lastAccessTimeBinary = value.ToBinary();
			}
		}

		internal ExDateTime LastLogTime
		{
			get
			{
				return ExDateTime.FromBinary(this.lastLogTimeBinary);
			}
			set
			{
				this.lastLogTimeBinary = value.ToBinary();
			}
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.ropDriver);
			Util.DisposeIfPresent(this.handler);
			this.connectionId = 0;
			Util.DisposeIfPresent(this.clientSecurityContext);
			this.connectionInfo.Activity.RegisterBudget(null);
			Util.DisposeIfPresent(this.budget);
			if (this.user != null)
			{
				this.user.Release();
				this.user = null;
			}
			if (this.referencedActivityScope != null)
			{
				this.referencedActivityScope.Release();
			}
			base.InternalDispose();
		}

		internal void BeginServerPerformanceCounting()
		{
			if (this.serverPerformanceObject == null && this.TargetServer != null)
			{
				this.serverPerformanceObject = new ServerPerformanceObject(this.TargetServer);
			}
			if (this.serverPerformanceObject != null)
			{
				this.serverPerformanceObject.Start();
			}
		}

		internal void EndServerPerformanceCounting()
		{
			if (this.serverPerformanceObject != null)
			{
				this.serverPerformanceObject.StopAndCollectPerformanceData();
				if (this.serverPerformanceObject.LastRpcLatency != null)
				{
					ProtocolLog.UpdateMailboxServerRpcProcessingTime(this.serverPerformanceObject.LastRpcLatency.Value);
				}
			}
		}

		internal bool TryIncrementUsageCount()
		{
			bool result;
			lock (this.usageCountLock)
			{
				if (this.removeAction != null)
				{
					result = false;
				}
				else
				{
					this.usageCount++;
					result = true;
				}
			}
			return result;
		}

		internal void DecrementUsageCount()
		{
			bool flag = false;
			lock (this.usageCountLock)
			{
				this.usageCount--;
				if (this.usageCount == 0 && this.removeAction != null)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.removeAction();
			}
		}

		internal void MarkForRemoval(Action action)
		{
			lock (this.usageCountLock)
			{
				if (this.removeAction == null)
				{
					this.removeAction = action;
				}
			}
		}

		internal void StartNewActivityScope()
		{
			lock (this.referencedActivityScopeLock)
			{
				ReferencedActivityScope referencedActivityScope = null;
				bool flag2 = false;
				try
				{
					referencedActivityScope = ReferencedActivityScope.Create(this.referencedActivityScope.ActivityScope.Metadata);
					if (this.referencedActivityScope != null)
					{
						this.referencedActivityScope.Release();
					}
					this.referencedActivityScope = referencedActivityScope;
					flag2 = true;
				}
				finally
				{
					if (!flag2 && referencedActivityScope != null)
					{
						referencedActivityScope.Release();
					}
				}
			}
		}

		internal ReferencedActivityScope GetReferencedActivityScope()
		{
			ReferencedActivityScope result;
			lock (this.referencedActivityScopeLock)
			{
				this.referencedActivityScope.AddRef();
				result = this.referencedActivityScope;
			}
			return result;
		}

		internal void UpdateBudgetBalance()
		{
			string budgetBalance;
			if (this.budget != null && this.referencedActivityScope != null && this.budget.TryGetBudgetBalance(out budgetBalance))
			{
				WorkloadManagementLogger.SetBudgetBalance(budgetBalance, this.referencedActivityScope.ActivityScope);
			}
		}

		public override string ToString()
		{
			return string.Format("UserDn = {0}, SessionId = {1}, ProtocolSequence = {2}, ClientIpAddress = {3}, ClientVersion = {4}, ServerIpAddress = {5}, LocaleInfo = {6}. IsDead = {7}, isDisposed = {8}, CompletionActionAssigned = {9}.", new object[]
			{
				this.connectionInfo.UserDn,
				this.InternalSessionId,
				this.connectionInfo.ProtocolSequence,
				this.connectionInfo.ClientIpAddress,
				this.connectionInfo.ClientVersion,
				this.connectionInfo.ServerIpAddress,
				this.connectionInfo.LocaleInfo,
				this.isDead,
				base.IsDisposed,
				this.CompletionActionAssigned
			});
		}

		private static void OnBeforeRopExecuted(Rop rop, ServerObjectHandleTable serverObjectHandleTable)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				currentActivityScope.Action = rop.RopId.ToString();
			}
		}

		private static void OnAfterRopExecuted(Rop rop, ServerObjectHandleTable serverObjectHandleTable)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				currentActivityScope.Action = null;
			}
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.OperationsRate.Increment();
		}

		private static bool IsMimumResponseSizeEnforcementEnabled()
		{
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).RpcClientAccess.MimumResponseSizeEnforcement.Enabled;
		}

		private ushort InternalSessionId
		{
			get
			{
				return (ushort)(this.connectionId & 65535);
			}
		}

		public const string WebServiceProtocolSequence = "xrop";

		private readonly ConnectionInfo connectionInfo;

		private readonly ClientInfo clientInfo;

		private readonly IConnectionHandler handler;

		private readonly IRopDriver ropDriver;

		private readonly IBudget budget;

		private readonly object usageCountLock = new object();

		private readonly CultureInfo cultureInfo;

		private readonly object referencedActivityScopeLock = new object();

		private IUser user;

		private ClientSecurityContext clientSecurityContext;

		private int connectionId;

		private Action<bool, int> completionAction;

		private bool isDead;

		private long lastAccessTimeBinary;

		private long lastLogTimeBinary;

		private int usageCount;

		private Action removeAction;

		private ServerPerformanceObject serverPerformanceObject;

		private ReferencedActivityScope referencedActivityScope;
	}
}
