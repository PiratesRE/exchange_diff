using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class SearchPerformanceData
	{
		internal static void WriteIndividualLog(OwaContext owaContext, string key, object value)
		{
			string text = string.Format("{0}={1}", key, value);
			if (Globals.CollectPerRequestPerformanceStats)
			{
				owaContext.OwaPerformanceData.TraceOther(text);
			}
			owaContext.HttpContext.Response.AppendToLog("&" + text);
		}

		private static bool GetSearchFolderData(UserContext userContext, out int searchFolderItemCount, out SearchState searchState, out bool isRemoteSession)
		{
			searchFolderItemCount = -1;
			searchState = SearchState.None;
			isRemoteSession = false;
			bool result = false;
			if (userContext.SearchFolderId != null)
			{
				try
				{
					using (SearchFolder searchFolder = SearchFolder.Bind(userContext.SearchFolderId.GetSession(userContext), userContext.SearchFolderId.StoreObjectId, SearchPerformanceData.searchFolderProperties))
					{
						object obj = searchFolder.TryGetProperty(FolderSchema.SearchFolderItemCount);
						if (obj is int)
						{
							searchFolderItemCount = (int)obj;
						}
						searchState = searchFolder.GetSearchCriteria().SearchState;
						isRemoteSession = searchFolder.Session.IsRemote;
					}
					result = true;
				}
				catch (ObjectNotFoundException)
				{
				}
			}
			return result;
		}

		internal void StartSearch(string searchString)
		{
			this.searchId = Guid.NewGuid();
			this.searchString = searchString;
			this.firstPageLatency = new SearchPerformanceData.NotificationEventLatency("srchfp");
			this.completeLatency = new SearchPerformanceData.NotificationEventLatency("srchcomp");
			this.refreshLatency = new SearchPerformanceData.RefreshLatency("srchref");
			this.writeSearchFolderData = false;
			this.watch.Reset();
			this.watch.Start();
		}

		internal void FirstPage(int itemCount)
		{
			this.firstPageLatency.Latency = this.watch.ElapsedMilliseconds;
			this.firstPageLatency.ItemCount = itemCount;
			this.writeSearchFolderData = true;
		}

		internal void Complete(bool isTimeout, bool isSync)
		{
			this.completeLatency.Latency = this.watch.ElapsedMilliseconds;
			this.isTimeout = isTimeout;
			this.isSync = isSync;
			this.writeSearchFolderData = true;
		}

		internal void RefreshStart()
		{
			this.refreshLatency.RefreshStart = this.watch.ElapsedMilliseconds;
		}

		internal void RefreshEnd()
		{
			this.refreshLatency.RefreshEnd = this.watch.ElapsedMilliseconds;
		}

		internal void NotificationPickedUpByPendingGet(SearchNotificationType searchNotificationType)
		{
			if ((searchNotificationType & SearchNotificationType.FirstPage) != SearchNotificationType.None && this.firstPageLatency.PendingGetPickup == -1L)
			{
				this.firstPageLatency.PendingGetPickup = this.watch.ElapsedMilliseconds;
			}
			if ((searchNotificationType & SearchNotificationType.Complete) != SearchNotificationType.None)
			{
				this.completeLatency.PendingGetPickup = this.watch.ElapsedMilliseconds;
			}
		}

		internal void SearchFailed()
		{
			this.searchFailed = true;
		}

		internal void SlowSearchEnabled()
		{
			this.slowSearchEnabled = true;
		}

		internal void WriteLog()
		{
			OwaContext owaContext = OwaContext.Current;
			if (owaContext == null)
			{
				throw new InvalidOperationException("OwaContext.Current is null. SearchPerformanceData.WriteLog should be called in the context of a request.");
			}
			SearchPerformanceData.WriteIndividualLog(owaContext, "srchid", this.searchId);
			if (Globals.CollectSearchStrings && this.searchString != null)
			{
				SearchPerformanceData.WriteIndividualLog(owaContext, "srchstr", this.searchString);
				this.searchString = null;
			}
			if (this.refreshLatency != null)
			{
				this.refreshLatency.WriteLog(owaContext);
			}
			if (this.firstPageLatency != null)
			{
				this.firstPageLatency.WriteLog(owaContext);
			}
			if (this.completeLatency != null)
			{
				this.completeLatency.WriteLog(owaContext);
			}
			if (this.completeLatency.Latency >= 0L)
			{
				if (this.isTimeout)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, "srchto", 1);
					this.isTimeout = false;
				}
				if (this.isSync)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, "srchsync", 1);
					this.isSync = false;
				}
			}
			if (this.searchFailed)
			{
				SearchPerformanceData.WriteIndividualLog(owaContext, "srchfl", "1");
			}
			if (this.slowSearchEnabled)
			{
				SearchPerformanceData.WriteIndividualLog(owaContext, "srchslowenabled", "1");
			}
			if (this.writeSearchFolderData)
			{
				SearchState searchState = SearchState.None;
				int num = -1;
				bool flag = false;
				bool searchFolderData = SearchPerformanceData.GetSearchFolderData(owaContext.UserContext, out num, out searchState, out flag);
				if (searchFolderData)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, "srchsess", flag);
				}
				if (searchState != SearchState.None)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, "srchstate", string.Format(NumberFormatInfo.InvariantInfo, "0x{0:X8}", new object[]
					{
						(int)searchState
					}));
				}
				if (num != -1)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, "srchcount", num);
				}
				this.writeSearchFolderData = false;
			}
		}

		private const string FirstPageKey = "srchfp";

		private const string CompleteKey = "srchcomp";

		private const string RefreshKey = "srchref";

		private const string IsTimeoutKey = "srchto";

		private const string IsSyncKey = "srchsync";

		private const string SearchIdKey = "srchid";

		private const string SearchStringKey = "srchstr";

		private const string SearchFolderItemCountKey = "srchcount";

		private const string SearchStateKey = "srchstate";

		private const string IsRemoteSession = "srchsess";

		private const string SearchFailedKey = "srchfl";

		private const string SlowSearchEnabledKey = "srchslowenabled";

		private static readonly StorePropertyDefinition[] searchFolderProperties = new StorePropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.ItemCount,
			FolderSchema.SearchFolderItemCount
		};

		private Stopwatch watch = new Stopwatch();

		private Guid searchId;

		private SearchPerformanceData.NotificationEventLatency firstPageLatency;

		private SearchPerformanceData.NotificationEventLatency completeLatency;

		private SearchPerformanceData.RefreshLatency refreshLatency;

		private bool isTimeout;

		private bool isSync;

		private string searchString;

		private bool writeSearchFolderData;

		private bool searchFailed;

		private bool slowSearchEnabled;

		internal class NotificationEventLatency
		{
			internal long Latency { get; set; }

			internal long PendingGetPickup { get; set; }

			internal int ItemCount { get; set; }

			internal NotificationEventLatency(string key)
			{
				this.Latency = -1L;
				this.ItemCount = -1;
				this.PendingGetPickup = -1L;
				this.key = key;
			}

			internal void WriteLog(OwaContext owaContext)
			{
				if (this.Latency >= 0L)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, this.key, this.Latency);
					this.Latency = -1L;
				}
				if (this.ItemCount >= 0)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, this.key + "ic", this.ItemCount);
					this.ItemCount = -1;
				}
				if (this.PendingGetPickup >= 0L)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, this.key + "pg", this.PendingGetPickup);
					this.PendingGetPickup = -1L;
				}
			}

			private const string ItemCountSuffix = "ic";

			private const string PendingGetPickupTimeSuffix = "pg";

			private readonly string key;
		}

		internal class RefreshLatency
		{
			internal long RefreshStart { get; set; }

			internal long RefreshEnd { get; set; }

			internal RefreshLatency(string key)
			{
				this.RefreshStart = -1L;
				this.RefreshEnd = -1L;
				this.key = key;
			}

			internal void WriteLog(OwaContext owaContext)
			{
				if (this.RefreshStart >= 0L)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, this.key + "s", this.RefreshStart);
					this.RefreshStart = -1L;
				}
				if (this.RefreshEnd >= 0L)
				{
					SearchPerformanceData.WriteIndividualLog(owaContext, this.key + "e", this.RefreshEnd);
					this.RefreshEnd = -1L;
				}
			}

			private const string RequestStartSuffix = "s";

			private const string RequestEndSuffix = "e";

			private readonly string key;
		}
	}
}
