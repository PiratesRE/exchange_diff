using System;
using System.Collections.Generic;
using msclr;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal abstract class RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear> : RpcHttpConnectionRegistrationAsyncDispatchRpcState
	{
		public static RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear CreateFromPool()
		{
			@lock @lock = null;
			RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear rpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear = null;
			@lock lock2 = new @lock(RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>.freePoolLock);
			try
			{
				@lock = lock2;
				if (RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>.freePool.Count > 0)
				{
					rpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear = RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>.freePool.Pop();
				}
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
			if (rpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear == null)
			{
				rpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear = new RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear();
			}
			return rpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear;
		}

		public virtual int PoolSize()
		{
			return 100;
		}

		public override void ReleaseToPool()
		{
			@lock @lock = null;
			RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear item = (RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear)this;
			@lock lock2 = new @lock(RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>.freePoolLock);
			try
			{
				@lock = lock2;
				if (RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>.freePool.Count < this.PoolSize())
				{
					RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>.freePool.Push(item);
				}
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
		}

		public RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>()
		{
		}

		private static readonly object freePoolLock = new object();

		private static readonly Stack<RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear> freePool = new Stack<RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>();
	}
}
