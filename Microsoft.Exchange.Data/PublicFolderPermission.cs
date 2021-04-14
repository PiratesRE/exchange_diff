using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum PublicFolderPermission
	{
		None = 0,
		ReadItems = 1,
		CreateItems = 2,
		EditOwnedItems = 8,
		DeleteOwnedItems = 16,
		EditAllItems = 32,
		DeleteAllItems = 64,
		CreateSubfolders = 128,
		FolderOwner = 256,
		FolderContact = 512,
		FolderVisible = 1024
	}
}
