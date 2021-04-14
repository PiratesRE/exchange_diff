using System;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;
using Microsoft.Exchange.InfoWorker.Common;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxTargetFolderProvider : TargetFolderProvider<string, BaseFolderType, ITargetMailbox>
	{
		public MailboxTargetFolderProvider(IExportContext exportContext, ITargetMailbox targetMailbox)
		{
			Util.ThrowIfNull(exportContext, "exportContext");
			Util.ThrowIfNull(targetMailbox, "targetMailbox");
			this.exportContext = exportContext;
			this.targetMailbox = targetMailbox;
		}

		protected override BaseFolderType CreateFolder(ITargetMailbox targetSession, BaseFolderType parentFolder, string folderName)
		{
			BaseFolderType baseFolderType = targetSession.GetFolderByName(parentFolder.FolderId, folderName);
			if (baseFolderType == null)
			{
				baseFolderType = targetSession.CreateFolder(parentFolder.FolderId, folderName, false);
			}
			return baseFolderType;
		}

		protected override string GenerateTopLevelFolderName(bool isArchive)
		{
			return isArchive ? Strings.ArchiveMailbox : Strings.PrimaryMailbox;
		}

		protected override BaseFolderType GetFolder(ITargetMailbox targetSession, string folderId)
		{
			return targetSession.GetFolder(folderId);
		}

		protected override string GetFolderId(BaseFolderType folder)
		{
			return folder.FolderId.Id;
		}

		protected override void InitializeTargetFolderHierarchy()
		{
			string resultFolderName;
			if (this.exportContext.ExportMetadata.IncludeDuplicates)
			{
				string arg = base.DataContext.IsPublicFolder ? "Public Folders" : base.DataContext.SourceName;
				resultFolderName = string.Format("{0}-{1}", arg, this.targetMailbox.ExportSettings.ExportTime);
			}
			else
			{
				resultFolderName = string.Format("{0}-{1}", "Results", this.targetMailbox.ExportSettings.ExportTime);
			}
			string text = this.targetMailbox.CreateResultFolder(resultFolderName);
			if (base.DataContext.IsUnsearchable)
			{
				FolderIdType parentFolderId = new FolderIdType
				{
					Id = text
				};
				BaseFolderType baseFolderType = this.targetMailbox.GetFolderByName(parentFolderId, Strings.Unsearchable);
				if (baseFolderType == null)
				{
					baseFolderType = this.targetMailbox.CreateFolder(parentFolderId, Strings.Unsearchable, false);
				}
				text = baseFolderType.FolderId.Id;
			}
			base.FolderMapping.Add("", text);
		}

		private const string Results = "Results";

		private readonly ITargetMailbox targetMailbox;

		private readonly IExportContext exportContext;
	}
}
