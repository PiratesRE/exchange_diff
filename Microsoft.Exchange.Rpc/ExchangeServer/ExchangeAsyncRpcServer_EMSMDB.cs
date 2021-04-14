using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal abstract class ExchangeAsyncRpcServer_EMSMDB : ExchangeAsyncRpcServer
	{
		public override void RundownContext(IntPtr contextHandle)
		{
			IExchangeAsyncDispatch asyncDispatch = this.GetAsyncDispatch();
			if (asyncDispatch != null)
			{
				asyncDispatch.ContextHandleRundown(contextHandle);
			}
		}

		public unsafe override void StartRundownQueue()
		{
			RundownQueue_ExCXH* ptr = null;
			if (<Module>.g_pRundownQueue_ExCXH != null)
			{
				throw new InvalidOperationException("Rundown queue was already started!");
			}
			try
			{
				RundownQueue_ExCXH* ptr2 = <Module>.@new(96UL);
				RundownQueue_ExCXH* ptr3;
				try
				{
					if (ptr2 != null)
					{
						ptr3 = <Module>.RundownQueue_ExCXH.{ctor}(ptr2);
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
					throw new OutOfMemoryException("Unable to allocate RundownQueue_ExCXH");
				}
				if (<Module>.BaseRundownQueue<void\u0020*>.FInitialize(ptr3) == null)
				{
					throw new OutOfMemoryException("Unable to initialize RundownQueue_ExCXH");
				}
				<Module>.g_pRundownQueue_ExCXH = ptr3;
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
			if (<Module>.g_pRundownQueue_ExCXH != null)
			{
				BaseRundownQueue<void\u0020*>* g_pRundownQueue_ExCXH = <Module>.g_pRundownQueue_ExCXH;
				<Module>.g_pRundownQueue_ExCXH = null;
				<Module>.BaseRundownQueue<void\u0020*>.Uninitialize(g_pRundownQueue_ExCXH);
			}
		}

		public abstract ushort GetVersionDelta();

		public ExchangeAsyncRpcServer_EMSMDB()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ExchangeAsyncRpcServer_EMSMDB()
		{
			IntPtr rpcIntfHandle = new IntPtr(<Module>.emsmdbAsync_v0_81_s_ifspec);
			ExchangeAsyncRpcServer_EMSMDB.RpcIntfHandle = rpcIntfHandle;
		}

		public static readonly IntPtr RpcIntfHandle;
	}
}
