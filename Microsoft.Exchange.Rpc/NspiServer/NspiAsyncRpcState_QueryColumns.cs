using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_QueryColumns : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_QueryColumns,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiQueryColumnsFlags flags, NspiQueryColumnsMapiFlags mapiFlags, IntPtr ppColumns)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.mapiFlags = mapiFlags;
			this.ppColumns = ppColumns;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiQueryColumnsFlags.None;
			this.mapiFlags = NspiQueryColumnsMapiFlags.None;
			this.ppColumns = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			if (this.ppColumns != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppColumns, IntPtr.Zero);
			}
			base.AsyncDispatch.BeginQueryColumns(null, this.contextHandle, this.flags, this.mapiFlags, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			PropertyTag[] array = null;
			NspiStatus result = NspiStatus.Success;
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			try
			{
				array = null;
				result = base.AsyncDispatch.EndQueryColumns(asyncResult, out array);
				if (this.ppColumns != IntPtr.Zero && array != null)
				{
					safeRpcMemoryHandle = MarshalHelper.ConvertPropertyTagArrayToSPropTagArray(array, true);
					if (safeRpcMemoryHandle != null)
					{
						IntPtr val = safeRpcMemoryHandle.Detach();
						Marshal.WriteIntPtr(this.ppColumns, val);
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

		private NspiQueryColumnsFlags flags;

		private NspiQueryColumnsMapiFlags mapiFlags;

		private IntPtr ppColumns;
	}
}
