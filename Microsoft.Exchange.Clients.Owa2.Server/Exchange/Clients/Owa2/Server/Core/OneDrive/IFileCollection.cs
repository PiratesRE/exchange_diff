using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IFileCollection : IClientObject<FileCollection>
	{
		IFile Add(FileCreationInformation parameters);
	}
}
