using System;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IFile : IClientObject<File>
	{
		string Name { get; }

		string ServerRelativeUrl { get; }

		long Length { get; }

		IListItem ListItemAllFields { get; }

		bool Exists { get; }

		string LinkingUrl { get; }

		IClientResult<Stream> OpenBinaryStream();
	}
}
