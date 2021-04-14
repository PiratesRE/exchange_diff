using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal abstract class TargetFolderProvider<TFolderId, TFolder, TTargetSession> : TargetFolderProviderBase where TFolder : class
	{
		private protected DataContext DataContext { protected get; private set; }

		private protected Dictionary<string, TFolderId> FolderMapping { protected get; private set; }

		public void Reset(DataContext dataContext)
		{
			this.DataContext = dataContext;
			this.FolderMapping = new Dictionary<string, TFolderId>();
			this.folderCache = new Dictionary<TFolderId, TFolder>();
			this.InitializeTargetFolderHierarchy();
		}

		public TFolder GetParentFolder(TTargetSession targetSession, string parentFolderPath, bool mapFolderHierarchy)
		{
			if (parentFolderPath == null)
			{
				return default(TFolder);
			}
			if (!mapFolderHierarchy)
			{
				parentFolderPath = "";
			}
			return this.CreateMappedFolder(targetSession, parentFolderPath);
		}

		protected abstract void InitializeTargetFolderHierarchy();

		protected abstract TFolder GetFolder(TTargetSession targetSession, TFolderId folderId);

		protected abstract string GenerateTopLevelFolderName(bool isArchive);

		protected abstract TFolder CreateFolder(TTargetSession targetSession, TFolder parentFolder, string folderName);

		protected abstract TFolderId GetFolderId(TFolder folder);

		private TFolder CreateMappedFolder(TTargetSession targetSession, string serverFolderPath)
		{
			string text = serverFolderPath;
			TFolderId tfolderId;
			while (!this.FolderMapping.TryGetValue(text, out tfolderId))
			{
				text = text.Substring(0, text.LastIndexOf('\\'));
			}
			TFolder tfolder = default(TFolder);
			if (!this.folderCache.TryGetValue(tfolderId, out tfolder))
			{
				tfolder = this.GetFolder(targetSession, tfolderId);
				this.folderCache.Add(tfolderId, tfolder);
			}
			string text2 = serverFolderPath.Substring(text.Length, serverFolderPath.Length - text.Length);
			if (text2.Length > 0)
			{
				string[] array = text2.Split(new char[]
				{
					'\\'
				}, StringSplitOptions.RemoveEmptyEntries);
				StringBuilder stringBuilder = new StringBuilder(text);
				string[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					string text3 = array2[i];
					stringBuilder.Append("\\");
					stringBuilder.Append(text3);
					text = stringBuilder.ToString();
					string a;
					if ((a = text) == null)
					{
						goto IL_105;
					}
					string folderName;
					if (!(a == "\\root"))
					{
						if (!(a == "\\archive"))
						{
							goto IL_105;
						}
						folderName = this.GenerateTopLevelFolderName(true);
					}
					else
					{
						folderName = this.GenerateTopLevelFolderName(false);
					}
					IL_109:
					tfolder = this.CreateFolder(targetSession, tfolder, folderName);
					TFolderId folderId = this.GetFolderId(tfolder);
					this.FolderMapping.Add(text, folderId);
					this.folderCache.Add(folderId, tfolder);
					i++;
					continue;
					IL_105:
					folderName = text3;
					goto IL_109;
				}
			}
			return tfolder;
		}

		private Dictionary<TFolderId, TFolder> folderCache;
	}
}
