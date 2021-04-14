using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class FileCollectionWrapper : ClientObjectWrapper<FileCollection>, IFileCollection, IClientObject<FileCollection>
	{
		public FileCollectionWrapper(FileCollection files) : base(files)
		{
			this.backingFileCollection = files;
		}

		public IFile Add(FileCreationInformation parameters)
		{
			return new FileWrapper(this.backingFileCollection.Add(parameters));
		}

		private FileCollection backingFileCollection;
	}
}
