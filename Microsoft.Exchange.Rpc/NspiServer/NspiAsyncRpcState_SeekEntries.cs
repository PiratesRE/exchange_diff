using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_SeekEntries : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_SeekEntries,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiSeekEntriesFlags flags, IntPtr pState, IntPtr pTarget, IntPtr pRestriction, IntPtr pPropTags, IntPtr ppRows)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.pState = pState;
			this.pTarget = pTarget;
			this.pRestriction = pRestriction;
			this.pPropTags = pPropTags;
			this.ppRows = ppRows;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiSeekEntriesFlags.None;
			this.pState = IntPtr.Zero;
			this.pTarget = IntPtr.Zero;
			this.pRestriction = IntPtr.Zero;
			this.pPropTags = IntPtr.Zero;
			this.ppRows = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			NspiState state = null;
			int[] restriction = null;
			PropertyTag[] propertyTags = null;
			PropertyValue? target = null;
			if (this.ppRows != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppRows, IntPtr.Zero);
			}
			if (this.pState != IntPtr.Zero)
			{
				state = new NspiState(this.pState);
			}
			if (this.pTarget != IntPtr.Zero)
			{
				target = MarshalHelper.ConvertSPropValueToPropertyValue(this.pTarget, MarshalHelper.GetString8CodePage(state));
			}
			if (this.pRestriction != IntPtr.Zero)
			{
				restriction = NspiHelper.ConvertCountedIntArrayFromNative(this.pRestriction);
			}
			if (this.pPropTags != IntPtr.Zero)
			{
				propertyTags = MarshalHelper.ConvertSPropTagArrayToPropertyTagArray(this.pPropTags);
			}
			base.AsyncDispatch.BeginSeekEntries(null, this.contextHandle, this.flags, state, target, restriction, propertyTags, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			PropertyValue[][] array = null;
			NspiStatus result = NspiStatus.Success;
			NspiState nspiState = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			try
			{
				array = null;
				result = base.AsyncDispatch.EndSeekEntries(asyncResult, out nspiState, out array);
				if (this.pState != IntPtr.Zero && nspiState != null)
				{
					nspiState.MarshalToNative(this.pState);
				}
				if (this.ppRows != IntPtr.Zero && array != null)
				{
					safeRpcMemoryHandle = MarshalHelper.ConvertPropertyValueArraysToSRowSet(array, MarshalHelper.GetString8CodePage(nspiState));
					if (safeRpcMemoryHandle != null)
					{
						IntPtr val = safeRpcMemoryHandle.Detach();
						Marshal.WriteIntPtr(this.ppRows, val);
					}
				}
			}
			finally
			{
				if (safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
			}
			return (int)result;
		}

		private IntPtr contextHandle;

		private NspiSeekEntriesFlags flags;

		private IntPtr pState;

		private IntPtr pTarget;

		private IntPtr pRestriction;

		private IntPtr pPropTags;

		private IntPtr ppRows;
	}
}
