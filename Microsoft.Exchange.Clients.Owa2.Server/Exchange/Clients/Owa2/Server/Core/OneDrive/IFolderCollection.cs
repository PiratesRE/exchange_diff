using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IFolderCollection : IClientObject<FolderCollection>
	{
		IFolder Add(string url);

		IFolder GetByUrl(string url);
	}
}
