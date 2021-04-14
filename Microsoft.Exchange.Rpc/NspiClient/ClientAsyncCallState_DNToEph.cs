using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;

namespace Microsoft.Exchange.Rpc.NspiClient
{
	internal class ClientAsyncCallState_DNToEph : ClientAsyncCallState
	{
		private void Cleanup()
		{
			SafeStringArrayHandle stringsHandle = this.m_stringsHandle;
			if (stringsHandle != null)
			{
				((IDisposable)stringsHandle).Dispose();
				this.m_stringsHandle = null;
			}
			SafeRpcMemoryHandle midsOutHandle = this.m_midsOutHandle;
			if (midsOutHandle != null)
			{
				((IDisposable)midsOutHandle).Dispose();
				this.m_midsOutHandle = null;
			}
			if (this.m_ppmids != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_ppmids);
				this.m_ppmids = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_DNToEph(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr contextHandle, NspiDNToEphFlags flags, string[] DNs) : base("DNToEph", asyncCallback, asyncState)
		{
			try
			{
				this.m_contextHandle = contextHandle;
				this.m_flags = flags;
				this.m_stringsHandle = null;
				this.m_midsOutHandle = null;
				this.m_ppmids = IntPtr.Zero;
				bool flag = false;
				try
				{
					this.m_stringsHandle = new SafeStringArrayHandle(DNs, true);
					IntPtr ppmids = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_ppmids = ppmids;
					Marshal.WriteIntPtr(this.m_ppmids, IntPtr.Zero);
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

		private void ~ClientAsyncCallState_DNToEph()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			SafeStringArrayHandle stringsHandle = this.m_stringsHandle;
			void* ptr;
			if (stringsHandle != null)
			{
				ptr = stringsHandle.DangerousGetHandle().ToPointer();
			}
			else
			{
				ptr = null;
			}
			<Module>.cli_NspiDNToEph((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_contextHandle.ToPointer(), this.m_flags, (_StringsArray*)ptr, (_SPropTagArray_r**)this.m_ppmids.ToPointer());
		}

		public NspiStatus End(out int[] mids)
		{
			NspiStatus result;
			try
			{
				NspiStatus nspiStatus = (NspiStatus)base.CheckCompletion();
				SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(Marshal.ReadIntPtr(this.m_ppmids));
				this.m_midsOutHandle = safeRpcMemoryHandle;
				IntPtr pPropTagArray = safeRpcMemoryHandle.DangerousGetHandle();
				mids = MarshalHelper.ConvertSPropTagArrayToIntArray(pPropTagArray);
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

		private IntPtr m_contextHandle;

		private NspiDNToEphFlags m_flags;

		private SafeStringArrayHandle m_stringsHandle;

		private SafeRpcMemoryHandle m_midsOutHandle;

		private IntPtr m_ppmids;
	}
}
