using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("NavigationNode")]
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	internal sealed class NavigationNodeEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(NavigationNodeEventHandler));
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("tgtid", typeof(StoreObjectId), false, true)]
		[OwaEventParameter("mt", typeof(int), false, true)]
		[OwaEvent("AddToFavorites")]
		[OwaEventVerb(OwaEventVerb.Post)]
		public void AddToFavorites()
		{
			base.ThrowIfCannotActAsOwner();
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			if (owaStoreObjectId.OwaStoreObjectIdType != OwaStoreObjectIdType.MailBoxObject && owaStoreObjectId.OwaStoreObjectIdType != OwaStoreObjectIdType.ArchiveMailboxObject)
			{
				throw new OwaInvalidRequestException("Only mailbox objects can be added to favorites.");
			}
			using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, owaStoreObjectId, FolderList.FolderTreeQueryProperties))
			{
				if (Utilities.IsDefaultFolder(folder, DefaultFolderType.Root))
				{
					throw new OwaInvalidRequestException("Cannot add root folder to favorites.");
				}
				if (!string.IsNullOrEmpty(folder.ClassName) && !ObjectClass.IsOfClass(folder.ClassName, "IPF.Note"))
				{
					throw new OwaInvalidRequestException("Only mail folder can be added to favorites.");
				}
				if (Utilities.IsDefaultFolder(folder, DefaultFolderType.SearchFolders))
				{
					throw new OwaInvalidRequestException("Cannot add search folder root to favorites.");
				}
				NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, NavigationNodeGroupSection.First);
				navigationNodeCollection.RemoveFolderByLegacyDNandId(((MailboxSession)folder.Session).MailboxOwnerLegacyDN, owaStoreObjectId.StoreObjectId);
				if (base.IsParameterSet("tgtid"))
				{
					StoreObjectId storeObjectId = (StoreObjectId)base.GetParameter("tgtid");
				}
				if (base.IsParameterSet("tgtid"))
				{
					int num = navigationNodeCollection[0].Children.FindChildByNodeId((StoreObjectId)base.GetParameter("tgtid"));
					if (num < 0)
					{
						base.RenderPartialFailure(817935633, OwaEventHandlerErrorCode.NotSet);
					}
					else
					{
						if (base.IsParameterSet("mt"))
						{
							int num2 = (int)base.GetParameter("mt");
							if (num2 != 1 && num2 != 2)
							{
								throw new OwaInvalidRequestException("Only before or after is valid.");
							}
							if (num2 == 2)
							{
								num++;
							}
						}
						navigationNodeCollection.InsertMyFolderToFavorites(folder, num);
					}
				}
				else
				{
					navigationNodeCollection.AppendFolderToFavorites(folder);
				}
				navigationNodeCollection.Save(base.UserContext.MailboxSession);
			}
			NavigationHost.RenderFavoritesAndNavigationTrees(this.Writer, base.UserContext, null, new NavigationNodeGroupSection[]
			{
				NavigationNodeGroupSection.First
			});
		}

		[OwaEventParameter("GS", typeof(NavigationNodeGroupSection))]
		[OwaEventParameter("SB", typeof(string))]
		[OwaEvent("Rename")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("srcNId", typeof(StoreObjectId))]
		public void Rename()
		{
			base.ThrowIfCannotActAsOwner();
			string text = ((string)base.GetParameter("SB")).Trim();
			string s = text;
			NavigationNodeGroupSection navigationNodeGroupSection = (NavigationNodeGroupSection)base.GetParameter("GS");
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
			NavigationNode navigationNode = navigationNodeCollection.FindNavigationNodeByNodeId((StoreObjectId)base.GetParameter("srcNId"));
			if (navigationNode == null)
			{
				throw new OwaEventHandlerException("Cannot find specified navigation node", LocalizedStrings.GetNonEncoded(-289549140), true);
			}
			if (text.Length != 0)
			{
				if (navigationNode is NavigationNodeFolder)
				{
					NavigationNodeFolder navigationNodeFolder = navigationNode as NavigationNodeFolder;
					if (!navigationNodeFolder.IsValid)
					{
						throw new OwaInvalidRequestException("This is not a valid navigation node");
					}
					if (navigationNodeFolder.NavigationNodeType == NavigationNodeType.SmartFolder && !navigationNodeFolder.IsFilteredView)
					{
						throw new OwaInvalidRequestException("Cannot rename search folders");
					}
					OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromNavigationNodeFolder(base.UserContext, navigationNodeFolder);
					MailboxSession mailboxSession = owaStoreObjectId.IsArchive ? ((MailboxSession)owaStoreObjectId.GetSession(base.UserContext)) : base.UserContext.MailboxSession;
					if (owaStoreObjectId.IsArchive && navigationNodeFolder.NavigationNodeType == NavigationNodeType.NormalFolder)
					{
						s = string.Format(LocalizedStrings.GetNonEncoded(-83764036), text, Utilities.GetMailboxOwnerDisplayName(mailboxSession));
					}
					if (navigationNodeFolder.IsFolderInSpecificMailboxSession(mailboxSession))
					{
						using (Folder folder = Folder.Bind(mailboxSession, navigationNodeFolder.FolderId, new PropertyDefinition[]
						{
							FolderSchema.ExtendedFolderFlags
						}))
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
						}
					}
				}
				navigationNode.Subject = text;
				navigationNodeCollection.Save(base.UserContext.MailboxSession);
				this.Writer.Write("<div id=tn>");
				Utilities.HtmlEncode(text, this.Writer, true);
				this.Writer.Write("</div><div id=ntn>");
				Utilities.HtmlEncode(s, this.Writer, true);
				this.Writer.Write("</div>");
				return;
			}
			if (navigationNode is NavigationNodeGroup)
			{
				throw new OwaEventHandlerException("User did not provide name for new group", LocalizedStrings.GetNonEncoded(-1749891264), true);
			}
			throw new OwaEventHandlerException("User did not provide name for new folder", LocalizedStrings.GetNonEncoded(-41080803), true);
		}

		[OwaEventParameter("cms", typeof(NavigationModule), true)]
		[OwaEvent("Remove")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("srcNId", typeof(StoreObjectId))]
		[OwaEventParameter("GS", typeof(NavigationNodeGroupSection))]
		public void Remove()
		{
			base.ThrowIfCannotActAsOwner();
			StoreObjectId nodeId = (StoreObjectId)base.GetParameter("srcNId");
			NavigationNodeGroupSection navigationNodeGroupSection = (NavigationNodeGroupSection)base.GetParameter("GS");
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
			List<OwaStoreObjectId> list = new List<OwaStoreObjectId>();
			NavigationNode navigationNode = navigationNodeCollection.FindNavigationNodeByNodeId(nodeId);
			if (navigationNode == null)
			{
				base.RenderPartialFailure(-289549140, OwaEventHandlerErrorCode.NotSet);
			}
			else
			{
				OperationResult operationResult = (OperationResult)0;
				if (navigationNodeGroupSection != NavigationNodeGroupSection.First)
				{
					List<NavigationNodeFolder> list2 = new List<NavigationNodeFolder>();
					if (navigationNode is NavigationNodeFolder)
					{
						list2.Add(navigationNode as NavigationNodeFolder);
					}
					else if (navigationNode is NavigationNodeGroup)
					{
						NavigationNodeGroup navigationNodeGroup = navigationNode as NavigationNodeGroup;
						foreach (NavigationNodeFolder item in navigationNodeGroup.Children)
						{
							list2.Add(item);
						}
					}
					foreach (NavigationNodeFolder navigationNodeFolder in list2)
					{
						if (navigationNodeFolder.IsValid)
						{
							OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromNavigationNodeFolder(base.UserContext, navigationNodeFolder);
							MailboxSession mailboxSession = owaStoreObjectId.IsArchive ? ((MailboxSession)owaStoreObjectId.GetSession(base.UserContext)) : base.UserContext.MailboxSession;
							if (navigationNodeFolder.IsFolderInSpecificMailboxSession(mailboxSession))
							{
								if (Utilities.IsSpecialFolderForSession(mailboxSession, navigationNodeFolder.FolderId))
								{
									throw new OwaEventHandlerException("Cannot delete default folders.", LocalizedStrings.GetNonEncoded(-1164567320), true);
								}
								if (operationResult == (OperationResult)0)
								{
									operationResult = OperationResult.Succeeded;
								}
								AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
								{
									navigationNodeFolder.FolderId
								});
								if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
								{
									list.Add(OwaStoreObjectId.CreateFromNavigationNodeFolder(base.UserContext, navigationNodeFolder));
								}
								else
								{
									operationResult = OperationResult.PartiallySucceeded;
								}
							}
						}
					}
					if (operationResult != (OperationResult)0 && list.Count == 0)
					{
						operationResult = OperationResult.Failed;
					}
					if (operationResult == OperationResult.Failed)
					{
						base.RenderPartialFailure(1041829989, OwaEventHandlerErrorCode.NotSet);
					}
					else if (operationResult == OperationResult.PartiallySucceeded)
					{
						base.RenderPartialFailure(995407892, OwaEventHandlerErrorCode.NotSet);
					}
				}
				else
				{
					NavigationNodeFolder navigationNodeFolder2 = navigationNode as NavigationNodeFolder;
					if (navigationNodeFolder2 != null && navigationNodeFolder2.IsFilteredView)
					{
						OwaStoreObjectId owaStoreObjectId2 = OwaStoreObjectId.CreateFromNavigationNodeFolder(base.UserContext, navigationNodeFolder2);
						using (SearchFolder searchFolder = SearchFolder.Bind(owaStoreObjectId2.GetSession(base.UserContext), owaStoreObjectId2.StoreObjectId, FolderList.FolderTreeQueryProperties))
						{
							searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
							searchFolder.DisplayName = Utilities.GetRandomNameForTempFilteredView(base.UserContext);
							searchFolder.Save();
						}
					}
				}
				if (operationResult == (OperationResult)0 || operationResult == OperationResult.Succeeded)
				{
					if (navigationNodeCollection.RemoveFolderOrGroupByNodeId(nodeId) != null)
					{
						navigationNodeCollection.Save(base.UserContext.MailboxSession);
					}
				}
				else if (operationResult == OperationResult.PartiallySucceeded)
				{
					foreach (OwaStoreObjectId owaStoreObjectId3 in list)
					{
						navigationNodeCollection.RemoveFolderByLegacyDNandId(owaStoreObjectId3.MailboxOwnerLegacyDN ?? base.UserContext.MailboxOwnerLegacyDN, owaStoreObjectId3.StoreObjectId);
					}
					navigationNodeCollection.Save(base.UserContext.MailboxSession);
				}
			}
			NavigationTreeDirtyFlag navigationTreeDirtyFlag = NavigationTreeDirtyFlag.None;
			if (navigationNodeGroupSection == NavigationNodeGroupSection.First)
			{
				navigationTreeDirtyFlag = NavigationTreeDirtyFlag.Favorites;
			}
			else
			{
				if (list.Count > 0)
				{
					List<FolderTreeNode> list3 = FolderTreeNode.CreateDeletedNodesWithDirtyCheck(base.UserContext, list, out navigationTreeDirtyFlag);
					this.Writer.Write("<div id=tn>");
					foreach (FolderTreeNode folderTreeNode in list3)
					{
						folderTreeNode.RenderUndecoratedNode(this.Writer);
					}
					this.Writer.Write("</div>");
				}
				switch (navigationNodeGroupSection)
				{
				case NavigationNodeGroupSection.Calendar:
					navigationTreeDirtyFlag |= NavigationTreeDirtyFlag.Calendar;
					break;
				case NavigationNodeGroupSection.Contacts:
					navigationTreeDirtyFlag |= NavigationTreeDirtyFlag.Contact;
					break;
				case NavigationNodeGroupSection.Tasks:
					navigationTreeDirtyFlag |= NavigationTreeDirtyFlag.Task;
					break;
				}
			}
			RenderingUtilities.RenderNavigationTreeDirtyFlag(this.Writer, base.UserContext, navigationTreeDirtyFlag, (NavigationModule[])base.GetParameter("cms"));
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("GS", typeof(NavigationNodeGroupSection))]
		[OwaEvent("Move")]
		[OwaEventParameter("srcNId", typeof(StoreObjectId))]
		[OwaEventParameter("tgtid", typeof(StoreObjectId))]
		[OwaEventParameter("mt", typeof(int), false, true)]
		public void Move()
		{
			base.ThrowIfCannotActAsOwner();
			StoreObjectId nodeId = (StoreObjectId)base.GetParameter("srcNId");
			StoreObjectId nodeId2 = (StoreObjectId)base.GetParameter("tgtid");
			int num = base.IsParameterSet("mt") ? ((int)base.GetParameter("mt")) : 0;
			NavigationNodeGroupSection navigationNodeGroupSection = (NavigationNodeGroupSection)base.GetParameter("GS");
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
			NavigationNode navigationNode = navigationNodeCollection.RemoveFolderOrGroupByNodeId(nodeId);
			if (navigationNode == null)
			{
				base.RenderPartialFailure(-289549140, OwaEventHandlerErrorCode.NotSet);
			}
			else
			{
				int num2;
				int num3;
				if (navigationNodeCollection.TryFindGroupAndNodeIndexByNodeId(nodeId2, out num2, out num3))
				{
					if (navigationNode is NavigationNodeGroup)
					{
						if (num3 >= 0)
						{
							throw new OwaInvalidRequestException("The target node must be a navigation node group");
						}
						if (num != 1 && num != 2)
						{
							throw new OwaInvalidRequestException("Can only move a group before or after another group");
						}
						navigationNodeCollection.Insert((num == 2) ? (num2 + 1) : num2, navigationNode as NavigationNodeGroup);
					}
					else if (navigationNode is NavigationNodeFolder)
					{
						if (num3 >= 0)
						{
							if (num != 1 && num != 2)
							{
								throw new OwaInvalidRequestException("Can only move a folder before or after another folder");
							}
							navigationNodeCollection[num2].Children.Insert((num == 2) ? (num3 + 1) : num3, navigationNode as NavigationNodeFolder);
						}
						else
						{
							navigationNodeCollection[num2].Children.Add(navigationNode as NavigationNodeFolder);
						}
					}
				}
				else
				{
					base.RenderPartialFailure((navigationNodeGroupSection == NavigationNodeGroupSection.First) ? 817935633 : 444965652, OwaEventHandlerErrorCode.NotSet);
				}
				navigationNodeCollection.Save(base.UserContext.MailboxSession);
			}
			NavigationHost.RenderFavoritesAndNavigationTrees(this.Writer, base.UserContext, null, new NavigationNodeGroupSection[]
			{
				navigationNodeGroupSection
			});
		}

		[OwaEvent("AddGroup")]
		[OwaEventParameter("GS", typeof(NavigationNodeGroupSection))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SB", typeof(string))]
		public void AddNewGroup()
		{
			base.ThrowIfCannotActAsOwner();
			string text = ((string)base.GetParameter("SB")).Trim();
			if (text.Length == 0)
			{
				throw new OwaEventHandlerException("User did not provide name for new group", LocalizedStrings.GetNonEncoded(-1749891264), true);
			}
			NavigationNodeGroupSection navigationNodeGroupSection = (NavigationNodeGroupSection)base.GetParameter("GS");
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
			NavigationNodeGroup navigationNodeGroup = NavigationNodeCollection.CreateCustomizedGroup(navigationNodeGroupSection, text);
			Guid navigationNodeGroupClassId = navigationNodeGroup.NavigationNodeGroupClassId;
			navigationNodeCollection.Add(navigationNodeGroup);
			navigationNodeCollection.Save(base.UserContext.MailboxSession);
			navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
			navigationNodeGroup = navigationNodeCollection[navigationNodeCollection.FindGroupById(navigationNodeGroupClassId)];
			NavigationGroupHeaderTreeNode navigationGroupHeaderTreeNode = new NavigationGroupHeaderTreeNode(base.UserContext, navigationNodeGroup);
			NavigationGroupHeaderTreeNode navigationGroupHeaderTreeNode2 = navigationGroupHeaderTreeNode;
			navigationGroupHeaderTreeNode2.CustomAttributes += " _NF=1";
			this.Writer.Write("<div id=ntn>");
			navigationGroupHeaderTreeNode.RenderUndecoratedNode(this.Writer);
			this.Writer.Write("</div>");
		}

		[OwaEvent("AddFolder")]
		[OwaEventParameter("GS", typeof(NavigationNodeGroupSection))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SB", typeof(string))]
		[OwaEventParameter("pid", typeof(StoreObjectId))]
		public void AddNewFolder()
		{
			base.ThrowIfCannotActAsOwner();
			string text = ((string)base.GetParameter("SB")).Trim();
			if (text.Length == 0)
			{
				throw new OwaEventHandlerException("User did not provide name for new folder", LocalizedStrings.GetNonEncoded(-41080803), true);
			}
			NavigationNodeGroupSection navigationNodeGroupSection = (NavigationNodeGroupSection)base.GetParameter("GS");
			StoreObjectType folderType;
			string className;
			DefaultFolderType defaultFolderType;
			switch (navigationNodeGroupSection)
			{
			case NavigationNodeGroupSection.Calendar:
				folderType = StoreObjectType.CalendarFolder;
				className = "IPF.Appointment";
				defaultFolderType = DefaultFolderType.Calendar;
				break;
			case NavigationNodeGroupSection.Contacts:
				folderType = StoreObjectType.ContactsFolder;
				className = "IPF.Contact";
				defaultFolderType = DefaultFolderType.Contacts;
				break;
			case NavigationNodeGroupSection.Tasks:
				folderType = StoreObjectType.TasksFolder;
				className = "IPF.Task";
				defaultFolderType = DefaultFolderType.Tasks;
				break;
			default:
				throw new OwaInvalidRequestException("Invalid group section: " + navigationNodeGroupSection.ToString());
			}
			StoreObjectId nodeId = (StoreObjectId)base.GetParameter("pid");
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
			int num = navigationNodeCollection.FindChildByNodeId(nodeId);
			if (num < 0)
			{
				throw new OwaEventHandlerException("The parent group doesn't exist.", LocalizedStrings.GetNonEncoded(-289549140), true);
			}
			using (Folder folder = Utilities.CreateSubFolder(Utilities.GetDefaultFolderId(base.UserContext, base.UserContext.MailboxSession, defaultFolderType), folderType, text, base.UserContext))
			{
				folder.ClassName = className;
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
				folder.Load(FolderList.FolderTreeQueryProperties);
				StoreObjectId objectId = folder.Id.ObjectId;
				navigationNodeCollection.AddFolderToGroup(num, folder, base.UserContext, folder.DisplayName, NavigationNodeType.NormalFolder);
				navigationNodeCollection.Save(base.UserContext.MailboxSession);
				navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
				NavigationFolderTreeNode navigationFolderTreeNode = new NavigationFolderTreeNode(base.UserContext, navigationNodeCollection.FindFoldersById(objectId)[0], folder);
				NavigationFolderTreeNode navigationFolderTreeNode2 = navigationFolderTreeNode;
				navigationFolderTreeNode2.CustomAttributes += " _NF=1";
				this.Writer.Write("<div id=ntn>");
				navigationFolderTreeNode.RenderUndecoratedNode(this.Writer);
				this.Writer.Write("</div>");
			}
		}

		[OwaEventParameter("GS", typeof(NavigationNodeGroupSection))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("srcNId", typeof(StoreObjectId))]
		[OwaEventParameter("exp", typeof(bool))]
		[OwaEvent("PersistExpand")]
		public void PersistExpandStatus()
		{
			base.ThrowIfCannotActAsOwner();
			if (!base.UserContext.IsWebPartRequest)
			{
				NavigationNodeGroupSection navigationNodeGroupSection = (NavigationNodeGroupSection)base.GetParameter("GS");
				NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, navigationNodeGroupSection);
				int num = navigationNodeCollection.FindChildByNodeId((StoreObjectId)base.GetParameter("srcNId"));
				if (num >= 0)
				{
					navigationNodeCollection[num].IsExpanded = (bool)base.GetParameter("exp");
					navigationNodeCollection.Save(base.UserContext.MailboxSession);
				}
			}
		}

		[OwaEventParameter("Recips", typeof(RecipientInfoAC), true, true)]
		[OwaEvent("OpenOthersFolder")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("GS", typeof(NavigationNodeGroupSection))]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("RCP", typeof(RecipientInfoAC), false, true)]
		public void OpenOtherUserFolder()
		{
			base.ThrowIfCannotActAsOwner();
			if (base.UserContext.IsExplicitLogon)
			{
				throw new OwaInvalidRequestException("Cannot open other's folder in explict logon mode");
			}
			NavigationNodeGroupSection navigationNodeGroupSection = (NavigationNodeGroupSection)base.GetParameter("GS");
			if (navigationNodeGroupSection == NavigationNodeGroupSection.Contacts || navigationNodeGroupSection == NavigationNodeGroupSection.Tasks)
			{
				throw new OwaInvalidRequestException("Cannot open other's contacts/tasks folder");
			}
			DefaultFolderType defaultFolderType;
			switch (navigationNodeGroupSection)
			{
			case NavigationNodeGroupSection.Mail:
				defaultFolderType = DefaultFolderType.Inbox;
				break;
			case NavigationNodeGroupSection.Calendar:
				defaultFolderType = DefaultFolderType.Calendar;
				break;
			case NavigationNodeGroupSection.Contacts:
				defaultFolderType = DefaultFolderType.Contacts;
				break;
			case NavigationNodeGroupSection.Tasks:
				defaultFolderType = DefaultFolderType.Tasks;
				break;
			default:
				throw new OwaInvalidRequestException("Invalid group section: " + navigationNodeGroupSection.ToString());
			}
			RecipientInfoAC[] array = (RecipientInfoAC[])base.GetParameter("Recips");
			if (array != null && array.Length != 0)
			{
				AutoCompleteCache.UpdateAutoCompleteCacheFromRecipientInfoList(array, base.OwaContext.UserContext);
			}
			if (defaultFolderType != DefaultFolderType.Calendar)
			{
				Folder folder = null;
				if (base.IsParameterSet("RCP"))
				{
					RecipientInfoAC recipientInfoAC = (RecipientInfoAC)base.GetParameter("RCP");
					ExchangePrincipal exchangePrincipal = null;
					if (base.UserContext.DelegateSessionManager.TryGetExchangePrincipal(recipientInfoAC.RoutingAddress, out exchangePrincipal))
					{
						if (string.Equals(base.UserContext.MailboxSession.MailboxOwnerLegacyDN, exchangePrincipal.LegacyDn, StringComparison.OrdinalIgnoreCase))
						{
							throw new OwaEventHandlerException("Cannot open own folder", LocalizedStrings.GetNonEncoded(-1770024075), true);
						}
						folder = this.GetOtherUserFolder(exchangePrincipal, defaultFolderType);
					}
				}
				else
				{
					if (!base.IsParameterSet("Id"))
					{
						throw new OwaInvalidRequestException("Must specific one of recipient and folder Id.");
					}
					OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
					if (!owaStoreObjectId.IsOtherMailbox)
					{
						throw new OwaEventHandlerException("Cannot open own folder", LocalizedStrings.GetNonEncoded(-1770024075), true);
					}
					folder = Utilities.GetFolder<Folder>(base.UserContext, owaStoreObjectId, FolderList.FolderTreeQueryProperties);
				}
				try
				{
					if (folder == null || !Utilities.CanReadItemInFolder(folder))
					{
						Strings.IDs localizedId = 1414246128;
						switch (navigationNodeGroupSection)
						{
						case NavigationNodeGroupSection.Mail:
							localizedId = -236167850;
							break;
						case NavigationNodeGroupSection.Contacts:
							localizedId = -1505241540;
							break;
						case NavigationNodeGroupSection.Tasks:
							localizedId = -65263937;
							break;
						}
						throw new OwaEventHandlerException(LocalizedStrings.GetNonEncoded(995407892), LocalizedStrings.GetNonEncoded(localizedId), true);
					}
					MailboxSession mailboxSession = folder.Session as MailboxSession;
					if (navigationNodeGroupSection == NavigationNodeGroupSection.Mail)
					{
						OtherMailboxConfigEntry otherMailboxConfigEntry = OtherMailboxConfiguration.AddOtherMailboxSession(base.UserContext, mailboxSession);
						if (otherMailboxConfigEntry != null)
						{
							NavigationHost.RenderOtherMailboxFolderTree(this.Writer, base.UserContext, otherMailboxConfigEntry, true);
						}
					}
					else
					{
						NavigationNodeCollection.AddNonMailFolderToSharedFoldersGroup(base.UserContext, folder, navigationNodeGroupSection);
						OwaStoreObjectId owaStoreObjectId2 = OwaStoreObjectId.CreateFromStoreObject(folder);
						NavigationHost.RenderFavoritesAndNavigationTrees(this.Writer, base.UserContext, owaStoreObjectId2, new NavigationNodeGroupSection[]
						{
							navigationNodeGroupSection
						});
						this.RenderOpenOtherUserFolderReponse(folder.ClassName, owaStoreObjectId2);
					}
				}
				finally
				{
					if (folder != null)
					{
						folder.Dispose();
					}
				}
				return;
			}
			if (!base.IsParameterSet("RCP"))
			{
				throw new OwaInvalidRequestException("Recipient is missing for open other user's calendar request");
			}
			this.OpenOtherUserCalendar((RecipientInfoAC)base.GetParameter("RCP"));
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEvent("RemoveOtherInbox")]
		[OwaEventVerb(OwaEventVerb.Post)]
		public void RemoveOtherInbox()
		{
			base.ThrowIfCannotActAsOwner();
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			if (owaStoreObjectId.MailboxOwnerLegacyDN == null)
			{
				throw new OwaInvalidRequestException("Can't find legacyDN information.");
			}
			OtherMailboxConfiguration.RemoveOtherMailbox(base.UserContext, owaStoreObjectId.MailboxOwnerLegacyDN);
			try
			{
				MailboxSession mailboxSession = owaStoreObjectId.GetSession(base.UserContext) as MailboxSession;
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				if (defaultFolderId != null)
				{
					OwaStoreObjectId owaStoreObjectId2 = OwaStoreObjectId.CreateFromSessionFolderId(base.UserContext, mailboxSession, defaultFolderId);
					if (base.UserContext.IsPushNotificationsEnabled)
					{
						base.UserContext.MapiNotificationManager.UnsubscribeFolderCounts(owaStoreObjectId2, mailboxSession);
						base.UserContext.MapiNotificationManager.UnsubscribeFolderChanges(owaStoreObjectId2, mailboxSession);
					}
					if (base.UserContext.IsPullNotificationsEnabled)
					{
						base.UserContext.NotificationManager.DeleteDelegateFolderCountAdvisor(owaStoreObjectId2);
						base.UserContext.NotificationManager.DeleteOwaConditionAdvisor(owaStoreObjectId2);
					}
				}
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<ObjectNotFoundException>(0L, "NavigationNodeEventHandler.RemoveOtherInbox() exception {0}", arg);
			}
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEvent("GetWebCalendarUrl")]
		[OwaEventVerb(OwaEventVerb.Post)]
		public void GetWebCalendarUrl()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			if (owaStoreObjectId.IsGSCalendar)
			{
				throw new OwaInvalidRequestException("Cannot pass GS Calendar folder id");
			}
			MailboxSession mailboxSession = base.UserContext.MailboxSession;
			StoreObjectId storeObjectId = owaStoreObjectId.StoreObjectId;
			string value = null;
			if (mailboxSession != null && storeObjectId != null)
			{
				value = CalendarUtilities.GetWebCalendarUrl(mailboxSession, storeObjectId);
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new OwaInvalidRequestException("The requested folder does not have web calendar url");
			}
			this.Writer.Write(value);
		}

		private Folder GetOtherUserFolder(ExchangePrincipal exchangePrincipal, DefaultFolderType defaultFolderType)
		{
			try
			{
				MailboxSession session;
				StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(base.UserContext, exchangePrincipal, defaultFolderType, out session);
				if (storeObjectId != null)
				{
					return Folder.Bind(session, storeObjectId, FolderList.FolderTreeQueryProperties);
				}
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<ObjectNotFoundException, DefaultFolderType>(0L, "NavigationNodeEventHandler.GetOtherUserFolder() exception {0} for folder type {1}", arg, defaultFolderType);
			}
			return null;
		}

		private void OpenOtherUserCalendar(RecipientInfoAC recipientInfo)
		{
			ExchangePrincipal exchangePrincipal = null;
			if (base.UserContext.DelegateSessionManager.TryGetExchangePrincipal(recipientInfo.RoutingAddress, out exchangePrincipal))
			{
				if (string.Equals(base.UserContext.MailboxSession.MailboxOwnerLegacyDN, exchangePrincipal.LegacyDn, StringComparison.OrdinalIgnoreCase))
				{
					throw new OwaEventHandlerException("Cannot open own folder", LocalizedStrings.GetNonEncoded(-1770024075), true);
				}
				try
				{
					NavigationNodeCollection.AddGSCalendarToSharedFoldersGroup(base.UserContext, exchangePrincipal);
					OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromGSCalendarLegacyDN(exchangePrincipal.LegacyDn);
					NavigationHost.RenderFavoritesAndNavigationTrees(this.Writer, base.UserContext, owaStoreObjectId, new NavigationNodeGroupSection[]
					{
						NavigationNodeGroupSection.Calendar
					});
					this.RenderOpenOtherUserFolderReponse("IPF.Appointment", owaStoreObjectId);
					return;
				}
				catch (OwaSharedFromOlderVersionException)
				{
					throw new OwaEventHandlerException(LocalizedStrings.GetNonEncoded(995407892), LocalizedStrings.GetNonEncoded(1354015881), true);
				}
			}
			throw new OwaEventHandlerException("Cannot get the exchange principal of the target user when open other user's calendar", LocalizedStrings.GetNonEncoded(1988379659), true);
		}

		private void RenderOpenOtherUserFolderReponse(string folderClassName, OwaStoreObjectId folderId)
		{
			this.Writer.Write("<div id=fldrurl>");
			this.Writer.Write("?ae=Folder&t=");
			Utilities.HtmlEncode(Utilities.UrlEncode(folderClassName), this.Writer);
			this.Writer.Write("&id=");
			Utilities.HtmlEncode(Utilities.UrlEncode(folderId.ToBase64String()), this.Writer);
			this.Writer.Write("</div>");
		}

		public const string EventNamespace = "NavigationNode";

		public const string MethodRename = "Rename";

		public const string MethodRemove = "Remove";

		public const string MethodMove = "Move";

		public const string MethodAddNewFolder = "AddFolder";

		public const string MethodAddNewGroup = "AddGroup";

		public const string MethodOpenOtherUserFolder = "OpenOthersFolder";

		public const string MethodAddToFavorites = "AddToFavorites";

		public const string MethodRemoveOtherInbox = "RemoveOtherInbox";

		public const string MethodPersistExpand = "PersistExpand";

		public const string MethodGetWebCalendarUrl = "GetWebCalendarUrl";

		public const string Id = "Id";

		public const string SourceNodeId = "srcNId";

		public const int SourceNodeBeforeTarget = 1;

		public const int SourceNodeAfterTarget = 2;

		public const string GroupSection = "GS";

		public const string Subject = "SB";

		public const string Recipient = "RCP";

		public const string Recipients = "Recips";

		public const string TargetNodeId = "tgtid";

		public const string ParentGroupId = "pid";

		public const string MoveType = "mt";

		public const string IsExpanded = "exp";

		public const string ClientNavigationModules = "cms";
	}
}
