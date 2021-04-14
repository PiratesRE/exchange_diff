using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class FolderSearch
	{
		private static List<string> NonContentIndexedProperties
		{
			get
			{
				if (FolderSearch.nonContentIndexedProperties == null)
				{
					FolderSearch.nonContentIndexedProperties = new List<string>();
					FolderSearch.nonContentIndexedProperties.Add("IsRead");
					FolderSearch.nonContentIndexedProperties.Add("HasAttachment");
					FolderSearch.nonContentIndexedProperties.Add("FlagStatusProperty");
					FolderSearch.nonContentIndexedProperties.Add("Category");
					FolderSearch.nonContentIndexedProperties.Add("Categories");
				}
				return FolderSearch.nonContentIndexedProperties;
			}
		}

		public QueryFilter AdvancedQueryFilter
		{
			get
			{
				return this.advancedQueryFilter;
			}
			set
			{
				this.advancedQueryFilter = value;
			}
		}

		public Folder Execute(UserContext userContext, Folder folder, SearchScope searchScope, string searchString, bool newSearch, bool asyncSearch)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			MailboxSession mailboxSession = (MailboxSession)folder.Session;
			QueryFilter queryFilter = SearchFilterGenerator.Execute(searchString, mailboxSession.Mailbox.IsContentIndexingEnabled, userContext.UserCulture, new PolicyTagMailboxProvider(userContext.MailboxSession), folder, searchScope, this.advancedQueryFilter);
			string text = (queryFilter == null) ? string.Empty : queryFilter.ToString();
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromStoreObject(folder);
			SearchFolder searchFolder = null;
			bool flag = false;
			Folder result;
			try
			{
				if (userContext.SearchFolderId != null)
				{
					if (!newSearch && userContext.LastSearchFolderId.Equals(owaStoreObjectId) && userContext.LastSearchQueryFilter.CompareTo(text) == 0 && userContext.LastSearchScope == searchScope)
					{
						try
						{
							searchFolder = SearchFolder.Bind(userContext.SearchFolderId.GetSession(userContext), userContext.SearchFolderId.StoreObjectId, FolderSearch.PrefetchProperties);
						}
						catch (ObjectNotFoundException)
						{
						}
						if (searchFolder != null)
						{
							if (asyncSearch)
							{
								SearchPerformanceData searchPerformanceData = userContext.MapiNotificationManager.GetSearchPerformanceData(mailboxSession);
								if (searchPerformanceData != null)
								{
									searchPerformanceData.RefreshStart();
								}
								OwaContext.Current.SearchPerformanceData = searchPerformanceData;
							}
							flag = true;
							return searchFolder;
						}
					}
					if (userContext.IsPushNotificationsEnabled)
					{
						userContext.MapiNotificationManager.CancelSearchNotifications(mailboxSession);
						userContext.MapiNotificationManager.AddSearchFolderDeleteList(mailboxSession, userContext.SearchFolderId.StoreObjectId);
					}
					else
					{
						mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
						{
							userContext.SearchFolderId.StoreObjectId
						});
					}
					userContext.SearchFolderId = null;
				}
				using (Folder folder2 = Folder.Bind(mailboxSession, userContext.GetSearchFoldersId(mailboxSession).StoreObjectId))
				{
					searchFolder = SearchFolder.Create(mailboxSession, folder2.Id.ObjectId, "OWA Search " + userContext.Key.UserContextId + " " + DateTime.UtcNow.ToString("o"), CreateMode.CreateNew);
				}
				searchFolder.Save();
				searchFolder.Load(FolderSearch.PrefetchProperties);
				userContext.SearchFolderId = OwaStoreObjectId.CreateFromStoreObject(searchFolder);
				userContext.LastSearchFolderId = owaStoreObjectId;
				userContext.LastSearchQueryFilter = text;
				userContext.LastSearchScope = searchScope;
				if (queryFilter == null)
				{
					flag = true;
					result = searchFolder;
				}
				else
				{
					bool flag2 = FolderSearch.FailNonContentIndexedSearch(folder, queryFilter);
					bool flag3;
					StoreId[] folderScope;
					if (searchScope == SearchScope.SelectedFolder || !mailboxSession.Mailbox.IsContentIndexingEnabled)
					{
						flag3 = false;
						folderScope = new StoreId[]
						{
							folder.Id.ObjectId
						};
					}
					else if (searchScope == SearchScope.SelectedAndSubfolders)
					{
						flag3 = true;
						folderScope = new StoreId[]
						{
							folder.Id.ObjectId
						};
					}
					else
					{
						if (searchScope != SearchScope.AllFoldersAndItems && searchScope != SearchScope.AllItemsInModule)
						{
							throw new ArgumentOutOfRangeException();
						}
						flag3 = true;
						folderScope = new StoreId[]
						{
							userContext.GetRootFolderId(mailboxSession)
						};
					}
					if (searchScope != SearchScope.SelectedFolder)
					{
						if (!folder.Id.ObjectId.Equals(userContext.JunkEmailFolderId))
						{
							QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.ParentItemId, userContext.JunkEmailFolderId);
							queryFilter = new AndFilter(new QueryFilter[]
							{
								queryFilter,
								queryFilter2
							});
						}
						StoreObjectId storeObjectId = userContext.GetDeletedItemsFolderId((MailboxSession)folder.Session).StoreObjectId;
						if (!folder.Id.ObjectId.Equals(storeObjectId))
						{
							QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.ParentItemId, storeObjectId);
							queryFilter = new AndFilter(new QueryFilter[]
							{
								queryFilter,
								queryFilter3
							});
						}
					}
					ExTraceGlobals.MailDataTracer.TraceDebug((long)this.GetHashCode(), "Search\nFilter: {0}\nDeep Traversal: {1}\nFolder: {2}\nContent Indexing Enabled: {3}", new object[]
					{
						queryFilter,
						flag3,
						folder.Id,
						mailboxSession.Mailbox.IsContentIndexingEnabled ? "Yes" : "No"
					});
					SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(queryFilter, folderScope);
					searchFolderCriteria.DeepTraversal = flag3;
					searchFolderCriteria.MaximumResultsCount = new int?(1000);
					searchFolderCriteria.FailNonContentIndexedSearch = flag2;
					SearchPerformanceData searchPerformanceData;
					if (!userContext.IsPushNotificationsEnabled || !asyncSearch)
					{
						searchPerformanceData = new SearchPerformanceData();
						searchPerformanceData.StartSearch(searchString);
						IAsyncResult asyncResult = searchFolder.BeginApplyOneTimeSearch(searchFolderCriteria, null, null);
						Stopwatch stopwatch = Utilities.StartWatch();
						bool flag4 = asyncResult.AsyncWaitHandle.WaitOne(30000, false);
						searchPerformanceData.Complete(!flag4, true);
						if (flag4)
						{
							searchFolder.EndApplyOneTimeSearch(asyncResult);
						}
						else
						{
							ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "FolderSearch.Execute. Search timed out.");
							if (Globals.ArePerfCountersEnabled)
							{
								OwaSingleCounters.SearchesTimedOut.Increment();
							}
						}
						Utilities.StopWatch(stopwatch, "FolderSearch.Execute (Wait for search to complete)");
						if (Globals.ArePerfCountersEnabled)
						{
							PerformanceCounterManager.UpdateSearchTimePerformanceCounter(stopwatch.ElapsedMilliseconds);
							OwaSingleCounters.TotalSearches.Increment();
						}
						searchFolder.Save();
						searchFolder.Load(FolderSearch.PrefetchProperties);
					}
					else
					{
						userContext.MapiNotificationManager.InitSearchNotifications(mailboxSession, userContext.SearchFolderId.StoreObjectId, searchFolder, searchFolderCriteria, searchString);
						searchPerformanceData = userContext.MapiNotificationManager.GetSearchPerformanceData(mailboxSession);
					}
					if (!flag2)
					{
						searchPerformanceData.SlowSearchEnabled();
					}
					OwaContext.Current.SearchPerformanceData = searchPerformanceData;
					flag = true;
					result = searchFolder;
				}
			}
			finally
			{
				if (!flag && searchFolder != null)
				{
					searchFolder.Dispose();
				}
			}
			return result;
		}

		private static bool FailNonContentIndexedSearch(Folder folder, QueryFilter searchFilter)
		{
			if (folder is SearchFolder || folder.Session is PublicFolderSession)
			{
				return false;
			}
			SinglePropertyFilter singlePropertyFilter = null;
			if (searchFilter is SinglePropertyFilter)
			{
				singlePropertyFilter = (searchFilter as SinglePropertyFilter);
			}
			else
			{
				if (searchFilter is NotFilter)
				{
					return FolderSearch.FailNonContentIndexedSearch(folder, ((NotFilter)searchFilter).Filter);
				}
				if (searchFilter is FalseFilter)
				{
					return true;
				}
				if (searchFilter is CompositeFilter)
				{
					return true;
				}
			}
			return !FolderSearch.NonContentIndexedProperties.Contains(singlePropertyFilter.Property.Name);
		}

		internal static void ClearSearchFolders(MailboxSession mailboxSession)
		{
			using (Folder folder = Utilities.SafeFolderBind(mailboxSession, DefaultFolderType.SearchFolders, new PropertyDefinition[0]))
			{
				if (folder != null)
				{
					TextFilter textFilter = new TextFilter(StoreObjectSchema.DisplayName, "OWA Search ", MatchOptions.Prefix, MatchFlags.Default);
					ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.LessThan, StoreObjectSchema.LastModifiedTime, DateTime.UtcNow.AddDays(-7.0));
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, new AndFilter(new QueryFilter[]
					{
						comparisonFilter,
						textFilter
					}), null, new PropertyDefinition[]
					{
						FolderSchema.Id
					}))
					{
						List<StoreId> list = new List<StoreId>();
						for (;;)
						{
							object[][] rows = queryResult.GetRows(10000);
							if (rows.Length <= 0)
							{
								break;
							}
							for (int i = 0; i < rows.Length; i++)
							{
								list.Add((StoreId)rows[i][0]);
							}
						}
						if (list.Count > 0)
						{
							mailboxSession.Delete(DeleteItemFlags.HardDelete, list.ToArray());
						}
					}
				}
			}
		}

		internal const string OwaSearchFolderName = "OWA Search ";

		internal const int MaximumSearchResults = 1000;

		private const int MaximumSearchTime = 30;

		private static readonly StorePropertyDefinition[] PrefetchProperties = new StorePropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ContainerClass,
			FolderSchema.ItemCount,
			FolderSchema.UnreadCount,
			FolderSchema.SearchFolderItemCount
		};

		private static List<string> nonContentIndexedProperties;

		private QueryFilter advancedQueryFilter;
	}
}
