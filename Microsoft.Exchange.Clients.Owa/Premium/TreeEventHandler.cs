using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Tree")]
	internal sealed class TreeEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterEnum(typeof(FolderTreeRenderType));
			OwaEventRegistry.RegisterHandler(typeof(TreeEventHandler));
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEventParameter("rdt", typeof(FolderTreeRenderType))]
		[OwaEvent("Expand")]
		[OwaEventParameter("id", typeof(string))]
		public void Expand()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "TreeEventHandler.Expand");
			string text = (string)base.GetParameter("id");
			bool flag = false;
			if (OwaStoreObjectId.IsDummyArchiveFolder(text))
			{
				text = base.UserContext.GetArchiveRootFolderIdString();
				flag = !string.IsNullOrEmpty(text);
			}
			this.RenderFolderTreeChangedNode(OwaStoreObjectId.CreateFromString(text), null, true, flag, (FolderTreeRenderType)base.GetParameter("rdt"));
			if (flag)
			{
				NavigationHost.RenderFavoritesAndNavigationTrees(this.Writer, base.UserContext, null, new NavigationNodeGroupSection[]
				{
					NavigationNodeGroupSection.First
				});
			}
		}

		[OwaEventParameter("exp", typeof(bool))]
		[OwaEventParameter("fSrcD", typeof(bool))]
		[OwaEventParameter("fDstD", typeof(bool))]
		[OwaEvent("Move")]
		[OwaEventParameter("rdt", typeof(FolderTreeRenderType))]
		[OwaEventParameter("cms", typeof(NavigationModule), true)]
		[OwaEventParameter("destId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		public void Move()
		{
			this.CopyOrMoveFolder(false);
		}

		[OwaEventParameter("fSrcD", typeof(bool))]
		[OwaEventParameter("destId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("fDstD", typeof(bool))]
		[OwaEventParameter("exp", typeof(bool))]
		[OwaEventParameter("rdt", typeof(FolderTreeRenderType))]
		[OwaEventParameter("cms", typeof(NavigationModule), true)]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		[OwaEvent("Copy")]
		public void Copy()
		{
			this.CopyOrMoveFolder(true);
		}

		private void CopyOrMoveFolder(bool isCopy)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "TreeEventHandler." + (isCopy ? "Copy" : "Move"));
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			OwaStoreObjectId owaStoreObjectId2 = (OwaStoreObjectId)base.GetParameter("destId");
			bool isExpanded = (bool)base.GetParameter("exp");
			if (owaStoreObjectId.IsOtherMailbox || owaStoreObjectId2.IsOtherMailbox)
			{
				throw new OwaInvalidRequestException("Cannot copy or move a shared folder");
			}
			if (Utilities.IsDefaultFolderId(base.UserContext, owaStoreObjectId, DefaultFolderType.SearchFolders) || Utilities.IsDefaultFolderId(base.UserContext, owaStoreObjectId2, DefaultFolderType.SearchFolders))
			{
				throw new OwaInvalidRequestException("Cannot Copy or Move Search Folder");
			}
			NavigationTreeDirtyFlag navigationTreeDirtyFlag = NavigationTreeDirtyFlag.None;
			string displayName;
			using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, owaStoreObjectId, new PropertyDefinition[]
			{
				FolderSchema.DisplayName,
				StoreObjectSchema.ContainerClass,
				FolderSchema.IsOutlookSearchFolder,
				FolderSchema.AdminFolderFlags,
				StoreObjectSchema.ParentEntryId
			}))
			{
				displayName = folder.DisplayName;
				string className = folder.ClassName;
				if (Utilities.IsOutlookSearchFolder(folder))
				{
					throw new OwaInvalidRequestException("Cannot Copy or Move Search Folders");
				}
				if (!this.CanFolderHaveSubFolders(owaStoreObjectId2))
				{
					throw new OwaInvalidRequestException("Cannot Copy or Move a folder to this destination");
				}
				if (Utilities.IsELCFolder(folder))
				{
					throw new OwaInvalidRequestException(string.Format("Cannot {0} ELC folders.", isCopy ? "Copy" : "Move"));
				}
				if (!isCopy && ((!owaStoreObjectId.IsPublic && Utilities.IsSpecialFolderForSession(folder.Session as MailboxSession, owaStoreObjectId.StoreObjectId)) || Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
				{
					ExtendedFolderFlags.RemoteHierarchy
				})))
				{
					throw new OwaInvalidRequestException("Cannot move folders that are special or controlled remotely.");
				}
				if (base.UserContext.IsPublicFolderRootId(owaStoreObjectId.StoreObjectId))
				{
					throw new OwaEventHandlerException("Copy/move public root folder is not supported", LocalizedStrings.GetNonEncoded(-177785786), true);
				}
				bool flag = owaStoreObjectId.IsPublic || (bool)base.GetParameter("fSrcD");
				bool flag2 = owaStoreObjectId2.IsPublic || (bool)base.GetParameter("fDstD");
				bool flag3 = !isCopy && owaStoreObjectId.IsArchive != owaStoreObjectId2.IsArchive;
				if (((!flag || !flag2) && (!isCopy || !flag2) && (isCopy || flag || flag2)) || flag3)
				{
					navigationTreeDirtyFlag = this.CheckNavigationTreeDirtyFlag(folder, true);
					if (isCopy || flag)
					{
						navigationTreeDirtyFlag &= ~NavigationTreeDirtyFlag.Favorites;
					}
				}
			}
			if (owaStoreObjectId2.IsArchive)
			{
				navigationTreeDirtyFlag |= NavigationTreeDirtyFlag.Favorites;
			}
			OperationResult operationResult = Utilities.CopyOrMoveFolder(base.UserContext, isCopy, owaStoreObjectId2, new OwaStoreObjectId[]
			{
				owaStoreObjectId
			}).OperationResult;
			if (operationResult == OperationResult.Failed)
			{
				throw new OwaEventHandlerException(isCopy ? "Copy returned an OperationResult.Failed" : "Move returned an OperationResult.Failed", LocalizedStrings.GetNonEncoded(-1597406995));
			}
			if (operationResult == OperationResult.PartiallySucceeded)
			{
				throw new OwaEventHandlerException((isCopy ? "Copy" : "Move") + " returned an OperationResult.PartiallySucceeded", LocalizedStrings.GetNonEncoded(2109230231));
			}
			bool flag4 = true;
			if (!isCopy && owaStoreObjectId.IsPublic == owaStoreObjectId2.IsPublic && owaStoreObjectId.IsArchive == owaStoreObjectId2.IsArchive && StringComparer.InvariantCultureIgnoreCase.Equals(owaStoreObjectId.MailboxOwnerLegacyDN, owaStoreObjectId2.MailboxOwnerLegacyDN))
			{
				flag4 = false;
			}
			OwaStoreObjectId newFolderId;
			if (flag4)
			{
				newFolderId = this.GetSubFolderIdByName(owaStoreObjectId2, displayName);
			}
			else
			{
				newFolderId = owaStoreObjectId;
			}
			this.RenderFolderTreeChangedNode(owaStoreObjectId2, newFolderId, isExpanded, owaStoreObjectId2.IsArchive, (FolderTreeRenderType)base.GetParameter("rdt"));
			RenderingUtilities.RenderNavigationTreeDirtyFlag(this.Writer, base.UserContext, navigationTreeDirtyFlag, (NavigationModule[])base.GetParameter("cms"));
		}

		[OwaEventParameter("fC", typeof(string))]
		[OwaEventParameter("cms", typeof(NavigationModule), true)]
		[OwaEvent("New")]
		[OwaEventParameter("destId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("fDstD", typeof(bool))]
		[OwaEventParameter("fN", typeof(string))]
		[OwaEventParameter("exp", typeof(bool))]
		[OwaEventParameter("rdt", typeof(FolderTreeRenderType))]
		public void New()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "TreeEventHandler.New");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("destId");
			if (owaStoreObjectId.IsOtherMailbox)
			{
				throw new OwaInvalidRequestException("Cannot add new folder underneath a shared folder");
			}
			string text = (string)base.GetParameter("fC");
			string text2 = (string)base.GetParameter("fN");
			bool isExpanded = (bool)base.GetParameter("exp");
			if (Utilities.IsDefaultFolderId(base.UserContext, owaStoreObjectId, DefaultFolderType.SearchFolders))
			{
				throw new OwaInvalidRequestException("Cannot Create new Search Folder through OWA");
			}
			if (Utilities.IsELCRootFolder(owaStoreObjectId, base.UserContext))
			{
				throw new OwaInvalidRequestException("Cannot create new folders under the root ELC folder.");
			}
			text2 = text2.Trim();
			if (text2.Length == 0)
			{
				throw new OwaEventHandlerException("User did not provide name for new folder", LocalizedStrings.GetNonEncoded(-41080803), true);
			}
			StoreObjectType objectType = owaStoreObjectId.StoreObjectId.ObjectType;
			if (!this.CanFolderHaveSubFolders(owaStoreObjectId))
			{
				throw new OwaInvalidRequestException("Cannot Create new Search Folder through OWA");
			}
			using (Folder folder = Utilities.CreateSubFolder(owaStoreObjectId, objectType, text2, base.UserContext))
			{
				folder.ClassName = text;
				try
				{
					folder.Save();
				}
				catch (ObjectExistedException)
				{
					throw;
				}
				catch (StoragePermanentException innerException)
				{
					throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(995407892), innerException);
				}
				folder.Load();
				OwaStoreObjectId newFolderId = OwaStoreObjectId.CreateFromStoreObject(folder);
				this.RenderFolderTreeChangedNode(owaStoreObjectId, newFolderId, isExpanded, owaStoreObjectId.IsArchive, (FolderTreeRenderType)base.GetParameter("rdt"));
				NavigationTreeDirtyFlag navigationTreeDirtyFlag = FolderTreeNode.GetAffectedTreeFromContainerClass(text);
				if (owaStoreObjectId.IsArchive)
				{
					navigationTreeDirtyFlag |= NavigationTreeDirtyFlag.Favorites;
				}
				if (navigationTreeDirtyFlag != NavigationTreeDirtyFlag.Favorites || owaStoreObjectId.IsArchive)
				{
					RenderingUtilities.RenderNavigationTreeDirtyFlag(this.Writer, base.UserContext, navigationTreeDirtyFlag, (NavigationModule[])base.GetParameter("cms"));
				}
			}
		}

		[OwaEventParameter("destId", typeof(OwaStoreObjectId))]
		[OwaEvent("Rename")]
		[OwaEventParameter("fN", typeof(string))]
		public void Rename()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "TreeEventHandler.Rename");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("destId");
			string text = ((string)base.GetParameter("fN")).Trim();
			string s = text;
			if (text.Length == 0)
			{
				throw new OwaEventHandlerException("User did not provide name for new folder", LocalizedStrings.GetNonEncoded(-41080803), true);
			}
			using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]))
			{
				if (!Utilities.CanFolderBeRenamed(base.UserContext, folder))
				{
					throw new OwaInvalidRequestException("Folder cannot be renamed.");
				}
				folder.DisplayName = text;
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					if (Utilities.IsFolderNameConflictError(folderSaveResult))
					{
						throw new OwaEventHandlerException("Folder rename did not return OperationResult.Succeeded", LocalizedStrings.GetNonEncoded(1602494619), OwaEventHandlerErrorCode.FolderNameExists, true);
					}
					throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(995407892));
				}
				else
				{
					if (owaStoreObjectId.IsArchive)
					{
						s = string.Format(LocalizedStrings.GetNonEncoded(-83764036), text, Utilities.GetMailboxOwnerDisplayName((MailboxSession)folder.Session));
					}
					this.Writer.Write("<div id=tn>");
					Utilities.HtmlEncode(text, this.Writer, true);
					this.Writer.Write("</div><div id=ntn>");
					Utilities.HtmlEncode(s, this.Writer, true);
					this.Writer.Write("</div>");
				}
			}
		}

		[OwaEventParameter("fSrcD", typeof(bool))]
		[OwaEvent("Delete")]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("cms", typeof(NavigationModule), true)]
		[OwaEventParameter("pd", typeof(bool))]
		public void Delete()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			if (owaStoreObjectId.IsOtherMailbox || owaStoreObjectId.IsGSCalendar)
			{
				throw new OwaInvalidRequestException("Cannot perform delete on shared folder");
			}
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "TreeEventHandler.Delete");
			if (Utilities.IsDefaultFolderId(base.UserContext, owaStoreObjectId, DefaultFolderType.SearchFolders))
			{
				throw new OwaInvalidRequestException("Cannot Delete Search Folders");
			}
			bool flag = (bool)base.GetParameter("fSrcD");
			bool flag2 = flag || owaStoreObjectId.IsPublic || (bool)base.GetParameter("pd");
			NavigationTreeDirtyFlag flag3 = NavigationTreeDirtyFlag.None;
			using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, owaStoreObjectId, new PropertyDefinition[]
			{
				StoreObjectSchema.ContainerClass,
				FolderSchema.IsOutlookSearchFolder,
				FolderSchema.AdminFolderFlags,
				StoreObjectSchema.ParentEntryId
			}))
			{
				string className = folder.ClassName;
				if (Utilities.IsOutlookSearchFolder(folder))
				{
					throw new OwaInvalidRequestException("Cannot Delete Search Folders");
				}
				if (Utilities.IsELCFolder(folder))
				{
					throw new OwaInvalidRequestException("Cannot Delete ELC folders.");
				}
				if (Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
				{
					ExtendedFolderFlags.RemoteHierarchy
				}))
				{
					throw new OwaInvalidRequestException("Cannot delete a folder that is controlled remotely.");
				}
				if (!flag2 || (!owaStoreObjectId.IsPublic && !flag))
				{
					flag3 = this.CheckNavigationTreeDirtyFlag(folder, true);
				}
			}
			OperationResult operationResult = Utilities.Delete(base.UserContext, flag2, new OwaStoreObjectId[]
			{
				owaStoreObjectId
			}).OperationResult;
			if (operationResult == OperationResult.Failed)
			{
				Strings.IDs localizedId = flag2 ? -1691273193 : 1041829989;
				throw new OwaEventHandlerException("Delete returned an OperationResult.Failed", LocalizedStrings.GetNonEncoded(localizedId));
			}
			if (operationResult == OperationResult.PartiallySucceeded)
			{
				throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(995407892));
			}
			if (!flag2)
			{
				this.RenderFolderTreeChangedNode(base.UserContext.GetDeletedItemsFolderId((MailboxSession)owaStoreObjectId.GetSession(base.UserContext)), owaStoreObjectId, false, false, FolderTreeRenderType.None);
			}
			RenderingUtilities.RenderNavigationTreeDirtyFlag(this.Writer, base.UserContext, flag3, (NavigationModule[])base.GetParameter("cms"));
		}

		[OwaEvent("MarkAllAsRead")]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		public void MarkAllAsRead()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			if (Utilities.IsDefaultFolderId(base.UserContext, owaStoreObjectId, DefaultFolderType.SearchFolders))
			{
				throw new OwaInvalidRequestException("Cannot perform any operation on Search Folder Root");
			}
			using (Folder folderForContent = Utilities.GetFolderForContent<Folder>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]))
			{
				bool suppressReadReceipts = Utilities.ShouldSuppressReadReceipt(base.UserContext);
				if (owaStoreObjectId.StoreObjectId.Equals(base.UserContext.JunkEmailFolderId) || !base.UserContext.IsInMyMailbox(folderForContent))
				{
					suppressReadReceipts = true;
				}
				folderForContent.MarkAllAsRead(suppressReadReceipts);
			}
		}

		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		[OwaEvent("EmptyDeletedItems")]
		public void EmptyDeletedItems()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			if (!Utilities.IsDefaultFolderId(base.UserContext, owaStoreObjectId, DefaultFolderType.DeletedItems))
			{
				throw new OwaInvalidRequestException("Can only perform EmptyDeletedItems operation on DeletedItems folder");
			}
			owaStoreObjectId.GetSession(base.UserContext).DeleteAllObjects(DeleteItemFlags.SoftDelete, owaStoreObjectId.StoreObjectId);
		}

		[OwaEventParameter("cms", typeof(NavigationModule), true)]
		[OwaEvent("EmptyJunkEmail")]
		public void EmptyJunkEmail()
		{
			Folder folder = Utilities.SafeFolderBind(base.UserContext.MailboxSession, DefaultFolderType.JunkEmail, new PropertyDefinition[0]);
			NavigationTreeDirtyFlag flag = NavigationTreeDirtyFlag.None;
			if (folder != null)
			{
				using (folder)
				{
					flag = this.CheckNavigationTreeDirtyFlag(folder, false);
				}
			}
			base.UserContext.MailboxSession.DeleteAllObjects(DeleteItemFlags.SoftDelete | DeleteItemFlags.SuppressReadReceipt, base.UserContext.JunkEmailFolderId);
			RenderingUtilities.RenderNavigationTreeDirtyFlag(this.Writer, base.UserContext, flag, (NavigationModule[])base.GetParameter("cms"));
		}

		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		[OwaEvent("EmptyFolder")]
		public void EmptyFolder()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			if (Utilities.IsDefaultFolderId(base.UserContext, owaStoreObjectId, DefaultFolderType.SearchFolders))
			{
				throw new OwaInvalidRequestException("Cannot perform empty folder on Search Folder Root");
			}
			using (Folder folderForContent = Utilities.GetFolderForContent<Folder>(base.UserContext, owaStoreObjectId, new PropertyDefinition[]
			{
				FolderSchema.IsOutlookSearchFolder
			}))
			{
				if (!Utilities.IsOutlookSearchFolder(folderForContent))
				{
					DeleteItemFlags flags = owaStoreObjectId.IsPublic ? DeleteItemFlags.SoftDelete : DeleteItemFlags.MoveToDeletedItems;
					OperationResult operationResult = folderForContent.DeleteAllItems(flags).OperationResult;
					if (operationResult != OperationResult.Succeeded)
					{
						throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(166628739));
					}
				}
			}
		}

		[OwaEvent("GetMailboxUsage")]
		public void GetMailboxUsage()
		{
			RenderingUtilities.RenderMailboxQuota(this.Writer, base.UserContext);
		}

		[OwaEvent("GetRootPublicFolderId")]
		public void GetRootPublicFolderId()
		{
			string text = base.UserContext.TryGetPublicFolderRootIdString();
			if (text != null)
			{
				this.Writer.Write(text);
				return;
			}
			throw new OwaEventHandlerException("Cannot get the id of the root public folder", LocalizedStrings.GetNonEncoded(2071101893), true);
		}

		[OwaEvent("GetPublicFolderReplicaType")]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		public void GetPublicFolderReplicaType()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			this.Writer.Write("var iHasRplc = 1;");
		}

		[OwaEventParameter("mts", typeof(bool), false, true)]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("fts", typeof(bool), false, true)]
		[OwaEvent("PersistExpand")]
		[OwaEventParameter("bts", typeof(bool), false, true)]
		public void PersistExpandStatus()
		{
			base.ThrowIfCannotActAsOwner();
			if (base.UserContext.IsWebPartRequest)
			{
				return;
			}
			using (Folder folder = Utilities.SafeFolderBind(base.UserContext.MailboxSession, DefaultFolderType.Root, new PropertyDefinition[]
			{
				ViewStateProperties.TreeNodeCollapseStatus
			}))
			{
				if (folder != null)
				{
					int num = Utilities.GetFolderProperty<int>(folder, ViewStateProperties.TreeNodeCollapseStatus, 0);
					num = this.SetCollapsedTreeNodes(num, "fts", StatusPersistTreeNodeType.FavoritesRoot);
					num = this.SetCollapsedTreeNodes(num, "mts", StatusPersistTreeNodeType.CurrentNode);
					num = this.SetCollapsedTreeNodes(num, "bts", StatusPersistTreeNodeType.BuddyListRoot);
					folder[ViewStateProperties.TreeNodeCollapseStatus] = num;
					folder.Save();
				}
			}
		}

		private int SetCollapsedTreeNodes(int originalValue, string parameterName, StatusPersistTreeNodeType treeNodeType)
		{
			if (base.IsParameterSet(parameterName))
			{
				if ((bool)base.GetParameter(parameterName))
				{
					originalValue &= (int)(~(int)treeNodeType);
				}
				else
				{
					originalValue |= (int)treeNodeType;
				}
			}
			return originalValue;
		}

		private void RenderFolderTreeChangedNode(OwaStoreObjectId parentFolderId, OwaStoreObjectId newFolderId, bool isExpanded, bool updateFolderId, FolderTreeRenderType renderType)
		{
			this.Writer.Write("<div id=tn");
			if (base.UserContext.ArchiveAccessed && parentFolderId.Equals(base.UserContext.GetArchiveRootFolderId()))
			{
				this.Writer.Write(" archiveroot=\"1\"");
				MailboxSession mailboxSession = parentFolderId.GetSession(base.UserContext) as MailboxSession;
				if (mailboxSession != null && mailboxSession.MailboxOwner.MailboxInfo.IsRemote)
				{
					this.Writer.Write(" isremote=\"1\"");
				}
			}
			if (updateFolderId)
			{
				this.Writer.Write(" ufid=\"f");
				Utilities.HtmlEncode(parentFolderId.ToString(), this.Writer);
				this.Writer.Write("\"");
			}
			this.Writer.Write(">");
			if (isExpanded)
			{
				this.RenderSiblingNodes(parentFolderId, newFolderId, renderType);
			}
			else
			{
				if (newFolderId == null)
				{
					throw new ArgumentNullException("newFolderId");
				}
				FolderTreeNode folderTreeNode = FolderTreeNode.Load(base.UserContext, newFolderId, renderType);
				if (folderTreeNode != null)
				{
					FolderTreeNode folderTreeNode2 = folderTreeNode;
					folderTreeNode2.CustomAttributes += " _NF=1";
					folderTreeNode.RenderUndecoratedNode(this.Writer);
				}
			}
			this.Writer.Write("</div>");
		}

		private void RenderSiblingNodes(OwaStoreObjectId parentFolderId, OwaStoreObjectId newFolderId, FolderTreeRenderType renderType)
		{
			FolderTreeNode folderTreeNode;
			if (parentFolderId.IsPublic)
			{
				folderTreeNode = FolderTreeNode.CreatePublicFolderTreeNode(base.UserContext, parentFolderId.StoreObjectId);
			}
			else if (parentFolderId.IsOtherMailbox)
			{
				folderTreeNode = FolderTreeNode.CreateOtherMailboxRootNode(base.UserContext, parentFolderId, string.Empty, true);
				if (folderTreeNode == null)
				{
					throw new OwaEventHandlerException("User cannot view other's Inbox", LocalizedStrings.GetNonEncoded(995407892), true);
				}
			}
			else
			{
				folderTreeNode = FolderTreeNode.CreateMailboxFolderTreeNode(base.UserContext, (MailboxSession)parentFolderId.GetSession(base.UserContext), parentFolderId.StoreObjectId, renderType);
			}
			if (folderTreeNode == null)
			{
				return;
			}
			if (newFolderId != null)
			{
				foreach (TreeNode treeNode in folderTreeNode.Children)
				{
					FolderTreeNode folderTreeNode2 = (FolderTreeNode)treeNode;
					if (folderTreeNode2.FolderId.Equals(newFolderId))
					{
						FolderTreeNode folderTreeNode3 = folderTreeNode2;
						folderTreeNode3.CustomAttributes += " _NF=1";
					}
				}
			}
			folderTreeNode.RenderUndecoratedChildrenNode(this.Writer);
		}

		private NavigationTreeDirtyFlag CheckNavigationTreeDirtyFlag(Folder folder, bool includeSelf)
		{
			NavigationTreeDirtyFlag navigationTreeDirtyFlag = includeSelf ? FolderTreeNode.GetAffectedTreeFromContainerClass(folder.ClassName) : NavigationTreeDirtyFlag.None;
			object[][] array;
			using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, new PropertyDefinition[]
			{
				FolderSchema.IsHidden,
				StoreObjectSchema.ContainerClass
			}))
			{
				array = Utilities.FetchRowsFromQueryResult(queryResult, 10000);
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i][0] is bool) || !(bool)array[i][0])
				{
					navigationTreeDirtyFlag |= FolderTreeNode.GetAffectedTreeFromContainerClass(array[i][1] as string);
				}
			}
			return navigationTreeDirtyFlag;
		}

		private OwaStoreObjectId GetSubFolderIdByName(OwaStoreObjectId parentFolderId, string subFolderName)
		{
			OwaStoreObjectId result;
			using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, parentFolderId, new PropertyDefinition[0]))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
				{
					FolderSchema.Id
				}))
				{
					ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, subFolderName);
					bool flag = queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
					object[][] array = null;
					if (flag)
					{
						array = queryResult.GetRows(1);
					}
					if (array == null || array.Length == 0 || array[0].Length == 0)
					{
						ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "Can't find any subfolders of the destinationFolder with DisplayName matching the source folder's DisplayName");
						throw new OwaEventHandlerException("Can't find any subfolders of the destinationFolder with DisplayName matching the source folder's DisplayName", LocalizedStrings.GetNonEncoded(1073923836));
					}
					StoreObjectId objectId = ((VersionedId)array[0][0]).ObjectId;
					result = OwaStoreObjectId.CreateFromStoreObjectId(objectId, parentFolderId);
				}
			}
			return result;
		}

		private bool CanFolderHaveSubFolders(OwaStoreObjectId folderId)
		{
			if (folderId.IsPublic)
			{
				return true;
			}
			using (Folder folder = Folder.Bind(folderId.GetSession(base.UserContext), folderId.StoreObjectId, new PropertyDefinition[]
			{
				FolderSchema.IsOutlookSearchFolder,
				FolderSchema.ExtendedFolderFlags
			}))
			{
				if (Utilities.IsDefaultFolder(folder, DefaultFolderType.SearchFolders))
				{
					return false;
				}
				if (Utilities.IsOutlookSearchFolder(folder))
				{
					return false;
				}
				if (Utilities.IsELCRootFolder(folder))
				{
					return false;
				}
				if (Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
				{
					ExtendedFolderFlags.ExclusivelyBound,
					ExtendedFolderFlags.RemoteHierarchy
				}))
				{
					return false;
				}
			}
			return true;
		}

		public const string EventNamespace = "Tree";

		public const string MethodExpand = "Expand";

		public const string MethodPersistExpand = "PersistExpand";

		public const string MethodMove = "Move";

		public const string MethodCopy = "Copy";

		public const string MethodNew = "New";

		public const string MethodRename = "Rename";

		public const string MethodDelete = "Delete";

		public const string MethodMarkAllAsRead = "MarkAllAsRead";

		public const string MethodEmptyDeletedItems = "EmptyDeletedItems";

		public const string MethodEmptyJunkEmail = "EmptyJunkEmail";

		public const string MethodEmptyFolder = "EmptyFolder";

		public const string MethodGetMailboxUsage = "GetMailboxUsage";

		public const string MethodGetPublicFolderReplicaType = "GetPublicFolderReplicaType";

		public const string MethodGetRootPublicFolderId = "GetRootPublicFolderId";

		public const string Type = "t";

		public const string SourceFolderId = "id";

		public const string DestinationFolderId = "destId";

		public const string IsPermanentDelete = "pd";

		public const string FolderClass = "fC";

		public const string FolderName = "fN";

		public const string RenderType = "rdt";

		public const string FavoritesTreeStatus = "fts";

		public const string MailboxFolderTreeStatus = "mts";

		public const string BuddyListTreeStatus = "bts";

		public const string IsExpanded = "exp";

		public const string IsSourceFolderInDeletedItems = "fSrcD";

		public const string IsDestinationFolderInDeletedItems = "fDstD";

		public const string ClientNavigationModules = "cms";
	}
}
