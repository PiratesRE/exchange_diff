using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeClient
{
	internal class ClientAsyncCallState_Connect : ClientAsyncCallState
	{
		private void Cleanup()
		{
			if (this.m_szUserDn != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_szUserDn);
				this.m_szUserDn = IntPtr.Zero;
			}
			if (this.m_pbAuxIn != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pbAuxIn);
				this.m_pbAuxIn = IntPtr.Zero;
			}
			if (this.m_pbAuxOut != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pbAuxOut);
				this.m_pbAuxOut = IntPtr.Zero;
			}
			if (this.m_pcbAuxOut != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pcbAuxOut);
				this.m_pcbAuxOut = IntPtr.Zero;
			}
			if (this.m_pcmsPollsMax != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pcmsPollsMax);
				this.m_pcmsPollsMax = IntPtr.Zero;
			}
			if (this.m_pcRetry != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pcRetry);
				this.m_pcRetry = IntPtr.Zero;
			}
			if (this.m_pcmsRetryDelay != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pcmsRetryDelay);
				this.m_pcmsRetryDelay = IntPtr.Zero;
			}
			if (this.m_picxr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_picxr);
				this.m_picxr = IntPtr.Zero;
			}
			if (this.m_pszDNPrefix != IntPtr.Zero)
			{
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszDNPrefix);
				IntPtr intPtr2 = intPtr;
				if (intPtr != IntPtr.Zero)
				{
					<Module>.MIDL_user_free(intPtr2.ToPointer());
				}
				Marshal.FreeHGlobal(this.m_pszDNPrefix);
				this.m_pszDNPrefix = IntPtr.Zero;
			}
			if (this.m_pszDisplayName != IntPtr.Zero)
			{
				IntPtr intPtr3 = Marshal.ReadIntPtr(this.m_pszDisplayName);
				IntPtr intPtr4 = intPtr3;
				if (intPtr3 != IntPtr.Zero)
				{
					<Module>.MIDL_user_free(intPtr4.ToPointer());
				}
				Marshal.FreeHGlobal(this.m_pszDisplayName);
				this.m_pszDisplayName = IntPtr.Zero;
			}
			if (this.m_rgwClientVersion != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_rgwClientVersion);
				this.m_rgwClientVersion = IntPtr.Zero;
			}
			if (this.m_rgwServerVersion != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_rgwServerVersion);
				this.m_rgwServerVersion = IntPtr.Zero;
			}
			if (this.m_rgwBestVersion != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_rgwBestVersion);
				this.m_rgwBestVersion = IntPtr.Zero;
			}
			if (this.m_pulTimeStamp != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pulTimeStamp);
				this.m_pulTimeStamp = IntPtr.Zero;
			}
			if (this.m_pExCXH != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pExCXH);
				this.m_pExCXH = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_Connect(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr pRpcBindingHandle, string userDn, int flags, int conMod, int cpid, int lcidString, int lcidSort, short[] clientVersion, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut) : base("Connect", asyncCallback, asyncState)
		{
			try
			{
				if (userDn == null)
				{
					throw new ArgumentNullException("userDn");
				}
				this.m_pRpcBindingHandle = pRpcBindingHandle;
				this.m_segmentExtendedAuxOut = segmentExtendedAuxOut;
				this.m_ulFlags = flags;
				this.m_ulConMod = conMod;
				this.m_ulCpid = cpid;
				this.m_ulLcidString = lcidString;
				this.m_ulLcidSort = lcidSort;
				this.m_szUserDn = IntPtr.Zero;
				this.m_pbAuxIn = IntPtr.Zero;
				this.m_pbAuxOut = IntPtr.Zero;
				this.m_pcbAuxOut = IntPtr.Zero;
				this.m_pcmsPollsMax = IntPtr.Zero;
				this.m_pcRetry = IntPtr.Zero;
				this.m_pcmsRetryDelay = IntPtr.Zero;
				this.m_picxr = IntPtr.Zero;
				this.m_pszDNPrefix = IntPtr.Zero;
				this.m_pszDisplayName = IntPtr.Zero;
				this.m_rgwClientVersion = IntPtr.Zero;
				this.m_rgwServerVersion = IntPtr.Zero;
				this.m_rgwBestVersion = IntPtr.Zero;
				this.m_pulTimeStamp = IntPtr.Zero;
				this.m_pExCXH = IntPtr.Zero;
				bool flag = false;
				try
				{
					IntPtr szUserDn = Marshal.StringToHGlobalAnsi(userDn);
					this.m_szUserDn = szUserDn;
					IntPtr pcmsPollsMax = Marshal.AllocHGlobal(4);
					this.m_pcmsPollsMax = pcmsPollsMax;
					IntPtr pcRetry = Marshal.AllocHGlobal(4);
					this.m_pcRetry = pcRetry;
					IntPtr pcmsRetryDelay = Marshal.AllocHGlobal(4);
					this.m_pcmsRetryDelay = pcmsRetryDelay;
					IntPtr picxr = Marshal.AllocHGlobal(4);
					this.m_picxr = picxr;
					IntPtr pszDNPrefix = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pszDNPrefix = pszDNPrefix;
					Marshal.WriteIntPtr(this.m_pszDNPrefix, IntPtr.Zero);
					IntPtr pszDisplayName = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pszDisplayName = pszDisplayName;
					Marshal.WriteIntPtr(this.m_pszDisplayName, IntPtr.Zero);
					IntPtr rgwClientVersion = Marshal.AllocHGlobal(6);
					this.m_rgwClientVersion = rgwClientVersion;
					MapiVersionConversion.Legacy(clientVersion, this.m_rgwClientVersion, 0);
					IntPtr rgwServerVersion = Marshal.AllocHGlobal(6);
					this.m_rgwServerVersion = rgwServerVersion;
					IntPtr rgwBestVersion = Marshal.AllocHGlobal(6);
					this.m_rgwBestVersion = rgwBestVersion;
					IntPtr pulTimeStamp = Marshal.AllocHGlobal(4);
					this.m_pulTimeStamp = pulTimeStamp;
					int count = segmentExtendedAuxIn.Count;
					this.m_cbAuxIn = count;
					IntPtr pbAuxIn = Marshal.AllocHGlobal(count + 8);
					this.m_pbAuxIn = pbAuxIn;
					int cbAuxIn = this.m_cbAuxIn;
					if (cbAuxIn > 0)
					{
						Marshal.Copy(segmentExtendedAuxIn.Array, segmentExtendedAuxIn.Offset, this.m_pbAuxIn, cbAuxIn);
					}
					int count2 = segmentExtendedAuxOut.Count;
					IntPtr pbAuxOut = Marshal.AllocHGlobal(count2 + 8);
					this.m_pbAuxOut = pbAuxOut;
					IntPtr pcbAuxOut = Marshal.AllocHGlobal(4);
					this.m_pcbAuxOut = pcbAuxOut;
					Marshal.WriteInt32(this.m_pcbAuxOut, count2);
					IntPtr pExCXH = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pExCXH = pExCXH;
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

		private void ~ClientAsyncCallState_Connect()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			<Module>.cli_Async_EcDoConnectEx((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_pRpcBindingHandle.ToPointer(), (void**)this.m_pExCXH.ToPointer(), (byte*)this.m_szUserDn.ToPointer(), this.m_ulFlags, this.m_ulConMod, 0, this.m_ulCpid, this.m_ulLcidString, this.m_ulLcidSort, -1, 1, (uint*)this.m_pcmsPollsMax.ToPointer(), (uint*)this.m_pcRetry.ToPointer(), (uint*)this.m_pcmsRetryDelay.ToPointer(), (ushort*)this.m_picxr.ToPointer(), (byte**)this.m_pszDNPrefix.ToPointer(), (byte**)this.m_pszDisplayName.ToPointer(), this.m_rgwClientVersion.ToPointer(), this.m_rgwServerVersion.ToPointer(), this.m_rgwBestVersion.ToPointer(), (uint*)this.m_pulTimeStamp.ToPointer(), this.m_pbAuxIn.ToPointer(), this.m_cbAuxIn, this.m_pbAuxOut.ToPointer(), (uint*)this.m_pcbAuxOut.ToPointer());
		}

		public int End(out IntPtr contextHandle, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, out short[] serverVersion, out ArraySegment<byte> segmentExtendedAuxOut)
		{
			int result;
			try
			{
				int num = base.CheckCompletion();
				TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)Marshal.ReadInt32(this.m_pcmsPollsMax));
				pollsMax = timeSpan;
				retryCount = Marshal.ReadInt32(this.m_pcRetry);
				TimeSpan timeSpan2 = TimeSpan.FromMilliseconds((double)Marshal.ReadInt32(this.m_pcRetry));
				retryDelay = timeSpan2;
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszDNPrefix);
				if (intPtr != IntPtr.Zero)
				{
					dnPrefix = Marshal.PtrToStringAnsi(intPtr);
				}
				else
				{
					dnPrefix = null;
				}
				IntPtr intPtr2 = Marshal.ReadIntPtr(this.m_pszDisplayName);
				if (intPtr2 != IntPtr.Zero)
				{
					displayName = Marshal.PtrToStringAnsi(intPtr2);
				}
				else
				{
					displayName = null;
				}
				short[] array = new short[4];
				serverVersion = array;
				MapiVersionConversion.Normalize(this.m_rgwServerVersion, array);
				int num2 = Marshal.ReadInt32(this.m_pcbAuxOut);
				if (num2 > this.m_segmentExtendedAuxOut.Count)
				{
					throw new Exception(string.Format("Server returned more data then requested; what? cbAuxOut={0}, buffer.Count={1}", num2, this.m_segmentExtendedAuxOut.Count));
				}
				if (num2 > 0)
				{
					Marshal.Copy(this.m_pbAuxOut, this.m_segmentExtendedAuxOut.Array, this.m_segmentExtendedAuxOut.Offset, num2);
				}
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(this.m_segmentExtendedAuxOut.Array, this.m_segmentExtendedAuxOut.Offset, num2);
				segmentExtendedAuxOut = arraySegment;
				IntPtr intPtr3 = Marshal.ReadIntPtr(this.m_pExCXH);
				contextHandle = intPtr3;
				result = num;
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

		private IntPtr m_pRpcBindingHandle;

		private ArraySegment<byte> m_segmentExtendedAuxOut;

		private IntPtr m_szUserDn;

		private int m_ulFlags;

		private int m_ulConMod;

		private int m_ulCpid;

		private int m_ulLcidString;

		private int m_ulLcidSort;

		private IntPtr m_pbAuxIn;

		private int m_cbAuxIn;

		private IntPtr m_pbAuxOut;

		private IntPtr m_pcbAuxOut;

		private IntPtr m_pcmsPollsMax;

		private IntPtr m_pcRetry;

		private IntPtr m_pcmsRetryDelay;

		private IntPtr m_picxr;

		private IntPtr m_pszDNPrefix;

		private IntPtr m_pszDisplayName;

		private IntPtr m_rgwClientVersion;

		private IntPtr m_rgwServerVersion;

		private IntPtr m_rgwBestVersion;

		private IntPtr m_pulTimeStamp;

		private IntPtr m_pExCXH;
	}
}
