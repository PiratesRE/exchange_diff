using System;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockFile : MockClientObject<File>, IFile, IClientObject<File>
	{
		public string Name { get; private set; }

		public string ServerRelativeUrl { get; private set; }

		public long Length { get; private set; }

		public bool Exists { get; private set; }

		public string LinkingUrl
		{
			get
			{
				return new Uri(new Uri(this.context.Url), this.ServerRelativeUrl).ToString();
			}
		}

		public IListItem ListItemAllFields { get; private set; }

		public MockFile(string relativeLocation, MockClientContext context)
		{
			this.ServerRelativeUrl = relativeLocation;
			this.context = context;
		}

		public MockFile(MockListItem mockListItem, MockClientContext context)
		{
			this.ListItemAllFields = mockListItem;
			this.context = context;
		}

		public MockFile(FileCreationInformation parameters, string folderRelativeUrl, MockClientContext context)
		{
			this.fileCreationInformation = parameters;
			this.folderRelativeUrl = folderRelativeUrl;
			this.context = context;
		}

		public override void LoadMockData()
		{
			if (this.ServerRelativeUrl != null)
			{
				this.actualPath = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, this.ServerRelativeUrl);
				FileInfo fileInfo = new FileInfo(this.actualPath);
				if (this.Exists = fileInfo.Exists)
				{
					this.ListItemAllFields = new MockListItem(fileInfo, Path.GetDirectoryName(this.ServerRelativeUrl), this.context);
					this.Name = fileInfo.Name;
					this.Length = fileInfo.Length;
				}
			}
			else if (this.ListItemAllFields != null)
			{
				this.ServerRelativeUrl = this.ListItemAllFields["FileRef"].ToString();
				this.Name = this.ListItemAllFields["FileLeafRef"].ToString();
				this.Length = (long)this.ListItemAllFields["File_x0020_Size"];
				this.actualPath = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, this.ServerRelativeUrl);
				this.Exists = new FileInfo(this.actualPath).Exists;
			}
			else if (this.fileCreationInformation != null)
			{
				this.ServerRelativeUrl = Path.Combine(this.folderRelativeUrl, this.fileCreationInformation.Url);
				this.actualPath = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, this.ServerRelativeUrl);
				FileInfo fileInfo2 = new FileInfo(this.actualPath);
				if (!fileInfo2.Exists || this.fileCreationInformation.Overwrite)
				{
					using (FileStream fileStream = fileInfo2.Create())
					{
						Stream contentStream = this.fileCreationInformation.ContentStream;
						contentStream.CopyTo(fileStream);
					}
				}
				fileInfo2 = new FileInfo(this.actualPath);
				this.ListItemAllFields = new MockListItem(fileInfo2, Path.GetDirectoryName(this.ServerRelativeUrl), this.context);
				this.Name = fileInfo2.Name;
				this.Length = fileInfo2.Length;
				this.Exists = fileInfo2.Exists;
			}
			if (this.openBinaryStreamResult != null)
			{
				Stream stream = new FileStream(this.actualPath, FileMode.Open);
				this.context.AddToDisposeList(stream);
				this.openBinaryStreamResult.Value = stream;
			}
		}

		public IClientResult<Stream> OpenBinaryStream()
		{
			return this.openBinaryStreamResult = new MockClientResult<Stream>();
		}

		private readonly string folderRelativeUrl;

		private MockClientContext context;

		private string actualPath;

		private FileCreationInformation fileCreationInformation;

		private MockClientResult<Stream> openBinaryStreamResult;
	}
}
