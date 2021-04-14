using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class FolderCollectionWrapper : ClientObjectWrapper<FolderCollection>, IFolderCollection, IClientObject<FolderCollection>
	{
		public FolderCollectionWrapper(FolderCollection folders) : base(folders)
		{
			this.backingFolderCollection = folders;
		}

		public IFolder Add(string url)
		{
			return new FolderWrapper(this.backingFolderCollection.Add(url));
		}

		public IFolder GetByUrl(string url)
		{
			return new FolderWrapper(this.backingFolderCollection.GetByUrl(url));
		}

		private FolderCollection backingFolderCollection;
	}
}
