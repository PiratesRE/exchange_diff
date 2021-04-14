using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MockTeamMailboxClientOperations : TeamMailboxClientOperations
	{
		public MockTeamMailboxClientOperations(MiniRecipient miniRecipient) : base(null, false, miniRecipient, TimeSpan.MinValue, false)
		{
		}

		protected override void SharePointCreateFolder(string siteUrl, string parentFolderUrl, Guid listId, string folderName, out Guid? uniqueId, out string folderUrl)
		{
			MockTeamMailboxClientOperations.SharePointServer.CreateFolder(parentFolderUrl, folderName, out uniqueId, out folderUrl);
		}

		protected override void SharePointlDeleteFolder(string siteUrl, string folderUrl)
		{
			MockTeamMailboxClientOperations.SharePointServer.DeleteFolder(folderUrl);
		}

		protected override void SharePointCreateFile(string siteUrl, string folderUrl, string fileName, Stream contentStream, out Guid uniqueId, out string fileUrl, out int fileSize)
		{
			MockTeamMailboxClientOperations.SharePointServer.CreateFile(folderUrl, fileName, out uniqueId, out fileUrl, out fileSize);
		}

		protected override void SharePointDeleteFile(string siteUrl, string fileUrl, bool isSharePointRecycleBinEnabled)
		{
			MockTeamMailboxClientOperations.SharePointServer.DeleteFile(fileUrl);
		}

		protected override void SharePointMoveOrCopyFile(bool isCopy, string siteUrl, string sourceFileUrl, string destinationFolderUrl, out Guid uniqueId, out string destinationFileUrl)
		{
			throw new NotImplementedException();
		}

		protected override bool SharePointIsRecycleBinEnabled(string siteUrl)
		{
			return true;
		}

		public const string MockFailedCreateFileName = "6e2e937a-238f-4362-b4a1-51b5acc04c2f";

		private static readonly MockTeamMailboxClientOperations.MockSharePointServer SharePointServer = new MockTeamMailboxClientOperations.MockSharePointServer();

		private class SharePointItem
		{
			public Guid UniqueId { get; private set; }

			public string Url { get; private set; }

			public SharePointItem(string url)
			{
				this.Url = url;
				this.UniqueId = Guid.NewGuid();
			}
		}

		private sealed class SharePointFile : MockTeamMailboxClientOperations.SharePointItem
		{
			public int Size { get; private set; }

			public SharePointFile(string url) : base(url)
			{
				Random random = new Random();
				this.Size = random.Next(1024, 10485760);
			}
		}

		private sealed class SharePointFolder : MockTeamMailboxClientOperations.SharePointItem
		{
			public SharePointFolder(string url) : base(url)
			{
			}

			public bool CreateFile(string url, out MockTeamMailboxClientOperations.SharePointFile file)
			{
				file = null;
				if (this.files.ContainsKey(url))
				{
					return false;
				}
				file = new MockTeamMailboxClientOperations.SharePointFile(url);
				this.files.Add(url, file);
				return true;
			}

			public void DeleteFile(string url)
			{
				if (this.files.ContainsKey(url))
				{
					this.files.Remove(url);
				}
			}

			private readonly Dictionary<string, MockTeamMailboxClientOperations.SharePointFile> files = new Dictionary<string, MockTeamMailboxClientOperations.SharePointFile>();
		}

		private sealed class MockSharePointServer
		{
			public void CreateFile(string folderUrl, string fileName, out Guid uniqueId, out string fileUrl, out int fileSize)
			{
				MockTeamMailboxClientOperations.SharePointFolder sharePointFolder = null;
				uniqueId = Guid.Empty;
				fileUrl = null;
				fileSize = 0;
				if (fileName.Equals("6e2e937a-238f-4362-b4a1-51b5acc04c2f", StringComparison.OrdinalIgnoreCase))
				{
					throw new SharePointException(folderUrl, new LocalizedString("Failed to create file"));
				}
				lock (this.syncObject)
				{
					if (!this.folders.TryGetValue(folderUrl, out sharePointFolder))
					{
						sharePointFolder = new MockTeamMailboxClientOperations.SharePointFolder(folderUrl);
						this.folders.Add(folderUrl, sharePointFolder);
					}
					fileUrl = folderUrl + "/" + fileName;
					MockTeamMailboxClientOperations.SharePointFile sharePointFile = null;
					if (!sharePointFolder.CreateFile(fileUrl, out sharePointFile))
					{
						fileUrl = null;
						throw new SharePointException(fileUrl, new LocalizedString(string.Format("CreateFile:File {0} already existed in {1}", fileName, folderUrl)));
					}
					uniqueId = sharePointFile.UniqueId;
					fileSize = sharePointFile.Size;
				}
			}

			public void DeleteFile(string fileUrl)
			{
				lock (this.syncObject)
				{
					string key = fileUrl.Substring(0, fileUrl.LastIndexOf("/"));
					if (this.folders.ContainsKey(key))
					{
						this.folders[key].DeleteFile(fileUrl);
					}
				}
			}

			public void CreateFolder(string parentFolderUrl, string folderName, out Guid? uniqueId, out string folderUrl)
			{
				lock (this.syncObject)
				{
					uniqueId = new Guid?(Guid.Empty);
					folderUrl = parentFolderUrl + "/" + folderName;
					if (this.folders.ContainsKey(folderUrl))
					{
						folderUrl = null;
						throw new SharePointException(folderUrl, new LocalizedString(string.Format("CreateFolder:Folder {0} already under in {1}", folderName, parentFolderUrl)));
					}
					MockTeamMailboxClientOperations.SharePointFolder sharePointFolder = new MockTeamMailboxClientOperations.SharePointFolder(folderUrl);
					this.folders.Add(folderUrl, sharePointFolder);
					uniqueId = new Guid?(sharePointFolder.UniqueId);
				}
			}

			public void DeleteFolder(string folderUrl)
			{
				lock (this.syncObject)
				{
					if (this.folders.ContainsKey(folderUrl))
					{
						this.folders.Remove(folderUrl);
					}
				}
			}

			private readonly object syncObject = new object();

			private readonly Dictionary<string, MockTeamMailboxClientOperations.SharePointFolder> folders = new Dictionary<string, MockTeamMailboxClientOperations.SharePointFolder>();
		}
	}
}
