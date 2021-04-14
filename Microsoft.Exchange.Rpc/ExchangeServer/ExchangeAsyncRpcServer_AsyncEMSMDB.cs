using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal abstract class ExchangeAsyncRpcServer_AsyncEMSMDB : ExchangeAsyncRpcServer
	{
		public override void RundownContext(IntPtr contextHandle)
		{
			IExchangeAsyncDispatch asyncDispatch = this.GetAsyncDispatch();
			if (asyncDispatch != null)
			{
				asyncDispatch.NotificationContextHandleRundown(contextHandle);
			}
		}

		public unsafe override void StartRundownQueue()
		{
			RundownQueue_ExACXH* ptr = null;
			if (<Module>.g_pRundownQueue_ExACXH != null)
			{
				throw new InvalidOperationException("Rundown queue was already started!");
			}
			try
			{
				RundownQueue_ExACXH* ptr2 = <Module>.@new(96UL);
				RundownQueue_ExACXH* ptr3;
				try
				{
					if (ptr2 != null)
					{
						ptr3 = <Module>.RundownQueue_ExACXH.{ctor}(ptr2);
					}
					else
					{
						ptr3 = 0L;
					}
				}
				catch
				{
					<Module>.delete((void*)ptr2);
					throw;
				}
				ptr = ptr3;
				if (ptr3 == null)
				{
					throw new OutOfMemoryException("Unable to allocate RundownQueue_ExACXH");
				}
				if (<Module>.BaseRundownQueue<void\u0020*>.FInitialize(ptr3) == null)
				{
					throw new OutOfMemoryException("Unable to initialize RundownQueue_ExACXH");
				}
				<Module>.g_pRundownQueue_ExACXH = ptr3;
				ptr = null;
			}
			finally
			{
				if (ptr != null)
				{
					object obj = calli(System.Void* modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32), ptr, 1, *(*(long*)ptr + 24L));
				}
			}
		}

		public unsafe override void StopRundownQueue()
		{
			if (<Module>.g_pRundownQueue_ExACXH != null)
			{
				BaseRundownQueue<void\u0020*>* g_pRundownQueue_ExACXH = <Module>.g_pRundownQueue_ExACXH;
				<Module>.g_pRundownQueue_ExACXH = null;
				<Module>.BaseRundownQueue<void\u0020*>.Uninitialize(g_pRundownQueue_ExACXH);
			}
		}

		public ExchangeAsyncRpcServer_AsyncEMSMDB()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ExchangeAsyncRpcServer_AsyncEMSMDB()
		{
			IntPtr rpcIntfHandle = new IntPtr(<Module>.asyncemsmdbAsync_v0_1_s_ifspec);
			ExchangeAsyncRpcServer_AsyncEMSMDB.RpcIntfHandle = rpcIntfHandle;
		}

		public static readonly IntPtr RpcIntfHandle;
	}
}
