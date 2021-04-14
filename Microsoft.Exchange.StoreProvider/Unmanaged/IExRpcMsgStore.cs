using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("37FB08C3-F6C8-4de8-B8DA-AB7E41D01ECE")]
	[ComImport]
	internal interface IExRpcMsgStore
	{
		[PreserveSig]
		int CreateEntryId(long fid, long mid, [MarshalAs(UnmanagedType.Bool)] bool fMessage, [MarshalAs(UnmanagedType.Bool)] bool fLongTerm, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[PreserveSig]
		int GetShortTermIdsFromLongTermEntryId(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.Bool)] out bool pfMessage, out long pFid, out long pMid);

		[PreserveSig]
		int CreateEntryIdFromLegacyDN([MarshalAs(UnmanagedType.LPStr)] string lpszLegacyDN, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[PreserveSig]
		int GetParentEntryId(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int lpcbParentEntryId, out SafeExLinkedMemoryHandle lppParentEntryId);

		[PreserveSig]
		int GetAddressTypes(out int cAddressTypes, out IntPtr lppszAddressTypes);

		[PreserveSig]
		int BackoffNow(out int iNow);

		[PreserveSig]
		int CompressEntryId(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int lpcbCompressedEntryId, out SafeExLinkedMemoryHandle lppCompressedEntryId);

		[PreserveSig]
		int ExpandEntryId(int cbCompressedEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpCompressedEntryID, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[PreserveSig]
		int CreateGlobalIdFromId(long id, out SafeExLinkedMemoryHandle lppGid);

		[PreserveSig]
		int CreateIdFromGlobalId([MarshalAs(UnmanagedType.LPArray)] [In] byte[] gid, out long id);

		[PreserveSig]
		unsafe int MapActionsToMDBActions(_Actions* pactions, out uint cbBuf, out SafeExMemoryHandle pbBuf);

		[PreserveSig]
		int GetMailboxInstanceGuid(out Guid guidMailboxInstanceGuid);

		[PreserveSig]
		int GetMdbIdMapping(out ushort replidServer, out Guid guidServer);

		[PreserveSig]
		int GetReceiveFolderInfo([PointerType("SRowSet*")] out SafeExLinkedMemoryHandle lpSRowSet);

		[PreserveSig]
		int GetSpoolerQueueFid(out long fidSpoolerQ);

		[PreserveSig]
		int GetLocalRepIds(uint cid, out MapiLtidNative ltid);

		[PreserveSig]
		int CreatePublicEntryId(long fid, long mid, [MarshalAs(UnmanagedType.Bool)] bool fMessage, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[PreserveSig]
		int GetTransportQueueFolderId(out long fidTransportQueue);

		[PreserveSig]
		int GetIsRulesInterfaceAvailable([MarshalAs(UnmanagedType.Bool)] out bool rulesInterfaceAvailable);

		[PreserveSig]
		int SetSpooler();

		[PreserveSig]
		int SpoolerSetMessageLockState(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, int ulLockState);

		[PreserveSig]
		int SpoolerNotifyMessageNewMail(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.LPStr)] string lpszMsgClass, int ulMessageFlags);

		[PreserveSig]
		unsafe int PrereadMessages(_SBinaryArray* sbaEntryIDs);
	}
}
