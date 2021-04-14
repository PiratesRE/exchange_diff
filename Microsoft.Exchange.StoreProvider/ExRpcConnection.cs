using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Security;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExRpcConnection : MapiUnk
	{
		internal ExRpcConnection(IExRpcConnectionInterface iExRpcConnection, WebServiceConnection webServiceConnection, bool isCrossServer, TimeSpan callTimeout) : base(iExRpcConnection, null, null)
		{
			this.iExRpcConnection = iExRpcConnection;
			this.webServiceConnection = webServiceConnection;
			this.IsCrossServer = isCrossServer;
			this.mapiStoreList = new List<MapiStore>(2);
			this.creationTime = DateTime.UtcNow;
			this.apartmentState = Thread.CurrentThread.GetApartmentState();
			this.creationThreadId = Environment.CurrentManagedThreadId;
			int serverVersion = this.iExRpcConnection.GetServerVersion(out this.versionMajor, out this.versionMinor, out this.buildMajor, out this.buildMinor);
			if (serverVersion != 0)
			{
				base.ThrowIfError("Unable to get server version information.", serverVersion);
			}
			this.threadLockCount = 0U;
			this.owningThread = null;
			this.callTimeoutTimer = null;
			if (callTimeout != TimeSpan.Zero)
			{
				this.CrashTimeout = callTimeout + TimeSpan.FromHours(1.0);
			}
			else
			{
				this.CrashTimeout = TimeSpan.Zero;
			}
			ExRpcPerf.ExRpcConnectionBirth();
		}

		public int VersionMajor
		{
			get
			{
				return this.versionMajor;
			}
		}

		public int VersionMinor
		{
			get
			{
				return this.versionMinor;
			}
		}

		public int BuildMajor
		{
			get
			{
				return this.buildMajor;
			}
		}

		public int BuildMinor
		{
			get
			{
				return this.buildMinor;
			}
		}

		public TimeSpan CrashTimeout { get; private set; }

		public bool IsCrossServer { get; private set; }

		public void ClearStorePerRPCStats()
		{
			this.iExRpcConnection.ClearStorePerRPCStats();
		}

		public PerRPCPerformanceStatistics GetStorePerRPCStats()
		{
			PerRpcStats nativeStats;
			uint storePerRPCStats = this.iExRpcConnection.GetStorePerRPCStats(out nativeStats);
			return PerRPCPerformanceStatistics.CreateFromNative(storePerRPCStats, nativeStats);
		}

		public void ClearRpcStatistics()
		{
			this.iExRpcConnection.ClearRPCStats();
		}

		public RpcStatistics GetRpcStatistics()
		{
			RpcStats nativeStats;
			int rpcstats = this.iExRpcConnection.GetRPCStats(out nativeStats);
			if (rpcstats != 0)
			{
				base.ThrowIfError("Unable to get RPC statistics.", rpcstats);
			}
			return RpcStatistics.CreateFromNative(nativeStats);
		}

		public void ExecuteWithInternalAccess(Action actionDelegate)
		{
			int num = this.iExRpcConnection.SetInternalAccess();
			if (num != 0)
			{
				base.ThrowIfError("Unable to set internal access elevation", num);
			}
			bool flag = false;
			try
			{
				actionDelegate();
				flag = true;
			}
			finally
			{
				num = this.iExRpcConnection.ClearInternalAccess();
				if (flag && num != 0)
				{
					base.ThrowIfError("Failure during clearing of internal access elevation", num);
				}
			}
		}

		public void CheckForNotifications()
		{
			if (this.IsWebServiceConnection)
			{
				this.iExRpcConnection.CheckForNotifications();
			}
		}

		internal bool IsWebServiceConnection
		{
			get
			{
				return this.webServiceConnection != null;
			}
		}

		internal Exception InternalLowLevelException
		{
			get
			{
				if (this.webServiceConnection != null)
				{
					return this.webServiceConnection.LastException;
				}
				return null;
			}
		}

		protected override void MapiInternalDispose()
		{
			ExRpcPerf.ExRpcConnectionGone();
			lock (this)
			{
				if (this.mapiStoreList != null)
				{
					for (int i = this.mapiStoreList.Count; i > 0; i--)
					{
						if (this.mapiStoreList[i - 1] != null)
						{
							this.mapiStoreList[i - 1].Dispose();
						}
					}
				}
				this.iExRpcConnection = null;
				this.mapiStoreList = null;
			}
			base.MapiInternalDispose();
		}

		protected override void PostMapiInternalDispose()
		{
			if (this.webServiceConnection != null)
			{
				this.webServiceConnection.Dispose();
				this.webServiceConnection = null;
			}
			if (this.callTimeoutTimer != null)
			{
				this.callTimeoutTimer.Dispose();
				this.callTimeoutTimer = null;
			}
			base.PostMapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExRpcConnection>(this);
		}

		internal void RemoveStoreReference(MapiStore mapiStore)
		{
			bool flag = false;
			lock (this)
			{
				this.mapiStoreList.Remove(mapiStore);
				flag = (this.mapiStoreList.Count == 0);
			}
			if (flag)
			{
				this.Dispose();
			}
		}

		internal void Lock()
		{
			lock (this)
			{
				if (this.threadLockCount > 0U)
				{
					if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
					{
						StackTrace owningThreadStack = this.GetOwningThreadStack();
						throw MapiExceptionHelper.ObjectReenteredException(string.Concat(new object[]
						{
							"ExRpcConnection (MapiStore) object is already being used by thread ",
							this.owningThread.ManagedThreadId,
							".",
							(owningThreadStack != null) ? ("\nCall stack of the thread using the connection:\n" + owningThreadStack.ToString()) : ""
						}));
					}
					this.threadLockCount += 1U;
					if (this.threadLockCount == 0U)
					{
						throw MapiExceptionHelper.ObjectLockCountOverflowException("ExRpcConnection object lock count has overflowed.");
					}
				}
				else
				{
					this.threadLockCount = 1U;
					this.owningThread = Thread.CurrentThread;
					if (this.CrashTimeout > TimeSpan.Zero && this.CrashTimeout <= ExRpcConnection.MaxCallTimeout)
					{
						if (this.callTimeoutTimer == null)
						{
							this.callTimeoutTimer = new Timer(new TimerCallback(this.CrashOnCallTimeout));
						}
						this.callTimeoutTimer.Change((int)this.CrashTimeout.TotalMilliseconds, -1);
					}
				}
				if (this.webServiceConnection != null)
				{
					this.webServiceConnection.LastException = null;
				}
			}
		}

		internal void Unlock()
		{
			lock (this)
			{
				if (this.threadLockCount <= 0U)
				{
					throw MapiExceptionHelper.ObjectNotLockedException("MapiStore object is being unlocked, but not currently locked.");
				}
				if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
				{
					StackTrace owningThreadStack = this.GetOwningThreadStack();
					throw MapiExceptionHelper.ObjectNotLockedException(string.Concat(new object[]
					{
						"ExRpcConnection (MapiStore) object is being unlocked, but currently locked by thread ",
						this.owningThread.ManagedThreadId,
						".",
						(owningThreadStack != null) ? ("\nCall stack of the thread using the connection:\n" + owningThreadStack.ToString()) : ""
					}));
				}
				this.threadLockCount -= 1U;
				if (this.webServiceConnection != null)
				{
					this.webServiceConnection.LastException = null;
				}
				if (this.threadLockCount == 0U)
				{
					if (this.callTimeoutTimer != null)
					{
						this.callTimeoutTimer.Change(-1, -1);
					}
					if (ComponentTrace<MapiNetTags>.CheckEnabled(70) && this.RpcSentToServer)
					{
						StackTrace owningThreadStack2 = this.GetOwningThreadStack();
						ComponentTrace<MapiNetTags>.Trace<string>(31442, 70, (long)this.GetHashCode(), "RPC went to server\r\n{0}", owningThreadStack2.ToString());
					}
					this.owningThread = null;
				}
			}
		}

		internal MapiStore OpenMsgStore(OpenStoreFlag storeFlags, string mailboxDn, Guid mailboxGuid, Guid mdbGuid, out string correctServerDn, ClientIdentityInfo clientIdentityAs, string userDnAs, bool unifiedLogon, string applicationId, byte[] tenantHint, CultureInfo cultureInfo)
		{
			IExMapiStore exMapiStore = null;
			MapiStore mapiStore = null;
			int num = 0;
			if ((storeFlags & (OpenStoreFlag)((ulong)-2147483648)) != (OpenStoreFlag)((ulong)-2147483648))
			{
				num = 1;
			}
			correctServerDn = null;
			this.Lock();
			bool flag = false;
			MapiStore result;
			try
			{
				lock (this)
				{
					if (this.mapiStoreList.Count >= 256)
					{
						throw MapiExceptionHelper.ExceededMapiStoreLimitException("Can only have 256 MapiStore objects open on same connection.");
					}
				}
				int ulLcidString;
				int ulLcidSort;
				int ulCpid;
				MapiCultureInfo.RetrieveConnectParameters(cultureInfo, out ulLcidString, out ulLcidSort, out ulCpid);
				int num2;
				if (clientIdentityAs != null && IntPtr.Zero != clientIdentityAs.hAuthZ)
				{
					byte[] array = new byte[clientIdentityAs.sidUser.BinaryLength];
					byte[] array2 = new byte[clientIdentityAs.sidPrimaryGroup.BinaryLength];
					clientIdentityAs.sidUser.GetBinaryForm(array, 0);
					clientIdentityAs.sidPrimaryGroup.GetBinaryForm(array2, 0);
					num2 = this.iExRpcConnection.OpenMsgStore(num | 2, (long)storeFlags, mailboxDn, (mailboxGuid.CompareTo(Guid.Empty) == 0) ? null : mailboxGuid.ToByteArray(), (mdbGuid.CompareTo(Guid.Empty) == 0) ? null : mdbGuid.ToByteArray(), out correctServerDn, clientIdentityAs.hAuthZ, array, array2, userDnAs, ulLcidString, ulLcidSort, ulCpid, unifiedLogon, applicationId, tenantHint, (tenantHint != null) ? tenantHint.Length : 0, out exMapiStore);
				}
				else
				{
					num2 = this.iExRpcConnection.OpenMsgStore(num, (long)storeFlags, mailboxDn, (mailboxGuid.CompareTo(Guid.Empty) == 0) ? null : mailboxGuid.ToByteArray(), (mdbGuid.CompareTo(Guid.Empty) == 0) ? null : mdbGuid.ToByteArray(), out correctServerDn, (clientIdentityAs != null) ? clientIdentityAs.hToken : ((IntPtr)null), null, null, userDnAs, ulLcidString, ulLcidSort, ulCpid, unifiedLogon, applicationId, tenantHint, (tenantHint != null) ? tenantHint.Length : 0, out exMapiStore);
				}
				if (num2 < 0)
				{
					try
					{
						MapiExceptionHelper.ThrowIfError("Unable to open message store.", num2, (SafeExInterfaceHandle)this.iExRpcConnection, this.InternalLowLevelException);
					}
					catch (MapiExceptionWrongServer)
					{
					}
				}
				if (exMapiStore == null || exMapiStore.IsInvalid)
				{
					result = null;
				}
				else
				{
					mapiStore = new MapiStore(exMapiStore, null, this, applicationId);
					exMapiStore = null;
					lock (this)
					{
						if (this.mapiStoreList.Count >= 256)
						{
							throw MapiExceptionHelper.ExceededMapiStoreLimitException("Can only have 256 MapiStore objects open on same connection.");
						}
						this.mapiStoreList.Add(mapiStore);
					}
					flag = true;
					result = mapiStore;
				}
			}
			finally
			{
				exMapiStore.DisposeIfValid();
				if (!flag && mapiStore != null)
				{
					mapiStore.Dispose();
				}
				this.Unlock();
			}
			return result;
		}

		internal void SendAuxBuffer(int ulFlags, byte[] auxBuffer, bool forceSend)
		{
			int num = this.iExRpcConnection.SendAuxBuffer(ulFlags, auxBuffer.Length, auxBuffer, forceSend ? 1 : 0);
			if (num != 0)
			{
				base.ThrowIfError("Unable to set/send auxillary buffer on connection.", num);
			}
		}

		internal void FlushRPCBuffer(bool forceSend)
		{
			int num = this.iExRpcConnection.FlushRPCBuffer(forceSend);
			if (num != 0)
			{
				base.ThrowIfError("Unable to flush ROP buffer on connection.", num);
			}
		}

		internal bool IsDead
		{
			get
			{
				bool result = false;
				if (!base.IsDisposed && this.iExRpcConnection != null)
				{
					int num = this.iExRpcConnection.IsDead(out result);
					if (num != 0)
					{
						base.ThrowIfError("Unable to get connection dead state.", num);
					}
				}
				else
				{
					result = true;
				}
				return result;
			}
		}

		internal bool IsMapiMT
		{
			get
			{
				bool result;
				int num = this.iExRpcConnection.IsMapiMT(out result);
				if (num != 0)
				{
					base.ThrowIfError("Unable to find if the connection is using MapiMT interface.", num);
				}
				return result;
			}
		}

		internal static void SetForceMapiRpc(bool forceMapiRpc)
		{
			NativeMethods.SetForceMapiRpc(forceMapiRpc);
		}

		internal bool IsConnectedToMapiServer
		{
			get
			{
				bool result;
				int num = this.iExRpcConnection.IsConnectedToMapiServer(out result);
				if (num != 0)
				{
					base.ThrowIfError("Unable to get if the connection is accessing MapiServer or Store.", num);
				}
				return result;
			}
		}

		internal bool RpcSentToServer
		{
			get
			{
				bool result = false;
				if (!base.IsDisposed && this.iExRpcConnection != null)
				{
					int num = this.iExRpcConnection.RpcSentToServer(out result);
					if (num != 0)
					{
						base.ThrowIfError("Unable to determine if we should trace the RPC stack.", num);
					}
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		private StackTrace GetOwningThreadStack()
		{
			StackTrace result = null;
			lock (this)
			{
				if (this.owningThread != null)
				{
					try
					{
						if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
						{
							this.owningThread.Suspend();
						}
						try
						{
							result = new StackTrace(this.owningThread, true);
						}
						finally
						{
							if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
							{
								this.owningThread.Resume();
							}
						}
					}
					catch
					{
					}
				}
			}
			return result;
		}

		private void CrashOnCallTimeout(object state)
		{
			StackTrace owningThreadStack = this.GetOwningThreadStack();
			Exception exception = new TimeoutException(string.Format("MAPI call timed out after {0}, thread {1}, stack {2}", this.CrashTimeout, (this.owningThread != null) ? this.owningThread.ManagedThreadId : -1, (owningThreadStack != null) ? owningThreadStack.ToString() : ""));
			if (this.owningThread != null)
			{
				this.owningThread.Abort();
				this.owningThread.Join(TimeSpan.FromMinutes(2.0));
			}
			ExWatson.SendReportAndCrashOnAnotherThread(exception);
		}

		private static readonly TimeSpan MaxCallTimeout = TimeSpan.FromDays(1.0);

		private IExRpcConnectionInterface iExRpcConnection;

		private int versionMajor;

		private int versionMinor;

		private int buildMajor;

		private int buildMinor;

		private List<MapiStore> mapiStoreList;

		private uint threadLockCount;

		private Thread owningThread;

		private DateTime creationTime;

		private ApartmentState apartmentState;

		private int creationThreadId;

		private WebServiceConnection webServiceConnection;

		private Timer callTimeoutTimer;
	}
}
