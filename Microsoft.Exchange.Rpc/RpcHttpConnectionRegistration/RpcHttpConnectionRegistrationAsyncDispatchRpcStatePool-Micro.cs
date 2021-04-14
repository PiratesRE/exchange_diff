using System;
using System.Collections.Generic;
using msclr;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal abstract class RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register> : RpcHttpConnectionRegistrationAsyncDispatchRpcState
	{
		public static RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register CreateFromPool()
		{
			@lock @lock = null;
			RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register rpcHttpConnectionRegistrationAsyncDispatchRpcState_Register = null;
			@lock lock2 = new @lock(RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.freePoolLock);
			try
			{
				@lock = lock2;
				if (RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.freePool.Count > 0)
				{
					rpcHttpConnectionRegistrationAsyncDispatchRpcState_Register = RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.freePool.Pop();
				}
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
			if (rpcHttpConnectionRegistrationAsyncDispatchRpcState_Register == null)
			{
				rpcHttpConnectionRegistrationAsyncDispatchRpcState_Register = new RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register();
			}
			return rpcHttpConnectionRegistrationAsyncDispatchRpcState_Register;
		}

		public static Guid CopyIntPtrToGuid(IntPtr guidPtr)
		{
			return <Module>.FromGUID(guidPtr.ToPointer());
		}

		public virtual int PoolSize()
		{
			return 100;
		}

		public override void ReleaseToPool()
		{
			@lock @lock = null;
			RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register item = (RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register)this;
			@lock lock2 = new @lock(RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.freePoolLock);
			try
			{
				@lock = lock2;
				if (RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.freePool.Count < this.PoolSize())
				{
					RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.freePool.Push(item);
				}
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
		}

		public RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>()
		{
		}

		private static readonly object freePoolLock = new object();

		private static readonly Stack<RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register> freePool = new Stack<RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>();
	}
}
