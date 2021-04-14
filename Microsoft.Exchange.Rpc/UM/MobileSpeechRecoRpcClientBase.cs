using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.Rpc.UM
{
	internal abstract class MobileSpeechRecoRpcClientBase : RpcClientBase
	{
		protected Guid RequestId
		{
			get
			{
				return this.requestId;
			}
		}

		protected MobileSpeechRecoRpcClientBase(Guid requestId, string machineName) : base(machineName)
		{
			try
			{
				ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Entering MobileSpeechRecoRpcClientBase constructor for requestId='{0}'", requestId);
				this.requestId = requestId;
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		[HandleProcessCorruptedStateExceptions]
		protected unsafe IAsyncResult BeginExecuteStep(byte[] inBlob, AsyncCallback callback)
		{
			ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Entering BeginExecuteStep for requestId='{0}'", this.requestId);
			if (null == inBlob)
			{
				throw new ArgumentNullException("inBlob");
			}
			if (null == callback)
			{
				throw new ArgumentNullException("callback");
			}
			MobileSpeechRecoRpcClientBase.MobileSpeechRecoAsyncResult mobileSpeechRecoAsyncResult = null;
			int num = 0;
			byte* ptr = null;
			bool flag = false;
			try
			{
				ptr = <Module>.MToUBytesClient(inBlob, &num);
				mobileSpeechRecoAsyncResult = new MobileSpeechRecoRpcClientBase.MobileSpeechRecoAsyncResult(this.requestId, callback);
				int num2 = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)mobileSpeechRecoAsyncResult.NativeState(), 112U);
				if (num2 != null)
				{
					ExTraceGlobals.RpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "BeginExecuteStep: RpcAsyncInitializeHandle Exception - requestId='{0}' rpcStatus='{1}", this.requestId, num2);
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num2, "BeginExecuteStep, RpcAsyncInitializeHandle");
				}
				*(long*)(mobileSpeechRecoAsyncResult.NativeState() + 24L / (long)sizeof(MobileSpeechRecoAsyncState)) = 0L;
				*(int*)(mobileSpeechRecoAsyncResult.NativeState() + 44L / (long)sizeof(MobileSpeechRecoAsyncState)) = 1;
				IntPtr handle = mobileSpeechRecoAsyncResult.AsyncWaitHandle.Handle;
				*(long*)(mobileSpeechRecoAsyncResult.NativeState() + 48L / (long)sizeof(MobileSpeechRecoAsyncState)) = handle.ToPointer();
				try
				{
					ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "BeginExecuteStep: Executing step async for requestId='{0}'", this.requestId);
					<Module>.cli_ExecuteStepAsync((_RPC_ASYNC_STATE*)mobileSpeechRecoAsyncResult.NativeState(), base.BindingHandle, num, ptr, (int*)(mobileSpeechRecoAsyncResult.NativeState() + 112L / (long)sizeof(MobileSpeechRecoAsyncState)), (byte**)(mobileSpeechRecoAsyncResult.NativeState() + 120L / (long)sizeof(MobileSpeechRecoAsyncState)));
					ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "BeginExecuteStep: Executed step async for requestId='{0}'", this.requestId);
					flag = true;
				}
				catch when (endfilter(true))
				{
					num2 = Marshal.GetExceptionCode();
					ExTraceGlobals.RpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "BeginExecuteStep: ExecuteStepAsync Exception - requestId='{0}' rpcStatus='{1}", this.requestId, num2);
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num2, "BeginExecuteStep, ExecuteStepAsync");
				}
			}
			finally
			{
				ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "BeginExecuteStep: Freeing input bytes for requestId='{0}'", this.requestId);
				if (null != ptr)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (!flag)
				{
					ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "BeginExecuteStep: Disposing async result for requestId='{0}'", this.requestId);
					if (mobileSpeechRecoAsyncResult != null)
					{
						((IDisposable)mobileSpeechRecoAsyncResult).Dispose();
						mobileSpeechRecoAsyncResult = null;
					}
				}
			}
			return mobileSpeechRecoAsyncResult;
		}

		protected unsafe int EndExecuteStep(IAsyncResult asyncResult, out byte[] outBlob, out bool timedOut)
		{
			ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Entering EndExecuteStep for requestId='{0}'", this.requestId);
			int result = 0;
			outBlob = null;
			timedOut = false;
			if (null == asyncResult)
			{
				throw new ArgumentNullException("asyncResult");
			}
			MobileSpeechRecoRpcClientBase.MobileSpeechRecoAsyncResult mobileSpeechRecoAsyncResult = asyncResult as MobileSpeechRecoRpcClientBase.MobileSpeechRecoAsyncResult;
			if (null == mobileSpeechRecoAsyncResult)
			{
				throw new ArgumentException("Invalid type", "asyncResult");
			}
			try
			{
				ExTraceGlobals.RpcTracer.TraceDebug<Guid, bool>((long)this.GetHashCode(), "EndExecuteStep: Completing async operation for requestId='{0}', timedOut='{1}'", this.requestId, mobileSpeechRecoAsyncResult.TimedOut);
				result = mobileSpeechRecoAsyncResult.Complete();
				byte timedOut2 = mobileSpeechRecoAsyncResult.TimedOut ? 1 : 0;
				timedOut = (timedOut2 != 0);
				if (timedOut2 == 0)
				{
					ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "EndExecuteStep: Getting outBlob for requestId='{0}'", this.requestId);
					outBlob = <Module>.UToMBytes(*(int*)(mobileSpeechRecoAsyncResult.NativeState() + 112L / (long)sizeof(MobileSpeechRecoAsyncState)), *(long*)(mobileSpeechRecoAsyncResult.NativeState() + 120L / (long)sizeof(MobileSpeechRecoAsyncState)));
				}
			}
			finally
			{
				ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "EndExecuteStep: Disposing result for requestId='{0}'", this.requestId);
				if (mobileSpeechRecoAsyncResult != null)
				{
					((IDisposable)mobileSpeechRecoAsyncResult).Dispose();
				}
			}
			return result;
		}

		private Guid requestId;

		private class MobileSpeechRecoAsyncResult : IAsyncResult, IDisposable
		{
			public unsafe MobileSpeechRecoAsyncResult(Guid requestId, AsyncCallback callback)
			{
				ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Entering MobileSpeechRecoAsyncResult constructor for requestId='{0}'", requestId);
				MobileSpeechRecoAsyncState* ptr = <Module>.@new(128UL);
				MobileSpeechRecoAsyncState* ptr2;
				try
				{
					if (ptr != null)
					{
						*(int*)(ptr + 112L / (long)sizeof(MobileSpeechRecoAsyncState)) = 0;
						*(long*)(ptr + 120L / (long)sizeof(MobileSpeechRecoAsyncState)) = 0L;
						ptr2 = ptr;
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
				this.pAsyncState = ptr2;
				if (0L == ptr2)
				{
					throw new OutOfMemoryException();
				}
				this.requestId = requestId;
				this.callback = callback;
				this.timedOut = false;
				this.completedEvent = new ManualResetEvent(false);
				this.callbackWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.completedEvent, new WaitOrTimerCallback(this.OnRpcComplete), null, MobileSpeechRecoRpcClientBase.MobileSpeechRecoAsyncResult.RequestTimeout, true);
			}

			private unsafe void ~MobileSpeechRecoAsyncResult()
			{
				ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Disposing MobileSpeechRecoAsyncResult for requestId='{0}'", this.requestId);
				MobileSpeechRecoAsyncState* ptr = this.pAsyncState;
				if (null != ptr)
				{
					MobileSpeechRecoAsyncState* ptr2 = ptr;
					ulong num = (ulong)(*(long*)(ptr2 + 120L / (long)sizeof(MobileSpeechRecoAsyncState)));
					if (0UL != num)
					{
						<Module>.MIDL_user_free(num);
					}
					<Module>.delete((void*)ptr2);
					this.pAsyncState = null;
				}
				ManualResetEvent manualResetEvent = this.completedEvent;
				if (null != manualResetEvent)
				{
					RegisteredWaitHandle registeredWaitHandle = this.callbackWaitHandle;
					if (null != registeredWaitHandle)
					{
						registeredWaitHandle.Unregister(manualResetEvent);
						this.callbackWaitHandle = null;
					}
					IDisposable disposable = this.completedEvent;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.completedEvent = null;
				}
			}

			public virtual bool IsCompleted
			{
				[return: MarshalAs(UnmanagedType.U1)]
				get
				{
					return this.completedEvent.WaitOne(0);
				}
			}

			public virtual WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.completedEvent;
				}
			}

			public virtual object AsyncState
			{
				get
				{
					return null;
				}
			}

			public virtual bool CompletedSynchronously
			{
				[return: MarshalAs(UnmanagedType.U1)]
				get
				{
					return false;
				}
			}

			public bool TimedOut
			{
				[return: MarshalAs(UnmanagedType.U1)]
				get
				{
					return this.timedOut;
				}
			}

			public void OnRpcComplete(object state, [MarshalAs(UnmanagedType.U1)] bool timedOut)
			{
				ExTraceGlobals.RpcTracer.TraceDebug<Guid, bool>((long)this.GetHashCode(), "Entering MobileSpeechRecoAsyncResult.OnRpcComplete for requestId='{0}', timedOut='{1}'", this.requestId, timedOut);
				this.timedOut = timedOut;
				this.callback(this);
			}

			public unsafe int Complete()
			{
				int result = 0;
				ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Entering MobileSpeechRecoAsyncResult.Complete for requestId='{0}'", this.requestId);
				if (this.timedOut)
				{
					ExTraceGlobals.RpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Complete: requestId='{0}' timed out", this.requestId);
					int num = <Module>.RpcAsyncCancelCall((_RPC_ASYNC_STATE*)this.pAsyncState, 1);
					if (num != null)
					{
						ExTraceGlobals.RpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "Complete: RpcAsyncCancelCall Exception - requestId='{0}' rpcStatus='{1}", this.requestId, num);
						<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "Complete, RpcAsyncCancelCall");
					}
					this.completedEvent.WaitOne();
				}
				int num2 = <Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)this.pAsyncState, (void*)(&result));
				if (num2 != null)
				{
					ExTraceGlobals.RpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "Complete: RpcAsyncCompleteCall Exception - requestId='{0}' rpcStatus='{1}", this.requestId, num2);
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num2, "Complete, RpcAsyncCompleteCall");
				}
				return result;
			}

			public unsafe MobileSpeechRecoAsyncState* NativeState()
			{
				return this.pAsyncState;
			}

			protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
			{
				if (A_0)
				{
					this.~MobileSpeechRecoAsyncResult();
				}
				else
				{
					base.Finalize();
				}
			}

			public sealed void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Note: this type is marked as 'beforefieldinit'.
			static MobileSpeechRecoAsyncResult()
			{
				TimeSpan requestTimeout = TimeSpan.FromSeconds(90.0);
				MobileSpeechRecoRpcClientBase.MobileSpeechRecoAsyncResult.RequestTimeout = requestTimeout;
			}

			private static TimeSpan RequestTimeout;

			private Guid requestId;

			private AsyncCallback callback;

			private bool timedOut;

			private ManualResetEvent completedEvent;

			private RegisteredWaitHandle callbackWaitHandle;

			private unsafe MobileSpeechRecoAsyncState* pAsyncState;
		}
	}
}
