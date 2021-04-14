using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_GetNamesFromIDs : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_GetNamesFromIDs,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiGetNamesFromIDsFlags flags, IntPtr pGuid, IntPtr pPropTags, IntPtr ppReturnedPropTags, IntPtr ppNames)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.pGuid = pGuid;
			this.pPropTags = pPropTags;
			this.ppReturnedPropTags = ppReturnedPropTags;
			this.ppNames = ppNames;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiGetNamesFromIDsFlags.None;
			this.pGuid = IntPtr.Zero;
			this.pPropTags = IntPtr.Zero;
			this.ppReturnedPropTags = IntPtr.Zero;
			this.ppNames = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			PropertyTag[] propertyTags = null;
			if (this.ppReturnedPropTags != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppReturnedPropTags, IntPtr.Zero);
			}
			if (this.ppNames != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppNames, IntPtr.Zero);
			}
			Guid? guid = null;
			if (this.pGuid != IntPtr.Zero)
			{
				Guid value = NspiHelper.ConvertGuidFromNative(this.pGuid);
				Guid? guid2 = new Guid?(value);
				guid = guid2;
			}
			if (this.pPropTags != IntPtr.Zero)
			{
				propertyTags = MarshalHelper.ConvertSPropTagArrayToPropertyTagArray(this.pPropTags);
			}
			base.AsyncDispatch.BeginGetNamesFromIDs(null, this.contextHandle, this.flags, guid, propertyTags, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			PropertyTag[] array = null;
			NspiStatus result = NspiStatus.Success;
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle2 = null;
			try
			{
				array = null;
				result = base.AsyncDispatch.EndGetNamesFromIDs(asyncResult, out array, out safeRpcMemoryHandle);
				if (this.ppReturnedPropTags != IntPtr.Zero && array != null)
				{
					safeRpcMemoryHandle2 = MarshalHelper.ConvertPropertyTagArrayToSPropTagArray(array, true);
					if (safeRpcMemoryHandle2 != null)
					{
						IntPtr val = safeRpcMemoryHandle2.Detach();
						Marshal.WriteIntPtr(this.ppReturnedPropTags, val);
					}
				}
				if (this.ppNames != IntPtr.Zero && safeRpcMemoryHandle != null)
				{
					IntPtr val2 = safeRpcMemoryHandle.Detach();
					Marshal.WriteIntPtr(this.ppNames, val2);
				}
			}
			finally
			{
				if (safeRpcMemoryHandle2 != null)
				{
					((IDisposable)safeRpcMemoryHandle2).Dispose();
				}
				if (safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
			}
			return (int)result;
		}

		private IntPtr contextHandle;

		private NspiGetNamesFromIDsFlags flags;

		private IntPtr pGuid;

		private IntPtr pPropTags;

		private IntPtr ppReturnedPropTags;

		private IntPtr ppNames;
	}
}
