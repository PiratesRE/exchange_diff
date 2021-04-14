using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeServer;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.MapiDisp;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal class ProxyMapiRpcServer : IProxyServer
	{
		internal ProxyMapiRpcServer(IPoolSessionManager manager)
		{
			this.manager = manager;
		}

		public int EcDoConnectEx(ClientSecurityContext callerSecurityContext, out IntPtr contextHandle, string userDn, uint flags, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, out uint maximumPolls, out uint retryCount, out uint retryDelay, out string displayName, short[] clientVersion, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			ProxyMapiRpcServer.TraceStartRpcMarker("EcDoConnectEx", IntPtr.Zero);
			ErrorCode errorCode = ErrorCode.NoError;
			uint num = 0U;
			contextHandle = IntPtr.Zero;
			displayName = "ProxySession";
			maximumPolls = (uint)MapiRpc.PollsMaxDefault.TotalMilliseconds;
			retryCount = 60U;
			retryDelay = (uint)MapiRpc.RetryDelayDefault.TotalMilliseconds;
			auxiliaryOut = null;
			try
			{
				errorCode = this.manager.CreateProxySession(callerSecurityContext, flags, userDn, connectionMode, codePageId, localeIdString, localeIdSort, clientVersion, auxiliaryIn, null, out num, out auxiliaryOut);
				if (errorCode == ErrorCode.NoError)
				{
					ErrorHelper.AssertRetail(0U == (num & 2147483648U), "Session handle value overlaps with asyc handle flag.");
					contextHandle = new IntPtr((int)num);
					num = 0U;
				}
			}
			finally
			{
				if (num != 0U)
				{
					this.manager.CloseSession(num);
				}
				ProxyMapiRpcServer.TraceEndRpcMarker("EcDoConnectEx", contextHandle, errorCode);
			}
			return (int)errorCode;
		}

		public int EcDoDisconnect(ref IntPtr contextHandle)
		{
			ErrorCode noError = ErrorCode.NoError;
			ProxyMapiRpcServer.TraceStartRpcMarker("EcDoDisconnect", contextHandle);
			int result;
			try
			{
				uint sessionHandle = (uint)contextHandle.ToInt32();
				this.manager.CloseSession(sessionHandle);
				contextHandle = IntPtr.Zero;
				result = (int)noError;
			}
			finally
			{
				ProxyMapiRpcServer.TraceEndRpcMarker("EcDoDisconnect", contextHandle, noError);
			}
			return result;
		}

		public int EcDoRpcExt2(ref IntPtr contextHandle, ref uint flags, ArraySegment<byte> request, uint maximumResponseSize, out ArraySegment<byte> response, ArraySegment<byte> auxiliaryIn, out ArraySegment<byte> auxiliaryOut)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			ProxyMapiRpcServer.TraceStartRpcMarker("EcDoRpcExt2", contextHandle);
			int result;
			try
			{
				response = RpcServerBase.EmptyArraySegment;
				auxiliaryOut = RpcServerBase.EmptyArraySegment;
				uint num = (uint)contextHandle.ToInt32();
				uint num2 = num;
				using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
				{
					ProxyMapiRpcServer.DoRpcHelper doRpcHelper = new ProxyMapiRpcServer.DoRpcHelper(manualResetEvent);
					errorCode = this.manager.BeginPoolDoRpc(ref num2, flags, maximumResponseSize, request, auxiliaryIn, new DoRpcCompleteCallback(doRpcHelper.OnDoRpcComplete), new Action<RpcException>(doRpcHelper.OnDoRpcException));
					if (ErrorCode.NoError != errorCode)
					{
						if (num2 == 0U)
						{
							contextHandle = IntPtr.Zero;
						}
						return (int)errorCode;
					}
					errorCode = doRpcHelper.WaitForDoRpc(out flags, out response, out auxiliaryOut);
					if (ErrorCodeValue.MdbNotInitialized == errorCode || ErrorCodeValue.Exiting == errorCode)
					{
						this.manager.CloseSession(num);
						contextHandle = IntPtr.Zero;
					}
				}
				result = (int)errorCode;
			}
			finally
			{
				ProxyMapiRpcServer.TraceEndRpcMarker("EcDoRpcExt2", contextHandle, errorCode);
			}
			return result;
		}

		public int EcDoAsyncConnectEx(IntPtr contextHandle, out IntPtr asynchronousContextHandle)
		{
			ErrorCode noError = ErrorCode.NoError;
			ProxyMapiRpcServer.TraceStartRpcMarker("EcDoAsyncConnectEx", contextHandle);
			int result;
			try
			{
				uint num = (uint)contextHandle.ToInt32();
				ErrorHelper.AssertRetail(0U == (num & 2147483648U), "Session handle value overlaps with asyc handle flag.");
				num |= 2147483648U;
				asynchronousContextHandle = new IntPtr((int)num);
				result = (int)noError;
			}
			finally
			{
				ProxyMapiRpcServer.TraceEndRpcMarker("EcDoAsyncConnectEx", contextHandle, noError);
			}
			return result;
		}

		public int DoAsyncWaitEx(IntPtr asynchronousContextHandle, uint flags, IProxyAsyncWaitCompletion completionObject)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			ProxyMapiRpcServer.TraceStartRpcMarker("DoAsyncWaitEx", asynchronousContextHandle);
			int result;
			try
			{
				try
				{
					uint num = (uint)asynchronousContextHandle.ToInt32();
					if ((num & 2147483648U) != 0U)
					{
						num &= (num & 2147483647U);
						errorCode = this.manager.QueueNotificationWait(ref num, completionObject);
						if (ErrorCode.NoError == errorCode)
						{
							completionObject = null;
						}
					}
					else
					{
						ExTraceGlobals.ProxyMapiTracer.TraceFunction(0L, "Notification rejected: invalid context handle");
					}
					result = 0;
				}
				finally
				{
					if (completionObject != null)
					{
						completionObject.CompleteAsyncCall(false, 2030);
						ExTraceGlobals.ProxyMapiTracer.TraceFunction(0L, "Notification rejected: error or exception occured");
					}
				}
			}
			finally
			{
				ProxyMapiRpcServer.TraceEndRpcMarker("DoAsyncWaitEx", asynchronousContextHandle, errorCode);
			}
			return result;
		}

		public ushort GetVersionDelta()
		{
			return 0;
		}

		private static void TraceStartRpcMarker(string rpcName, IntPtr contextHandle)
		{
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(60);
				stringBuilder.Append("MARK PROXY CALL [MAPI][");
				stringBuilder.Append(rpcName);
				stringBuilder.Append("] session:[");
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
		}

		private static void TraceEndRpcMarker(string rpcName, IntPtr contextHandle, ErrorCode error)
		{
			if (ExTraceGlobals.ProxyMapiTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(60);
				stringBuilder.Append("MARK PROXY CALL END [MAPI][");
				stringBuilder.Append(rpcName);
				stringBuilder.Append("] session:[");
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
		}

		private const uint AsyncHandleFlag = 2147483648U;

		private IPoolSessionManager manager;

		private class DoRpcHelper
		{
			public DoRpcHelper(ManualResetEvent completeEvent)
			{
				this.completeEvent = completeEvent;
				this.response = RpcServerBase.EmptyArraySegment;
				this.auxiliaryOut = RpcServerBase.EmptyArraySegment;
				this.result = ErrorCode.NoError;
				this.flags = 0U;
				this.exception = null;
			}

			public void OnDoRpcComplete(ErrorCode result, uint flags, ArraySegment<byte> rpcResponse, ArraySegment<byte> auxOut)
			{
				this.result = result;
				this.flags = flags;
				this.auxiliaryOut = auxOut;
				this.response = rpcResponse;
				this.completeEvent.Set();
			}

			public void OnDoRpcException(RpcException exception)
			{
				this.exception = exception;
				this.completeEvent.Set();
			}

			public ErrorCode WaitForDoRpc(out uint flags, out ArraySegment<byte> response, out ArraySegment<byte> auxiliaryOut)
			{
				this.completeEvent.WaitOne();
				if (this.exception != null)
				{
					throw new RpcException("RPC exception thrown by worker process. See inner exception for details", this.exception.ErrorCode, this.exception);
				}
				flags = this.flags;
				response = this.response;
				auxiliaryOut = this.auxiliaryOut;
				return this.result;
			}

			private ManualResetEvent completeEvent;

			private RpcException exception;

			private ArraySegment<byte> response;

			private ArraySegment<byte> auxiliaryOut;

			private ErrorCode result;

			private uint flags;
		}
	}
}
