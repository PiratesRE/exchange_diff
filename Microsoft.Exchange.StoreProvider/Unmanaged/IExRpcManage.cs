using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("74B90D3B-56F1-434c-B9FC-677440094D50")]
	[ComImport]
	internal interface IExRpcManage
	{
		[PreserveSig]
		int Connect(int ulConnectFlags, [MarshalAs(UnmanagedType.LPStr)] string lpszServerDN, [MarshalAs(UnmanagedType.LPStr)] string lpszUserDN, [MarshalAs(UnmanagedType.LPWStr)] string lpwszUser, [MarshalAs(UnmanagedType.LPWStr)] string lpwszDomain, [MarshalAs(UnmanagedType.LPWStr)] string lpwszPassword, [MarshalAs(UnmanagedType.Interface)] out IExRpcConnection iExRpcConnection);

		[PreserveSig]
		int ConnectEx(int ulFlags, int ulConnectFlags, [MarshalAs(UnmanagedType.LPStr)] string lpszServerDN, [MarshalAs(UnmanagedType.LPStr)] string lpszUserDN, [MarshalAs(UnmanagedType.LPWStr)] string lpwszUser, [MarshalAs(UnmanagedType.LPWStr)] string lpwszDomain, [MarshalAs(UnmanagedType.LPWStr)] string lpwszPassword, [MarshalAs(UnmanagedType.LPStr)] string lpszHTTPProxyServerName, int ulConMod, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, int cbAuxBufferSize, [MarshalAs(UnmanagedType.Interface)] out IExRpcConnection iExRpcConnection);

		[PreserveSig]
		int FromIStg(IStorage pIStorage, IMessage pIMessage);

		[PreserveSig]
		int ToIStg(IStorage pIStorage, IMessage pIMessage, int ulFlags);

		[PreserveSig]
		int AdminConnect([MarshalAs(UnmanagedType.LPStr)] string lpszClientId, [MarshalAs(UnmanagedType.LPStr)] string lpszServer, [MarshalAs(UnmanagedType.LPWStr)] string lpwszUser, [MarshalAs(UnmanagedType.LPWStr)] string lpwszDomain, [MarshalAs(UnmanagedType.LPWStr)] string lpwszPassword, out IExRpcAdmin iExRpcAdmin);

		[PreserveSig]
		int FromIStream(IStream pIStream, IMessage pIMessage);

		[PreserveSig]
		int ToIStream(IStream pIStream, IMessage pIMessage, int ulFlags);

		[PreserveSig]
		int CreateAddressBookEntryIdFromLegacyDN([MarshalAs(UnmanagedType.LPStr)] string lpszLegacyDN, out int lpcbEntryId, out SafeExMemoryHandle lppEntryId);

		[PreserveSig]
		int CreateLegacyDNFromAddressBookEntryId(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out SafeExMemoryHandle lpszLegacyDN);

		[PreserveSig]
		int GetEntryIdType(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int ulType);

		[PreserveSig]
		int GetFolderEntryIdFromMessageEntryId(int cbMessageEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpMessageEntryID, out int lpcbFolderEntryId, out SafeExLinkedMemoryHandle lppFolderEntryId);

		[PreserveSig]
		int CreateAddressBookEntryIdFromLocalDirectorySID(int cbSid, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpSid, out int lpcbEntryId, out SafeExMemoryHandle lppEntryId);

		[PreserveSig]
		int CreateLocalDirectorySIDFromAddressBookEntryId(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int lpcbSid, out SafeExMemoryHandle lpSid);
	}
}
