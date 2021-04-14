using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal abstract class NspiAsyncRpcServer : BaseAsyncRpcServer<Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public override void DroppedConnection(IntPtr contextHandle)
		{
		}

		public override void RundownContext(IntPtr contextHandle)
		{
			INspiAsyncDispatch asyncDispatch = this.GetAsyncDispatch();
			if (asyncDispatch != null)
			{
				asyncDispatch.ContextHandleRundown(contextHandle);
			}
		}

		public unsafe override void StartRundownQueue()
		{
			RundownQueue_NSPI_HANDLE* ptr = null;
			if (<Module>.g_pRundownQueue_NSPI_HANDLE != null)
			{
				throw new InvalidOperationException("Rundown queue was already started!");
			}
			try
			{
				RundownQueue_NSPI_HANDLE* ptr2 = <Module>.@new(96UL);
				RundownQueue_NSPI_HANDLE* ptr3;
				try
				{
					if (ptr2 != null)
					{
						ptr3 = <Module>.RundownQueue_NSPI_HANDLE.{ctor}(ptr2);
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
					throw new OutOfMemoryException("Unable to allocate RundownQueue_NSPI_HANDLE");
				}
				if (<Module>.BaseRundownQueue<void\u0020*>.FInitialize(ptr3) == null)
				{
					throw new OutOfMemoryException("Unable to initialize RundownQueue_NSPI_HANDLE");
				}
				<Module>.g_pRundownQueue_NSPI_HANDLE = ptr3;
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
			if (<Module>.g_pRundownQueue_NSPI_HANDLE != null)
			{
				BaseRundownQueue<void\u0020*>* g_pRundownQueue_NSPI_HANDLE = <Module>.g_pRundownQueue_NSPI_HANDLE;
				<Module>.g_pRundownQueue_NSPI_HANDLE = null;
				<Module>.BaseRundownQueue<void\u0020*>.Uninitialize(g_pRundownQueue_NSPI_HANDLE);
			}
		}

		public NspiAsyncRpcServer()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static NspiAsyncRpcServer()
		{
			IntPtr rpcIntfHandle = new IntPtr(<Module>.nspi_v56_0_s_ifspec);
			NspiAsyncRpcServer.RpcIntfHandle = rpcIntfHandle;
		}

		public static readonly IntPtr RpcIntfHandle;
	}
}
