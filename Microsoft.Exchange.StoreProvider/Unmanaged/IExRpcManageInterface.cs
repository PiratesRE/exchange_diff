using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExRpcManageInterface : IExInterface, IDisposeTrackable, IDisposable
	{
		int Connect(int ulConnectFlags, string lpszServerDN, string lpszUserDN, string lpwszUser, string lpwszDomain, string lpwszPassword, out IExRpcConnectionInterface iExRpcConnection);

		int ConnectEx(int ulFlags, int ulConnectFlags, string lpszServerDN, string lpszUserDN, string lpwszUser, string lpwszDomain, string lpwszPassword, string lpszHTTPProxyServerName, int ulConMod, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, int cbAuxBufferSize, out IExRpcConnectionInterface iExRpcConnection);

		int ConnectEx2(int ulFlags, int ulConnectFlags, string lpszServerDN, byte[] pbMdbGuid, string lpszUserDN, string lpwszUser, string lpwszDomain, string lpwszPassword, string lpszHTTPProxyServerName, int ulConMod, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, int cbAuxBufferSize, int cbClientSessionInfoSize, byte[] pbClientSessionInfo, IntPtr connectDelegate, IntPtr executeDelegate, IntPtr disconnectDelegate, int ulConnectionTimeoutMSecs, int ulCallTimeoutMSecs, out IExRpcConnectionInterface iExRpcConnection);

		int FromIStg(IStorage iStorage, IntPtr iMessage);

		int ToIStg(IStorage iStorage, IntPtr iMessage, int ulFlags);

		int FromIStream(IStream iStream, IntPtr iMessage);

		int ToIStream(IStream iStream, IntPtr iMessage, int ulFlags);

		int CreateAddressBookEntryIdFromLegacyDN(string lpszLegacyDN, out byte[] entryID);

		int CreateLegacyDNFromAddressBookEntryId(int cbEntryId, byte[] lpEntryID, out string lpszLegacyDN);

		int GetEntryIdType(int cbEntryId, byte[] lpEntryID, out int ulType);

		int GetFolderEntryIdFromMessageEntryId(int cbMessageEntryId, byte[] lpMessageEntryID, out byte[] folderEntryId);

		int CreateAddressBookEntryIdFromLocalDirectorySID(int cbSid, byte[] lpSid, out byte[] entryId);

		int CreateLocalDirectorySIDFromAddressBookEntryId(int cbEntryId, byte[] lpEntryID, out byte[] lpSid);

		int CreateIdSetBlobFromIStream(PropTag ptag, IStream iStream, out byte[] idSetBlob);
	}
}
