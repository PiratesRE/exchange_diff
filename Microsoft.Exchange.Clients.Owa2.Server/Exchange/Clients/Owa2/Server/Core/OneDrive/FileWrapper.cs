using System;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class FileWrapper : ClientObjectWrapper<File>, IFile, IClientObject<File>
	{
		public FileWrapper(File file) : base(file)
		{
			this.backingFile = file;
		}

		public string Name
		{
			get
			{
				return this.backingFile.Name;
			}
		}

		public string ServerRelativeUrl
		{
			get
			{
				return this.backingFile.ServerRelativeUrl;
			}
		}

		public long Length
		{
			get
			{
				return this.backingFile.Length;
			}
		}

		public IListItem ListItemAllFields
		{
			get
			{
				IListItem result;
				if ((result = this.listItem) == null)
				{
					result = (this.listItem = new ListItemWrapper(this.backingFile.ListItemAllFields));
				}
				return result;
			}
		}

		public bool Exists
		{
			get
			{
				return this.backingFile.Exists;
			}
		}

		public string LinkingUrl
		{
			get
			{
				return this.backingFile.LinkingUrl;
			}
		}

		public IClientResult<Stream> OpenBinaryStream()
		{
			return new ClientResultWrapper<Stream>(this.backingFile.OpenBinaryStream());
		}

		private File backingFile;

		private IListItem listItem;
	}
}
