using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Flags]
	public enum MailboxFolderMemberRights
	{
		[LocDescription(Strings.IDs.MalboxFolderRightsNone)]
		None = 0,
		[LocDescription(Strings.IDs.MalboxFolderRightsReadItems)]
		ReadItems = 1,
		[LocDescription(Strings.IDs.MalboxFolderRightsCreateItems)]
		CreateItems = 2,
		[LocDescription(Strings.IDs.MalboxFolderRightsEditOwnedItems)]
		EditOwnedItems = 8,
		[LocDescription(Strings.IDs.MalboxFolderRightsDeleteOwnedItems)]
		DeleteOwnedItems = 16,
		[LocDescription(Strings.IDs.MalboxFolderRightsEditAllItems)]
		EditAllItems = 32,
		[LocDescription(Strings.IDs.MalboxFolderRightsDeleteAllItems)]
		DeleteAllItems = 64,
		[LocDescription(Strings.IDs.MalboxFolderRightsCreateSubfolders)]
		CreateSubfolders = 128,
		[LocDescription(Strings.IDs.MalboxFolderRightsFolderOwner)]
		FolderOwner = 256,
		[LocDescription(Strings.IDs.MalboxFolderRightsFolderContact)]
		FolderContact = 512,
		[LocDescription(Strings.IDs.MalboxFolderRightsFolderVisible)]
		FolderVisible = 1024
	}
}
