using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;

namespace Microsoft.Exchange.Rpc.NspiClient
{
	internal class ClientAsyncCallState_Bind : ClientAsyncCallState
	{
		private void Cleanup()
		{
			SafeRpcMemoryHandle stateHandle = this.m_stateHandle;
			if (stateHandle != null)
			{
				((IDisposable)stateHandle).Dispose();
				this.m_stateHandle = null;
			}
			SafeRpcMemoryHandle guidHandle = this.m_guidHandle;
			if (guidHandle != null)
			{
				((IDisposable)guidHandle).Dispose();
				this.m_guidHandle = null;
			}
			if (this.m_pContextHandle != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pContextHandle);
				this.m_pContextHandle = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_Bind(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr bindingHandle, NspiBindFlags flags, NspiState state, Guid? guid) : base("Bind", asyncCallback, asyncState)
		{
			try
			{
				this.m_pRpcBindingHandle = bindingHandle;
				this.m_pContextHandle = IntPtr.Zero;
				this.m_flags = flags;
				this.m_stateHandle = null;
				this.m_guidHandle = null;
				bool flag = false;
				try
				{
					this.m_stateHandle = NspiHelper.ConvertNspiStateToNative(state);
					if (guid != null)
					{
						Guid value = guid.Value;
						this.m_guidHandle = NspiHelper.ConvertGuidToNative(value);
					}
					IntPtr pContextHandle = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pContextHandle = pContextHandle;
					Marshal.WriteIntPtr(this.m_pContextHandle, IntPtr.Zero);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.Cleanup();
					}
				}
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		private void ~ClientAsyncCallState_Bind()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			SafeRpcMemoryHandle guidHandle = this.m_guidHandle;
			void* ptr;
			if (guidHandle != null)
			{
				ptr = guidHandle.DangerousGetHandle().ToPointer();
			}
			else
			{
				ptr = null;
			}
			SafeRpcMemoryHandle stateHandle = this.m_stateHandle;
			void* ptr2;
			if (stateHandle != null)
			{
				ptr2 = stateHandle.DangerousGetHandle().ToPointer();
			}
			else
			{
				ptr2 = null;
			}
			<Module>.cli_NspiBind((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_pRpcBindingHandle.ToPointer(), this.m_flags, (__MIDL_nspi_0002*)ptr2, (__MIDL_nspi_0001*)ptr, (void**)this.m_pContextHandle.ToPointer());
		}

		public NspiStatus End(out Guid? guid, out IntPtr contextHandle)
		{
			NspiStatus result;
			try
			{
				NspiStatus nspiStatus = (NspiStatus)base.CheckCompletion();
				guid = null;
				SafeRpcMemoryHandle guidHandle = this.m_guidHandle;
				if (guidHandle != null)
				{
					Guid value = NspiHelper.ConvertGuidFromNative(guidHandle.DangerousGetHandle());
					Guid? guid2 = new Guid?(value);
					guid = guid2;
				}
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pContextHandle);
				contextHandle = intPtr;
				result = nspiStatus;
			}
			finally
			{
				this.Cleanup();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					this.Cleanup();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private IntPtr m_pRpcBindingHandle;

		private NspiBindFlags m_flags;

		private SafeRpcMemoryHandle m_stateHandle;

		private SafeRpcMemoryHandle m_guidHandle;

		private IntPtr m_pContextHandle;
	}
}
