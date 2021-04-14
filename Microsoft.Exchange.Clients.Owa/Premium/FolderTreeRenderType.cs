using System;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[Flags]
	public enum FolderTreeRenderType
	{
		None = 0,
		HideSearchFolders = 1,
		HideGeekFoldersWithSpecificOrder = 2,
		MailFoldersOnly = 4,
		MailFolderWithoutSearchFolders = 5
	}
}
