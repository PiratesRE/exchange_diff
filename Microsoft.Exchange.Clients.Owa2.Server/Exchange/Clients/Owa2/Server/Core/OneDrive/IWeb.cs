using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IWeb : IClientObject<Web>
	{
		IListCollection Lists { get; }

		IFolder GetFolderByServerRelativeUrl(string serverRelativeUrl);

		IFile GetFileByServerRelativeUrl(string relativeLocation);

		IList GetList(string url);
	}
}
