using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_GetProps : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_GetProps,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiGetPropsFlags flags, IntPtr pState, IntPtr pPropTags, IntPtr ppRow)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.pState = pState;
			this.pPropTags = pPropTags;
			this.ppRow = ppRow;
			this.codePage = 0;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiGetPropsFlags.None;
			this.pState = IntPtr.Zero;
			this.pPropTags = IntPtr.Zero;
			this.ppRow = IntPtr.Zero;
			this.codePage = 0;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			NspiState state = null;
			PropertyTag[] propertyTags = null;
			if (this.ppRow != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppRow, IntPtr.Zero);
			}
			if (this.pState != IntPtr.Zero)
			{
				state = new NspiState(this.pState);
				this.codePage = MarshalHelper.GetString8CodePage(state);
			}
			if (this.pPropTags != IntPtr.Zero)
			{
				propertyTags = MarshalHelper.ConvertSPropTagArrayToPropertyTagArray(this.pPropTags);
			}
			base.AsyncDispatch.BeginGetProps(null, this.contextHandle, this.flags, state, propertyTags, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			PropertyValue[] array = null;
			NspiStatus result = NspiStatus.Success;
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			try
			{
				int num = 0;
				array = null;
				result = base.AsyncDispatch.EndGetProps(asyncResult, out num, out array);
				byte condition;
				if (array != null && num != this.codePage)
				{
					condition = 0;
				}
				else
				{
					condition = 1;
				}
				ExAssert.Assert(condition != 0, "Code page changed across dispatch layer.");
				if (this.ppRow != IntPtr.Zero && array != null)
				{
					safeRpcMemoryHandle = MarshalHelper.ConvertPropertyValueArrayToSRow(array, num);
					if (safeRpcMemoryHandle != null)
					{
						IntPtr val = safeRpcMemoryHandle.Detach();
						Marshal.WriteIntPtr(this.ppRow, val);
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

		private NspiGetPropsFlags flags;

		private IntPtr pState;

		private IntPtr pPropTags;

		private IntPtr ppRow;

		private int codePage;
	}
}
