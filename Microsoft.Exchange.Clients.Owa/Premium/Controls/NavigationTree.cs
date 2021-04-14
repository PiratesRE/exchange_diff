using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class NavigationTree : Tree
	{
		private NavigationTree(UserContext userContext, InvisibleRootTreeNode rootNode, NavigationNodeGroupSection groupSection) : base(userContext, rootNode)
		{
			this.groupSection = groupSection;
		}

		protected override void RenderAdditionalProperties(TextWriter writer)
		{
			base.RenderAdditionalProperties(writer);
			writer.Write(" _iGS=");
			writer.Write((int)this.groupSection);
		}

		private static NavigationTree CreateFavoriteTreeByFolderLists(UserContext userContext, NavigationNodeCollection navigationCollection, NavigationTree.NormalAndSearchFolderList[] normalAndSearchFolderLists)
		{
			if (navigationCollection.GroupSection != NavigationNodeGroupSection.First)
			{
				throw new ArgumentException("Should pass a collection represents favorites.");
			}
			NavigationTree navigationTree = new NavigationTree(userContext, new InvisibleRootTreeNode(userContext), NavigationNodeGroupSection.First);
			List<NavigationNodeFolder> list = new List<NavigationNodeFolder>();
			List<StoreObjectId>[] array = new List<StoreObjectId>[normalAndSearchFolderLists.Length];
			for (int i = 0; i < normalAndSearchFolderLists.Length; i++)
			{
				array[i] = new List<StoreObjectId>();
			}
			NavigationNodeGroup navigationNodeGroup = navigationCollection[0];
			NavigationGroupHeaderTreeNode navigationGroupHeaderTreeNode = new NavigationGroupHeaderTreeNode(userContext, navigationNodeGroup);
			foreach (NavigationNodeFolder navigationNodeFolder in navigationNodeGroup.Children)
			{
				for (int j = 0; j < normalAndSearchFolderLists.Length; j++)
				{
					MailboxSession mailboxSession = normalAndSearchFolderLists[j].MailboxSession;
					FolderList deepHierarchyFolderList = normalAndSearchFolderLists[j].DeepHierarchyFolderList;
					FolderList searchFolderList = normalAndSearchFolderLists[j].SearchFolderList;
					Dictionary<Guid, StoreObjectId> searchFolderGuidToIdMapping = normalAndSearchFolderLists[j].SearchFolderGuidToIdMapping;
					if (navigationNodeFolder.IsValid && navigationNodeFolder.IsFolderInSpecificMailboxSession(mailboxSession) && navigationNodeFolder.FolderId != null && !navigationNodeFolder.IsFlagSet(NavigationNodeFlags.PublicFolderFavorite))
					{
						bool flag = navigationNodeFolder.NavigationNodeType == NavigationNodeType.SmartFolder;
						if (!flag || (searchFolderList != null && userContext.IsFeatureEnabled(Feature.SearchFolders)))
						{
							StoreObjectId folderId;
							if (flag && !navigationNodeFolder.AssociatedSearchFolderId.Equals(Guid.Empty))
							{
								searchFolderGuidToIdMapping.TryGetValue(navigationNodeFolder.AssociatedSearchFolderId, out folderId);
							}
							else
							{
								folderId = navigationNodeFolder.FolderId;
							}
							if (folderId != null)
							{
								object[] array2 = flag ? searchFolderList.GetFolderProperties(folderId) : deepHierarchyFolderList.GetFolderProperties(folderId);
								if (array2 != null)
								{
									string text = (flag ? searchFolderList.GetFolderProperty(folderId, StoreObjectSchema.ContainerClass) : deepHierarchyFolderList.GetFolderProperty(folderId, StoreObjectSchema.ContainerClass)) as string;
									if (string.IsNullOrEmpty(text) || ObjectClass.IsOfClass(text, "IPF.Note"))
									{
										if (navigationNodeFolder.IsFilteredView && !deepHierarchyFolderList.ContainsFolder(navigationNodeFolder.FilterSourceFolder) && !searchFolderList.ContainsFolder(navigationNodeFolder.FilterSourceFolder))
										{
											list.Add(navigationNodeFolder);
											array[j].Add(folderId);
										}
										else
										{
											StoreObjectId storeObjectId = null;
											if (!flag)
											{
												StoreObjectId storeObjectId2 = folderId;
												while (storeObjectId == null && storeObjectId2 != null)
												{
													object folderProperty = deepHierarchyFolderList.GetFolderProperty(storeObjectId2, FolderSchema.Id);
													if (folderProperty == null)
													{
														break;
													}
													storeObjectId2 = ((VersionedId)folderProperty).ObjectId;
													DefaultFolderType defaultFolderType = Utilities.GetDefaultFolderType(mailboxSession, storeObjectId2);
													if (defaultFolderType == DefaultFolderType.Root)
													{
														break;
													}
													object folderProperty2 = deepHierarchyFolderList.GetFolderProperty(storeObjectId2, FolderSchema.AdminFolderFlags);
													if (folderProperty2 is int && (int)folderProperty2 != 0)
													{
														storeObjectId = storeObjectId2;
													}
													else if (Utilities.IsSpecialFolderType(defaultFolderType) && !string.IsNullOrEmpty(deepHierarchyFolderList.GetFolderProperty(storeObjectId2, FolderSchema.ELCPolicyIds) as string))
													{
														storeObjectId = storeObjectId2;
													}
													else
													{
														object folderProperty3 = deepHierarchyFolderList.GetFolderProperty(storeObjectId2, StoreObjectSchema.ParentEntryId);
														if (folderProperty3 is byte[])
														{
															storeObjectId2 = StoreObjectId.FromProviderSpecificId(folderProperty3 as byte[], StoreObjectType.Folder);
														}
														else
														{
															storeObjectId2 = null;
														}
													}
												}
											}
											Dictionary<PropertyDefinition, int> propertyMap = flag ? searchFolderList.QueryPropertyMap : deepHierarchyFolderList.QueryPropertyMap;
											navigationGroupHeaderTreeNode.AddChild(new NavigationFolderTreeNode(userContext, navigationNodeFolder, storeObjectId, array2, propertyMap));
										}
									}
									else if (!flag)
									{
										list.Add(navigationNodeFolder);
									}
								}
								else if (navigationNodeFolder.IsFilteredView || !flag)
								{
									list.Add(navigationNodeFolder);
								}
							}
						}
					}
				}
			}
			object folderProperty4 = normalAndSearchFolderLists[0].DeepHierarchyFolderList.GetFolderProperty(userContext.GetRootFolderId(userContext.MailboxSession), ViewStateProperties.TreeNodeCollapseStatus);
			if (folderProperty4 is int)
			{
				navigationGroupHeaderTreeNode.IsExpanded = !Utilities.IsFlagSet((int)folderProperty4, 2);
			}
			else
			{
				navigationGroupHeaderTreeNode.IsExpanded = true;
			}
			navigationTree.RootNode.AddChild(navigationGroupHeaderTreeNode);
			if (list.Count > 0)
			{
				foreach (NavigationNodeFolder navigationNodeFolder2 in list)
				{
					if (navigationNodeFolder2.NavigationNodeId != null)
					{
						navigationCollection.RemoveFolderOrGroupByNodeId(navigationNodeFolder2.NavigationNodeId.ObjectId);
					}
				}
				navigationCollection.Save(userContext.MailboxSession);
			}
			for (int k = 0; k < array.Length; k++)
			{
				List<StoreObjectId> list2 = array[k];
				if (list2.Count > 0)
				{
					Utilities.DeleteFolders(normalAndSearchFolderLists[k].MailboxSession, DeleteItemFlags.SoftDelete, list2.ToArray());
				}
			}
			return navigationTree;
		}

		private static void CreateNormalAndSearchFolderLists(UserContext userContext, bool filterGeekFolder, bool includeSearchFolder, out NavigationTree.NormalAndSearchFolderList mailboxNormalAndSearchFolderList, out NavigationTree.NormalAndSearchFolderList[] archiveNormalAndSearchFolderLists)
		{
			FolderList deepHierarchyFolderList;
			FolderList searchFolderList;
			Dictionary<Guid, StoreObjectId> searchFolderGuidToIdMapping;
			NavigationTree.CreateNormalAndSearchFolderList(userContext, userContext.MailboxSession, filterGeekFolder, includeSearchFolder, out deepHierarchyFolderList, out searchFolderList, out searchFolderGuidToIdMapping);
			mailboxNormalAndSearchFolderList = new NavigationTree.NormalAndSearchFolderList
			{
				MailboxSession = userContext.MailboxSession,
				DeepHierarchyFolderList = deepHierarchyFolderList,
				SearchFolderList = searchFolderList,
				SearchFolderGuidToIdMapping = searchFolderGuidToIdMapping
			};
			List<NavigationTree.NormalAndSearchFolderList> normalAndSearchFolderLists = new List<NavigationTree.NormalAndSearchFolderList>();
			if (userContext.ArchiveAccessed)
			{
				userContext.TryLoopArchiveMailboxes(delegate(MailboxSession archiveSession)
				{
					NavigationTree.CreateNormalAndSearchFolderList(userContext, archiveSession, filterGeekFolder, includeSearchFolder, out deepHierarchyFolderList, out searchFolderList, out searchFolderGuidToIdMapping);
					normalAndSearchFolderLists.Add(new NavigationTree.NormalAndSearchFolderList
					{
						MailboxSession = archiveSession,
						DeepHierarchyFolderList = deepHierarchyFolderList,
						SearchFolderList = searchFolderList,
						SearchFolderGuidToIdMapping = searchFolderGuidToIdMapping
					});
				});
			}
			archiveNormalAndSearchFolderLists = normalAndSearchFolderLists.ToArray();
		}

		private static void CreateNormalAndSearchFolderList(UserContext userContext, MailboxSession mailboxSession, bool filterGeekFolder, bool includeSearchFolder, out FolderList deepHierarchyFolderList, out FolderList searchFolderList, out Dictionary<Guid, StoreObjectId> searchFolderGuidToIdMapping)
		{
			QueryFilter queryFilter = null;
			if (filterGeekFolder)
			{
				queryFilter = new NotFilter(Utilities.GetObjectClassTypeFilter(true, new string[]
				{
					"IPF.Appointment",
					"IPF.Contact",
					"IPF.Task",
					"IPF.Journal",
					userContext.IsFeatureEnabled(Feature.StickyNotes) ? null : "IPF.StickyNote"
				}));
			}
			FolderList parentFolderList = new FolderList(userContext, mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration), queryFilter, 10000, false, true, true, null, FolderList.FolderTreeQueryProperties);
			deepHierarchyFolderList = new FolderList(userContext, mailboxSession, userContext.GetRootFolderId(mailboxSession), 10000, false, true, true, parentFolderList);
			searchFolderList = null;
			searchFolderGuidToIdMapping = null;
			if (includeSearchFolder && userContext.IsFeatureEnabled(Feature.SearchFolders))
			{
				searchFolderList = new FolderList(userContext, mailboxSession, userContext.GetSearchFoldersId(mailboxSession).StoreObjectId, 10000, false, false, false, parentFolderList);
				searchFolderGuidToIdMapping = new Dictionary<Guid, StoreObjectId>(searchFolderList.Count);
				foreach (StoreObjectId storeObjectId in searchFolderList.GetFolderIds())
				{
					object folderProperty = searchFolderList.GetFolderProperty(storeObjectId, FolderSchema.OutlookSearchFolderClsId);
					if (folderProperty != null && folderProperty is Guid)
					{
						searchFolderGuidToIdMapping[(Guid)folderProperty] = storeObjectId;
					}
				}
			}
		}

		public static void CreateFavoriteAndMailboxTreeAndGetBuddyListStatus(UserContext userContext, out NavigationTree favoritesTree, out MailboxFolderTree mailboxFolderTree, out MailboxFolderTree[] archiveFolderTrees, out bool expandBuddyList)
		{
			NavigationTree.NormalAndSearchFolderList item;
			NavigationTree.NormalAndSearchFolderList[] array;
			NavigationTree.CreateNormalAndSearchFolderLists(userContext, true, true, out item, out array);
			NavigationNodeCollection navigationCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(userContext, userContext.MailboxSession, NavigationNodeGroupSection.First);
			List<NavigationTree.NormalAndSearchFolderList> list = new List<NavigationTree.NormalAndSearchFolderList>(1 + array.Length);
			list.Add(item);
			list.AddRange(array);
			favoritesTree = NavigationTree.CreateFavoriteTreeByFolderLists(userContext, navigationCollection, list.ToArray());
			mailboxFolderTree = MailboxFolderTree.CreateStartPageMailboxFolderTree(userContext, item.DeepHierarchyFolderList, item.SearchFolderList);
			StatusPersistTreeNodeType valueToTest = StatusPersistTreeNodeType.None;
			object folderProperty = item.DeepHierarchyFolderList.GetFolderProperty(userContext.GetRootFolderId(userContext.MailboxSession), ViewStateProperties.TreeNodeCollapseStatus);
			if (folderProperty is int)
			{
				valueToTest = (StatusPersistTreeNodeType)folderProperty;
			}
			expandBuddyList = !Utilities.IsFlagSet((int)valueToTest, 4);
			bool flag = !Utilities.IsFlagSet((int)valueToTest, 2);
			bool flag2 = !Utilities.IsFlagSet((int)valueToTest, 1);
			OwaStoreObjectId folderId = OwaStoreObjectId.CreateFromMailboxFolderId(userContext.InboxFolderId);
			if (!flag || flag2 || !favoritesTree.RootNode.SelectSpecifiedFolder(folderId))
			{
				mailboxFolderTree.RootNode.SelectSpecifiedFolder(folderId);
			}
			archiveFolderTrees = null;
			if (array.Length > 0)
			{
				List<MailboxFolderTree> list2 = new List<MailboxFolderTree>(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					list2.Add(MailboxFolderTree.CreateStartPageArchiveMailboxFolderTree(userContext, array[i].DeepHierarchyFolderList, array[i].SearchFolderList));
				}
				archiveFolderTrees = list2.ToArray();
			}
		}

		public static NavigationTree[] CreateFavoriteAndNavigationTrees(UserContext userContext, params NavigationNodeGroupSection[] groupSections)
		{
			bool includeSearchFolder = false;
			NavigationTree[] array = new NavigationTree[groupSections.Length];
			NavigationNodeCollection navigationCollection = null;
			for (int i = 0; i < groupSections.Length; i++)
			{
				if (groupSections[i] == NavigationNodeGroupSection.First)
				{
					includeSearchFolder = true;
				}
			}
			NavigationTree.NormalAndSearchFolderList item;
			NavigationTree.NormalAndSearchFolderList[] array2;
			NavigationTree.CreateNormalAndSearchFolderLists(userContext, false, includeSearchFolder, out item, out array2);
			NavigationNodeCollection[] array3 = NavigationNodeCollection.TryCreateNavigationNodeCollections(userContext, userContext.MailboxSession, groupSections);
			List<NavigationNodeCollection> list = new List<NavigationNodeCollection>();
			for (int j = 0; j < array3.Length; j++)
			{
				NavigationNodeGroupSection navigationNodeGroupSection = array3[j].GroupSection;
				if (navigationNodeGroupSection != NavigationNodeGroupSection.First)
				{
					if ((navigationNodeGroupSection == NavigationNodeGroupSection.Calendar && userContext.IsFeatureEnabled(Feature.Calendar)) || (navigationNodeGroupSection == NavigationNodeGroupSection.Contacts && userContext.IsFeatureEnabled(Feature.Contacts)) || (navigationNodeGroupSection == NavigationNodeGroupSection.Tasks && userContext.IsFeatureEnabled(Feature.Tasks)))
					{
						list.Add(array3[j]);
					}
				}
				else
				{
					navigationCollection = array3[j];
				}
			}
			List<NavigationTree.NormalAndSearchFolderList> list2 = new List<NavigationTree.NormalAndSearchFolderList>(1 + array2.Length);
			list2.Add(item);
			list2.AddRange(array2);
			List<FolderList> list3 = new List<FolderList>();
			foreach (NavigationTree.NormalAndSearchFolderList normalAndSearchFolderList in list2)
			{
				list3.Add(normalAndSearchFolderList.DeepHierarchyFolderList);
			}
			NavigationTree[] array4 = NavigationTree.CreateNavigationTreeByFolderLists(userContext, list3.ToArray(), list.ToArray());
			for (int k = 0; k < groupSections.Length; k++)
			{
				if (groupSections[k] == NavigationNodeGroupSection.First)
				{
					array[k] = NavigationTree.CreateFavoriteTreeByFolderLists(userContext, navigationCollection, list2.ToArray());
				}
				else
				{
					array[k] = null;
					for (int l = 0; l < array4.Length; l++)
					{
						if (array4[l].groupSection == groupSections[k])
						{
							array[k] = array4[l];
							break;
						}
					}
				}
			}
			return array;
		}

		public static NavigationTree CreateNavigationTree(UserContext userContext, NavigationNodeGroupSection groupSection)
		{
			List<FolderList> folderLists = new List<FolderList>();
			folderLists.Add(new FolderList(userContext, userContext.MailboxSession, new string[]
			{
				NavigationNode.GetFolderClass(groupSection)
			}, 10000, false, null, FolderList.FolderTreeQueryProperties));
			if (userContext.ArchiveAccessed)
			{
				userContext.TryLoopArchiveMailboxes(delegate(MailboxSession archiveSession)
				{
					folderLists.Add(new FolderList(userContext, archiveSession, new string[]
					{
						NavigationNode.GetFolderClass(groupSection)
					}, 10000, false, null, FolderList.FolderTreeQueryProperties));
				});
			}
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(userContext, userContext.MailboxSession, groupSection);
			return NavigationTree.CreateNavigationTreeByFolderLists(userContext, folderLists.ToArray(), new NavigationNodeCollection[]
			{
				navigationNodeCollection
			})[0];
		}

		private static NavigationTree[] CreateNavigationTreeByFolderLists(UserContext userContext, FolderList[] folderLists, params NavigationNodeCollection[] collections)
		{
			if (folderLists == null || folderLists.Length == 0)
			{
				throw new ArgumentNullException("folderLists");
			}
			if (collections.Length == 0)
			{
				return new NavigationTree[0];
			}
			string[] array = new string[collections.Length];
			StoreObjectId[] array2 = new StoreObjectId[collections.Length];
			for (int i = 0; i < collections.Length; i++)
			{
				array[i] = NavigationNode.GetFolderClass(collections[i].GroupSection);
				if (ObjectClass.IsOfClass(array[i], "IPF.Note"))
				{
					throw new ArgumentException("Invalid group section. Can only be Calendar, Contact, Task");
				}
				switch (collections[i].GroupSection)
				{
				case NavigationNodeGroupSection.Calendar:
					array2[i] = Utilities.TryGetDefaultFolderId(userContext.MailboxSession, DefaultFolderType.Calendar);
					break;
				case NavigationNodeGroupSection.Contacts:
					array2[i] = Utilities.TryGetDefaultFolderId(userContext.MailboxSession, DefaultFolderType.Contacts);
					break;
				case NavigationNodeGroupSection.Tasks:
					array2[i] = Utilities.TryGetDefaultFolderId(userContext.MailboxSession, DefaultFolderType.Tasks);
					break;
				default:
					throw new ArgumentException("Invalid group section. Can only be Calendar, Contact, Task");
				}
			}
			Dictionary<string, StoreObjectId>[] array3 = new Dictionary<string, StoreObjectId>[collections.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array3[j] = new Dictionary<string, StoreObjectId>();
			}
			foreach (FolderList folderList in folderLists)
			{
				foreach (StoreObjectId storeObjectId in folderList.GetFolderIds())
				{
					string itemClass = folderList.GetFolderProperty(storeObjectId, StoreObjectSchema.ContainerClass) as string;
					for (int m = 0; m < array.Length; m++)
					{
						if (ObjectClass.IsOfClass(itemClass, array[m]))
						{
							string key = folderList.MailboxSession.MailboxOwnerLegacyDN.ToLowerInvariant() + storeObjectId.ToString();
							array3[m][key] = storeObjectId;
							break;
						}
					}
				}
			}
			bool flag = false;
			NavigationNodeGroupSection[] array4 = new NavigationNodeGroupSection[collections.Length];
			StoreObjectId storeObjectId2 = userContext.TryGetMyDefaultFolderId(DefaultFolderType.ToDoSearch);
			List<NavigationNodeFolder> list = new List<NavigationNodeFolder>();
			for (int n = 0; n < collections.Length; n++)
			{
				list.Clear();
				bool flag2 = false;
				bool flag3 = false;
				NavigationNodeCollection navigationNodeCollection = collections[n];
				NavigationNodeGroupSection navigationNodeGroupSection = navigationNodeCollection.GroupSection;
				array4[n] = navigationNodeGroupSection;
				foreach (NavigationNodeGroup navigationNodeGroup in navigationNodeCollection)
				{
					foreach (NavigationNodeFolder navigationNodeFolder in navigationNodeGroup.Children)
					{
						if (navigationNodeFolder.IsValid && navigationNodeFolder.FolderId != null && !navigationNodeFolder.IsFlagSet(NavigationNodeFlags.PublicFolderFavorite))
						{
							bool flag4 = false;
							foreach (FolderList folderList2 in folderLists)
							{
								if (navigationNodeFolder.IsFolderInSpecificMailboxSession(folderList2.MailboxSession))
								{
									flag4 = true;
									break;
								}
							}
							if (flag4)
							{
								string key2 = navigationNodeFolder.MailboxLegacyDN.ToLowerInvariant() + navigationNodeFolder.FolderId.ToString();
								if (!array3[n].Remove(key2))
								{
									if (navigationNodeGroupSection == NavigationNodeGroupSection.Tasks && storeObjectId2 != null && storeObjectId2.Equals(navigationNodeFolder.FolderId))
									{
										flag3 = true;
									}
									else
									{
										list.Add(navigationNodeFolder);
									}
								}
								if (navigationNodeFolder.FolderId.Equals(array2[n]))
								{
									foreach (FolderList folderList3 in folderLists)
									{
										if (navigationNodeFolder.IsFolderInSpecificMailboxSession(folderList3.MailboxSession))
										{
											string text = (string)folderList3.GetFolderProperty(navigationNodeFolder.FolderId, FolderSchema.DisplayName);
											string subject = navigationNodeFolder.Subject;
											if (text != null && !text.Equals(subject))
											{
												navigationNodeFolder.Subject = text;
												flag2 = true;
											}
										}
									}
								}
							}
						}
					}
				}
				if (navigationNodeGroupSection == NavigationNodeGroupSection.Tasks && !flag3 && storeObjectId2 != null)
				{
					navigationNodeCollection.InsertToDoFolderToGroup(userContext);
					flag2 = true;
				}
				if (array3[n].Count > 0)
				{
					foreach (KeyValuePair<string, StoreObjectId> keyValuePair in array3[n])
					{
						StoreObjectId value = keyValuePair.Value;
						string key3 = keyValuePair.Key;
						string y = key3.Substring(0, key3.LastIndexOf(value.ToString()));
						foreach (FolderList folderList4 in folderLists)
						{
							if (StringComparer.OrdinalIgnoreCase.Equals(folderList4.MailboxSession.MailboxOwnerLegacyDN, y))
							{
								navigationNodeCollection.AddMyFolderToGroup(userContext, folderList4.MailboxSession, folderList4.GetFolderProperties(value), folderList4.QueryPropertyMap);
								break;
							}
						}
					}
					flag2 = true;
				}
				if (list.Count > 0)
				{
					foreach (NavigationNodeFolder navigationNodeFolder2 in list)
					{
						navigationNodeCollection.RemoveFolderOrGroupByNodeId(navigationNodeFolder2.NavigationNodeId.ObjectId);
					}
					flag2 = true;
				}
				if (flag2)
				{
					navigationNodeCollection.Save(userContext.MailboxSession);
					flag = true;
				}
			}
			if (flag)
			{
				collections = NavigationNodeCollection.TryCreateNavigationNodeCollections(userContext, userContext.MailboxSession, array4);
			}
			NavigationTree[] array5 = new NavigationTree[collections.Length];
			for (int num4 = 0; num4 < collections.Length; num4++)
			{
				NavigationNodeCollection navigationNodeCollection2 = collections[num4];
				NavigationTree navigationTree = new NavigationTree(userContext, new InvisibleRootTreeNode(userContext), navigationNodeCollection2.GroupSection);
				foreach (NavigationNodeGroup navigationNodeGroup2 in navigationNodeCollection2)
				{
					if (!navigationNodeGroup2.IsNew)
					{
						if ((navigationNodeCollection2.GroupSection == NavigationNodeGroupSection.Contacts || navigationNodeCollection2.GroupSection == NavigationNodeGroupSection.Tasks) && NavigationNodeCollection.PeoplesFoldersClassId.Equals(navigationNodeGroup2.NavigationNodeGroupClassId))
						{
							bool flag5 = false;
							foreach (NavigationNodeFolder navigationNodeFolder3 in navigationNodeGroup2.Children)
							{
								if (navigationNodeFolder3.NavigationNodeType != NavigationNodeType.SharedFolder)
								{
									flag5 = true;
									break;
								}
							}
							if (!flag5)
							{
								continue;
							}
						}
						NavigationGroupHeaderTreeNode navigationGroupHeaderTreeNode = new NavigationGroupHeaderTreeNode(userContext, navigationNodeGroup2);
						foreach (NavigationNodeFolder navigationNodeFolder4 in navigationNodeGroup2.Children)
						{
							if (((navigationNodeCollection2.GroupSection != NavigationNodeGroupSection.Contacts && navigationNodeCollection2.GroupSection != NavigationNodeGroupSection.Tasks) || navigationNodeFolder4.NavigationNodeType != NavigationNodeType.SharedFolder) && navigationNodeFolder4.IsValid && (navigationNodeFolder4.IsGSCalendar || navigationNodeFolder4.FolderId != null))
							{
								NavigationFolderTreeNode navigationFolderTreeNode = null;
								foreach (FolderList folderList5 in folderLists)
								{
									object[] array6 = null;
									if (navigationNodeFolder4.FolderId != null)
									{
										array6 = folderList5.GetFolderProperties(navigationNodeFolder4.FolderId);
									}
									if (array6 != null)
									{
										navigationFolderTreeNode = new NavigationFolderTreeNode(userContext, navigationNodeFolder4, null, array6, folderList5.QueryPropertyMap);
										break;
									}
								}
								if (navigationFolderTreeNode == null && (!navigationNodeFolder4.IsFolderInSpecificMailboxSession(userContext.MailboxSession) || (navigationNodeFolder4.FolderId != null && Utilities.IsDefaultFolderId(userContext.MailboxSession, navigationNodeFolder4.FolderId, DefaultFolderType.ToDoSearch))))
								{
									navigationFolderTreeNode = new NavigationFolderTreeNode(userContext, navigationNodeFolder4);
								}
								if (navigationFolderTreeNode != null)
								{
									navigationGroupHeaderTreeNode.AddChild(navigationFolderTreeNode);
								}
							}
						}
						navigationTree.RootNode.AddChild(navigationGroupHeaderTreeNode);
					}
				}
				array5[num4] = navigationTree;
			}
			return array5;
		}

		private readonly NavigationNodeGroupSection groupSection;

		private struct NormalAndSearchFolderList
		{
			internal MailboxSession MailboxSession;

			internal FolderList DeepHierarchyFolderList;

			internal FolderList SearchFolderList;

			internal Dictionary<Guid, StoreObjectId> SearchFolderGuidToIdMapping;
		}
	}
}
