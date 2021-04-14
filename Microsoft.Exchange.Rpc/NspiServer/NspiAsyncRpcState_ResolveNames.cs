using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_ResolveNames : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_ResolveNames,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiResolveNamesFlags flags, IntPtr pState, IntPtr pPropTags, IntPtr pNames, IntPtr ppMids, IntPtr ppRows)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.pState = pState;
			this.pPropTags = pPropTags;
			this.pNames = pNames;
			this.ppMids = ppMids;
			this.ppRows = ppRows;
			this.codePage = 0;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiResolveNamesFlags.None;
			this.pState = IntPtr.Zero;
			this.pPropTags = IntPtr.Zero;
			this.pNames = IntPtr.Zero;
			this.ppMids = IntPtr.Zero;
			this.ppRows = IntPtr.Zero;
			this.codePage = 0;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			NspiState state = null;
			PropertyTag[] propertyTags = null;
			byte[][] names = null;
			if (this.ppMids != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppMids, IntPtr.Zero);
			}
			if (this.ppRows != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppRows, IntPtr.Zero);
			}
			if (this.pState != IntPtr.Zero)
			{
				state = new NspiState(this.pState);
			}
			this.codePage = MarshalHelper.GetString8CodePage(state);
			if (this.pPropTags != IntPtr.Zero)
			{
				propertyTags = MarshalHelper.ConvertSPropTagArrayToPropertyTagArray(this.pPropTags);
			}
			if (this.pNames != IntPtr.Zero)
			{
				names = NspiHelper.ConvertCountedByteStringArrayFromNative(this.pNames);
			}
			base.AsyncDispatch.BeginResolveNames(null, this.contextHandle, this.flags, state, propertyTags, names, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			int[] array = null;
			PropertyValue[][] array2 = null;
			NspiStatus result = NspiStatus.Success;
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle2 = null;
			try
			{
				int num = 0;
				array = null;
				array2 = null;
				result = base.AsyncDispatch.EndResolveNames(asyncResult, out num, out array, out array2);
				byte condition;
				if (array2 != null && num != this.codePage)
				{
					condition = 0;
				}
				else
				{
					condition = 1;
				}
				ExAssert.Assert(condition != 0, "Code page changed across dispatch layer.");
				if (this.ppMids != IntPtr.Zero && array != null)
				{
					safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(array, true);
					if (safeRpcMemoryHandle != null)
					{
						IntPtr val = safeRpcMemoryHandle.Detach();
						Marshal.WriteIntPtr(this.ppMids, val);
					}
				}
				if (this.ppRows != IntPtr.Zero && array2 != null)
				{
					safeRpcMemoryHandle2 = MarshalHelper.ConvertPropertyValueArraysToSRowSet(array2, num);
					if (safeRpcMemoryHandle2 != null)
					{
						IntPtr val2 = safeRpcMemoryHandle2.Detach();
						Marshal.WriteIntPtr(this.ppRows, val2);
					}
				}
			}
			finally
			{
				if (safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
				if (safeRpcMemoryHandle2 != null)
				{
					((IDisposable)safeRpcMemoryHandle2).Dispose();
				}
			}
			return (int)result;
		}

		private IntPtr contextHandle;

		private NspiResolveNamesFlags flags;

		private IntPtr pState;

		private IntPtr pPropTags;

		private IntPtr pNames;

		private IntPtr ppMids;

		private IntPtr ppRows;

		private int codePage;
	}
}
