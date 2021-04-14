using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.PoolRpc;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal class RpcInstancePool : IDisposable
	{
		public RpcInstancePool(Guid instance, int generation)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.Generation = generation;
				this.handleToUse = 0L;
				this.pool = new PoolRpcClient("localhost", instance);
				this.contextHandles = new IntPtr[32];
				Guid empty = Guid.Empty;
				for (int i = 0; i < this.contextHandles.Length; i++)
				{
					IRpcAsyncResult asyncResult = this.pool.BeginEcPoolConnect(0U, empty, RpcServerBase.EmptyArraySegment, null, null);
					ArraySegment<byte> emptyArraySegment = RpcServerBase.EmptyArraySegment;
					IntPtr intPtr;
					try
					{
						uint num;
						uint num2;
						ErrorCode errorCode = ErrorCode.CreateWithLid((LID)56184U, (ErrorCodeValue)this.pool.EndEcPoolConnect(asyncResult, out intPtr, out num, out num2, out empty, out emptyArraySegment));
						if (ErrorCode.NoError != errorCode)
						{
							throw new FailRpcException("Failed to connect to instance pool", (int)errorCode);
						}
					}
					finally
					{
						RpcBufferPool.ReleaseBuffer(emptyArraySegment.Array);
					}
					this.contextHandles[i] = intPtr;
				}
				disposeGuard.Success();
			}
		}

		public int Generation { get; private set; }

		public IRpcAsyncResult BeginEcPoolCreateSession(byte[] sessionSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, ArraySegment<byte> auxiliaryIn, AsyncCallback callback, object state)
		{
			return this.pool.BeginEcPoolCreateSession(this.GetContextHandle(), sessionSecurityContext, flags, userDn, connectionMode, codePageId, localeIdString, localeIdSort, clientVersion, auxiliaryIn, callback, state);
		}

		public ErrorCode EndEcPoolCreateSession(IRpcAsyncResult asyncResult, out uint sessionHandle, out string displayName, out uint maximumPolls, out uint retryCount, out uint retryDelay, out uint timeStamp, out short[] serverVersion, out short[] bestVersion, out ArraySegment<byte> auxOut)
		{
			return ErrorCode.CreateWithLid((LID)43896U, (ErrorCodeValue)this.pool.EndEcPoolCreateSession(asyncResult, out sessionHandle, out displayName, out maximumPolls, out retryCount, out retryDelay, out timeStamp, out serverVersion, out bestVersion, out auxOut));
		}

		public void BeginEcPoolCloseSession(uint sessionHandle, Action<object> callback, object state)
		{
			this.pool.BeginEcPoolCloseSession(this.GetContextHandle(), sessionHandle, delegate(IAsyncResult asyncResult)
			{
				try
				{
					this.pool.EndEcPoolCloseSession((IRpcAsyncResult)asyncResult);
				}
				catch (RpcException exception)
				{
					NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception);
				}
				finally
				{
					callback(asyncResult.AsyncState);
				}
			}, state);
		}

		public IRpcAsyncResult BeginEcPoolSessionDoRpc(uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, AsyncCallback callback, object state)
		{
			return this.pool.BeginEcPoolSessionDoRpc(this.GetContextHandle(), sessionHandle, flags, maximumResponseSize, request, auxiliaryIn, callback, state);
		}

		public ErrorCode EndEcPoolSessionDoRpc(IRpcAsyncResult asyncResult, out uint flags, out ArraySegment<byte> response, out ArraySegment<byte> auxOut)
		{
			return ErrorCode.CreateWithLid((LID)45628U, (ErrorCodeValue)this.pool.EndEcPoolSessionDoRpc(asyncResult, out flags, out response, out auxOut));
		}

		public IRpcAsyncResult BeginEcPoolWaitForNotificationsAsync(AsyncCallback callback, object state)
		{
			return this.pool.BeginEcPoolWaitForNotificationsAsync(this.GetContextHandle(), callback, state);
		}

		public ErrorCode EndEcPoolWaitForNotificationsAsync(IRpcAsyncResult asyncResult, out uint[] sessionHandles)
		{
			return ErrorCode.CreateWithLid((LID)60280U, (ErrorCodeValue)this.pool.EndEcPoolWaitForNotificationsAsync(asyncResult, out sessionHandles));
		}

		public void Dispose()
		{
			if (this.pool != null)
			{
				foreach (IntPtr intPtr in this.contextHandles)
				{
					if (IntPtr.Zero != intPtr)
					{
						this.pool.EcPoolDisconnect(intPtr);
					}
				}
				this.pool.Dispose();
				this.pool = null;
			}
		}

		private IntPtr GetContextHandle()
		{
			long num;
			long value;
			do
			{
				num = this.handleToUse;
				value = (num + 1L) % (long)this.contextHandles.Length;
			}
			while (num != Interlocked.CompareExchange(ref this.handleToUse, value, num));
			return this.contextHandles[(int)(checked((IntPtr)num))];
		}

		private PoolRpcClient pool;

		private IntPtr[] contextHandles;

		private long handleToUse;
	}
}
