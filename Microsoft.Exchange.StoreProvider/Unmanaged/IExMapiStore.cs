using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiStore : IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		int AdviseEx(byte[] lpEntryId, AdviseFlags ulEventMask, IntPtr iOnNotifyDelegate, ulong callbackId, out IntPtr piConnection);

		int Unadvise(IntPtr iConnection);

		int OpenEntry(byte[] lpEntryID, Guid lpInterface, int ulFlags, out int lpulObjType, out IExInterface iObj);

		int GetPerUser(byte[] ltid, bool fSendOnlyIfChanged, int ibStream, int cbDataLimit, out byte[] pb, out bool fLast);

		int SetPerUser(byte[] ltid, Guid? guidReplica, int lib, byte[] pb, int cb, bool fLast);

		int SetReceiveFolder(string lpwszMessageClass, int ulFlags, byte[] lpEntryID);

		int GetReceiveFolder(string lpwszMessageClass, int ulFlags, out byte[] lppEntryId, out string lppszExplicitClass);

		int GetReceiveFolderInfo(out PropValue[][] lpSRowSet);

		int StoreLogoff(ref int ulFlags);

		int AbortSubmit(byte[] lpEntryID, int ulFlags);

		int CreateEntryId(long fid, long mid, bool fMessage, bool fLongTerm, out byte[] lppEntryId);

		int GetShortTermIdsFromLongTermEntryId(byte[] lpEntryID, out bool pfMessage, out long pFid, out long pMid);

		int CreateEntryIdFromLegacyDN(string lpszLegacyDN, out byte[] lppEntryId);

		int GetParentEntryId(byte[] lpEntryID, out byte[] lppParentEntryId);

		int GetAddressTypes(out string[] lppszAddressTypes);

		int BackoffNow(out int iNow);

		int CompressEntryId(byte[] lpEntryID, out byte[] lppCompressedEntryId);

		int ExpandEntryId(byte[] lpCompressedEntryID, out byte[] lppEntryId);

		int CreateGlobalIdFromId(long id, out byte[] lppGid);

		int CreateIdFromGlobalId(byte[] gid, out long id);

		int MapActionsToMDBActions(RuleAction[] actions, out byte[] mdbActions);

		int GetMailboxInstanceGuid(out Guid guidMailboxInstanceGuid);

		int GetMdbIdMapping(out ushort replidServer, out Guid guidServer);

		int GetSpoolerQueueFid(out long fidSpoolerQ);

		int GetLocalRepIds(uint cid, out MapiLtidNative ltid);

		int CreatePublicEntryId(long fid, long mid, bool fMessage, out byte[] lppEntryId);

		int GetTransportQueueFolderId(out long fidTransportQueue);

		int GetIsRulesInterfaceAvailable(out bool rulesInterfaceAvailable);

		int SetSpooler();

		int SpoolerSetMessageLockState(byte[] lpEntryID, int ulLockState);

		int SpoolerNotifyMessageNewMail(byte[] lpEntryID, string lpszMsgClass, int ulMessageFlags);

		int GetPerUserGuid(MapiLtidNative ltid, out Guid guid);

		int GetPerUserLtids(Guid guid, out MapiLtidNative[] ltids);

		int GetEffectiveRights(byte[] lpAddressBookEntryId, byte[] lpEntryId, out uint rights);

		int PrereadMessages(byte[][] entryIds);

		int GetInTransitStatus(out uint inTransitStatus);
	}
}
