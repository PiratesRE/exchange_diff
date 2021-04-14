using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_GetTemplateInfo : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_GetTemplateInfo,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiGetTemplateInfoFlags flags, int type, IntPtr pDN, int codePage, int locale, IntPtr ppRow)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.type = type;
			this.pDN = pDN;
			this.codePage = codePage;
			this.locale = locale;
			this.ppRow = ppRow;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiGetTemplateInfoFlags.None;
			this.type = 0;
			this.pDN = IntPtr.Zero;
			this.codePage = 0;
			this.locale = 0;
			this.ppRow = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			string dn = null;
			if (this.ppRow != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppRow, IntPtr.Zero);
			}
			if (this.pDN != IntPtr.Zero)
			{
				dn = Marshal.PtrToStringAnsi(this.pDN);
			}
			base.AsyncDispatch.BeginGetTemplateInfo(null, this.contextHandle, this.flags, this.type, dn, this.codePage, this.locale, asyncCallback, this);
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
				result = base.AsyncDispatch.EndGetTemplateInfo(asyncResult, out num, out array);
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

		private NspiGetTemplateInfoFlags flags;

		private int type;

		private IntPtr pDN;

		private int codePage;

		private int locale;

		private IntPtr ppRow;
	}
}
