using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IFolder : IClientObject<Folder>
	{
		int ItemCount { get; }

		string ServerRelativeUrl { get; }

		IListItem ListItemAllFields { get; }

		IFolderCollection Folders { get; }

		IFileCollection Files { get; }

		void DeleteObject();
	}
}
