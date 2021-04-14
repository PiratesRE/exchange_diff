using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FxProxyFolderWrapper : FxProxyWrapper, IMAPIFolder, IMAPIContainer, IMAPIProp
	{
		internal FxProxyFolderWrapper(IMapiFxCollector iFxCollector) : base(iFxCollector)
		{
		}

		public int GetContentsTable(int ulFlags, out IMAPITable iMAPITable)
		{
			iMAPITable = null;
			return -2147221246;
		}

		public int GetHierarchyTable(int ulFlags, out IMAPITable iMAPITable)
		{
			iMAPITable = null;
			return -2147221246;
		}

		public int OpenEntry(int cbEntryID, byte[] lpEntryID, Guid lpInterface, int ulFlags, out int lpulObjType, out object obj)
		{
			lpulObjType = 0;
			obj = null;
			return -2147221246;
		}

		public unsafe int SetSearchCriteria(SRestriction* lpRestriction, _SBinaryArray* lpContainerList, int ulSearchFlags)
		{
			return -2147221246;
		}

		public int GetSearchCriteria(int ulFlags, out SafeExLinkedMemoryHandle lpRestriction, out SafeExLinkedMemoryHandle lpContainerList, out int ulSearchState)
		{
			lpRestriction = null;
			lpContainerList = null;
			ulSearchState = 0;
			return -2147221246;
		}

		public int CreateMessage(IntPtr lpInterface, int ulFlags, out IMessage iMessage)
		{
			iMessage = null;
			return -2147221246;
		}

		public unsafe int CopyMessages(_SBinaryArray* sbinArray, IntPtr lpInterface, IMAPIFolder destFolder, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			return -2147221246;
		}

		public unsafe int DeleteMessages(_SBinaryArray* sbinArray, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			return -2147221246;
		}

		public int CreateFolder(int ulFolderType, string lpwszFolderName, string lpwszFolderComment, IntPtr lpInterface, int ulFlags, out IMAPIFolder iMAPIFolder)
		{
			iMAPIFolder = null;
			return -2147221246;
		}

		public int CopyFolder(int cbEntryId, byte[] lpEntryId, IntPtr lpInterface, IMAPIFolder destFolder, string lpwszNewFolderName, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			return -2147221246;
		}

		public int DeleteFolder(int cbEntryId, byte[] lpEntryId, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			return -2147221246;
		}

		public unsafe int SetReadFlags(_SBinaryArray* sbinArray, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			return -2147221246;
		}

		public int GetMessageStatus(int cbEntryId, byte[] lpEntryId, int ulFlags, out MessageStatus pulMessageStatus)
		{
			pulMessageStatus = MessageStatus.None;
			return -2147221246;
		}

		public int SetMessageStatus(int cbEntryId, byte[] lpEntryId, MessageStatus ulNewStatus, MessageStatus ulNewStatusMask, out MessageStatus pulOldStatus)
		{
			pulOldStatus = MessageStatus.None;
			return -2147221246;
		}

		public int Slot1c()
		{
			return -2147221246;
		}

		public int EmptyFolder(IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			return -2147221246;
		}
	}
}
