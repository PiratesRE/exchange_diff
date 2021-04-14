using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeServer;
using Microsoft.Exchange.Rpc.PoolRpc;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.MapiDisp;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal class ProxySessionManager : IPoolSessionManager
	{
		public ProxySessionManager(IRpcInstanceManager manager)
		{
			this.sessionMap = PersistentAvlTree<uint, ProxySession>.Empty;
			this.manager = manager;
			this.manager.NotificationsReceived += this.OnNotificationsReceived;
			this.manager.RpcInstanceClosed += this.OnRpcInstanceClosed;
			this.periodicWaitExpirationTimer = new Timer(new TimerCallback(this.PeriodicExpireNotificationWait), null, ProxySessionManager.WaitExpirationTimerDelay, ProxySessionManager.WaitExpirationTimerDelay);
		}

		private static byte[] DefaultConnectAuxiliaryBuffer
		{
			get
			{
				if (ProxySessionManager.defaultConnectAuxiliaryBuffer == null)
				{
					ProxySessionManager.defaultConnectAuxiliaryBuffer = ProxySessionManager.ComputeDefaultConnectAuxiliaryOutBuffer();
				}
				return ProxySessionManager.defaultConnectAuxiliaryBuffer;
			}
		}

		public ErrorCode CreateProxySession(ClientSecurityContext callerSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, byte[] auxiliaryIn, Action<ErrorCode, uint> notificationPendingCallback, out uint sessionHandle, out byte[] auxiliaryOut)
		{
			sessionHandle = 0U;
			auxiliaryOut = ProxySessionManager.DefaultConnectAuxiliaryBuffer;
			using (LockManager.Lock(this.syncRoot))
			{
				PersistentAvlTree<uint, ProxySession> persistentAvlTree = this.GetSessionMap();
				if (persistentAvlTree == null)
				{
					return ErrorCode.CreateMdbNotInitialized((LID)47992U);
				}
				CreateSessionInfo createInfo = new CreateSessionInfo
				{
					Flags = flags,
					UserDn = userDn,
					ConnectionMode = connectionMode,
					CodePageId = codePageId,
					LocaleIdString = localeIdString,
					LocaleIdSort = localeIdSort,
					ClientVersion = clientVersion,
					AuxiliaryIn = auxiliaryIn,
					NotificationPendingCallback = notificationPendingCallback
				};
				ProxySession proxySession = new ProxySession(this.manager, AuthenticationContextFactory.CreateSerialization(string.Empty, callerSecurityContext), this.NextSessionId(), createInfo);
				persistentAvlTree = persistentAvlTree.Add(proxySession.ProxySessionHandle, proxySession);
				Interlocked.Exchange<PersistentAvlTree<uint, ProxySession>>(ref this.sessionMap, persistentAvlTree);
				sessionHandle = proxySession.ProxySessionHandle;
			}
			return ErrorCode.NoError;
		}

		public ErrorCode BeginPoolDoRpc(ref uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, DoRpcCompleteCallback callback, Action<RpcException> exceptionCallback)
		{
			PersistentAvlTree<uint, ProxySession> persistentAvlTree = this.GetSessionMap();
			ProxySession proxySession = null;
			if (persistentAvlTree == null || !persistentAvlTree.TryGetValue(sessionHandle, out proxySession))
			{
				ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "Request rejected: session handle was not found on the session manager.");
				sessionHandle = 0U;
				return ErrorCode.CreateMdbNotInitialized((LID)64376U);
			}
			return proxySession.QueueDoRpcRequest(flags, maximumResponseSize, request, auxiliaryIn, callback, exceptionCallback);
		}

		public ErrorCode QueueNotificationWait(ref uint sessionHandle, IProxyAsyncWaitCompletion completion)
		{
			PersistentAvlTree<uint, ProxySession> persistentAvlTree = this.GetSessionMap();
			ProxySession proxySession = null;
			if (persistentAvlTree == null || !persistentAvlTree.TryGetValue(sessionHandle, out proxySession))
			{
				ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "Notification rejected: session handle was not found on the session manager.");
				sessionHandle = 0U;
				return ErrorCode.CreateMdbNotInitialized((LID)33912U);
			}
			proxySession.QueueNotificationWait(completion);
			return ErrorCode.NoError;
		}

		public void CloseSession(uint sessionHandle)
		{
			ProxySession proxySession = null;
			try
			{
				using (LockManager.Lock(this.syncRoot))
				{
					PersistentAvlTree<uint, ProxySession> persistentAvlTree = this.GetSessionMap();
					if (persistentAvlTree != null && persistentAvlTree.TryGetValue(sessionHandle, out proxySession))
					{
						persistentAvlTree = persistentAvlTree.Remove(sessionHandle);
						Interlocked.Exchange<PersistentAvlTree<uint, ProxySession>>(ref this.sessionMap, persistentAvlTree);
					}
				}
			}
			finally
			{
				if (proxySession != null)
				{
					proxySession.RequestClose(null);
				}
			}
		}

		public void StopAcceptingClientRequests()
		{
			PersistentAvlTree<uint, ProxySession> persistentAvlTree = null;
			Timer timer = null;
			using (LockManager.Lock(this.syncRoot))
			{
				timer = this.periodicWaitExpirationTimer;
				this.periodicWaitExpirationTimer = null;
				persistentAvlTree = this.GetSessionMap();
				Interlocked.Exchange<PersistentAvlTree<uint, ProxySession>>(ref this.sessionMap, null);
			}
			try
			{
				if (persistentAvlTree != null)
				{
					foreach (ProxySession proxySession in persistentAvlTree.GetValuesLmr())
					{
						proxySession.RequestClose(null);
					}
				}
			}
			finally
			{
				if (timer != null)
				{
					timer.Dispose();
				}
			}
		}

		private static byte[] ComputeDefaultConnectAuxiliaryOutBuffer()
		{
			byte[] array = MapiRpc.DefaultConnectAuxiliaryBuffer;
			if (array == null || array.Length == 0)
			{
				return null;
			}
			byte[] array2 = new byte[array.Length + 20];
			uint num = 0U;
			PoolRpcServerCommonBase.PackBuffer(new ArraySegment<byte>(array), new ArraySegment<byte>(array2), false, false, out num);
			if (num > 0U)
			{
				byte[] array3 = new byte[num];
				Buffer.BlockCopy(array2, 0, array3, 0, (int)num);
				return array3;
			}
			return null;
		}

		private PersistentAvlTree<uint, ProxySession> GetSessionMap()
		{
			return Interlocked.CompareExchange<PersistentAvlTree<uint, ProxySession>>(ref this.sessionMap, null, null);
		}

		private uint NextSessionId()
		{
			uint num;
			do
			{
				num = this.nextSessionId;
				this.nextSessionId += 1U;
				if (this.nextSessionId == 1073741824U)
				{
					this.nextSessionId = 1U;
				}
			}
			while (this.sessionMap.Contains(num));
			return num;
		}

		private void PeriodicExpireNotificationWait(object state)
		{
			PersistentAvlTree<uint, ProxySession> persistentAvlTree = this.GetSessionMap();
			if (persistentAvlTree == null)
			{
				return;
			}
			foreach (ProxySession proxySession in persistentAvlTree.GetValuesLmr())
			{
				proxySession.ExpireNotificationWait(DateTime.UtcNow);
			}
		}

		private void OnNotificationsReceived(Guid instanceId, int generation, ErrorCode errorCode, uint[] sessions)
		{
			PersistentAvlTree<uint, ProxySession> persistentAvlTree = this.GetSessionMap();
			if (persistentAvlTree == null)
			{
				return;
			}
			foreach (ProxySession proxySession in persistentAvlTree.GetValuesLmr())
			{
				proxySession.OnNotificationReceived(instanceId, generation, errorCode, sessions);
			}
		}

		private void OnRpcInstanceClosed(Guid instanceId, int generation)
		{
			PersistentAvlTree<uint, ProxySession> persistentAvlTree = this.GetSessionMap();
			if (persistentAvlTree == null)
			{
				return;
			}
			foreach (ProxySession proxySession in persistentAvlTree.GetValuesLmr())
			{
				if (proxySession.IsBoundToRpcInstance(instanceId, generation))
				{
					this.CloseSession(proxySession.ProxySessionHandle);
				}
			}
		}

		public const int MaxSessionId = 1073741824;

		private static readonly TimeSpan WaitExpirationTimerDelay = TimeSpan.FromSeconds(10.0);

		private static byte[] defaultConnectAuxiliaryBuffer;

		private PersistentAvlTree<uint, ProxySession> sessionMap;

		private object syncRoot = new object();

		private IRpcInstanceManager manager;

		private uint nextSessionId = 1U;

		private Timer periodicWaitExpirationTimer;
	}
}
