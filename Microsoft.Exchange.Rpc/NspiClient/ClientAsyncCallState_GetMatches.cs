using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiClient
{
	internal class ClientAsyncCallState_GetMatches : ClientAsyncCallState
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
			SafeRpcMemoryHandle midsHandle = this.m_midsHandle;
			if (midsHandle != null)
			{
				((IDisposable)midsHandle).Dispose();
				this.m_midsHandle = null;
			}
			SafeRpcMemoryHandle restrictionHandle = this.m_restrictionHandle;
			if (restrictionHandle != null)
			{
				((IDisposable)restrictionHandle).Dispose();
				this.m_restrictionHandle = null;
			}
			SafeSRowSetHandle rowsetOutHandle = this.m_rowsetOutHandle;
			if (rowsetOutHandle != null)
			{
				((IDisposable)rowsetOutHandle).Dispose();
				this.m_rowsetOutHandle = null;
			}
			SafeRpcMemoryHandle midsOutHandle = this.m_midsOutHandle;
			if (midsOutHandle != null)
			{
				((IDisposable)midsOutHandle).Dispose();
				this.m_midsOutHandle = null;
			}
			if (this.m_ppRowSet != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_ppRowSet);
				this.m_ppRowSet = IntPtr.Zero;
			}
			if (this.m_ppMids != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_ppMids);
				this.m_ppMids = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_GetMatches(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr contextHandle, NspiGetMatchesFlags flags, NspiState state, int[] mids, int interfaceOptions, Restriction restriction, IntPtr propName, int maxRows, PropertyTag[] propertyTags) : base("GetMatches", asyncCallback, asyncState)
		{
			try
			{
				this.m_contextHandle = contextHandle;
				this.m_flags = flags;
				this.m_interfaceOptions = interfaceOptions;
				this.m_pPropName = propName;
				this.m_maxRows = maxRows;
				this.m_midsHandle = null;
				this.m_propTagsHandle = null;
				this.m_stateHandle = null;
				this.m_restrictionHandle = null;
				this.m_midsOutHandle = null;
				this.m_rowsetOutHandle = null;
				this.m_ppRowSet = IntPtr.Zero;
				this.m_ppMids = IntPtr.Zero;
				bool flag = false;
				try
				{
					this.m_stateHandle = NspiHelper.ConvertNspiStateToNative(state);
					if (mids != null)
					{
						SafeRpcMemoryHandle midsHandle = NspiHelper.ConvertIntArrayToPropTagArray(mids, false);
						this.m_midsHandle = midsHandle;
					}
					if (propertyTags != null)
					{
						this.m_propTagsHandle = MarshalHelper.ConvertPropertyTagArrayToSPropTagArray(propertyTags);
					}
					if (restriction != null)
					{
						this.m_restrictionHandle = MarshalHelper.ConvertRestrictionToSRestriction(restriction, MarshalHelper.GetString8CodePage(state));
					}
					IntPtr ppRowSet = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_ppRowSet = ppRowSet;
					Marshal.WriteIntPtr(this.m_ppRowSet, IntPtr.Zero);
					IntPtr ppMids = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_ppMids = ppMids;
					Marshal.WriteIntPtr(this.m_ppMids, IntPtr.Zero);
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

		private void ~ClientAsyncCallState_GetMatches()
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
			SafeRpcMemoryHandle restrictionHandle = this.m_restrictionHandle;
			void* ptr2;
			if (restrictionHandle != null)
			{
				ptr2 = restrictionHandle.DangerousGetHandle().ToPointer();
			}
			else
			{
				ptr2 = null;
			}
			SafeRpcMemoryHandle midsHandle = this.m_midsHandle;
			void* ptr3;
			if (midsHandle != null)
			{
				ptr3 = midsHandle.DangerousGetHandle().ToPointer();
			}
			else
			{
				ptr3 = null;
			}
			SafeRpcMemoryHandle stateHandle = this.m_stateHandle;
			void* ptr4;
			if (stateHandle != null)
			{
				ptr4 = stateHandle.DangerousGetHandle().ToPointer();
			}
			else
			{
				ptr4 = null;
			}
			<Module>.cli_NspiGetMatches((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_contextHandle.ToPointer(), this.m_flags, (__MIDL_nspi_0002*)ptr4, (_SPropTagArray_r*)ptr3, this.m_interfaceOptions, (_SRestriction_r*)ptr2, (_MAPINAMEID_r*)this.m_pPropName.ToPointer(), this.m_maxRows, (_SPropTagArray_r**)this.m_ppMids.ToPointer(), (_SPropTagArray_r*)ptr, (_SRowSet_r**)this.m_ppRowSet.ToPointer());
		}

		public NspiStatus End(out NspiState state, out int[] mids, out PropertyValue[][] rowset)
		{
			NspiStatus result;
			try
			{
				NspiStatus nspiStatus = (NspiStatus)base.CheckCompletion();
				state = NspiHelper.ConvertNspiStateFromNative(this.m_stateHandle);
				SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(Marshal.ReadIntPtr(this.m_ppMids));
				this.m_midsOutHandle = safeRpcMemoryHandle;
				IntPtr pPropTagArray = safeRpcMemoryHandle.DangerousGetHandle();
				mids = MarshalHelper.ConvertSPropTagArrayToIntArray(pPropTagArray);
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

		private NspiGetMatchesFlags m_flags;

		private int m_interfaceOptions;

		private IntPtr m_pPropName;

		private int m_maxRows;

		private SafeRpcMemoryHandle m_midsHandle;

		private SafeRpcMemoryHandle m_propTagsHandle;

		private SafeRpcMemoryHandle m_stateHandle;

		private SafeRpcMemoryHandle m_restrictionHandle;

		private SafeRpcMemoryHandle m_midsOutHandle;

		private SafeSRowSetHandle m_rowsetOutHandle;

		private IntPtr m_ppRowSet;

		private IntPtr m_ppMids;
	}
}
