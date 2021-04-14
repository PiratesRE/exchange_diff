using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc
{
	public class MockRequestQueueWrapper : IDisposable
	{
		public unsafe MockRequestQueueWrapper(int maxRequests, int maxThreads, RequestDelegate processRequest, RequestDelegate abortRequest)
		{
			this.localProcessRequestDelegate = processRequest;
			this.localAbortRequestDelegate = abortRequest;
			MockRequestQueue* ptr = <Module>.@new(152UL);
			MockRequestQueue* ptr2;
			try
			{
				if (ptr != null)
				{
					IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(this.localAbortRequestDelegate);
					ptr2 = <Module>.MockRequestQueue.{ctor}(ptr, Marshal.GetFunctionPointerForDelegate(this.localProcessRequestDelegate).ToPointer(), functionPointerForDelegate.ToPointer());
				}
				else
				{
					ptr2 = 0L;
				}
			}
			catch
			{
				<Module>.delete((void*)ptr);
				throw;
			}
			this.requestQueue = ptr2;
			<Module>.CRequestQueue<int>.FInitialize(ptr2, maxRequests, maxThreads);
		}

		private void ~MockRequestQueueWrapper()
		{
			this.!MockRequestQueueWrapper();
		}

		private unsafe void !MockRequestQueueWrapper()
		{
			MockRequestQueue* ptr = this.requestQueue;
			if (ptr != null)
			{
				object obj = calli(System.Void* modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32), ptr, 1, *(*(long*)ptr + 72L));
			}
			this.requestQueue = null;
		}

		public void Process(int requestData)
		{
			<Module>.CRequestQueue<int>.Process(this.requestQueue, ref requestData);
		}

		[HandleProcessCorruptedStateExceptions]
		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				this.!MockRequestQueueWrapper();
			}
			else
			{
				try
				{
					this.!MockRequestQueueWrapper();
				}
				finally
				{
					base.Finalize();
				}
			}
		}

		public sealed void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Finalize()
		{
			this.Dispose(false);
		}

		private RequestDelegate localProcessRequestDelegate;

		private RequestDelegate localAbortRequestDelegate;

		private unsafe MockRequestQueue* requestQueue;
	}
}
