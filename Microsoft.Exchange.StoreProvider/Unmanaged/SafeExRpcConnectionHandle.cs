using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExRpcConnectionHandle : SafeExInterfaceHandle, IExRpcConnectionInterface, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExRpcConnectionHandle()
		{
		}

		internal SafeExRpcConnectionHandle(IntPtr handle) : base(handle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExRpcConnectionHandle>(this);
		}

		public int OpenMsgStore(int ulFlags, long ullOpenFlags, string lpszMailboxDN, byte[] pbMailboxGuid, byte[] pbMdbGuid, out string lppszWrongServerDN, IntPtr hToken, byte[] pSidUser, byte[] pSidPrimaryGroup, string lpszUserDN, int ulLcidString, int ulLcidSort, int ulCpid, bool fUnifiedLogon, string lpszApplicationId, byte[] pbTenantHint, int cbTenantHint, out IExMapiStore iMsgStore)
		{
			lppszWrongServerDN = null;
			SafeExMemoryHandle safeExMemoryHandle = null;
			SafeExMapiStoreHandle safeExMapiStoreHandle = null;
			int result;
			try
			{
				int num = SafeExRpcConnectionHandle.IExRpcConnection_OpenMsgStore(this.handle, ulFlags, ullOpenFlags, lpszMailboxDN, pbMailboxGuid, pbMdbGuid, out safeExMemoryHandle, hToken, pSidUser, pSidPrimaryGroup, lpszUserDN, ulLcidString, ulLcidSort, ulCpid, fUnifiedLogon, lpszApplicationId, pbTenantHint, cbTenantHint, out safeExMapiStoreHandle);
				if (safeExMemoryHandle != null && !safeExMemoryHandle.IsInvalid)
				{
					lppszWrongServerDN = Marshal.PtrToStringAnsi(safeExMemoryHandle.DangerousGetHandle());
				}
				iMsgStore = safeExMapiStoreHandle;
				safeExMapiStoreHandle = null;
				result = num;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
				safeExMapiStoreHandle.DisposeIfValid();
			}
			return result;
		}

		public int SendAuxBuffer(int ulFlags, int cbAuxBuffer, byte[] pbAuxBuffer, int fForceSend)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_SendAuxBuffer(this.handle, ulFlags, cbAuxBuffer, pbAuxBuffer, fForceSend);
		}

		public int FlushRPCBuffer(bool fForceSend)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_FlushRPCBuffer(this.handle, fForceSend);
		}

		public int GetServerVersion(out int pulVersionMajor, out int pulVersionMinor, out int pulBuildMajor, out int pulBuildMinor)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_GetServerVersion(this.handle, out pulVersionMajor, out pulVersionMinor, out pulBuildMajor, out pulBuildMinor);
		}

		public int IsDead(out bool pfDead)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_IsDead(this.handle, out pfDead);
		}

		public int RpcSentToServer(out bool pfRpcSent)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_RpcSentToServer(this.handle, out pfRpcSent);
		}

		public int IsMapiMT(out bool pfMapiMT)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_IsMapiMT(this.handle, out pfMapiMT);
		}

		public int IsConnectedToMapiServer(out bool pfConnectedToMapiServer)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_IsConnectedToMapiServer(this.handle, out pfConnectedToMapiServer);
		}

		public void ClearStorePerRPCStats()
		{
			SafeExRpcConnectionHandle.IExRpcConnection_ClearStorePerRPCStats(this.handle);
		}

		public uint GetStorePerRPCStats(out PerRpcStats pPerRpcPerformanceStatistics)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_GetStorePerRPCStats(this.handle, out pPerRpcPerformanceStatistics);
		}

		public void ClearRPCStats()
		{
			SafeExRpcConnectionHandle.IExRpcConnection_ClearRPCStats(this.handle);
		}

		public int GetRPCStats(out RpcStats pRpcStats)
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_GetRPCStats(this.handle, out pRpcStats);
		}

		public int SetInternalAccess()
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_SetInternalAccess(this.handle);
		}

		public int ClearInternalAccess()
		{
			return SafeExRpcConnectionHandle.IExRpcConnection_ClearInternalAccess(this.handle);
		}

		public void CheckForNotifications()
		{
			SafeExRpcConnectionHandle.IExRpcConnection_CheckForNotifications(this.handle);
		}

		protected override void InternalReleaseHandle()
		{
			SafeExRpcConnectionHandle.IExRpcConnection_PrepareForRelease(this.handle);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_OpenMsgStore(IntPtr iExRpcConnection, int ulFlags, long ullOpenFlags, [MarshalAs(UnmanagedType.LPStr)] string lpszMailboxDN, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbMailboxGuid, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbMdbGuid, out SafeExMemoryHandle lppszWrongServerDN, IntPtr hToken, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pSidUser, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pSidPrimaryGroup, [MarshalAs(UnmanagedType.LPStr)] string lpszUserDN, int ulLcidString, int ulLcidSort, int ulCpid, [MarshalAs(UnmanagedType.Bool)] bool fUnifiedLogon, [MarshalAs(UnmanagedType.LPStr)] string lpszApplicationId, [MarshalAs(UnmanagedType.LPArray)] byte[] pbTenantHint, int cbTenantHint, out SafeExMapiStoreHandle iMsgStore);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_SendAuxBuffer(IntPtr iExRpcConnection, int ulFlags, int cbAuxBuffer, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbAuxBuffer, int fForceSend);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_FlushRPCBuffer(IntPtr iExRpcConnection, [MarshalAs(UnmanagedType.Bool)] bool fForceSend);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_GetServerVersion(IntPtr iExRpcConnection, out int pulVersionMajor, out int pulVersionMinor, out int pulBuildMajor, out int pulBuildMinor);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_IsDead(IntPtr iExRpcConnection, [MarshalAs(UnmanagedType.Bool)] out bool pfDead);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_RpcSentToServer(IntPtr iExRpcConnection, [MarshalAs(UnmanagedType.Bool)] out bool pfRpcSent);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_IsMapiMT(IntPtr iExRpcConnection, [MarshalAs(UnmanagedType.Bool)] out bool pfMapiMT);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_IsConnectedToMapiServer(IntPtr iExRpcConnection, [MarshalAs(UnmanagedType.Bool)] out bool pfConnectedToMapiServer);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern void IExRpcConnection_ClearStorePerRPCStats(IntPtr iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern uint IExRpcConnection_GetStorePerRPCStats(IntPtr iExRpcConnection, out PerRpcStats pPerRpcPerformanceStatistics);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern void IExRpcConnection_ClearRPCStats(IntPtr iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_GetRPCStats(IntPtr iExRpcConnection, out RpcStats pRpcStats);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_SetInternalAccess(IntPtr iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcConnection_ClearInternalAccess(IntPtr iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern void IExRpcConnection_PrepareForRelease(IntPtr iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern void IExRpcConnection_CheckForNotifications(IntPtr iExRpcConnection);
	}
}
