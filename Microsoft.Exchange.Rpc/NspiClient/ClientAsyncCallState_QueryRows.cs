using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiClient
{
	internal class ClientAsyncCallState_QueryRows : ClientAsyncCallState
	{
		private void Cleanup()
		{
			SafeRpcMemoryHandle propTagsHandle = this.m_propTagsHandle;
			if (propTagsHandle != null)
			{
				((IDisposable)propTagsHandle).Dispose();
				this.m_propTagsHandle = null;
			}
			SafeRpcMemoryHandle stateHandle = this.m_stateHandle;
			if (stateHandle != null)
			{
				((IDisposable)stateHandle).Dispose();
				this.m_stateHandle = null;
			}
			SafeSRowSetHandle rowsetOutHandle = this.m_rowsetOutHandle;
			if (rowsetOutHandle != null)
			{
				((IDisposable)rowsetOutHandle).Dispose();
				this.m_rowsetOutHandle = null;
			}
			if (this.m_pmids != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pmids);
				this.m_pmids = IntPtr.Zero;
			}
			if (this.m_ppRowSet != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_ppRowSet);
				this.m_ppRowSet = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_QueryRows(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr contextHandle, NspiQueryRowsFlags flags, NspiState state, int[] mids, int count, PropertyTag[] propertyTags) : base("QueryRows", asyncCallback, asyncState)
		{
			try
			{
				this.m_contextHandle = contextHandle;
				this.m_flags = flags;
				this.m_ulCount = count;
				this.m_pmids = IntPtr.Zero;
				this.m_cmids = 0;
				this.m_propTagsHandle = null;
				this.m_stateHandle = null;
				this.m_rowsetOutHandle = null;
				this.m_ppRowSet = IntPtr.Zero;
				bool flag = false;
				try
				{
					this.m_stateHandle = NspiHelper.ConvertNspiStateToNative(state);
					if (propertyTags != null)
					{
						this.m_propTagsHandle = MarshalHelper.ConvertPropertyTagArrayToSPropTagArray(propertyTags);
					}
					IntPtr ppRowSet = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_ppRowSet = ppRowSet;
					Marshal.WriteIntPtr(this.m_ppRowSet, IntPtr.Zero);
					if (mids != null)
					{
						int num = mids.Length;
						if (num > 0)
						{
							this.m_cmids = num;
							IntPtr pmids = Marshal.AllocHGlobal((int)((long)num * 4L));
							this.m_pmids = pmids;
							Marshal.Copy(mids, 0, this.m_pmids, this.m_cmids);
						}
					}
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

		private void ~ClientAsyncCallState_QueryRows()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			SafeRpcMemoryHandle propTagsHandle = this.m_propTagsHandle;
			void* ptr;
			if (propTagsHandle != null)
			{
				ptr = propTagsHandle.DangerousGetHandle().ToPointer();
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
			<Module>.cli_NspiQueryRows((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_contextHandle.ToPointer(), this.m_flags, (__MIDL_nspi_0002*)ptr2, this.m_cmids, (uint*)this.m_pmids.ToPointer(), this.m_ulCount, (_SPropTagArray_r*)ptr, (_SRowSet_r**)this.m_ppRowSet.ToPointer());
		}

		public NspiStatus End(out NspiState state, out PropertyValue[][] rowset)
		{
			NspiStatus result;
			try
			{
				NspiStatus nspiStatus = (NspiStatus)base.CheckCompletion();
				state = NspiHelper.ConvertNspiStateFromNative(this.m_stateHandle);
				SafeSRowSetHandle safeSRowSetHandle = new SafeSRowSetHandle(Marshal.ReadIntPtr(this.m_ppRowSet));
				this.m_rowsetOutHandle = safeSRowSetHandle;
				IntPtr pRowSet = safeSRowSetHandle.DangerousGetHandle();
				rowset = MarshalHelper.ConvertSRowSetToPropertyValueArrays(pRowSet, MarshalHelper.GetString8CodePage(state));
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

		private NspiQueryRowsFlags m_flags;

		private int m_ulCount;

		private int m_cmids;

		private IntPtr m_pmids;

		private SafeRpcMemoryHandle m_propTagsHandle;

		private SafeRpcMemoryHandle m_stateHandle;

		private SafeSRowSetHandle m_rowsetOutHandle;

		private IntPtr m_ppRowSet;
	}
}
