using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OwaFilterState
	{
		public OwaFilterState(StoreId currentFolderId, OwaViewFilter viewFilter, string fromFilter)
		{
			this.sourceFolderId = currentFolderId;
			this.viewFilter = viewFilter;
			this.fromFilter = fromFilter;
		}

		public OwaFilterState()
		{
		}

		public double Version
		{
			get
			{
				return this.version;
			}
		}

		public StoreId SourceFolderId
		{
			get
			{
				return this.sourceFolderId;
			}
		}

		public OwaViewFilter ViewFilter
		{
			get
			{
				return this.viewFilter;
			}
		}

		public bool IsCurrentVersion
		{
			get
			{
				return this.version == 15.1;
			}
		}

		public static SearchFolder CreateOrOpenOwaFilteredViewSearchFolder(MailboxSession mailboxSession, OwaSearchContext searchContext, StoreId searchFoldersRootId, SearchFolderCriteria searchFolderCriteria, bool flushStaleFolders = true)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<OwaViewFilter>(0L, "[OwaFilterState::CreateOrOpenOwaFilteredViewSearchFolder] Create or open the specified filtered view: {0}", searchContext.ViewFilter);
			SearchFolder searchFolder = null;
			bool flag = true;
			searchContext.ViewFilterActions = ViewFilterActions.None;
			try
			{
				searchContext.ClientSearchFolderIdentity = OwaFilterState.GetOwaFilteredViewSearchFolderName(mailboxSession, searchContext);
				if (searchContext.SearchFolderId == null && OwaFilterState.FilterToLinkPropertyDefinitionsMap.ContainsKey(searchContext.ViewFilter))
				{
					StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
					if (defaultFolderId.Equals(searchContext.FolderIdToSearch))
					{
						searchContext.SearchFolderId = OwaFilterState.GetLinkedFolderIdForFilteredView(mailboxSession, searchContext.FolderIdToSearch, searchContext.ViewFilter);
					}
				}
				if (searchContext.SearchFolderId != null)
				{
					searchContext.ViewFilterActions |= ViewFilterActions.BindToExisting;
					searchFolder = OwaFilterState.BindAndUpdateExistingFilteredViewSearchFolder(mailboxSession, searchContext);
				}
				if (searchFolder == null)
				{
					searchContext.ViewFilterActions |= ViewFilterActions.FindExisting;
					searchFolder = OwaFilterState.GetFilteredView(mailboxSession, searchContext, searchFoldersRootId, flushStaleFolders);
				}
				if (searchContext.IsSearchFailed && searchFolder != null)
				{
					ExTraceGlobals.StorageTracer.TraceError<string, StoreId>(0L, "[OwaFilterState::CreateOrOpenOwaFilteredViewSearchFolder] deleting search folder: {0} using searchContext.SearchFolderId: {1}", searchContext.ClientSearchFolderIdentity, searchContext.SearchFolderId);
					searchContext.ViewFilterActions |= ViewFilterActions.DeleteInvalidSearchFolder;
					mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						searchContext.SearchFolderId
					});
					searchContext.SearchFolderId = null;
					searchContext.IsSearchFailed = false;
					searchFolder.Dispose();
					searchFolder = null;
				}
				if (searchFolder == null)
				{
					searchContext.ViewFilterActions |= ViewFilterActions.CreateFilter;
					searchFolder = OwaFilterState.CreateAndUpdateFilteredViewSearchFolder(mailboxSession, searchContext, searchFoldersRootId, searchFolderCriteria);
				}
				ExTraceGlobals.StorageTracer.TraceDebug(0L, string.Format("{0}_{1}", searchContext.ClientSearchFolderIdentity, (int)searchContext.ViewFilterActions));
				flag = false;
			}
			finally
			{
				if (flag)
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, "[OwaFilterState::CreateOrOpenOwaFilteredViewSearchFolder] Creating/Opening of dynamic search folder failed. Setting search folder to null and disposing it");
					if (searchFolder != null)
					{
						searchFolder.Dispose();
						searchFolder = null;
					}
				}
			}
			return searchFolder;
		}

		public static StoreObjectId GetLinkedFolderIdForFilteredView(MailboxSession session, StoreId sourceFolderId, OwaViewFilter viewFilter)
		{
			StoreObjectId result = null;
			try
			{
				using (Folder folder = Folder.Bind(session, sourceFolderId))
				{
					byte[] folderProperty = OwaFilterState.GetFolderProperty<byte[]>(folder, OwaFilterState.FilterToLinkPropertyDefinitionsMap[viewFilter], null);
					if (folderProperty != null)
					{
						result = StoreObjectId.FromProviderSpecificId(folderProperty, StoreObjectType.Folder);
					}
				}
			}
			catch (CorruptDataException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<OwaViewFilter, CorruptDataException>(0L, "[OwaFilterState::GetLinkedFolderIdForFilteredView] Caught a CorruptDataException for OwaViewFilter {0}: {1}", viewFilter, arg);
			}
			return result;
		}

		public static bool LinkViewFolder(MailboxSession session, StoreId folderId, SearchFolder viewFolder, OwaViewFilter filter)
		{
			bool flag = false;
			Exception ex = null;
			if (OwaFilterState.FilterToLinkPropertyDefinitionsMap.ContainsKey(filter))
			{
				try
				{
					using (Folder folder = Folder.Bind(session, folderId))
					{
						bool flag2 = true;
						StoreId linkedFolderIdForFilteredView = OwaFilterState.GetLinkedFolderIdForFilteredView(session, folderId, filter);
						if (linkedFolderIdForFilteredView != null && viewFolder.StoreObjectId.Equals(linkedFolderIdForFilteredView))
						{
							flag2 = false;
							flag = true;
						}
						if (flag2)
						{
							folder.SafeSetProperty(OwaFilterState.FilterToLinkPropertyDefinitionsMap[filter], viewFolder.Id.ObjectId.ProviderLevelItemId);
							FolderSaveResult folderSaveResult = folder.Save();
							if (folderSaveResult.OperationResult == OperationResult.Succeeded)
							{
								ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::LinkViewFolder] Search Folder Name = {0} linked to physical folder", viewFolder.DisplayName);
								flag = true;
							}
							else
							{
								ex = folderSaveResult.Exception;
							}
						}
					}
				}
				catch (ObjectNotFoundException ex2)
				{
					ex = ex2;
				}
				if (!flag)
				{
					string arg = (ex != null) ? ex.ToString() : "[OwaFilterState::LinkViewFolder] FolderSaveResult.OperationResult != OperationResult.Succeeded";
					string message = string.Format("Linking the view folder ({0}) failed. Details= {1}", viewFolder.DisplayName, arg);
					ExTraceGlobals.StorageTracer.TraceError(0L, message);
				}
			}
			else
			{
				ExTraceGlobals.StorageTracer.TraceDebug<OwaViewFilter>(0L, "[OwaFilterState::LinkViewFolder] The specified OwaViewFilter {0} is not supported for linking to the source folder.", filter);
			}
			return flag;
		}

		public static OwaFilterState GetOwaFilterStateForExistingFolder(MailboxSession mailboxSession, StoreId searchFolderId)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<StoreId>(0L, "[OwaFilterState::GetOwaFilterStateForExistingFolder] Get the filter state for the specified folder id: {0}", searchFolderId);
			OwaFilterState result = null;
			OwaSearchContext searchContext = new OwaSearchContext();
			using (SearchFolder searchFolder = OwaFilterState.BindExistingFilteredViewSearchFolder(searchFolderId, mailboxSession, searchContext))
			{
				if (searchFolder != null)
				{
					result = OwaFilterState.ParseFromPropertyValue(OwaFilterState.GetFolderProperty<object>(searchFolder, OwaFilteredViewProperties.FilteredViewLabel, null));
				}
			}
			return result;
		}

		private static SearchFolder GetFilteredView(MailboxSession mailboxSession, OwaSearchContext searchContext, StoreId searchFoldersRootId, bool flushStaleFolders)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<OwaViewFilter>(0L, "[OwaFilterState::GetFilteredView] Get the specified filtered view: {0}", searchContext.ViewFilter);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			OwaFilterState newFilterState = new OwaFilterState(searchContext.FolderIdToSearch, searchContext.ViewFilter, searchContext.FromFilter);
			int num = 0;
			StoreObjectId storeObjectId = null;
			List<StoreId> list = new List<StoreId>();
			SearchFolder result = null;
			using (Folder folder = Folder.Bind(mailboxSession, searchFoldersRootId))
			{
				List<object[]> list2 = new List<object[]>();
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, OwaFilterState.FolderQueryProperties))
				{
					list2 = OwaFilterState.FetchRowsFromQueryResult(queryResult, 10000);
				}
				int num2 = OwaFilterState.FilterSearchFolderPropertyIndexes[FolderSchema.Id];
				int num3 = OwaFilterState.FilterSearchFolderPropertyIndexes[FolderSchema.SearchFolderAllowAgeout];
				int num4 = OwaFilterState.FilterSearchFolderPropertyIndexes[StoreObjectSchema.LastModifiedTime];
				int num5 = OwaFilterState.FilterSearchFolderPropertyIndexes[OwaFilteredViewProperties.FilteredViewLabel];
				ExDateTime other = ExDateTime.MaxValue;
				for (int i = 0; i < list2.Count; i++)
				{
					OwaFilterState owaFilterState = OwaFilterState.ParseFromPropertyValue(list2[i][num5]);
					if (owaFilterState != null)
					{
						if (OwaFilterState.AreEqual(newFilterState, owaFilterState))
						{
							searchContext.ViewFilterActions |= ViewFilterActions.FilterFound;
							searchContext.SearchFolderId = (list2[i][num2] as VersionedId).ObjectId;
							result = OwaFilterState.BindAndUpdateExistingFilteredViewSearchFolder(mailboxSession, searchContext);
							break;
						}
						if (list2[i][num3] is bool && (bool)list2[i][num3])
						{
							if (!owaFilterState.IsCurrentVersion && flushStaleFolders)
							{
								list.Add((StoreId)list2[i][num2]);
							}
							else
							{
								num++;
								ExDateTime exDateTime = ExDateTime.MinValue;
								if (!(list2[i][num4] is PropertyError))
								{
									exDateTime = (ExDateTime)list2[i][num4];
								}
								if (exDateTime.CompareTo(other) < 0)
								{
									StoreObjectId objectId = (list2[i][num2] as VersionedId).ObjectId;
									if (!defaultFolderId.Equals(owaFilterState.SourceFolderId))
									{
										other = exDateTime;
										storeObjectId = objectId;
									}
								}
							}
						}
					}
				}
			}
			if (num >= searchContext.MaximumTemporaryFilteredViewPerUser && flushStaleFolders && storeObjectId != null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int>(0L, "[OwaFilterState::GetFilteredView] Delete LRU filtered view search folder at hitting max filtered views: {0}", searchContext.MaximumTemporaryFilteredViewPerUser);
				list.Add(storeObjectId);
			}
			if (list.Count > 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "[OwaFilterState::GetFilteredView] deleting LRU / non current version of filtered views count: " + list.Count);
				mailboxSession.Delete(DeleteItemFlags.HardDelete, list.ToArray());
			}
			return result;
		}

		private static bool ApplyOwaContinuousSearch(SearchFolder searchFolder, SearchFolderCriteria searchCriteria, MailboxSession mailboxSession, OwaSearchContext searchContext)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::ApplyOwaContinuousSearch] Start for search folder: {0}", searchContext.ClientSearchFolderIdentity);
			StoreObjectId storeObjectId = searchFolder.StoreObjectId;
			bool flag = false;
			try
			{
				IAsyncResult asyncResult = searchFolder.BeginApplyContinuousSearch(searchCriteria, null, null);
				flag = asyncResult.AsyncWaitHandle.WaitOne(searchContext.SearchTimeoutInMilliseconds);
				if (flag)
				{
					searchFolder.EndApplyContinuousSearch(asyncResult);
					searchContext.ViewFilterActions |= ViewFilterActions.SearchCriteriaApplied;
					ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::ApplyOwaContinuousSearch] Search completed for search folder: {0}", searchContext.ClientSearchFolderIdentity);
				}
				else
				{
					flag = true;
					searchContext.ViewFilterActions |= ViewFilterActions.PopulateSearchFolderTimedOut;
					ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::ApplyOwaContinuousSearch] Search timed out for search folder: {0}", searchContext.ClientSearchFolderIdentity);
				}
			}
			catch (QueryInProgressException ex)
			{
				searchContext.ViewFilterActions |= ViewFilterActions.QueryInProgressException;
				string message = string.Format("[OwaFilterState::ApplyOwaContinuousSearch] Population of dynamic search folder: {0} failed due to QueryInProgressException: {1}. ViewFilterActions: {2}", searchContext.ClientSearchFolderIdentity, ex.ToString(), (int)searchContext.ViewFilterActions);
				ExTraceGlobals.StorageTracer.TraceError(0L, message);
				flag = OwaFilterState.EnsureSearchIsCompleted(searchFolder, mailboxSession, searchContext);
			}
			catch (CorruptDataException ex2)
			{
				searchContext.ViewFilterActions |= ViewFilterActions.CorruptDataException;
				string text = string.Format("[OwaFilterState::ApplyOwaContinuousSearch] Population of dynamic search folder: {0} failed due to CorruptDataException: {1}. ViewFilterActions = {2}.", searchContext.ClientSearchFolderIdentity, ex2.ToString(), (int)searchContext.ViewFilterActions);
				ExTraceGlobals.StorageTracer.TraceError(0L, text);
				OwaFilterState.SendWatsonWithoutDump(ex2, text);
			}
			catch (ObjectNotFoundException ex3)
			{
				searchContext.ViewFilterActions |= ViewFilterActions.ObjectNotFoundException;
				string message2 = string.Format("[OwaFilterState::ApplyOwaContinuousSearch] Population of dynamic search folder: {0} failed due to ObjectNotFoundException: {1}. ViewFilterActions = {2}.", searchContext.ClientSearchFolderIdentity, ex3.ToString(), (int)searchContext.ViewFilterActions);
				ExTraceGlobals.StorageTracer.TraceError(0L, message2);
			}
			catch (Exception ex4)
			{
				searchContext.ViewFilterActions |= ViewFilterActions.Exception;
				string text2 = string.Format("[OwaFilterState::ApplyOwaContinuousSearch] Population of dynamic search folder: {0} failed due to Exception: {1}. ViewFilterActions = {2}.", searchContext.ClientSearchFolderIdentity, ex4.ToString(), (int)searchContext.ViewFilterActions);
				ExTraceGlobals.StorageTracer.TraceError(0L, text2);
				OwaFilterState.SendWatsonWithoutDump(ex4, text2);
			}
			return flag;
		}

		private static bool EnsureSearchIsCompleted(SearchFolder searchFolder, MailboxSession mailboxSession, OwaSearchContext searchContext)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::EnsureSearchIsCompleted] Start. search folder: {0}", searchContext.ClientSearchFolderIdentity);
			Subscription subscription = null;
			bool searchCompleted = false;
			SearchState searchState = SearchState.Error;
			bool result;
			try
			{
				SearchFolderCriteria searchCriteria = searchFolder.GetSearchCriteria();
				if (searchCriteria != null)
				{
					searchState = searchCriteria.SearchState;
				}
				if ((searchState & SearchState.Rebuild) == SearchState.Rebuild)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::EnsureSearchIsCompleted] SearchState.Rebuild == true. search folder: {0}", searchContext.ClientSearchFolderIdentity);
					ManualResetEvent completedEvent = new ManualResetEvent(false);
					subscription = Subscription.Create(mailboxSession, delegate(Notification notification)
					{
						if ((notification.Type & NotificationType.SearchComplete) == NotificationType.SearchComplete)
						{
							searchCompleted = true;
							completedEvent.Set();
						}
					}, NotificationType.SearchComplete, searchFolder.StoreObjectId);
					completedEvent.WaitOne(searchContext.SearchTimeoutInMilliseconds);
					if (!searchCompleted)
					{
						searchCompleted = true;
						searchContext.ViewFilterActions |= ViewFilterActions.PopulateSearchFolderTimedOut;
						ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::EnsureSearchIsCompleted] Search timed out for search folder: {0}", searchContext.ClientSearchFolderIdentity);
					}
				}
				else
				{
					ExTraceGlobals.StorageTracer.TraceDebug<SearchState, string, string>(0L, "[OwaFilterState::EnsureSearchIsCompleted] SearchFolder not rebuilding. searchState: {0}. searchCriteria: {1} for search folder: {2}", searchState, searchCriteria.ToString(), searchContext.ClientSearchFolderIdentity);
					searchCompleted = ((searchState & SearchState.Error) != SearchState.Error);
				}
				ExTraceGlobals.StorageTracer.TraceDebug<bool, string>(0L, "[OwaFilterState::EnsureSearchIsCompleted] Is searchCompleted is: {0} for search folder: {1}", searchCompleted, searchContext.ClientSearchFolderIdentity);
				searchContext.ViewFilterActions |= ViewFilterActions.SearchCompleted;
				result = searchCompleted;
			}
			catch (ObjectNotInitializedException)
			{
				searchContext.ViewFilterActions |= ViewFilterActions.ObjectNotInitializedException;
				string message = string.Format("[OwaFilterState::EnsureSearchIsCompleted] ObjectNotInitializedException thrown, searchCompleted is false for search folder: {0}. ViewFilterActions: {1}", searchContext.ClientSearchFolderIdentity, (int)searchContext.ViewFilterActions);
				ExTraceGlobals.StorageTracer.TraceError(0L, message);
				result = false;
			}
			catch (CorruptDataException ex)
			{
				searchContext.ViewFilterActions |= ViewFilterActions.CorruptDataException;
				string text = string.Format("[OwaFilterState::EnsureSearchIsCompleted] CorruptDataException thrown, searchCompleted is false for search folder: {0}. Exception: {1}. ViewFilterActions: {2}", searchContext.ClientSearchFolderIdentity, ex.ToString(), (int)searchContext.ViewFilterActions);
				ExTraceGlobals.StorageTracer.TraceError(0L, text);
				OwaFilterState.SendWatsonWithoutDump(ex, text);
				result = false;
			}
			finally
			{
				if (subscription != null)
				{
					subscription.Dispose();
					subscription = null;
				}
			}
			return result;
		}

		private static string GetOwaFilteredViewSearchFolderName(MailboxSession mailboxSession, OwaSearchContext searchContext)
		{
			string text = StoreId.StoreIdToEwsId(mailboxSession.MailboxGuid, searchContext.FolderIdToSearch);
			string text2 = (!string.IsNullOrEmpty(searchContext.FromFilter)) ? searchContext.FromFilter.GetHashCode().ToString("X") : string.Empty;
			return string.Concat(new object[]
			{
				"OwaFV",
				15.1,
				searchContext.ViewFilter,
				text2,
				text
			});
		}

		private static bool AreEqual(OwaFilterState newFilterState, OwaFilterState existingFilterState)
		{
			return OwaFilterState.IdsAreEqual(newFilterState.SourceFolderId, existingFilterState.SourceFolderId) && newFilterState.ViewFilter == existingFilterState.ViewFilter && newFilterState.Version == existingFilterState.Version && newFilterState.fromFilter == existingFilterState.fromFilter;
		}

		private static bool IdsAreEqual(StoreId leftId, StoreId rightId)
		{
			return leftId == null == (rightId == null) && (leftId == null || leftId.Equals(rightId));
		}

		private static OwaFilterState ParseFromPropertyValue(object propertyValue)
		{
			string[] array = propertyValue as string[];
			if (array == null || array.Length < 2 || string.IsNullOrEmpty(array[0]))
			{
				return null;
			}
			OwaFilterState owaFilterState = new OwaFilterState();
			owaFilterState.version = 0.0;
			if (string.IsNullOrEmpty(array[1]))
			{
				return owaFilterState;
			}
			try
			{
				owaFilterState.sourceFolderId = StoreId.EwsIdToStoreObjectId(array[0]);
			}
			catch (Exception)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "[OwaFilterState::ParseFromPropertyValue] id of property was not valid EWS id: " + array[0]);
				return owaFilterState;
			}
			int num = 0;
			double num2 = 0.0;
			string[] array2 = array[1].Split(new char[]
			{
				':'
			});
			if (array2.Length < 2 || !int.TryParse(array2[0], out num) || !double.TryParse(array2[1], out num2))
			{
				return owaFilterState;
			}
			if (num2 != 15.1)
			{
				return owaFilterState;
			}
			bool flag = array2.Length == 3;
			OwaViewFilter owaViewFilter = (OwaViewFilter)num;
			if (!EnumValidator.IsValidValue<OwaViewFilter>(owaViewFilter))
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "[OwaFilterState::ParseFromPropertyValue] ViewFilter of property was not valid enum value: " + owaViewFilter.ToString());
				return owaFilterState;
			}
			if (owaViewFilter == OwaViewFilter.All && !flag)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "[OwaFilterState::ParseFromPropertyValue] ViewFilter All is not valid enum value if FromFilter is not specified.");
				return owaFilterState;
			}
			owaFilterState.version = num2;
			owaFilterState.viewFilter = owaViewFilter;
			if (flag)
			{
				owaFilterState.fromFilter = array2[2];
			}
			return owaFilterState;
		}

		private static string[] GetPropertyValueToSave(MailboxSession mailboxSession, StoreId sourceFolderId, OwaViewFilter viewFilter, string fromFilter)
		{
			List<string> list = new List<string>();
			string item = string.Empty;
			try
			{
				item = StoreId.StoreIdToEwsId(mailboxSession.MailboxGuid, sourceFolderId);
			}
			catch (Exception)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "[OwaFilterState::GetPropertyValueToSave] id of property was not valid EWS id");
				return null;
			}
			int num = (int)viewFilter;
			string text = (!string.IsNullOrEmpty(fromFilter)) ? (':' + fromFilter) : string.Empty;
			list.Add(item);
			list.Add(string.Concat(new object[]
			{
				num.ToString(),
				':',
				15.1,
				text
			}));
			return list.ToArray();
		}

		private static SearchFolder BindAndUpdateExistingFilteredViewSearchFolder(MailboxSession mailboxSession, OwaSearchContext searchContext)
		{
			SearchFolder searchFolder = OwaFilterState.BindExistingFilteredViewSearchFolder(searchContext.SearchFolderId, mailboxSession, searchContext);
			if (searchFolder != null)
			{
				string folderProperty = OwaFilterState.GetFolderProperty<string>(searchFolder, FolderSchema.DisplayName, string.Empty);
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "[OwaFilterState::BindAndUpdateExistingFilteredViewSearchFolder] Re-using existing filtered view search folder: {0}", folderProperty);
				searchContext.IsSearchFailed = !OwaFilterState.EnsureSearchIsCompleted(searchFolder, mailboxSession, searchContext);
				if (!searchContext.IsSearchFailed)
				{
					searchFolder[OwaFilteredViewProperties.FilteredViewAccessTime] = ExDateTime.Now;
					searchFolder.Save();
					searchFolder.Load();
					if ((searchContext.ViewFilterActions & ViewFilterActions.BindToExisting) == ViewFilterActions.None && OwaFilterState.LinkViewFolder(mailboxSession, searchContext.FolderIdToSearch, searchFolder, searchContext.ViewFilter))
					{
						searchContext.ViewFilterActions |= ViewFilterActions.LinkToSourceFolderSucceeded;
					}
				}
			}
			return searchFolder;
		}

		private static SearchFolder BindExistingFilteredViewSearchFolder(StoreId searchFolderId, MailboxSession mailboxSession, OwaSearchContext searchContext)
		{
			SearchFolder result = null;
			try
			{
				result = SearchFolder.Bind(mailboxSession, searchFolderId, OwaFilterState.FolderQueryProperties);
			}
			catch (ObjectNotFoundException)
			{
				searchContext.ViewFilterActions |= ViewFilterActions.ObjectNotFoundException;
				string message = string.Format("[OwaFilterState::BindExistingFilteredViewSearchFolder] Attempt to bind to search folder failed {0}. ViewFilterActions: {1}", searchFolderId.ToString(), (int)searchContext.ViewFilterActions);
				ExTraceGlobals.StorageTracer.TraceDebug(0L, message);
			}
			return result;
		}

		private static SearchFolder CreateAndUpdateFilteredViewSearchFolder(MailboxSession mailboxSession, OwaSearchContext searchContext, StoreId searchFoldersRootId, SearchFolderCriteria searchFolderCriteria)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<string, string>(0L, "[OwaFilterState::CreateAndUpdateFilteredViewSearchFolder] Create filtered view search folder for view filter: {0} search folder identity: {1} ", searchContext.ViewFilter.ToString(), searchContext.ClientSearchFolderIdentity);
			SearchFolder searchFolder = SearchFolder.Create(mailboxSession, searchFoldersRootId, searchContext.ClientSearchFolderIdentity, CreateMode.OpenIfExists);
			OwaFilterState.UpdateFilteredViewSearchFolder(mailboxSession, searchFolder, searchContext);
			searchContext.SearchFolderId = searchFolder.StoreObjectId;
			searchContext.IsSearchFailed = !OwaFilterState.ApplyOwaContinuousSearch(searchFolder, searchFolderCriteria, mailboxSession, searchContext);
			if (searchContext.IsSearchFailed)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "[OwaFilterState::CreateAndUpdateFilteredViewSearchFolder] deleting searchContext.SearchFolderId: " + searchContext.SearchFolderId);
				searchContext.ViewFilterActions |= ViewFilterActions.DeleteInvalidSearchFolder;
				mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					searchContext.SearchFolderId
				});
				searchFolder.Dispose();
				searchFolder = null;
			}
			else if (OwaFilterState.LinkViewFolder(mailboxSession, searchContext.FolderIdToSearch, searchFolder, searchContext.ViewFilter))
			{
				searchContext.ViewFilterActions |= ViewFilterActions.LinkToSourceFolderSucceeded;
			}
			return searchFolder;
		}

		private static void UpdateFilteredViewSearchFolder(MailboxSession mailboxSession, SearchFolder searchFolder, OwaSearchContext searchContext)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<string, OwaViewFilter>(0L, "[OwaFilterState::UpdateFilteredViewSearchFolder] updating search folder: {0} for filter: {1}", searchContext.ClientSearchFolderIdentity, searchContext.ViewFilter);
			searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
			searchFolder[OwaFilteredViewProperties.FilteredViewLabel] = OwaFilterState.GetPropertyValueToSave(mailboxSession, searchContext.FolderIdToSearch, searchContext.ViewFilter, searchContext.FromFilter);
			searchFolder[OwaFilteredViewProperties.FilteredViewAccessTime] = ExDateTime.Now;
			int folderProperty = OwaFilterState.GetFolderProperty<int>(searchFolder, FolderSchema.ExtendedFolderFlags, 0);
			searchFolder[FolderSchema.ExtendedFolderFlags] = (folderProperty | 4194304);
			searchFolder.Save();
			searchFolder.Load();
		}

		internal static List<object[]> FetchRowsFromQueryResult(QueryResult queryResult, int rowsToGet)
		{
			int estimatedRowCount = queryResult.EstimatedRowCount;
			int num = queryResult.EstimatedRowCount - queryResult.CurrentRow;
			if (rowsToGet > num)
			{
				rowsToGet = num;
			}
			List<object[]> list = new List<object[]>();
			bool flag = rowsToGet > 0;
			while (flag)
			{
				bool flag2;
				object[][] rows = queryResult.GetRows(rowsToGet, out flag2);
				int length = rows.GetLength(0);
				rowsToGet -= length;
				for (int i = 0; i < length; i++)
				{
					list.Add(rows[i]);
				}
				flag = (flag2 && rowsToGet > 0);
			}
			return list;
		}

		internal static T GetFolderProperty<T>(Folder folder, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = folder.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		internal static Dictionary<PropertyDefinition, int> LoadPropertyMap(PropertyDefinition[] properties)
		{
			Dictionary<PropertyDefinition, int> dictionary = new Dictionary<PropertyDefinition, int>();
			for (int i = 0; i < properties.Length; i++)
			{
				dictionary[properties[i]] = i;
			}
			return dictionary;
		}

		private static Dictionary<OwaViewFilter, PropertyDefinition> LoadLinkPropertyMap()
		{
			return new Dictionary<OwaViewFilter, PropertyDefinition>
			{
				{
					OwaViewFilter.NoClutter,
					FolderSchema.UnClutteredViewFolderEntryId
				},
				{
					OwaViewFilter.Clutter,
					FolderSchema.ClutteredViewFolderEntryId
				}
			};
		}

		private static void SendWatsonWithoutDump(Exception e, string information)
		{
			ExWatson.SendReport(e, ReportOptions.DoNotCollectDumps, information);
		}

		private const string FilteredViewSearchFolderPrefix = "OwaFV";

		private const double CurrentFilterConditionVersion = 15.1;

		private const char SplitCharBetweenViewFilterAndVersion = ':';

		public static readonly Dictionary<OwaViewFilter, PropertyDefinition> FilterToLinkPropertyDefinitionsMap = OwaFilterState.LoadLinkPropertyMap();

		private static readonly PropertyDefinition[] FolderQueryProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ParentEntryId,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.RecordKey,
			FolderSchema.SearchFolderAllowAgeout,
			FolderSchema.DisplayName,
			FolderSchema.ExtendedFolderFlags,
			StoreObjectSchema.LastModifiedTime,
			OwaFilteredViewProperties.FilteredViewLabel,
			OwaFilteredViewProperties.FilteredViewAccessTime
		};

		private static readonly PropertyDefinition[] SourceFolderQueryProperties = new PropertyDefinition[]
		{
			FolderSchema.UnClutteredViewFolderEntryId,
			FolderSchema.ClutteredViewFolderEntryId
		};

		private static readonly Dictionary<PropertyDefinition, int> FilterSearchFolderPropertyIndexes = OwaFilterState.LoadPropertyMap(OwaFilterState.FolderQueryProperties);

		private StoreId sourceFolderId;

		private double version = 15.1;

		private OwaViewFilter viewFilter;

		private string fromFilter;
	}
}
