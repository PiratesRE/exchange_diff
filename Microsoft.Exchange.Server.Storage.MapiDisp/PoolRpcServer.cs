using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.PoolRpc;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	public class PoolRpcServer : DisposableBase, IPoolRpcServer
	{
		private PoolRpcServer()
		{
			this.nextContextHandle = 1;
			this.poolContextMap = PoolRpcServer.PoolContextMap.Empty;
			this.periodicWaitExpirationTimer = new Timer(new TimerCallback(this.PeriodicExpireNotificationWait), null, 10000, 10000);
		}

		public static PoolRpcServer Instance
		{
			get
			{
				return PoolRpcServer.instance;
			}
		}

		public static void Initialize()
		{
			PoolRpcServer.instance = new PoolRpcServer();
		}

		public static void InitializeForTestOnly(uint maxRpcThreadCount)
		{
			PoolRpcServer.Initialize();
			PoolRpcServer.rpcThreadCounter = new LimitedCounter(maxRpcThreadCount);
		}

		public static void StopAcceptingCalls()
		{
			if (PoolRpcServer.instance != null)
			{
				PoolRpcServer.instance.StopAcceptingClientRequests();
			}
		}

		public static void Terminate()
		{
			if (PoolRpcServer.instance != null)
			{
				PoolRpcServer.instance.Dispose();
				PoolRpcServer.instance = null;
			}
			PoolRpcServer.rpcThreadCounter = null;
		}

		internal static bool StartInterface(Guid? instanceGuid, uint maxRpcThreadCount, bool isLocalOnly)
		{
			bool flag = false;
			if (PoolRpcServer.rpcThreadCounter == null)
			{
				PoolRpcServer.rpcThreadCounter = new LimitedCounter(maxRpcThreadCount);
			}
			if (PoolRpcServer.rpcEndpoint == null)
			{
				try
				{
					PoolRpcServer.rpcEndpoint = (PoolRpcServer.PoolRpcServerEndpoint)PoolRpcServerCommonBase.RegisterServerInstance(typeof(PoolRpcServer.PoolRpcServerEndpoint), instanceGuid, isLocalOnly, "EmsmdbPool Interface");
					PoolRpcServer.rpcNotifyEndpoint = (PoolRpcServer.PoolNotifyRpcServerEndpoint)PoolRpcServerCommonBase.RegisterAutoListenServerInstance(typeof(PoolRpcServer.PoolNotifyRpcServerEndpoint), instanceGuid, 65536, isLocalOnly, "EmsmdbPoolNotify Interface");
					flag = true;
				}
				catch (DuplicateRpcEndpointException exception)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DuplicatePoolRpcEndpoint, new object[]
					{
						(instanceGuid != null) ? instanceGuid.Value : Guid.Empty,
						DiagnosticsNativeMethods.GetCurrentProcessId().ToString()
					});
				}
				finally
				{
					if (!flag)
					{
						PoolRpcServer.StopInterface();
					}
				}
			}
			return flag;
		}

		internal static void StopInterface()
		{
			if (PoolRpcServer.rpcEndpoint != null)
			{
				RpcServerBase.UnregisterInterface(PoolRpcServerBase.RpcIntfHandle, true);
				PoolRpcServer.rpcEndpoint = null;
			}
			if (PoolRpcServer.rpcNotifyEndpoint != null)
			{
				RpcServerBase.UnregisterInterface(PoolNotifyRpcServerBase.RpcIntfHandle, true);
				PoolRpcServer.rpcNotifyEndpoint = null;
			}
		}

		private static void TraceStartRpcMarker(string rpcName, IntPtr contextHandle)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("ENTER CALL [MAPI][");
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
			ExTraceGlobals.RpcContextPoolTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TraceEndRpcMarker(string rpcName, IntPtr contextHandle, ErrorCode error)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("EXIT CALL [MAPI][");
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
			ExTraceGlobals.RpcContextPoolTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static ErrorCode BuildRopInputBufferList(ArraySegment<byte> requestIn, out List<ArraySegment<byte>> inputList, out List<byte[]> inputListLeasedBuffers)
		{
			inputList = null;
			inputListLeasedBuffers = null;
			List<ArraySegment<byte>> list = null;
			List<byte[]> list2 = null;
			byte[] array = null;
			ArraySegment<byte>? arraySegment = new ArraySegment<byte>?(requestIn);
			int num = 0;
			try
			{
				while (num < 96 && arraySegment != null)
				{
					ArraySegment<byte>? arraySegment2;
					ArraySegment<byte> item = PoolRpcServerCommonBase.UnpackBuffer(arraySegment.Value, out arraySegment2, out array);
					num++;
					if (num == 96 && arraySegment2 != null)
					{
						return ErrorCode.CreateNetworkError((LID)57752U);
					}
					if (array != null)
					{
						if (list2 == null)
						{
							if (arraySegment2 != null)
							{
								list2 = new List<byte[]>(96);
							}
							else
							{
								list2 = new List<byte[]>(1);
							}
						}
						list2.Add(array);
						array = null;
					}
					if (list == null)
					{
						if (arraySegment2 != null)
						{
							list = new List<ArraySegment<byte>>(96);
						}
						else
						{
							list = new List<ArraySegment<byte>>(1);
						}
					}
					list.Add(item);
					arraySegment = arraySegment2;
				}
				inputList = list;
				inputListLeasedBuffers = list2;
				list2 = null;
			}
			finally
			{
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
				if (list2 != null)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						RpcBufferPool.ReleaseBuffer(list2[i]);
					}
				}
			}
			return ErrorCode.NoError;
		}

		private static void TracePoolDetails(PoolRpcServer.ContextHandlePool pool)
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			stringBuilder.Append("MARK POOL pool:[");
			stringBuilder.Append(pool.PoolId);
			stringBuilder.Append("] handles:[");
			stringBuilder.Append(pool.ContextHandleCount);
			stringBuilder.Append("]");
			ExTraceGlobals.RpcContextPoolTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		public virtual int EcPoolConnect(uint flags, Guid poolGuid, ArraySegment<byte> auxiliaryIn, IPoolConnectCompletion completion)
		{
			PoolRpcServer.<>c__DisplayClass1 CS$<>8__locals1 = new PoolRpcServer.<>c__DisplayClass1();
			CS$<>8__locals1.flags = flags;
			CS$<>8__locals1.poolGuid = poolGuid;
			CS$<>8__locals1.auxiliaryIn = auxiliaryIn;
			CS$<>8__locals1.completion = completion;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.executionDiagnostics = new MapiExecutionDiagnostics();
			WatsonOnUnhandledException.Guard(CS$<>8__locals1.executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<EcPoolConnect>b__0)));
			return CS$<>8__locals1.localErrorCode;
		}

		internal int EcPoolConnect_Unwrapped(MapiExecutionDiagnostics executionDiagnostics, uint flags, Guid poolGuid, ArraySegment<byte> auxiliaryIn, IPoolConnectCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			IntPtr contextHandle = IntPtr.Zero;
			PoolRpcServer.PoolContextMap value = null;
			PoolRpcServer.ContextHandlePool contextHandlePool = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(null);
			bool flag4 = false;
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceStartRpcMarker("EcPoolConnect", IntPtr.Zero);
			}
			try
			{
				using (LockManager.Lock(this, executionDiagnostics))
				{
					PoolRpcServer.PoolContextMap contextMap = this.GetContextMap();
					if (contextMap != null)
					{
						if (poolGuid == Guid.Empty)
						{
							poolGuid = Guid.NewGuid();
							contextHandle = new IntPtr(this.GetNextContextHandle(contextMap));
							contextHandlePool = new PoolRpcServer.ContextHandlePool(poolGuid);
							flag4 = true;
							value = PoolRpcServer.PoolContextMap.Create(contextMap.ContextHandles.Add(contextHandle.ToInt32(), contextHandlePool), contextMap.Pools.Add(poolGuid, contextHandlePool));
						}
						else if (contextMap.Pools.TryGetValue(poolGuid, out contextHandlePool))
						{
							if (contextHandlePool.ContextHandleCount < 32)
							{
								contextHandle = new IntPtr(this.GetNextContextHandle(contextMap));
								value = PoolRpcServer.PoolContextMap.Create(contextMap.ContextHandles.Add(contextHandle.ToInt32(), contextHandlePool), contextMap.Pools);
							}
							else
							{
								errorCode = ErrorCode.CreateMaxPoolExceeded((LID)33176U);
								ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "CALL FAILED: error:[MaxPoolExceeded]. Max handle count exceeded");
							}
						}
						else
						{
							errorCode = ErrorCode.CreateInvalidPool((LID)49560U);
							ExTraceGlobals.RpcContextPoolTracer.TraceDebug<string>(0L, "CALL FAILED: error:[InvalidPool]. Invalid pool guid '{0}'.", poolGuid.ToString());
						}
						if (ErrorCode.NoError == errorCode)
						{
							contextHandlePool.ContextHandleCreated();
							flag2 = true;
							if (this.ecPoolConnectTestHook != null)
							{
								this.ecPoolConnectTestHook();
							}
							Interlocked.Exchange<PoolRpcServer.PoolContextMap>(ref this.poolContextMap, value);
							flag3 = true;
							if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
							{
								PoolRpcServer.TracePoolDetails(contextHandlePool);
							}
						}
					}
					else
					{
						errorCode = ErrorCode.CreateRpcServerUnavailable((LID)49048U);
						ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "CALL FAILED: error:[Exiting]. Server shutting down.");
					}
				}
				if (ErrorCode.NoError == errorCode)
				{
					completion.CompleteAsyncCall(contextHandle, 0U, 32U, poolGuid, RpcServerBase.EmptyArraySegment);
					if (databaseInstance != null)
					{
						databaseInstance.ContextHandlePoolHandles.Increment();
						if (flag4)
						{
							databaseInstance.ContextHandlePools.Increment();
						}
					}
					flag = true;
				}
				else
				{
					completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
				}
			}
			finally
			{
				if (!flag)
				{
					if (flag3)
					{
						ErrorCode.CreateWithLid((LID)56472U, (ErrorCodeValue)this.EcPoolDisconnect(contextHandle));
					}
					else if (flag2)
					{
						contextHandlePool.ContextHandleClosed();
					}
				}
				if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					PoolRpcServer.TraceEndRpcMarker("EcPoolConnect", contextHandle, errorCode);
				}
			}
			RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
			return (int)ErrorCode.NoError;
		}

		public virtual int EcPoolDisconnect(IntPtr contextHandle)
		{
			PoolRpcServer.<>c__DisplayClass4 CS$<>8__locals1 = new PoolRpcServer.<>c__DisplayClass4();
			CS$<>8__locals1.contextHandle = contextHandle;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.executionDiagnostics = new MapiExecutionDiagnostics();
			WatsonOnUnhandledException.Guard(CS$<>8__locals1.executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<EcPoolDisconnect>b__3)));
			return CS$<>8__locals1.localErrorCode;
		}

		internal int EcPoolDisconnect_Unwrapped(MapiExecutionDiagnostics executionDiagnostics, IntPtr contextHandle)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			PoolRpcServer.ContextHandlePool contextHandlePool = null;
			StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(null);
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceStartRpcMarker("EcPoolDisconnect", contextHandle);
			}
			using (LockManager.Lock(this, executionDiagnostics))
			{
				PoolRpcServer.PoolContextMap contextMap = this.GetContextMap();
				if (contextMap != null && contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out contextHandlePool))
				{
					bool flag = false;
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						PoolRpcServer.TracePoolDetails(contextHandlePool);
					}
					contextHandlePool.ContextHandleClosed();
					try
					{
						PoolRpcServer.PoolContextMap value;
						if (contextHandlePool.CanClose)
						{
							value = PoolRpcServer.PoolContextMap.Create(contextMap.ContextHandles.Remove(contextHandle.ToInt32()), contextMap.Pools.Remove(contextHandlePool.PoolId));
						}
						else
						{
							value = PoolRpcServer.PoolContextMap.Create(contextMap.ContextHandles.Remove(contextHandle.ToInt32()), contextMap.Pools);
						}
						if (databaseInstance != null)
						{
							databaseInstance.ContextHandlePoolHandles.Decrement();
						}
						if (this.ecPoolDisconnectTestHook != null)
						{
							this.ecPoolDisconnectTestHook();
						}
						Interlocked.Exchange<PoolRpcServer.PoolContextMap>(ref this.poolContextMap, value);
						flag = true;
						goto IL_153;
					}
					finally
					{
						if (!flag)
						{
							contextHandlePool.ContextHandleCreated();
						}
					}
				}
				if (contextMap == null)
				{
					errorCode = ErrorCode.CreateRpcServerUnavailable((LID)65432U);
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "CALL FAILED: error:[Exiting]. Server shutting down.");
				}
				else
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)40856U);
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug<int>(0L, "CALL FAILED: Invalid context handle '{0}'.", contextHandle.ToInt32());
				}
				IL_153:;
			}
			if (contextHandlePool != null && contextHandlePool.CanClose)
			{
				ExTraceGlobals.RpcContextPoolTracer.TraceDebug<string>(0L, "Pool '{0}' is closing.", contextHandlePool.PoolId.ToString());
				contextHandlePool.Close();
				if (databaseInstance != null)
				{
					databaseInstance.ContextHandlePools.Decrement();
				}
			}
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceEndRpcMarker("EcPoolDisconnect", contextHandle, errorCode);
			}
			return (int)errorCode;
		}

		public virtual int EcPoolCreateSession(IntPtr contextHandle, ClientSecurityContext callerSecurityContext, byte[] sessionSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, ArraySegment<byte> auxiliaryIn, IPoolCreateSessionCompletion completion)
		{
			PoolRpcServer.<>c__DisplayClass7 CS$<>8__locals1 = new PoolRpcServer.<>c__DisplayClass7();
			CS$<>8__locals1.contextHandle = contextHandle;
			CS$<>8__locals1.callerSecurityContext = callerSecurityContext;
			CS$<>8__locals1.sessionSecurityContext = sessionSecurityContext;
			CS$<>8__locals1.flags = flags;
			CS$<>8__locals1.userDn = userDn;
			CS$<>8__locals1.connectionMode = connectionMode;
			CS$<>8__locals1.codePageId = codePageId;
			CS$<>8__locals1.localeIdString = localeIdString;
			CS$<>8__locals1.localeIdSort = localeIdSort;
			CS$<>8__locals1.clientVersion = clientVersion;
			CS$<>8__locals1.auxiliaryIn = auxiliaryIn;
			CS$<>8__locals1.completion = completion;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.executionDiagnostics = new MapiExecutionDiagnostics();
			WatsonOnUnhandledException.Guard(CS$<>8__locals1.executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<EcPoolCreateSession>b__6)));
			return CS$<>8__locals1.localErrorCode;
		}

		internal int EcPoolCreateSession_Unwrapped(MapiExecutionDiagnostics executionDiagnostics, IntPtr contextHandle, ClientSecurityContext callerSecurityContext, byte[] sessionSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, ArraySegment<byte> auxiliaryIn, IPoolCreateSessionCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			ArraySegment<byte> auxiliaryOut = RpcServerBase.EmptyArraySegment;
			byte[] array = null;
			byte[] array2 = null;
			try
			{
				if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					PoolRpcServer.TraceStartRpcMarker("EcPoolCreateSession", contextHandle);
				}
				PoolRpcServer.PoolContextMap contextMap = this.GetContextMap();
				PoolRpcServer.ContextHandlePool contextHandlePool;
				if (contextMap == null)
				{
					errorCode = ErrorCode.CreateRpcServerUnavailable((LID)61336U);
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "CALL FAILED: error:[Exiting]. Server shutting down.");
				}
				else if (!contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out contextHandlePool))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)44952U);
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug<int>(0L, "CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32());
				}
				else
				{
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						PoolRpcServer.TracePoolDetails(contextHandlePool);
					}
					IntPtr zero = IntPtr.Zero;
					uint num = 0U;
					ArraySegment<byte> auxIn = RpcServerBase.EmptyArraySegment;
					if (auxiliaryIn.Count > 0)
					{
						if (auxiliaryIn.Count < 8)
						{
							errorCode = ErrorCode.CreateNetworkError((LID)57240U);
							goto IL_298;
						}
						ArraySegment<byte>? arraySegment;
						auxIn = PoolRpcServerCommonBase.UnpackBuffer(auxiliaryIn, out arraySegment, out array);
					}
					byte[] array3 = new byte[1024];
					int num2 = 0;
					TimeSpan timeSpan;
					int retryCount;
					TimeSpan timeSpan2;
					string text;
					string displayName;
					errorCode = ErrorCode.CreateWithLid((LID)52376U, (ErrorCodeValue)MapiRpc.Instance.DoConnect(executionDiagnostics, out zero, userDn, callerSecurityContext, sessionSecurityContext, (int)flags, (int)connectionMode, (int)codePageId, (int)localeIdString, (int)localeIdSort, out timeSpan, out retryCount, out timeSpan2, out text, out displayName, clientVersion, auxIn, ref array3, out num2, new Action<int>(contextHandlePool.NotificationPending)));
					try
					{
						if (num2 > 0)
						{
							uint num3 = 0U;
							array2 = RpcBufferPool.GetBuffer(4104);
							PoolRpcServerCommonBase.PackBuffer(new ArraySegment<byte>(array3, 0, num2), new ArraySegment<byte>(array2), false, false, out num3);
							if (num3 > 0U)
							{
								auxiliaryOut = new ArraySegment<byte>(array2, 0, (int)num3);
							}
						}
						if (!(errorCode != ErrorCode.NoError))
						{
							uint maximumPolls = (uint)timeSpan.TotalMilliseconds;
							uint retryDelay = (uint)timeSpan2.TotalMilliseconds;
							errorCode = contextHandlePool.SessionCreated(zero, out num);
							if (!(errorCode != ErrorCode.NoError))
							{
								completion.CompleteAsyncCall(num, displayName, maximumPolls, (uint)retryCount, retryDelay, 0, auxiliaryOut);
								if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									StringBuilder stringBuilder = new StringBuilder(80);
									stringBuilder.Append("New session created. Pool:[");
									stringBuilder.Append(contextHandlePool.PoolId.ToString());
									stringBuilder.Append("] session:[");
									stringBuilder.Append(zero);
									stringBuilder.Append("]");
									ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
								}
								zero = IntPtr.Zero;
								num = 0U;
							}
						}
					}
					finally
					{
						if (zero != IntPtr.Zero)
						{
							MapiRpc.Instance.DoDisconnect(executionDiagnostics, ref zero);
						}
						if (num != 0U)
						{
							contextHandlePool.SessionClosed(num);
						}
					}
				}
				IL_298:
				if (errorCode != ErrorCode.NoError)
				{
					completion.FailAsyncCall((int)errorCode, auxiliaryOut);
				}
				if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					PoolRpcServer.TraceEndRpcMarker("EcPoolCreateSession", contextHandle, errorCode);
				}
			}
			finally
			{
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
				if (array2 != null)
				{
					RpcBufferPool.ReleaseBuffer(array2);
				}
			}
			RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
			return (int)ErrorCode.NoError;
		}

		public virtual int EcPoolCloseSession(IntPtr contextHandle, uint sessionHandle, IPoolCloseSessionCompletion completion)
		{
			PoolRpcServer.<>c__DisplayClassa CS$<>8__locals1 = new PoolRpcServer.<>c__DisplayClassa();
			CS$<>8__locals1.contextHandle = contextHandle;
			CS$<>8__locals1.sessionHandle = sessionHandle;
			CS$<>8__locals1.completion = completion;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.executionDiagnostics = new MapiExecutionDiagnostics();
			WatsonOnUnhandledException.Guard(CS$<>8__locals1.executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<EcPoolCloseSession>b__9)));
			return CS$<>8__locals1.localErrorCode;
		}

		internal int EcPoolCloseSession_Unwrapped(MapiExecutionDiagnostics executionDiagnostics, IntPtr contextHandle, uint sessionHandle, IPoolCloseSessionCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceStartRpcMarker("EcPoolCloseSession", contextHandle);
			}
			PoolRpcServer.PoolContextMap contextMap = this.GetContextMap();
			if (contextMap != null)
			{
				PoolRpcServer.ContextHandlePool contextHandlePool;
				if (contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out contextHandlePool))
				{
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						PoolRpcServer.TracePoolDetails(contextHandlePool);
					}
					IntPtr zero = IntPtr.Zero;
					errorCode = contextHandlePool.GetSessionHandle(sessionHandle, out zero);
					if (!(errorCode == ErrorCode.NoError))
					{
						goto IL_162;
					}
					try
					{
						errorCode = ErrorCode.CreateWithLid((LID)46232U, (ErrorCodeValue)MapiRpc.Instance.DoDisconnect(executionDiagnostics, ref zero));
						goto IL_162;
					}
					finally
					{
						contextHandlePool.SessionClosed(sessionHandle);
						if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(80);
							stringBuilder.Append("Session closed. Pool:[");
							stringBuilder.Append(contextHandlePool.PoolId.ToString());
							stringBuilder.Append("] session:[");
							stringBuilder.Append(sessionHandle);
							stringBuilder.Append("]");
							ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
						}
					}
				}
				errorCode = ErrorCode.CreateInvalidParameter((LID)36760U);
				ExTraceGlobals.RpcContextPoolTracer.TraceDebug<int>(0L, "CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32());
			}
			else
			{
				errorCode = ErrorCode.CreateRpcServerUnavailable((LID)53144U);
				ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "CALL FAILED: error:[Exiting]. Server shutting down.");
			}
			IL_162:
			if (errorCode == ErrorCode.NoError)
			{
				completion.CompleteAsyncCall();
			}
			else
			{
				completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
			}
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceEndRpcMarker("EcPoolCloseSession", contextHandle, errorCode);
			}
			return (int)ErrorCode.NoError;
		}

		public virtual int EcPoolSessionDoRpc(IntPtr contextHandle, uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, IPoolSessionDoRpcCompletion completion)
		{
			PoolRpcServer.<>c__DisplayClassd CS$<>8__locals1 = new PoolRpcServer.<>c__DisplayClassd();
			CS$<>8__locals1.contextHandle = contextHandle;
			CS$<>8__locals1.sessionHandle = sessionHandle;
			CS$<>8__locals1.flags = flags;
			CS$<>8__locals1.maximumResponseSize = maximumResponseSize;
			CS$<>8__locals1.request = request;
			CS$<>8__locals1.auxiliaryIn = auxiliaryIn;
			CS$<>8__locals1.completion = completion;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			try
			{
				PoolRpcServer.<>c__DisplayClassf CS$<>8__locals2 = new PoolRpcServer.<>c__DisplayClassf();
				CS$<>8__locals2.CS$<>8__localse = CS$<>8__locals1;
				if (PoolRpcServer.rpcThreadCounter != null && PoolRpcServer.rpcThreadCounter.IsIncrementedValueOverLimit())
				{
					ErrorCode errorCode = ErrorCode.CreateRpcServerTooBusy((LID)47708U);
					CS$<>8__locals1.localErrorCode = (int)errorCode;
					return CS$<>8__locals1.localErrorCode;
				}
				CS$<>8__locals2.executionDiagnostics = new MapiExecutionDiagnostics();
				WatsonOnUnhandledException.Guard(CS$<>8__locals2.executionDiagnostics, new TryDelegate(CS$<>8__locals2, (UIntPtr)ldftn(<EcPoolSessionDoRpc>b__c)));
			}
			finally
			{
				if (PoolRpcServer.rpcThreadCounter != null)
				{
					PoolRpcServer.rpcThreadCounter.Decrement();
				}
			}
			return CS$<>8__locals1.localErrorCode;
		}

		internal int EcPoolSessionDoRpc_Unwrapped(MapiExecutionDiagnostics executionDiagnostics, IntPtr contextHandle, uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, IPoolSessionDoRpcCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceStartRpcMarker("EcPoolSessionDoRpc", contextHandle);
			}
			PoolRpcServer.PoolContextMap contextMap = this.GetContextMap();
			if (contextMap != null)
			{
				PoolRpcServer.ContextHandlePool contextHandlePool;
				if (contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out contextHandlePool))
				{
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						PoolRpcServer.TracePoolDetails(contextHandlePool);
					}
					IntPtr zero = IntPtr.Zero;
					errorCode = contextHandlePool.GetSessionHandle(sessionHandle, out zero);
					if (errorCode == ErrorCode.NoError)
					{
						errorCode = this.EcDoRpc(executionDiagnostics, ref zero, flags, maximumResponseSize, request, auxiliaryIn, completion);
						if (zero == IntPtr.Zero)
						{
							contextHandlePool.SessionClosed(sessionHandle);
							if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder = new StringBuilder(80);
								stringBuilder.Append("Session closed by server. Pool:[");
								stringBuilder.Append(contextHandlePool.PoolId.ToString());
								stringBuilder.Append("] session:[");
								stringBuilder.Append(sessionHandle);
								stringBuilder.Append("]");
								ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
							}
						}
					}
				}
				else
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)47000U);
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug<int>(0L, "CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32());
				}
			}
			else
			{
				errorCode = ErrorCode.CreateRpcServerUnavailable((LID)63384U);
				ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "CALL FAILED: error:[Exiting]. Server shutting down.");
			}
			if (errorCode != ErrorCode.NoError)
			{
				completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
			}
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceEndRpcMarker("EcPoolSessionDoRpc", contextHandle, errorCode);
			}
			RpcBufferPool.ReleaseBuffer(request.Array);
			RpcBufferPool.ReleaseBuffer(auxiliaryIn.Array);
			return (int)ErrorCode.NoError;
		}

		public virtual int EcPoolWaitForNotificationsAsync(IntPtr contextHandle, IPoolWaitForNotificationsCompletion completion)
		{
			PoolRpcServer.<>c__DisplayClass12 CS$<>8__locals1 = new PoolRpcServer.<>c__DisplayClass12();
			CS$<>8__locals1.contextHandle = contextHandle;
			CS$<>8__locals1.completion = completion;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.executionDiagnostics = new MapiExecutionDiagnostics();
			WatsonOnUnhandledException.Guard(CS$<>8__locals1.executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<EcPoolWaitForNotificationsAsync>b__11)));
			return CS$<>8__locals1.localErrorCode;
		}

		internal int EcPoolWaitForNotificationsAsync_Unwrapped(MapiExecutionDiagnostics executionDiagnostics, IntPtr contextHandle, IPoolWaitForNotificationsCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceStartRpcMarker("EcPoolWaitForNotificationsAsync", contextHandle);
			}
			PoolRpcServer.PoolContextMap contextMap = this.GetContextMap();
			if (contextMap != null)
			{
				PoolRpcServer.ContextHandlePool contextHandlePool;
				if (contextMap.ContextHandles.TryGetValue(contextHandle.ToInt32(), out contextHandlePool))
				{
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						PoolRpcServer.TracePoolDetails(contextHandlePool);
					}
					errorCode = contextHandlePool.QueueNotificationWait(completion);
				}
				else
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)38808U);
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug<int>(0L, "CALL FAILED: error:[InvalidParameter]. Invalid context handle '{0}'.", contextHandle.ToInt32());
				}
			}
			else
			{
				errorCode = ErrorCode.CreateRpcServerUnavailable((LID)55192U);
				ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "CALL FAILED: error:[Exiting]. Server shutting down.");
			}
			if (errorCode != ErrorCode.NoError)
			{
				completion.FailAsyncCall((int)errorCode, RpcServerBase.EmptyArraySegment);
			}
			if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				PoolRpcServer.TraceEndRpcMarker("EcPoolWaitForNotificationsAsync", contextHandle, errorCode);
			}
			return (int)ErrorCode.NoError;
		}

		public virtual ushort GetVersionDelta()
		{
			return 6000;
		}

		private int GetNextContextHandle(PoolRpcServer.PoolContextMap map)
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

		private PoolRpcServer.PoolContextMap GetContextMap()
		{
			return Interlocked.CompareExchange<PoolRpcServer.PoolContextMap>(ref this.poolContextMap, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PoolRpcServer>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.StopAcceptingClientRequests();
				if (this.periodicWaitExpirationTimer != null)
				{
					this.periodicWaitExpirationTimer.Dispose();
					this.periodicWaitExpirationTimer = null;
				}
			}
		}

		private void StopAcceptingClientRequests()
		{
			PoolRpcServer.PoolContextMap poolContextMap = null;
			if (this.poolContextMap != null)
			{
				using (LockManager.Lock(this))
				{
					poolContextMap = this.GetContextMap();
					Interlocked.Exchange<PoolRpcServer.PoolContextMap>(ref this.poolContextMap, null);
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, "Server shutdown initiated.");
					}
				}
				if (poolContextMap != null)
				{
					foreach (PoolRpcServer.ContextHandlePool contextHandlePool in poolContextMap.Pools.GetValuesLmr())
					{
						if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(60);
							stringBuilder.Append("Shutting down pool:[");
							stringBuilder.Append(contextHandlePool.PoolId.ToString());
							stringBuilder.Append("] handles:[");
							stringBuilder.Append(contextHandlePool.ContextHandleCount);
							stringBuilder.Append("]");
							ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
						}
						contextHandlePool.Close();
					}
					StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(null);
					if (databaseInstance != null)
					{
						databaseInstance.ContextHandlePoolHandles.IncrementBy((long)(-(long)poolContextMap.ContextHandles.Count));
						databaseInstance.ContextHandlePools.IncrementBy((long)(-(long)poolContextMap.Pools.Count));
					}
				}
			}
		}

		private void PeriodicExpireNotificationWait(object state)
		{
			PoolRpcServer.PoolContextMap contextMap = this.GetContextMap();
			if (contextMap == null)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			foreach (PoolRpcServer.ContextHandlePool contextHandlePool in contextMap.Pools.GetValuesLmr())
			{
				contextHandlePool.ExpireNotificationWait(ErrorCode.NoError, utcNow);
			}
		}

		private int SizeSubtractAndCap(uint minuend, uint subtrahend, uint maxResult)
		{
			if (minuend <= subtrahend)
			{
				return 0;
			}
			uint num = minuend - subtrahend;
			if (num > maxResult)
			{
				return (int)maxResult;
			}
			return (int)num;
		}

		private void UpdateHeaderFlags(byte[] response, uint headerIndex, PoolRpcServer.HeaderFlags flagsToSet, PoolRpcServer.HeaderFlags flagsToClear)
		{
			headerIndex += 2U;
			PoolRpcServer.HeaderFlags headerFlags = (PoolRpcServer.HeaderFlags)response[(int)((UIntPtr)headerIndex)];
			headerFlags |= (PoolRpcServer.HeaderFlags)(response[(int)((UIntPtr)(headerIndex + 1U))] << 8);
			headerFlags |= flagsToSet;
			headerFlags &= ~flagsToClear;
			response[(int)((UIntPtr)headerIndex)] = (byte)(headerFlags & (PoolRpcServer.HeaderFlags)255);
			response[(int)((UIntPtr)(headerIndex + 1U))] = (byte)((headerFlags & (PoolRpcServer.HeaderFlags)65280) >> 8);
		}

		private ErrorCode EcDoRpc(MapiExecutionDiagnostics executionDiagnostics, ref IntPtr sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, IPoolSessionDoRpcCompletion completion)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			bool compress = false;
			bool xor = false;
			bool flag = false;
			bool fakeRequest = false;
			uint num = 0U;
			uint count = 0U;
			if (maximumResponseSize <= 8U)
			{
				return ErrorCode.CreateNetworkError((LID)42904U);
			}
			if (maximumResponseSize > 98304U)
			{
				maximumResponseSize = 98304U;
			}
			if (request.Count == 0)
			{
				return ErrorCode.CreateRpcFormat((LID)59288U);
			}
			if (request.Count < 8)
			{
				return ErrorCode.CreateNetworkError((LID)34712U);
			}
			List<ArraySegment<byte>> list = null;
			List<byte[]> list2 = null;
			compress = ((flags & 1U) == 0U);
			xor = ((flags & 2U) == 0U);
			flag = ((flags & 4U) != 0U);
			byte[] array = null;
			byte[] array2 = null;
			byte[] array3 = null;
			try
			{
				ArraySegment<byte> auxIn = RpcServerBase.EmptyArraySegment;
				errorCode = PoolRpcServer.BuildRopInputBufferList(request, out list, out list2);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode;
				}
				if (auxiliaryIn.Count > 0)
				{
					if (auxiliaryIn.Count < 8)
					{
						return ErrorCode.CreateNetworkError((LID)51096U);
					}
					ArraySegment<byte>? arraySegment;
					auxIn = PoolRpcServerCommonBase.UnpackBuffer(auxiliaryIn, out arraySegment, out array);
				}
				array2 = RpcBufferPool.GetBuffer(98304);
				array3 = RpcBufferPool.GetBuffer(4104);
				int num2 = 4096;
				int num3 = 0;
				for (;;)
				{
					byte[] array4 = null;
					byte[] array5 = null;
					byte[] array6 = null;
					uint headerIndex = 0U;
					try
					{
						int num4 = this.SizeSubtractAndCap(maximumResponseSize, num + 8U, 32767U);
						array4 = RpcBufferPool.GetBuffer(num4);
						array5 = RpcBufferPool.GetBuffer(num2);
						int num5;
						int num6;
						errorCode = ErrorCode.CreateWithLid((LID)38040U, (ErrorCodeValue)MapiRpc.Instance.DoRpc(executionDiagnostics, ref sessionHandle, list, new ArraySegment<byte>(array4, 0, num4), out num5, (flags & 16U) != 0U, auxIn, new ArraySegment<byte>(array5, 0, num2), out num6, fakeRequest, out array6));
						if (num6 > 0)
						{
							if (num6 > num2)
							{
								return ErrorCode.CreateNetworkError((LID)64408U);
							}
							count = 0U;
							PoolRpcServerCommonBase.PackBuffer(new ArraySegment<byte>(array5, 0, num6), new ArraySegment<byte>(array3, 0, 4104), compress, xor, out count);
						}
						if (errorCode != ErrorCode.NoError)
						{
							completion.FailAsyncCall((int)errorCode, new ArraySegment<byte>(array3, 0, (int)count));
							return ErrorCode.NoError;
						}
						if (num5 == 0)
						{
							array6 = null;
							if (num > 0U)
							{
								this.UpdateHeaderFlags(array2, headerIndex, PoolRpcServer.HeaderFlags.Last, PoolRpcServer.HeaderFlags.None);
							}
						}
						else
						{
							if (num5 > num4 || num5 > array4.Length)
							{
								return ErrorCode.CreateNetworkError((LID)48024U);
							}
							uint num7 = 0U;
							PoolRpcServerCommonBase.PackBuffer(new ArraySegment<byte>(array4, 0, num5), new ArraySegment<byte>(array2, (int)num, (int)(maximumResponseSize - num)), compress, xor, out num7);
							headerIndex = num;
							num += num7;
						}
						if (!flag || array6 == null)
						{
							break;
						}
						if (num3 == 95)
						{
							break;
						}
						if (maximumResponseSize <= num)
						{
							break;
						}
						RopId ropId = (RopId)array6[2];
						uint num8;
						if (ropId != RopId.QueryRows)
						{
							if (ropId != RopId.FastTransferSourceGetBuffer && ropId != RopId.FastTransferSourceGetBufferExtended)
							{
								num8 = 8192U;
							}
							else
							{
								num8 = 16391U;
							}
						}
						else
						{
							num8 = 32775U;
						}
						if (maximumResponseSize - num < num8)
						{
							break;
						}
						if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(100);
							stringBuilder.Append("Preparing to compute chained response. rop:[");
							stringBuilder.Append(((RopId)array6[2]).ToString());
							stringBuilder.Append("] bufferSize:[");
							stringBuilder.Append(maximumResponseSize - num);
							stringBuilder.Append("]");
							ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
						}
						this.UpdateHeaderFlags(array2, headerIndex, PoolRpcServer.HeaderFlags.None, PoolRpcServer.HeaderFlags.Last);
						list.Clear();
						list.Add(new ArraySegment<byte>(array6));
						auxIn = RpcServerBase.EmptyArraySegment;
						num2 = 0;
						fakeRequest = true;
					}
					finally
					{
						if (array5 != null)
						{
							RpcBufferPool.ReleaseBuffer(array5);
							array5 = null;
						}
						if (array4 != null)
						{
							RpcBufferPool.ReleaseBuffer(array4);
							array4 = null;
						}
					}
					num3++;
				}
				completion.CompleteAsyncCall(flags, new ArraySegment<byte>(array2, 0, (int)num), new ArraySegment<byte>(array3, 0, (int)count));
			}
			finally
			{
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
				if (array2 != null)
				{
					RpcBufferPool.ReleaseBuffer(array2);
				}
				if (array3 != null)
				{
					RpcBufferPool.ReleaseBuffer(array3);
				}
				if (list2 != null)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						RpcBufferPool.ReleaseBuffer(list2[i]);
					}
				}
			}
			return errorCode;
		}

		private const int SizeOfRpcHeaderExt = 8;

		private const int MaximumResponseSize = 98304;

		private const int MaximumBufferSize = 32767;

		private const int MaximumAuxResponseBufferSize = 4104;

		private const int MaximumAuxBufferSize = 4096;

		private const int MaximumChainedBuffers = 96;

		private const uint MinimumChainSize = 8192U;

		private const uint MinimumQueryRowsChainSize = 32775U;

		private const uint MinimumFastTransferChainSize = 16391U;

		private const int WaitExpirationTimerDelay = 10000;

		private const int MaxContextHandles = 1879048192;

		private const int MaxHandlesInPool = 32;

		private const int MaxNotificationCalls = 65536;

		private static readonly TimeSpan WaitExpirationPeriod = TimeSpan.FromSeconds(60.0);

		private static PoolRpcServer instance = null;

		private static PoolRpcServer.PoolRpcServerEndpoint rpcEndpoint = null;

		private static PoolRpcServer.PoolNotifyRpcServerEndpoint rpcNotifyEndpoint = null;

		private static LimitedCounter rpcThreadCounter = null;

		private PoolRpcServer.PoolContextMap poolContextMap;

		private int nextContextHandle;

		private Timer periodicWaitExpirationTimer;

		private Action ecPoolConnectTestHook;

		private Action ecPoolDisconnectTestHook;

		[Flags]
		private enum HeaderFlags : ushort
		{
			None = 0,
			Compressed = 1,
			XorMagic = 2,
			Last = 4
		}

		[Flags]
		private enum DoRpcFlags : uint
		{
			None = 0U,
			NoCompression = 1U,
			NoXorMagic = 2U,
			Chain = 4U,
			ExtendedError = 8U,
			InternalAccessPrivileges = 16U
		}

		internal class ContextHandlePool
		{
			public ContextHandlePool(Guid guid)
			{
				this.sessionHandleMap = PersistentAvlTree<uint, IntPtr>.Empty;
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
					PersistentAvlTree<uint, IntPtr> persistentAvlTree;
					using (LockManager.Lock(this))
					{
						persistentAvlTree = this.sessionHandleMap;
						this.sessionHandleMap = null;
					}
					foreach (IntPtr intPtr in persistentAvlTree.GetValuesLmr())
					{
						IntPtr intPtr2 = intPtr;
						MapiRpc.Instance.DoDisconnect(null, ref intPtr2);
					}
				}
				finally
				{
					this.ExpireNotificationWait(ErrorCode.CreateRpcServerUnavailable((LID)54424U), DateTime.UtcNow);
				}
			}

			public ErrorCode GetSessionHandle(uint sessionHandle, out IntPtr handle)
			{
				PersistentAvlTree<uint, IntPtr> persistentAvlTree = Interlocked.CompareExchange<PersistentAvlTree<uint, IntPtr>>(ref this.sessionHandleMap, null, null);
				handle = IntPtr.Zero;
				if (persistentAvlTree == null)
				{
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("CALL FAILED: Pool is being shut down. pool:[");
						stringBuilder.Append(this.guid.ToString());
						stringBuilder.Append("]");
						ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					return ErrorCode.CreateInvalidPool((LID)56216U);
				}
				if (!persistentAvlTree.TryGetValue(sessionHandle, out handle))
				{
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder2 = new StringBuilder(100);
						stringBuilder2.Append("CALL FAILED: error[RpcInvalidSession]. Invalid pool session handle. pool:[");
						stringBuilder2.Append(this.guid.ToString());
						stringBuilder2.Append("] session:[");
						stringBuilder2.Append(sessionHandle);
						stringBuilder2.Append("]");
						ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder2.ToString());
					}
					return ErrorCode.CreateRpcInvalidSession((LID)39832U);
				}
				return ErrorCode.NoError;
			}

			public ErrorCode SessionCreated(IntPtr sessionHandle, out uint handle)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				handle = 0U;
				if (this.sessionHandleMap != null)
				{
					using (LockManager.Lock(this))
					{
						if (this.sessionHandleMap != null)
						{
							handle = (uint)sessionHandle.ToInt32();
							this.sessionHandleMap = this.sessionHandleMap.SetValue(handle, sessionHandle);
						}
						else
						{
							errorCode = ErrorCode.CreateInvalidPool((LID)43928U);
						}
						goto IL_6F;
					}
				}
				errorCode = ErrorCode.CreateInvalidPool((LID)60312U);
				IL_6F:
				if (errorCode == ErrorCodeValue.InvalidPool && ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("CALL FAILED: Pool is being shut down. pool:[");
					stringBuilder.Append(this.guid.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
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
				if (flag && ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("Pool session map not changed: pool is being shut down. pool:[");
					stringBuilder.Append(this.guid.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
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
									array = new uint[this.pendingNotificationSet.Count];
									this.pendingNotificationSet.CopyTo(array);
									this.pendingNotificationSet.Clear();
									this.waitQueuedTime = null;
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
								if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									StringBuilder stringBuilder = new StringBuilder(100);
									stringBuilder.Append("CALL FAILED: Another notification wait call is already queued on the pool. pool:[");
									stringBuilder.Append(this.guid.ToString());
									stringBuilder.Append("]");
									ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
								}
								errorCode = ErrorCode.CreateRejected((LID)42136U);
							}
						}
						else
						{
							errorCode = ErrorCode.CreateInvalidPool((LID)50328U);
						}
						goto IL_12A;
					}
				}
				errorCode = ErrorCode.CreateInvalidPool((LID)62616U);
				IL_12A:
				if (errorCode == ErrorCode.NoError)
				{
					if (completion != null)
					{
						completion.CompleteAsyncCall(array);
						if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder2 = new StringBuilder(100);
							stringBuilder2.Append("Notification wait RPC call completed successfully. pool:[");
							stringBuilder2.Append(this.guid.ToString());
							stringBuilder2.Append("]");
							ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder2.ToString());
						}
					}
					else
					{
						if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder3 = new StringBuilder(100);
							stringBuilder3.Append("Notification wait has been registered successfully. pool:[");
							stringBuilder3.Append(this.guid.ToString());
							stringBuilder3.Append("]");
							ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder3.ToString());
						}
						StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(null);
						if (databaseInstance != null)
						{
							databaseInstance.ContextHandlePoolParkedCalls.Increment();
						}
					}
				}
				else if (errorCode == ErrorCodeValue.InvalidPool && ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder4 = new StringBuilder(100);
					stringBuilder4.Append("CALL FAILED: Pool is being shut down. pool:[");
					stringBuilder4.Append(this.guid.ToString());
					stringBuilder4.Append("]");
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder4.ToString());
				}
				return errorCode;
			}

			public void ExpireNotificationWait(ErrorCode error, DateTime currentTime)
			{
				IPoolWaitForNotificationsCompletion poolWaitForNotificationsCompletion = null;
				uint[] array = null;
				using (LockManager.Lock(this))
				{
					if (this.pendingCompletion != null && !this.completionQueued && (error != ErrorCode.NoError || (this.waitQueuedTime != null && this.waitQueuedTime.Value < currentTime && currentTime.Subtract(this.waitQueuedTime.Value) >= PoolRpcServer.WaitExpirationPeriod)))
					{
						if (error == ErrorCode.NoError && this.pendingNotificationSet.Count > 0)
						{
							array = new uint[this.pendingNotificationSet.Count];
							this.pendingNotificationSet.CopyTo(array);
							this.pendingNotificationSet.Clear();
						}
						poolWaitForNotificationsCompletion = this.pendingCompletion;
						this.pendingCompletion = null;
						this.waitQueuedTime = null;
					}
				}
				if (poolWaitForNotificationsCompletion != null)
				{
					if (error == ErrorCode.NoError)
					{
						poolWaitForNotificationsCompletion.CompleteAsyncCall(array);
					}
					else
					{
						poolWaitForNotificationsCompletion.FailAsyncCall((int)error, RpcServerBase.EmptyArraySegment);
					}
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
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
						ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(null);
					if (databaseInstance != null)
					{
						databaseInstance.ContextHandlePoolParkedCalls.Decrement();
					}
				}
			}

			public void NotificationPending(int sessionId)
			{
				bool flag = true;
				if (this.sessionHandleMap != null)
				{
					using (LockManager.Lock(this))
					{
						if (this.sessionHandleMap != null)
						{
							flag = false;
							IntPtr zero = IntPtr.Zero;
							if (this.sessionHandleMap.TryGetValue((uint)sessionId, out zero))
							{
								this.pendingNotificationSet.Add((uint)sessionId);
								if (this.pendingCompletion != null && !this.completionQueued)
								{
									this.completionQueued = ThreadPool.QueueUserWorkItem(new WaitCallback(this.CompleteWait), null);
									if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										StringBuilder stringBuilder = new StringBuilder(100);
										stringBuilder.Append("Async RPC completion is queued to thread pool. pool:[");
										stringBuilder.Append(this.guid.ToString());
										stringBuilder.Append("] session:[");
										stringBuilder.Append((uint)sessionId);
										stringBuilder.Append("]");
										ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
									}
								}
							}
							else if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder2 = new StringBuilder(100);
								stringBuilder2.Append("CALLBACK IGNORED: Invalid pool session handle. pool:[");
								stringBuilder2.Append(this.guid.ToString());
								stringBuilder2.Append("] session:[");
								stringBuilder2.Append(sessionId);
								stringBuilder2.Append("]");
								ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder2.ToString());
							}
						}
					}
				}
				if (flag && ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder3 = new StringBuilder(100);
					stringBuilder3.Append("CALLBACK IGNORED: Pool is closed. pool:[");
					stringBuilder3.Append(this.guid.ToString());
					stringBuilder3.Append("] session:[");
					stringBuilder3.Append(sessionId);
					stringBuilder3.Append("]");
					ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder3.ToString());
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
					if (poolWaitForNotificationsCompletion != null && this.pendingNotificationSet.Count > 0)
					{
						array = new uint[this.pendingNotificationSet.Count];
						this.pendingNotificationSet.CopyTo(array);
						this.pendingNotificationSet.Clear();
					}
				}
				if (poolWaitForNotificationsCompletion != null)
				{
					poolWaitForNotificationsCompletion.CompleteAsyncCall(array);
					if (ExTraceGlobals.RpcContextPoolTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("Notification wait RPC call completed successfully. pool:[");
						stringBuilder.Append(this.guid.ToString());
						stringBuilder.Append("] sessions:[");
						stringBuilder.AppendAsString(array);
						stringBuilder.Append("]");
						ExTraceGlobals.RpcContextPoolTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(null);
					if (databaseInstance != null)
					{
						databaseInstance.ContextHandlePoolParkedCalls.Decrement();
					}
				}
			}

			private int contextHandles;

			private Guid guid;

			private HashSet<uint> pendingNotificationSet;

			private IPoolWaitForNotificationsCompletion pendingCompletion;

			private DateTime? waitQueuedTime;

			private bool completionQueued;

			private PersistentAvlTree<uint, IntPtr> sessionHandleMap;
		}

		internal class PoolContextMap
		{
			private PoolContextMap()
			{
				this.ContextHandles = PersistentAvlTree<int, PoolRpcServer.ContextHandlePool>.Empty;
				this.Pools = PersistentAvlTree<Guid, PoolRpcServer.ContextHandlePool>.Empty;
			}

			private PoolContextMap(PersistentAvlTree<int, PoolRpcServer.ContextHandlePool> contextHandles, PersistentAvlTree<Guid, PoolRpcServer.ContextHandlePool> pools)
			{
				this.ContextHandles = contextHandles;
				this.Pools = pools;
			}

			public static PoolRpcServer.PoolContextMap Create(PersistentAvlTree<int, PoolRpcServer.ContextHandlePool> contextHandles, PersistentAvlTree<Guid, PoolRpcServer.ContextHandlePool> pools)
			{
				if (contextHandles.Count != 0 || pools.Count != 0)
				{
					return new PoolRpcServer.PoolContextMap(contextHandles, pools);
				}
				return PoolRpcServer.PoolContextMap.Empty;
			}

			public static readonly PoolRpcServer.PoolContextMap Empty = new PoolRpcServer.PoolContextMap();

			public readonly PersistentAvlTree<int, PoolRpcServer.ContextHandlePool> ContextHandles;

			public readonly PersistentAvlTree<Guid, PoolRpcServer.ContextHandlePool> Pools;
		}

		private sealed class PoolRpcServerEndpoint : PoolRpcServerBase
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IPoolRpcServer instance)
			{
				instance = PoolRpcServer.Instance;
				if (instance == null)
				{
					return -2147221227;
				}
				return ErrorCode.NoError;
			}

			public override void ConnectionDropped(IntPtr contextHandle)
			{
				PoolRpcServer instance = PoolRpcServer.Instance;
				if (instance != null)
				{
					instance.EcPoolDisconnect(contextHandle);
				}
			}
		}

		private sealed class PoolNotifyRpcServerEndpoint : PoolNotifyRpcServerBase
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IPoolRpcServer instance)
			{
				instance = PoolRpcServer.Instance;
				if (instance == null)
				{
					return -2147221227;
				}
				return ErrorCode.NoError;
			}

			public override void ConnectionDropped(IntPtr contextHandle)
			{
				PoolRpcServer instance = PoolRpcServer.Instance;
				if (instance != null)
				{
					instance.EcPoolDisconnect(contextHandle);
				}
			}
		}
	}
}
