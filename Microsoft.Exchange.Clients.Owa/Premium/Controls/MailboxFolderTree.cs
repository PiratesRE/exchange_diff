using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class MailboxFolderTree : FolderTree
	{
		private MailboxFolderTree(UserContext userContext, FolderTreeNode rootNode, FolderTreeRenderType renderType) : base(userContext, rootNode, renderType)
		{
		}

		private MailboxFolderTree(UserContext userContext, MailboxSession mailboxSession, FolderTreeNode rootNode, FolderTreeRenderType renderType) : base(userContext, rootNode, renderType)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (!mailboxSession.MailboxOwner.MailboxInfo.IsArchive)
			{
				this.defaultCalendarFolderId = Utilities.TryGetDefaultFolderId(mailboxSession, DefaultFolderType.Calendar);
				this.defaultContactFolderId = Utilities.TryGetDefaultFolderId(mailboxSession, DefaultFolderType.Contacts);
				this.defaultTaskFolderId = Utilities.TryGetDefaultFolderId(mailboxSession, DefaultFolderType.Tasks);
			}
			this.isRemote = mailboxSession.MailboxOwner.MailboxInfo.IsRemote;
		}

		protected override void RenderAdditionalProperties(TextWriter writer)
		{
			base.RenderAdditionalProperties(writer);
			if (this.defaultCalendarFolderId != null)
			{
				writer.Write(" _DfCal=\"");
				Utilities.HtmlEncode(this.defaultCalendarFolderId.ToBase64String(), writer);
				writer.Write("\"");
			}
			if (this.defaultContactFolderId != null)
			{
				writer.Write(" _DfCnt=\"");
				Utilities.HtmlEncode(this.defaultContactFolderId.ToBase64String(), writer);
				writer.Write("\"");
			}
			if (this.defaultTaskFolderId != null)
			{
				writer.Write(" _DfTsk=\"");
				Utilities.HtmlEncode(this.defaultTaskFolderId.ToBase64String(), writer);
				writer.Write("\"");
			}
			if (this.isRemote)
			{
				writer.Write(" _IsRemote=\"1\"");
			}
		}

		internal static MailboxFolderTree CreateOtherMailboxFolderTree(UserContext userContext, OtherMailboxConfigEntry entry, bool isExpanded)
		{
			FolderTreeNode folderTreeNode = FolderTreeNode.CreateOtherMailboxRootNode(userContext, entry, isExpanded);
			if (folderTreeNode == null)
			{
				return null;
			}
			folderTreeNode.IsExpanded = isExpanded;
			FolderTreeNode folderTreeNode2 = folderTreeNode;
			folderTreeNode2.HighlightClassName += " trNdGpHdHl";
			return new MailboxFolderTree(userContext, folderTreeNode, FolderTreeRenderType.None);
		}

		internal static MailboxFolderTree CreateMailboxFolderTree(UserContext userContext, MailboxSession mailboxSession, FolderTreeRenderType renderType, bool selectInbox)
		{
			MailboxFolderTree mailboxFolderTree = new MailboxFolderTree(userContext, mailboxSession, FolderTreeNode.CreateMailboxFolderTreeRootNode(userContext, mailboxSession, renderType), renderType);
			mailboxFolderTree.RootNode.IsExpanded = true;
			FolderTreeNode rootNode = mailboxFolderTree.RootNode;
			rootNode.HighlightClassName += " trNdGpHdHl";
			if (selectInbox)
			{
				StoreObjectId defaultFolderId = Utilities.GetDefaultFolderId(mailboxSession, DefaultFolderType.Inbox);
				OwaStoreObjectId folderId;
				if (userContext.IsMyMailbox(mailboxSession))
				{
					folderId = OwaStoreObjectId.CreateFromMailboxFolderId(defaultFolderId);
				}
				else
				{
					folderId = OwaStoreObjectId.CreateFromOtherUserMailboxFolderId(defaultFolderId, mailboxSession.MailboxOwnerLegacyDN);
				}
				mailboxFolderTree.RootNode.SelectSpecifiedFolder(folderId);
			}
			return mailboxFolderTree;
		}

		internal static MailboxFolderTree CreateStartPageMailboxFolderTree(UserContext userContext, FolderList deepHierarchyFolderList, FolderList searchFolderList)
		{
			return new MailboxFolderTree(userContext, userContext.MailboxSession, FolderTreeNode.CreateStartPageMailboxRootNode(userContext, deepHierarchyFolderList, searchFolderList), FolderTreeRenderType.HideGeekFoldersWithSpecificOrder);
		}

		internal static MailboxFolderTree CreateStartPageArchiveMailboxFolderTree(UserContext userContext, FolderList deepHierarchyFolderList, FolderList searchFolderList)
		{
			return new MailboxFolderTree(userContext, deepHierarchyFolderList.MailboxSession, FolderTreeNode.CreateStartPageArchiveMailboxRootNode(userContext, deepHierarchyFolderList, searchFolderList), FolderTreeRenderType.HideGeekFoldersWithSpecificOrder);
		}

		internal static MailboxFolderTree CreateStartPageDummyArchiveMailboxFolderTree(UserContext userContext)
		{
			return new MailboxFolderTree(userContext, FolderTreeNode.CreateStartPageDummyArchiveMailboxRootNode(userContext), FolderTreeRenderType.HideGeekFoldersWithSpecificOrder);
		}

		private readonly StoreObjectId defaultCalendarFolderId;

		private readonly StoreObjectId defaultContactFolderId;

		private readonly StoreObjectId defaultTaskFolderId;

		private readonly bool isRemote;
	}
}
