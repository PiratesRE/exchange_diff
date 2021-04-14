using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_GetHierarchyInfo : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_GetHierarchyInfo,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiGetHierarchyInfoFlags flags, IntPtr pState, IntPtr pVersion, IntPtr ppRowSet)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.pState = pState;
			this.pVersion = pVersion;
			this.ppRowSet = ppRowSet;
			this.codePage = 0;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiGetHierarchyInfoFlags.None;
			this.pState = IntPtr.Zero;
			this.pVersion = IntPtr.Zero;
			this.ppRowSet = IntPtr.Zero;
			this.codePage = 0;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			NspiState state = null;
			if (this.ppRowSet != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.ppRowSet, IntPtr.Zero);
			}
			if (this.pState != IntPtr.Zero)
			{
				state = new NspiState(this.pState);
			}
			this.codePage = MarshalHelper.GetString8CodePage(state);
			int version = 0;
			if (this.pVersion != IntPtr.Zero)
			{
				version = Marshal.ReadInt32(this.pVersion);
			}
			base.AsyncDispatch.BeginGetHierarchyInfo(null, this.contextHandle, this.flags, state, version, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			PropertyValue[][] array = null;
			NspiStatus result = NspiStatus.Success;
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			try
			{
				int num = 0;
				int val = 0;
				array = null;
				result = base.AsyncDispatch.EndGetHierarchyInfo(asyncResult, out num, out val, out array);
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
				if (this.pVersion != IntPtr.Zero)
				{
					Marshal.WriteInt32(this.pVersion, val);
				}
				if (this.ppRowSet != IntPtr.Zero && array != null)
				{
					safeRpcMemoryHandle = MarshalHelper.ConvertPropertyValueArraysToSRowSet(array, num);
					if (safeRpcMemoryHandle != null)
					{
						IntPtr val2 = safeRpcMemoryHandle.Detach();
						Marshal.WriteIntPtr(this.ppRowSet, val2);
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

		private NspiGetHierarchyInfoFlags flags;

		private IntPtr pState;

		private IntPtr pVersion;

		private IntPtr ppRowSet;

		private int codePage;
	}
}
