using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExRpcManageHandle : SafeExInterfaceHandle, IExRpcManageInterface, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExRpcManageHandle()
		{
		}

		internal SafeExRpcManageHandle(IntPtr handle) : base(handle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExRpcManageHandle>(this);
		}

		public int Connect(int ulConnectFlags, string lpszServerDN, string lpszUserDN, string lpwszUser, string lpwszDomain, string lpwszPassword, out IExRpcConnectionInterface iExRpcConnection)
		{
			SafeExRpcConnectionHandle safeExRpcConnectionHandle = null;
			int result = SafeExRpcManageHandle.IExRpcManage_Connect(this.handle, ulConnectFlags, lpszServerDN, lpszUserDN, lpwszUser, lpwszDomain, lpwszPassword, out safeExRpcConnectionHandle);
			iExRpcConnection = safeExRpcConnectionHandle;
			return result;
		}

		public int ConnectEx(int ulFlags, int ulConnectFlags, string lpszServerDN, string lpszUserDN, string lpwszUser, string lpwszDomain, string lpwszPassword, string lpszHTTPProxyServerName, int ulConMod, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, int cbAuxBufferSize, out IExRpcConnectionInterface iExRpcConnection)
		{
			SafeExRpcConnectionHandle safeExRpcConnectionHandle = null;
			int result = SafeExRpcManageHandle.IExRpcManage_ConnectEx(this.handle, ulFlags, ulConnectFlags, lpszServerDN, lpszUserDN, lpwszUser, lpwszDomain, lpwszPassword, lpszHTTPProxyServerName, ulConMod, lcidString, lcidSort, cpid, cReconnectIntervalInMins, cbRpcBufferSize, cbAuxBufferSize, out safeExRpcConnectionHandle);
			iExRpcConnection = safeExRpcConnectionHandle;
			return result;
		}

		public int ConnectEx2(int ulFlags, int ulConnectFlags, string lpszServerDN, byte[] pbMdbGuid, string lpszUserDN, string lpwszUser, string lpwszDomain, string lpwszPassword, string lpszHTTPProxyServerName, int ulConMod, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, int cbAuxBufferSize, int cbClientSessionInfoSize, byte[] pbClientSessionInfo, IntPtr connectDelegate, IntPtr executeDelegate, IntPtr disconnectDelegate, int ulConnectionTimeoutMSecs, int ulCallTimeoutMSecs, out IExRpcConnectionInterface iExRpcConnection)
		{
			SafeExRpcConnectionHandle safeExRpcConnectionHandle = null;
			int result = SafeExRpcManageHandle.IExRpcManage_ConnectEx2(this.handle, ulFlags, ulConnectFlags, lpszServerDN, pbMdbGuid, lpszUserDN, lpwszUser, lpwszDomain, lpwszPassword, lpszHTTPProxyServerName, ulConMod, lcidString, lcidSort, cpid, cReconnectIntervalInMins, cbRpcBufferSize, cbAuxBufferSize, cbClientSessionInfoSize, pbClientSessionInfo, connectDelegate, executeDelegate, disconnectDelegate, ulConnectionTimeoutMSecs, ulCallTimeoutMSecs, out safeExRpcConnectionHandle);
			iExRpcConnection = safeExRpcConnectionHandle;
			return result;
		}

		public int FromIStg(IStorage iStorage, IntPtr iMessage)
		{
			return SafeExRpcManageHandle.IExRpcManage_FromIStg(this.handle, iStorage, iMessage);
		}

		public int ToIStg(IStorage iStorage, IntPtr iMessage, int ulFlags)
		{
			return SafeExRpcManageHandle.IExRpcManage_ToIStg(this.handle, iStorage, iMessage, ulFlags);
		}

		internal int AdminConnect(string lpszClientId, string lpszServer, string lpwszUser, string lpwszDomain, string lpwszPassword, out SafeExRpcAdminHandle iExRpcAdmin)
		{
			return SafeExRpcManageHandle.IExRpcManage_AdminConnect(this.handle, lpszClientId, lpszServer, lpwszUser, lpwszDomain, lpwszPassword, out iExRpcAdmin);
		}

		public int FromIStream(IStream iStream, IntPtr iMessage)
		{
			return SafeExRpcManageHandle.IExRpcManage_FromIStream(this.handle, iStream, iMessage);
		}

		public int ToIStream(IStream iStream, IntPtr iMessage, int ulFlags)
		{
			return SafeExRpcManageHandle.IExRpcManage_ToIStream(this.handle, iStream, iMessage, ulFlags);
		}

		public int CreateAddressBookEntryIdFromLegacyDN(string lpszLegacyDN, out byte[] entryID)
		{
			entryID = null;
			int num = 0;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int result;
			try
			{
				int num2 = SafeExRpcManageHandle.IExRpcManage_CreateAddressBookEntryIdFromLegacyDN(this.handle, lpszLegacyDN, out num, out safeExMemoryHandle);
				if (num2 == 0)
				{
					entryID = new byte[num];
					safeExMemoryHandle.CopyTo(entryID, 0, num);
				}
				result = num2;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int CreateLegacyDNFromAddressBookEntryId(int cbEntryId, byte[] lpEntryID, out string lpszLegacyDN)
		{
			lpszLegacyDN = null;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExRpcManageHandle.IExRpcManage_CreateLegacyDNFromAddressBookEntryId(this.handle, cbEntryId, lpEntryID, out safeExMemoryHandle);
				if (num == 0 && !safeExMemoryHandle.IsInvalid)
				{
					lpszLegacyDN = Marshal.PtrToStringAnsi(safeExMemoryHandle.DangerousGetHandle());
				}
				result = num;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetEntryIdType(int cbEntryId, byte[] lpEntryID, out int ulType)
		{
			return SafeExRpcManageHandle.IExRpcManage_GetEntryIdType(this.handle, cbEntryId, lpEntryID, out ulType);
		}

		public int GetFolderEntryIdFromMessageEntryId(int cbMessageEntryId, byte[] lpMessageEntryID, out byte[] folderEntryId)
		{
			folderEntryId = null;
			int num = 0;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2 = SafeExRpcManageHandle.IExRpcManage_GetFolderEntryIdFromMessageEntryId(this.handle, cbMessageEntryId, lpMessageEntryID, out num, out safeExLinkedMemoryHandle);
				if (num2 == 0)
				{
					folderEntryId = new byte[num];
					safeExLinkedMemoryHandle.CopyTo(folderEntryId, 0, num);
				}
				result = num2;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int CreateAddressBookEntryIdFromLocalDirectorySID(int cbSid, byte[] lpSid, out byte[] entryID)
		{
			entryID = null;
			int num = 0;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int result;
			try
			{
				int num2 = SafeExRpcManageHandle.IExRpcManage_CreateAddressBookEntryIdFromLocalDirectorySID(this.handle, cbSid, lpSid, out num, out safeExMemoryHandle);
				if (num2 == 0)
				{
					entryID = new byte[num];
					safeExMemoryHandle.CopyTo(entryID, 0, num);
				}
				result = num2;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int CreateLocalDirectorySIDFromAddressBookEntryId(int cbEntryId, byte[] lpEntryID, out byte[] lpSid)
		{
			lpSid = null;
			int num = 0;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int result;
			try
			{
				int num2 = SafeExRpcManageHandle.IExRpcManage_CreateLocalDirectorySIDFromAddressBookEntryId(this.handle, cbEntryId, lpEntryID, out num, out safeExMemoryHandle);
				if (num2 == 0)
				{
					lpSid = new byte[num];
					safeExMemoryHandle.CopyTo(lpSid, 0, lpSid.Length);
				}
				result = num2;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int CreateIdSetBlobFromIStream(PropTag ptag, IStream iStream, out byte[] idSetBlob)
		{
			SafeExMemoryHandle safeExMemoryHandle = null;
			int num = 0;
			int result;
			try
			{
				int num2 = SafeExRpcManageHandle.IExRpcManage_CreateIdSetBlobFromIStream(this.handle, (int)ptag, iStream, out num, out safeExMemoryHandle);
				if (num > 0 && safeExMemoryHandle != null)
				{
					idSetBlob = new byte[num];
					Marshal.Copy(safeExMemoryHandle.DangerousGetHandle(), idSetBlob, 0, num);
				}
				else
				{
					idSetBlob = null;
				}
				result = num2;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_Connect(IntPtr iExRpcManage, int ulConnectFlags, [MarshalAs(UnmanagedType.LPStr)] string lpszServerDN, [MarshalAs(UnmanagedType.LPStr)] string lpszUserDN, [MarshalAs(UnmanagedType.LPWStr)] string lpwszUser, [MarshalAs(UnmanagedType.LPWStr)] string lpwszDomain, [MarshalAs(UnmanagedType.LPWStr)] string lpwszPassword, out SafeExRpcConnectionHandle iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_ConnectEx(IntPtr iExRpcManage, int ulFlags, int ulConnectFlags, [MarshalAs(UnmanagedType.LPStr)] string lpszServerDN, [MarshalAs(UnmanagedType.LPStr)] string lpszUserDN, [MarshalAs(UnmanagedType.LPWStr)] string lpwszUser, [MarshalAs(UnmanagedType.LPWStr)] string lpwszDomain, [MarshalAs(UnmanagedType.LPWStr)] string lpwszPassword, [MarshalAs(UnmanagedType.LPStr)] string lpszHTTPProxyServerName, int ulConMod, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, int cbAuxBufferSize, out SafeExRpcConnectionHandle iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_ConnectEx2(IntPtr iExRpcManage, int ulFlags, int ulConnectFlags, [MarshalAs(UnmanagedType.LPStr)] string lpszServerDN, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbMdbGuid, [MarshalAs(UnmanagedType.LPStr)] string lpszUserDN, [MarshalAs(UnmanagedType.LPWStr)] string lpwszUser, [MarshalAs(UnmanagedType.LPWStr)] string lpwszDomain, [MarshalAs(UnmanagedType.LPWStr)] string lpwszPassword, [MarshalAs(UnmanagedType.LPStr)] string lpszHTTPProxyServerName, int ulConMod, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, int cbAuxBufferSize, int cbClientSessionInfoSize, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbClientSessionInfo, IntPtr pfnConnectDelegate, IntPtr pfnExecuteDelegate, IntPtr pfnDisconnectDelegate, int ulConnectionTimeoutMSecs, int ulCallTimeoutMSecs, out SafeExRpcConnectionHandle iExRpcConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_FromIStg(IntPtr iExRpcManage, IStorage iStorage, IntPtr iMessage);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_ToIStg(IntPtr iExRpcManage, IStorage iStorage, IntPtr iMessage, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_AdminConnect(IntPtr iExRpcManage, [MarshalAs(UnmanagedType.LPStr)] string lpszClientId, [MarshalAs(UnmanagedType.LPStr)] string lpszServer, [MarshalAs(UnmanagedType.LPWStr)] string lpwszUser, [MarshalAs(UnmanagedType.LPWStr)] string lpwszDomain, [MarshalAs(UnmanagedType.LPWStr)] string lpwszPassword, out SafeExRpcAdminHandle iExRpcAdmin);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_FromIStream(IntPtr iExRpcManage, IStream iStream, IntPtr iMessage);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_ToIStream(IntPtr iExRpcManage, IStream iStream, IntPtr iMessage, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_CreateAddressBookEntryIdFromLegacyDN(IntPtr iExRpcManage, [MarshalAs(UnmanagedType.LPStr)] string lpszLegacyDN, out int lpcbEntryId, out SafeExMemoryHandle lppEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_CreateLegacyDNFromAddressBookEntryId(IntPtr iExRpcManage, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out SafeExMemoryHandle lpszLegacyDN);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_GetEntryIdType(IntPtr iExRpcManage, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int ulType);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_GetFolderEntryIdFromMessageEntryId(IntPtr iExRpcManage, int cbMessageEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpMessageEntryID, out int lpcbFolderEntryId, out SafeExLinkedMemoryHandle lppFolderEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_CreateAddressBookEntryIdFromLocalDirectorySID(IntPtr iExRpcManage, int cbSid, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpSid, out int lpcbEntryId, out SafeExMemoryHandle lppEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_CreateLocalDirectorySIDFromAddressBookEntryId(IntPtr iExRpcManage, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int lpcbSid, out SafeExMemoryHandle lpSid);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcManage_CreateIdSetBlobFromIStream(IntPtr iExRpcManage, int ptag, IStream iStream, out int cbIdSetBlob, out SafeExMemoryHandle pbIdSetBlob);
	}
}
