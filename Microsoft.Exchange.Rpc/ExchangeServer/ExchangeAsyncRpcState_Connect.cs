using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class ExchangeAsyncRpcState_Connect : BaseAsyncRpcState<Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcState_Connect,Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcServer_EMSMDB,Microsoft::Exchange::Rpc::IExchangeAsyncDispatch>
	{
		private void FreeLeasedBuffers()
		{
			byte[] array = this.leasedAuxIn;
			if (array != null)
			{
				AsyncBufferPools.ReleaseBuffer(array);
				this.leasedAuxIn = null;
			}
			byte[] array2 = this.leasedAuxOut;
			if (array2 != null)
			{
				AsyncBufferPools.ReleaseBuffer(array2);
				this.leasedAuxOut = null;
			}
		}

		public void Initialize(SafeRpcAsyncStateHandle asyncState, ExchangeAsyncRpcServer_EMSMDB asyncServer, IntPtr bindingHandle, IntPtr pContextHandle, IntPtr pUserDn, uint flags, uint conMod, uint cpid, uint lcidString, uint lcidSort, uint icxrLink, IntPtr pPollsMax, IntPtr pRetry, IntPtr pRetryDelay, IntPtr pIcxr, IntPtr pDNPrefix, IntPtr pDisplayName, IntPtr pClientVersion, IntPtr pServerVersion, IntPtr pTimeStamp, IntPtr pAuxIn, uint sizeAuxIn, IntPtr pAuxOut, IntPtr pSizeAuxOut, uint maxAuxOut)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.bindingHandle = bindingHandle;
			this.pContextHandle = pContextHandle;
			this.pUserDn = pUserDn;
			this.flags = flags;
			this.conMod = conMod;
			this.cpid = cpid;
			this.lcidString = lcidString;
			this.lcidSort = lcidSort;
			this.icxrLink = icxrLink;
			this.pPollsMax = pPollsMax;
			this.pRetry = pRetry;
			this.pRetryDelay = pRetryDelay;
			this.pIcxr = pIcxr;
			this.pDNPrefix = pDNPrefix;
			this.pDisplayName = pDisplayName;
			this.pClientVersion = pClientVersion;
			this.pServerVersion = pServerVersion;
			this.pTimeStamp = pTimeStamp;
			this.pAuxIn = pAuxIn;
			this.sizeAuxIn = sizeAuxIn;
			this.pAuxOut = pAuxOut;
			this.pSizeAuxOut = pSizeAuxOut;
			this.maxAuxOut = maxAuxOut;
		}

		public override void InternalReset()
		{
			this.bindingHandle = IntPtr.Zero;
			this.pContextHandle = IntPtr.Zero;
			this.pUserDn = IntPtr.Zero;
			this.flags = 0U;
			this.conMod = 0U;
			this.cpid = 0U;
			this.lcidString = 0U;
			this.lcidSort = 0U;
			this.icxrLink = 0U;
			this.pPollsMax = IntPtr.Zero;
			this.pRetry = IntPtr.Zero;
			this.pRetryDelay = IntPtr.Zero;
			this.pIcxr = IntPtr.Zero;
			this.pDNPrefix = IntPtr.Zero;
			this.pDisplayName = IntPtr.Zero;
			this.pClientVersion = IntPtr.Zero;
			this.pServerVersion = IntPtr.Zero;
			this.pTimeStamp = IntPtr.Zero;
			this.pAuxIn = IntPtr.Zero;
			this.sizeAuxIn = 0U;
			this.pAuxOut = IntPtr.Zero;
			this.pSizeAuxOut = IntPtr.Zero;
			this.maxAuxOut = 0U;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			bool flag = false;
			try
			{
				string userDn = Marshal.PtrToStringAnsi(this.pUserDn);
				IntPtr binding = this.bindingHandle;
				short[] array = new short[4];
				MapiVersionConversion.Normalize(this.pClientVersion, array);
				ArraySegment<byte> segmentExtendedAuxIn = base.EmptyByteArraySegment;
				if (this.pAuxIn != IntPtr.Zero)
				{
					byte[] buffer = AsyncBufferPools.GetBuffer((int)this.sizeAuxIn);
					this.leasedAuxIn = buffer;
					Marshal.Copy(this.pAuxIn, buffer, 0, (int)this.sizeAuxIn);
					ArraySegment<byte> arraySegment = new ArraySegment<byte>(this.leasedAuxIn, 0, (int)this.sizeAuxIn);
					segmentExtendedAuxIn = arraySegment;
				}
				ArraySegment<byte> segmentExtendedAuxOut = base.EmptyByteArraySegment;
				if (this.pAuxOut != IntPtr.Zero)
				{
					byte[] buffer2 = AsyncBufferPools.GetBuffer((int)this.maxAuxOut);
					this.leasedAuxOut = buffer2;
					ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(buffer2, 0, (int)this.maxAuxOut);
					segmentExtendedAuxOut = arraySegment2;
				}
				base.AsyncDispatch.BeginConnect(null, new RpcClientBinding(binding, base.AsyncState), userDn, (int)this.flags, (int)this.conMod, (int)this.cpid, (int)this.lcidString, (int)this.lcidSort, array, segmentExtendedAuxIn, segmentExtendedAuxOut, asyncCallback, this);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.FreeLeasedBuffers();
				}
			}
		}

		public unsafe override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			string text = null;
			string text2 = null;
			short[] array = null;
			int result;
			try
			{
				IntPtr zero = IntPtr.Zero;
				TimeSpan timeSpan = default(TimeSpan);
				TimeSpan timeSpan2 = default(TimeSpan);
				text = null;
				text2 = null;
				array = null;
				ArraySegment<byte> arraySegment = default(ArraySegment<byte>);
				int val;
				int num = base.AsyncDispatch.EndConnect(asyncResult, out zero, out timeSpan, out val, out timeSpan2, out text, out text2, out array, out arraySegment);
				Marshal.WriteIntPtr(this.pContextHandle, zero);
				Marshal.WriteInt32(this.pPollsMax, (int)timeSpan.TotalMilliseconds);
				Marshal.WriteInt32(this.pRetry, val);
				Marshal.WriteInt32(this.pRetryDelay, (int)timeSpan2.TotalMilliseconds);
				if (array != null && array.Length == 4)
				{
					short delta = (short)base.AsyncServer.GetVersionDelta();
					MapiVersionConversion.Legacy(array, this.pServerVersion, delta);
				}
				if (text != null)
				{
					IntPtr val2 = (IntPtr)((void*)<Module>.StringToUnmanagedMultiByte(text, 0U));
					Marshal.WriteIntPtr(this.pDNPrefix, val2);
				}
				if (text2 != null)
				{
					IntPtr val3 = (IntPtr)((void*)<Module>.StringToUnmanagedMultiByte(text2, 0U));
					Marshal.WriteIntPtr(this.pDisplayName, val3);
				}
				if (this.icxrLink == 4294967295U)
				{
					DateTime utcNow = DateTime.UtcNow;
					Marshal.WriteInt32(this.pTimeStamp, (int)utcNow.ToFileTime());
				}
				Marshal.WriteInt16(this.pIcxr, (short)((int)zero.ToInt64()));
				if (this.pAuxOut != IntPtr.Zero && arraySegment.Count > 0)
				{
					Marshal.Copy(arraySegment.Array, arraySegment.Offset, this.pAuxOut, arraySegment.Count);
					Marshal.WriteInt32(this.pSizeAuxOut, arraySegment.Count);
				}
				else
				{
					Marshal.WriteInt32(this.pSizeAuxOut, 0);
				}
				result = num;
			}
			finally
			{
				this.FreeLeasedBuffers();
			}
			return result;
		}

		private IntPtr bindingHandle;

		private IntPtr pUserDn;

		private uint flags;

		private uint conMod;

		private uint cpid;

		private uint lcidString;

		private uint lcidSort;

		private uint icxrLink;

		private IntPtr pClientVersion;

		private IntPtr pAuxIn;

		private uint sizeAuxIn;

		private uint maxAuxOut;

		private IntPtr pContextHandle;

		private IntPtr pPollsMax;

		private IntPtr pRetry;

		private IntPtr pRetryDelay;

		private IntPtr pIcxr;

		private IntPtr pDNPrefix;

		private IntPtr pDisplayName;

		private IntPtr pServerVersion;

		private IntPtr pTimeStamp;

		private IntPtr pAuxOut;

		private IntPtr pSizeAuxOut;

		private byte[] leasedAuxIn;

		private byte[] leasedAuxOut;
	}
}
