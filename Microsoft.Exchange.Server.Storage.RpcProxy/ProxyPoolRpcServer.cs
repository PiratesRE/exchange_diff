using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.PoolRpc;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.MapiDisp;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal class ProxyPoolRpcServer : IPoolRpcServer
	{
		internal ProxyPoolRpcServer(IPoolSessionManager manager)
		{
			this.nextContextHandle = 1;
			this.poolContextMap = ProxyPoolRpcServer.ProxyPoolContextMap.Empty;
			this.manager = manager;
			this.periodicWaitExpirationTimer = new Timer(new TimerCallback(this.PeriodicExpireNotificationWait), null, ProxyPoolRpcServer.WaitExpirationTimerDelay, ProxyPoolRpcServer.WaitExpirationTimerDelay);
		}

		public int EcPoolConnect(uint flags, Guid poolGuid, ArraySegment<byte> auxiliaryIn, IPoolConnectCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			IntPtr contextHandle = IntPtr.Zero;
			ProxyPoolRpcServer.ProxyContextHandlePool proxyContextHandlePool = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ProxyPoolRpcServer.TraceStartRpcMarker("EcPoolConnect", IntPtr.Zero);
			}
			int result;
			try
			{
				if ((flags & 1U) != 0U)
				{
					errorCode = ErrorCode.CreateMdbNotInitialized((LID)52208U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[MdbNotInitialized]. E15 client connecting to proxy endpoint.");
					result = 0;
				}
				else
				{
					using (LockManager.Lock(this))
					{
						ProxyPoolRpcServer.ProxyPoolContextMap contextMap = this.GetContextMap();
						if (contextMap == null)
						{
							errorCode = ErrorCode.CreateRpcServerUnavailable((LID)61816U);
							ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[RpcServerUnavailable]. Server shutting down.");
							return 0;
						}
						ProxyPoolRpcServer.ProxyPoolContextMap value;
						if (poolGuid == Guid.Empty)
						{
							poolGuid = Guid.NewGuid();
							contextHandle = new IntPtr(this.GetNextContextHandle(contextMap));
							proxyContextHandlePool = new ProxyPoolRpcServer.ProxyContextHandlePool(poolGuid, this.manager);
							value = ProxyPoolRpcServer.ProxyPoolContextMap.Create(contextMap.ContextHandles.Add(contextHandle.ToInt32(), proxyContextHandlePool), contextMap.Pools.Add(poolGuid, proxyContextHandlePool));
						}
						else
						{
							if (!contextMap.Pools.TryGetValue(poolGuid, out proxyContextHandlePool))
							{
								errorCode = ErrorCode.CreateInvalidPool((LID)45432U);
								ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, string.Format("CALL FAILED: error:[InvalidPool]. Invalid pool guid '{0}'.", poolGuid.ToString()));
								return 0;
							}
							if (proxyContextHandlePool.ContextHandleCount >= 64)
							{
								errorCode = ErrorCode.CreateMaxPoolExceeded((LID)51576U);
								ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[MaxPoolExceeded]. Max handle count exceeded");
								return 0;
							}
							contextHandle = new IntPtr(this.GetNextContextHandle(contextMap));
							value = ProxyPoolRpcServer.ProxyPoolContextMap.Create(contextMap.ContextHandles.Add(contextHandle.ToInt32(), proxyContextHandlePool), contextMap.Pools);
						}
						proxyContextHandlePool.ContextHandleCreated();
						flag2 = true;
						Interlocked.Exchange<ProxyPoolRpcServer.ProxyPoolContextMap>(ref this.poolContextMap, value);
						flag3 = true;
						if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
						{
							ProxyPoolRpcServer.TracePoolDetails(proxyContextHandlePool);
						}
					}
					completion.CompleteAsyncCall(contextHandle, 0U, 64U, poolGuid, RpcServerBase.EmptyArraySegment);
					flag = true;
					RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
					result = 0;
				}
			}
			finally
			{
				if (!flag)
				{
					if (flag3)
					{
						ErrorCode.CreateWithLid((LID)49528U, (ErrorCodeValue)this.EcPoolDisconnect(contextHandle));
					}
					else if (flag2)
					{
						proxyContextHandlePool.ContextHandleClosed();
					}
				}
				if (ErrorCode.NoError != errorCode)
				{
					completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
				}
				if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ProxyPoolRpcServer.TraceEndRpcMarker("EcPoolConnect", contextHandle, errorCode);
				}
			}
			return result;
		}

		public int EcPoolDisconnect(IntPtr contextHandle)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			ProxyPoolRpcServer.ProxyContextHandlePool proxyContextHandlePool = null;
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ProxyPoolRpcServer.TraceStartRpcMarker("EcPoolDisconnect", contextHandle);
			}
			int result;
			try
			{
				using (LockManager.Lock(this))
				{
					ProxyPoolRpcServer.ProxyPoolContextMap contextMap = this.GetContextMap();
					if (contextMap == null)
					{
						errorCode = ErrorCode.CreateRpcServerUnavailable((LID)37240U);
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[RpcServerUnavailable]. Server shutting down.");
						return (int)errorCode;
					}
					if (!contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out proxyContextHandlePool))
					{
						errorCode = ErrorCode.CreateInvalidParameter((LID)53624U);
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, string.Format("CALL FAILED: Invalid context handle '{0}'.", contextHandle.ToInt32()));
						return (int)errorCode;
					}
					bool flag = false;
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						ProxyPoolRpcServer.TracePoolDetails(proxyContextHandlePool);
					}
					proxyContextHandlePool.ContextHandleClosed();
					try
					{
						ProxyPoolRpcServer.ProxyPoolContextMap value;
						if (proxyContextHandlePool.CanClose)
						{
							value = ProxyPoolRpcServer.ProxyPoolContextMap.Create(contextMap.ContextHandles.Remove(contextHandle.ToInt32()), contextMap.Pools.Remove(proxyContextHandlePool.PoolId));
						}
						else
						{
							value = ProxyPoolRpcServer.ProxyPoolContextMap.Create(contextMap.ContextHandles.Remove(contextHandle.ToInt32()), contextMap.Pools);
						}
						Interlocked.Exchange<ProxyPoolRpcServer.ProxyPoolContextMap>(ref this.poolContextMap, value);
						flag = true;
					}
					finally
					{
						if (!flag)
						{
							proxyContextHandlePool.ContextHandleCreated();
						}
					}
				}
				result = (int)ErrorCode.NoError;
			}
			finally
			{
				if (proxyContextHandlePool != null && proxyContextHandlePool.CanClose)
				{
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, string.Format("Proxy pool '{0}' is closing.", proxyContextHandlePool.PoolId.ToString()));
					proxyContextHandlePool.Close();
				}
				if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ProxyPoolRpcServer.TraceEndRpcMarker("EcPoolDisconnect", contextHandle, ErrorCode.NoError);
				}
			}
			return result;
		}

		public int EcPoolCreateSession(IntPtr contextHandle, ClientSecurityContext callerSecurityContext, byte[] sessionSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, ArraySegment<byte> auxiliaryIn, IPoolCreateSessionCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ProxyPoolRpcServer.TraceStartRpcMarker("EcPoolCreateSession", contextHandle);
			}
			int result;
			try
			{
				if (sessionSecurityContext != null)
				{
					errorCode = ErrorCode.CreateNotSupported((LID)33584U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[RpcServerUnavailable]. Server shutting down.");
					result = 0;
				}
				else
				{
					ProxyPoolRpcServer.ProxyPoolContextMap contextMap = this.GetContextMap();
					ProxyPoolRpcServer.ProxyContextHandlePool proxyContextHandlePool;
					if (contextMap == null)
					{
						errorCode = ErrorCode.CreateRpcServerUnavailable((LID)50552U);
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[RpcServerUnavailable]. Server shutting down.");
						result = 0;
					}
					else if (!contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out proxyContextHandlePool))
					{
						errorCode = ErrorCode.CreateInvalidParameter((LID)47480U);
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, string.Format("CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32()));
						result = 0;
					}
					else
					{
						if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
						{
							ProxyPoolRpcServer.TracePoolDetails(proxyContextHandlePool);
						}
						uint num = 0U;
						try
						{
							byte[] array;
							errorCode = this.manager.CreateProxySession(callerSecurityContext, flags, userDn, connectionMode, codePageId, localeIdString, localeIdSort, new short[]
							{
								clientVersion[0],
								clientVersion[2],
								clientVersion[3]
							}, ProxyPoolRpcServer.CloneArraySegment(auxiliaryIn), new Action<ErrorCode, uint>(proxyContextHandlePool.NotificationPending), out num, out array);
							if (errorCode != ErrorCode.NoError)
							{
								return 0;
							}
							errorCode = proxyContextHandlePool.SessionCreated(num);
							if (errorCode != ErrorCode.NoError)
							{
								return 0;
							}
							if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder = new StringBuilder(80);
								stringBuilder.Append("New session created. Proxy pool:[");
								stringBuilder.Append(proxyContextHandlePool.PoolId.ToString());
								stringBuilder.Append("] session:[");
								stringBuilder.Append(num);
								stringBuilder.Append("]");
								ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
							}
							completion.CompleteAsyncCall(num, "ProxySession", (uint)MapiRpc.PollsMaxDefault.TotalMilliseconds, 60U, (uint)MapiRpc.RetryDelayDefault.TotalMilliseconds, 0, new ArraySegment<byte>(array ?? Array<byte>.Empty));
							num = 0U;
						}
						finally
						{
							if (num != 0U)
							{
								this.manager.CloseSession(num);
							}
						}
						RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
						result = (int)ErrorCode.NoError;
					}
				}
			}
			finally
			{
				if (errorCode != ErrorCode.NoError)
				{
					completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
				}
				if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ProxyPoolRpcServer.TraceEndRpcMarker("EcPoolCreateSession", contextHandle, errorCode);
				}
			}
			return result;
		}

		public int EcPoolCloseSession(IntPtr contextHandle, uint sessionHandle, IPoolCloseSessionCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ProxyPoolRpcServer.TraceStartRpcMarker("EcPoolCloseSession", contextHandle);
			}
			int result;
			try
			{
				ProxyPoolRpcServer.ProxyPoolContextMap contextMap = this.GetContextMap();
				ProxyPoolRpcServer.ProxyContextHandlePool proxyContextHandlePool;
				if (contextMap == null)
				{
					errorCode = ErrorCode.CreateRpcServerUnavailable((LID)58744U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[RpcServerUnavailable]. Server shutting down.");
					result = 0;
				}
				else if (!contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out proxyContextHandlePool))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)34168U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, string.Format("CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32()));
					result = 0;
				}
				else
				{
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						ProxyPoolRpcServer.TracePoolDetails(proxyContextHandlePool);
					}
					errorCode = proxyContextHandlePool.ValidateSessionHandle(sessionHandle);
					if (errorCode != ErrorCode.NoError)
					{
						result = 0;
					}
					else
					{
						try
						{
							this.manager.CloseSession(sessionHandle);
						}
						finally
						{
							proxyContextHandlePool.SessionClosed(sessionHandle);
							if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder = new StringBuilder(80);
								stringBuilder.Append("Session closed. Proxy pool:[");
								stringBuilder.Append(proxyContextHandlePool.PoolId.ToString());
								stringBuilder.Append("] session:[");
								stringBuilder.Append(sessionHandle);
								stringBuilder.Append("]");
								ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
							}
							completion.CompleteAsyncCall();
						}
						result = 0;
					}
				}
			}
			finally
			{
				if (errorCode != ErrorCode.NoError)
				{
					completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
				}
				if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ProxyPoolRpcServer.TraceEndRpcMarker("EcPoolCloseSession", contextHandle, errorCode);
				}
			}
			return result;
		}

		public int EcPoolSessionDoRpc(IntPtr contextHandle, uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, IPoolSessionDoRpcCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ProxyPoolRpcServer.TraceStartRpcMarker("EcPoolSessionDoRpc", contextHandle);
			}
			int result2;
			try
			{
				ProxyPoolRpcServer.ProxyPoolContextMap contextMap = this.GetContextMap();
				ProxyPoolRpcServer.ProxyContextHandlePool pool;
				if (contextMap == null)
				{
					errorCode = ErrorCode.CreateRpcServerUnavailable((LID)38264U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[RpcServerUnavailable]. Server shutting down.");
					result2 = 0;
				}
				else if (!contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out pool))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)42360U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, string.Format("CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32()));
					result2 = 0;
				}
				else
				{
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						ProxyPoolRpcServer.TracePoolDetails(pool);
					}
					errorCode = pool.ValidateSessionHandle(sessionHandle);
					if (errorCode != ErrorCode.NoError)
					{
						result2 = 0;
					}
					else
					{
						uint sessionHandle2 = sessionHandle;
						errorCode = this.manager.BeginPoolDoRpc(ref sessionHandle2, flags, maximumResponseSize, request, auxiliaryIn, delegate(ErrorCode result, uint resultFlags, ArraySegment<byte> response, ArraySegment<byte> auxOut)
						{
							try
							{
								if (ErrorCode.NoError == result)
								{
									completion.CompleteAsyncCall(resultFlags, response, auxOut);
								}
								else
								{
									try
									{
										if (ErrorCodeValue.MdbNotInitialized != result)
										{
											if (ErrorCodeValue.Exiting != result)
											{
												goto IL_FC;
											}
										}
										try
										{
											this.manager.CloseSession(sessionHandle);
										}
										finally
										{
											pool.SessionClosed(sessionHandle);
											if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
											{
												StringBuilder stringBuilder2 = new StringBuilder(80);
												stringBuilder2.Append("Session closed by worker. Proxy pool:[");
												stringBuilder2.Append(pool.PoolId.ToString());
												stringBuilder2.Append("] session:[");
												stringBuilder2.Append(sessionHandle);
												stringBuilder2.Append("]");
												ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder2.ToString());
											}
										}
										IL_FC:;
									}
									finally
									{
										completion.FailAsyncCall((int)result, auxOut);
									}
								}
							}
							finally
							{
								if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
								{
									ProxyPoolRpcServer.TraceEndRpcMarker("EcPoolSessionDoRpc", contextHandle, result);
								}
								RpcBufferPool.ReleaseBuffer(request.Array);
								RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
								RpcBufferPool.ReleaseBuffer(response.Array);
								RpcBufferPool.ReleaseBuffer(auxOut.Array);
							}
						}, delegate(RpcException exception)
						{
							try
							{
								ProxyPoolRpcServer.AbortRpcCall(completion, exception, "EcPoolSessionDoRpc", pool, sessionHandle);
							}
							finally
							{
								RpcBufferPool.ReleaseBuffer(request.Array);
								RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
							}
						});
						if (sessionHandle2 == 0U)
						{
							pool.SessionClosed(sessionHandle);
							if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder = new StringBuilder(80);
								stringBuilder.Append("Session closed by server. Proxy pool:[");
								stringBuilder.Append(pool.PoolId.ToString());
								stringBuilder.Append("] session:[");
								stringBuilder.Append(sessionHandle);
								stringBuilder.Append("]");
								ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
							}
						}
						result2 = 0;
					}
				}
			}
			finally
			{
				if (errorCode != ErrorCode.NoError)
				{
					completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
					RpcBufferPool.ReleaseBuffer(request.Array);
					RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
				}
				if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ProxyPoolRpcServer.TraceEndRpcMarker("EcPoolSessionDoRpc", contextHandle, errorCode);
				}
			}
			return result2;
		}

		public int EcPoolWaitForNotificationsAsync(IntPtr contextHandle, IPoolWaitForNotificationsCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ProxyPoolRpcServer.TraceStartRpcMarker("EcPoolWaitForNotificationsAsync", contextHandle);
			}
			int result;
			try
			{
				ProxyPoolRpcServer.ProxyPoolContextMap contextMap = this.GetContextMap();
				ProxyPoolRpcServer.ProxyContextHandlePool proxyContextHandlePool;
				if (contextMap == null)
				{
					errorCode = ErrorCode.CreateRpcServerUnavailable((LID)62840U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "CALL FAILED: error:[Exiting]. Server shutting down.");
					result = 0;
				}
				else if (!contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out proxyContextHandlePool))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)54648U);
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, string.Format("CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32()));
					result = 0;
				}
				else
				{
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						ProxyPoolRpcServer.TracePoolDetails(proxyContextHandlePool);
					}
					errorCode = proxyContextHandlePool.QueueNotificationWait(completion);
					result = 0;
				}
			}
			finally
			{
				if (errorCode != ErrorCode.NoError)
				{
					completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
				}
				if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ProxyPoolRpcServer.TraceEndRpcMarker("EcPoolWaitForNotificationsAsync", contextHandle, errorCode);
				}
			}
			return result;
		}

		public ushort GetVersionDelta()
		{
			return 0;
		}

		public void StopAcceptingClientRequests()
		{
			ProxyPoolRpcServer.ProxyPoolContextMap proxyPoolContextMap = null;
			Timer timer = null;
			try
			{
				if (this.poolContextMap != null)
				{
					using (LockManager.Lock(this))
					{
						timer = this.periodicWaitExpirationTimer;
						this.periodicWaitExpirationTimer = null;
						proxyPoolContextMap = this.GetContextMap();
						Interlocked.Exchange<ProxyPoolRpcServer.ProxyPoolContextMap>(ref this.poolContextMap, null);
						if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, "Proxy shutdown initiated.");
						}
					}
					if (proxyPoolContextMap != null)
					{
						foreach (ProxyPoolRpcServer.ProxyContextHandlePool proxyContextHandlePool in proxyPoolContextMap.Pools.GetValuesLmr())
						{
							if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder = new StringBuilder(60);
								stringBuilder.Append("Shutting down proxy pool:[");
								stringBuilder.Append(proxyContextHandlePool.PoolId.ToString());
								stringBuilder.Append("] handles:[");
								stringBuilder.Append(proxyContextHandlePool.ContextHandleCount);
								stringBuilder.Append("]");
								ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
							}
							proxyContextHandlePool.Close();
						}
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

		private static void TraceStartRpcMarker(string rpcName, IntPtr contextHandle)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("ENTER CALL PROXY [MAPI][");
			stringBuilder.Append(rpcName);
			stringBuilder.Append("] context:[");
			if (contextHandle != IntPtr.Zero)
			{
				stringBuilder.Append(contextHandle.ToInt32());
			}
			else
			{
				stringBuilder.Append("new");
			}
			stringBuilder.Append("]");
			ExTraceGlobals.ProxyMapiTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TraceEndRpcMarker(string rpcName, IntPtr contextHandle, ErrorCode error)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("EXIT CALL PROXY [MAPI][");
			stringBuilder.Append(rpcName);
			stringBuilder.Append("] context:[");
			if (contextHandle != IntPtr.Zero)
			{
				stringBuilder.Append(contextHandle.ToInt32());
			}
			else
			{
				stringBuilder.Append("end");
			}
			if (error != ErrorCode.NoError)
			{
				stringBuilder.Append("] error:[");
				stringBuilder.Append(error);
			}
			stringBuilder.Append("]");
			ExTraceGlobals.ProxyMapiTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TracePoolDetails(ProxyPoolRpcServer.ProxyContextHandlePool pool)
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			stringBuilder.Append("MARK POOL proxy pool:[");
			stringBuilder.Append(pool.PoolId);
			stringBuilder.Append("] handles:[");
			stringBuilder.Append(pool.ContextHandleCount);
			stringBuilder.Append("]");
			ExTraceGlobals.ProxyMapiTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void AbortRpcCall(IRpcAsyncCompletion completion, RpcException exception, string functionName, ProxyPoolRpcServer.ProxyContextHandlePool pool, uint sessionHandle)
		{
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(300);
				stringBuilder.Append("RPC EXCEPTION: ");
				stringBuilder.Append(functionName);
				stringBuilder.Append(": proxy pool:[");
				stringBuilder.Append(pool.PoolId);
				stringBuilder.Append("] session:[");
				stringBuilder.Append(functionName);
				stringBuilder.Append("] Exception: [");
				stringBuilder.Append(exception.ToString());
				stringBuilder.Append("]");
				ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			completion.AbortAsyncCall((uint)exception.ErrorCode);
		}

		private static byte[] CloneArraySegment(ArraySegment<byte> segment)
		{
			if (segment.Count == 0)
			{
				return Array<byte>.Empty;
			}
			byte[] array = new byte[segment.Count];
			Buffer.BlockCopy(segment.Array, segment.Offset, array, 0, segment.Count);
			return array;
		}

		private int GetNextContextHandle(ProxyPoolRpcServer.ProxyPoolContextMap map)
		{
			int num;
			do
			{
				num = this.nextContextHandle;
				this.nextContextHandle++;
				if (this.nextContextHandle == 1879048192)
				{
					this.nextContextHandle = 1;
				}
			}
			while (map.ContextHandles.Contains(num));
			return num;
		}

		private ProxyPoolRpcServer.ProxyPoolContextMap GetContextMap()
		{
			return Interlocked.CompareExchange<ProxyPoolRpcServer.ProxyPoolContextMap>(ref this.poolContextMap, null, null);
		}

		private void PeriodicExpireNotificationWait(object state)
		{
			ProxyPoolRpcServer.ProxyPoolContextMap contextMap = this.GetContextMap();
			if (contextMap == null)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			foreach (ProxyPoolRpcServer.ProxyContextHandlePool proxyContextHandlePool in contextMap.Pools.GetValuesLmr())
			{
				proxyContextHandlePool.ExpireNotificationWait(ErrorCode.NoError, utcNow);
			}
		}

		internal const int MaxContextHandles = 1879048192;

		internal const int MaxHandlesInPool = 64;

		internal const int MaxSessionNotifications = 15360;

		internal static readonly TimeSpan WaitExpirationPeriod = TimeSpan.FromSeconds(60.0);

		private static readonly TimeSpan WaitExpirationTimerDelay = TimeSpan.FromSeconds(10.0);

		private ProxyPoolRpcServer.ProxyPoolContextMap poolContextMap;

		private int nextContextHandle;

		private IPoolSessionManager manager;

		private Timer periodicWaitExpirationTimer;

		internal class ProxyContextHandlePool
		{
			public ProxyContextHandlePool(Guid guid, IPoolSessionManager manager)
			{
				this.manager = manager;
				this.sessionHandleMap = PersistentAvlTree<uint, uint>.Empty;
				this.guid = guid;
				this.contextHandles = 0;
				this.pendingNotificationSet = new HashSet<uint>();
				this.pendingCompletion = null;
				this.completionQueued = false;
				this.waitQueuedTime = null;
			}

			public bool CanClose
			{
				get
				{
					return 0 == this.contextHandles;
				}
			}

			public int ContextHandleCount
			{
				get
				{
					return this.contextHandles;
				}
			}

			public Guid PoolId
			{
				get
				{
					return this.guid;
				}
			}

			public void ContextHandleCreated()
			{
				Interlocked.Increment(ref this.contextHandles);
			}

			public void ContextHandleClosed()
			{
				Interlocked.Decrement(ref this.contextHandles);
			}

			public void Close()
			{
				try
				{
					PersistentAvlTree<uint, uint> persistentAvlTree;
					using (LockManager.Lock(this))
					{
						persistentAvlTree = this.sessionHandleMap;
						this.sessionHandleMap = null;
					}
					foreach (uint sessionHandle in persistentAvlTree.GetValuesLmr())
					{
						this.manager.CloseSession(sessionHandle);
					}
				}
				finally
				{
					this.ExpireNotificationWait(ErrorCode.CreateRpcServerUnavailable((LID)46456U), DateTime.UtcNow);
				}
			}

			public ErrorCode ValidateSessionHandle(uint sessionHandle)
			{
				PersistentAvlTree<uint, uint> persistentAvlTree = Interlocked.CompareExchange<PersistentAvlTree<uint, uint>>(ref this.sessionHandleMap, null, null);
				if (persistentAvlTree == null)
				{
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("CALL FAILED: Pool is being shut down. pool:[");
						stringBuilder.Append(this.guid.ToString());
						stringBuilder.Append("]");
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					return ErrorCode.CreateInvalidPool((LID)36216U);
				}
				uint num;
				if (!persistentAvlTree.TryGetValue(sessionHandle, out num))
				{
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder2 = new StringBuilder(100);
						stringBuilder2.Append("CALL FAILED: error[NoAccess]. Invalid pool session handle. pool:[");
						stringBuilder2.Append(this.guid.ToString());
						stringBuilder2.Append("] session:[");
						stringBuilder2.Append(sessionHandle);
						stringBuilder2.Append("]");
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder2.ToString());
					}
					return ErrorCode.CreateMdbNotInitialized((LID)52600U);
				}
				return ErrorCode.NoError;
			}

			public ErrorCode SessionCreated(uint sessionHandle)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				if (this.sessionHandleMap != null)
				{
					using (LockManager.Lock(this))
					{
						if (this.sessionHandleMap != null)
						{
							this.sessionHandleMap = this.sessionHandleMap.SetValue(sessionHandle, sessionHandle);
						}
						else
						{
							errorCode = ErrorCode.CreateInvalidPool((LID)60792U);
						}
						goto IL_62;
					}
				}
				errorCode = ErrorCode.CreateInvalidPool((LID)49016U);
				IL_62:
				if (errorCode == ErrorCodeValue.InvalidPool && ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("CALL FAILED: Pool is being shut down. pool:[");
					stringBuilder.Append(this.guid.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
				}
				return errorCode;
			}

			public void SessionClosed(uint sessionHandle)
			{
				bool flag = true;
				if (this.sessionHandleMap != null)
				{
					using (LockManager.Lock(this))
					{
						if (this.sessionHandleMap != null)
						{
							flag = false;
							bool flag2;
							this.sessionHandleMap = this.sessionHandleMap.Remove(sessionHandle, out flag2);
						}
					}
				}
				if (flag && ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("Pool session map not changed: pool is being shut down. pool:[");
					stringBuilder.Append(this.guid.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
				}
			}

			public ErrorCode QueueNotificationWait(IPoolWaitForNotificationsCompletion completion)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				uint[] array = null;
				if (this.sessionHandleMap != null)
				{
					using (LockManager.Lock(this))
					{
						if (this.sessionHandleMap != null)
						{
							if (this.pendingCompletion == null)
							{
								if (this.pendingNotificationSet.Count > 0)
								{
									if (this.pendingNotificationSet.Count <= 15360)
									{
										array = new uint[this.pendingNotificationSet.Count];
										this.pendingNotificationSet.CopyTo(array);
										this.pendingNotificationSet.Clear();
									}
									else
									{
										array = new uint[15360];
										this.pendingNotificationSet.CopyTo(array, 0, 15360);
										this.pendingNotificationSet.ExceptWith(array);
									}
								}
								else
								{
									this.waitQueuedTime = new DateTime?(DateTime.UtcNow);
									this.pendingCompletion = completion;
									completion = null;
								}
							}
							else
							{
								if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									StringBuilder stringBuilder = new StringBuilder(100);
									stringBuilder.Append("CALL FAILED: Another notification wait call is already queued on the pool. pool:[");
									stringBuilder.Append(this.guid.ToString());
									stringBuilder.Append("]");
									ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
								}
								errorCode = ErrorCode.CreateRejected((LID)33144U);
							}
						}
						else
						{
							errorCode = ErrorCode.CreateInvalidPool((LID)41336U);
						}
						goto IL_161;
					}
				}
				errorCode = ErrorCode.CreateInvalidPool((LID)57720U);
				IL_161:
				if (errorCode == ErrorCode.NoError)
				{
					if (completion != null)
					{
						completion.CompleteAsyncCall(array);
						if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder2 = new StringBuilder(100);
							stringBuilder2.Append("Notification wait RPC call completed successfully. pool:[");
							stringBuilder2.Append(this.guid.ToString());
							stringBuilder2.Append("]");
							ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder2.ToString());
						}
					}
					else if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder3 = new StringBuilder(100);
						stringBuilder3.Append("Notification wait has been registered successfully. pool:[");
						stringBuilder3.Append(this.guid.ToString());
						stringBuilder3.Append("]");
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder3.ToString());
					}
				}
				else if (errorCode == ErrorCodeValue.InvalidPool && ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder4 = new StringBuilder(100);
					stringBuilder4.Append("CALL FAILED: Pool is being shut down. pool:[");
					stringBuilder4.Append(this.guid.ToString());
					stringBuilder4.Append("]");
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder4.ToString());
				}
				return errorCode;
			}

			public void ExpireNotificationWait(ErrorCode error, DateTime currentTime)
			{
				IPoolWaitForNotificationsCompletion poolWaitForNotificationsCompletion = null;
				uint[] sessionHandles = null;
				using (LockManager.Lock(this))
				{
					if (this.pendingCompletion != null && (error != ErrorCode.NoError || (this.waitQueuedTime != null && this.waitQueuedTime.Value < currentTime && currentTime.Subtract(this.waitQueuedTime.Value) >= ProxyPoolRpcServer.WaitExpirationPeriod)))
					{
						poolWaitForNotificationsCompletion = this.pendingCompletion;
						this.pendingCompletion = null;
						this.waitQueuedTime = null;
					}
				}
				if (poolWaitForNotificationsCompletion != null)
				{
					if (error == ErrorCode.NoError)
					{
						poolWaitForNotificationsCompletion.CompleteAsyncCall(sessionHandles);
					}
					else
					{
						poolWaitForNotificationsCompletion.FailAsyncCall((int)error, RpcServerBase.EmptyArraySegment);
					}
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("Notification wait RPC call expired. pool:[");
						stringBuilder.Append(this.guid.ToString());
						stringBuilder.Append("]");
						if (error != ErrorCode.NoError)
						{
							stringBuilder.Append(" error:[");
							stringBuilder.Append(error.ToString());
							stringBuilder.Append("]");
						}
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
					}
				}
			}

			public void NotificationPending(ErrorCode errorCode, uint sessionHandle)
			{
				bool flag = true;
				if (this.sessionHandleMap != null)
				{
					using (LockManager.Lock(this))
					{
						if (this.sessionHandleMap != null)
						{
							flag = false;
							uint num = 0U;
							if (this.sessionHandleMap.TryGetValue(sessionHandle, out num))
							{
								this.pendingNotificationSet.Add(sessionHandle);
								if (this.pendingCompletion != null && !this.completionQueued)
								{
									this.completionQueued = ThreadPool.QueueUserWorkItem(new WaitCallback(this.CompleteWait), null);
									if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										StringBuilder stringBuilder = new StringBuilder(100);
										stringBuilder.Append("Async RPC completion is queued to thread pool. pool:[");
										stringBuilder.Append(this.guid.ToString());
										stringBuilder.Append("] session:[");
										stringBuilder.Append(sessionHandle);
										stringBuilder.Append("]");
										ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
									}
								}
							}
							else if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder2 = new StringBuilder(100);
								stringBuilder2.Append("CALLBACK IGNORED: Invalid pool session handle. pool:[");
								stringBuilder2.Append(this.guid.ToString());
								stringBuilder2.Append("] session:[");
								stringBuilder2.Append(sessionHandle);
								stringBuilder2.Append("]");
								ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder2.ToString());
							}
						}
					}
				}
				if (flag && ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder3 = new StringBuilder(100);
					stringBuilder3.Append("CALLBACK IGNORED: Pool is closed. pool:[");
					stringBuilder3.Append(this.guid.ToString());
					stringBuilder3.Append("] session:[");
					stringBuilder3.Append(sessionHandle);
					stringBuilder3.Append("]");
					ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder3.ToString());
				}
			}

			private void CompleteWait(object state)
			{
				IPoolWaitForNotificationsCompletion poolWaitForNotificationsCompletion = null;
				uint[] array = null;
				using (LockManager.Lock(this))
				{
					this.completionQueued = false;
					poolWaitForNotificationsCompletion = this.pendingCompletion;
					this.pendingCompletion = null;
					this.waitQueuedTime = null;
					if (poolWaitForNotificationsCompletion != null && this.pendingNotificationSet.Count > 0)
					{
						if (this.pendingNotificationSet.Count <= 15360)
						{
							array = new uint[this.pendingNotificationSet.Count];
							this.pendingNotificationSet.CopyTo(array);
							this.pendingNotificationSet.Clear();
						}
						else
						{
							array = new uint[15360];
							this.pendingNotificationSet.CopyTo(array, 0, 15360);
							this.pendingNotificationSet.ExceptWith(array);
						}
					}
				}
				if (poolWaitForNotificationsCompletion != null)
				{
					poolWaitForNotificationsCompletion.CompleteAsyncCall(array);
					if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("Notification wait RPC call completed successfully. pool:[");
						stringBuilder.Append(this.guid.ToString());
						stringBuilder.Append("] sessions:[");
						stringBuilder.AppendAsString(array);
						stringBuilder.Append("]");
						ExTraceGlobals.ProxyMapiTracer.TraceDebug(0L, stringBuilder.ToString());
					}
				}
			}

			private IPoolSessionManager manager;

			private int contextHandles;

			private Guid guid;

			private HashSet<uint> pendingNotificationSet;

			private IPoolWaitForNotificationsCompletion pendingCompletion;

			private bool completionQueued;

			private DateTime? waitQueuedTime;

			private PersistentAvlTree<uint, uint> sessionHandleMap;
		}

		internal class ProxyPoolContextMap
		{
			private ProxyPoolContextMap()
			{
				this.ContextHandles = PersistentAvlTree<int, ProxyPoolRpcServer.ProxyContextHandlePool>.Empty;
				this.Pools = PersistentAvlTree<Guid, ProxyPoolRpcServer.ProxyContextHandlePool>.Empty;
			}

			private ProxyPoolContextMap(PersistentAvlTree<int, ProxyPoolRpcServer.ProxyContextHandlePool> contextHandles, PersistentAvlTree<Guid, ProxyPoolRpcServer.ProxyContextHandlePool> pools)
			{
				this.ContextHandles = contextHandles;
				this.Pools = pools;
			}

			public static ProxyPoolRpcServer.ProxyPoolContextMap Create(PersistentAvlTree<int, ProxyPoolRpcServer.ProxyContextHandlePool> contextHandles, PersistentAvlTree<Guid, ProxyPoolRpcServer.ProxyContextHandlePool> pools)
			{
				if (contextHandles.Count != 0 || pools.Count != 0)
				{
					return new ProxyPoolRpcServer.ProxyPoolContextMap(contextHandles, pools);
				}
				return ProxyPoolRpcServer.ProxyPoolContextMap.Empty;
			}

			public static readonly ProxyPoolRpcServer.ProxyPoolContextMap Empty = new ProxyPoolRpcServer.ProxyPoolContextMap();

			public readonly PersistentAvlTree<int, ProxyPoolRpcServer.ProxyContextHandlePool> ContextHandles;

			public readonly PersistentAvlTree<Guid, ProxyPoolRpcServer.ProxyContextHandlePool> Pools;
		}
	}
}
