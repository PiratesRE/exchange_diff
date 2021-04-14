using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Common
{
	internal static class PerformanceCounterManager
	{
		internal static bool ArePerfCountersEnabled { get; set; }

		internal static void InitializeIMQueueSizes(int signInFailureQueueSize, int sentMessageFailureQueueSize)
		{
			PerformanceCounterManager.instantMessagingLogonResultsQueueSize = signInFailureQueueSize;
			PerformanceCounterManager.instantMessagingLogonResultsQueue = new FixedSizeQueueBool(signInFailureQueueSize);
			PerformanceCounterManager.sentInstantMessageResultsQueueSize = sentMessageFailureQueueSize;
			PerformanceCounterManager.sentInstantMessageResultsQueue = new FixedSizeQueueBool(sentMessageFailureQueueSize);
		}

		internal static Dictionary<string, UniqueUserData> UniqueUserTable
		{
			get
			{
				return PerformanceCounterManager.uniqueUserTable;
			}
		}

		internal static int MailboxOfflineExResultsQueueSize
		{
			get
			{
				return PerformanceCounterManager.mailboxOfflineExResultsQueueSize;
			}
			set
			{
				PerformanceCounterManager.mailboxOfflineExResultsQueueSize = value;
			}
		}

		internal static int ConnectionFailedTransientExResultsQueueSize
		{
			get
			{
				return PerformanceCounterManager.connectionFailedTransientExResultsQueueSize;
			}
			set
			{
				PerformanceCounterManager.connectionFailedTransientExResultsQueueSize = value;
			}
		}

		internal static int StorageTransientExResultsQueueSize
		{
			get
			{
				return PerformanceCounterManager.storageTransientExResultsQueueSize;
			}
			set
			{
				PerformanceCounterManager.storageTransientExResultsQueueSize = value;
			}
		}

		internal static int StoragePermanentExResultsQueueSize
		{
			get
			{
				return PerformanceCounterManager.storagePermanentExResultsQueueSize;
			}
			set
			{
				PerformanceCounterManager.storagePermanentExResultsQueueSize = value;
			}
		}

		internal static void InitializeExPerfCountersQueueSizes()
		{
			PerformanceCounterManager.mailboxOfflineExResultsQueue = new FixedSizeQueueBool(PerformanceCounterManager.mailboxOfflineExResultsQueueSize);
			PerformanceCounterManager.connectionFailedTransientExResultsQueue = new FixedSizeQueueBool(PerformanceCounterManager.connectionFailedTransientExResultsQueueSize);
			PerformanceCounterManager.storageTransientExResultsQueue = new FixedSizeQueueBool(PerformanceCounterManager.storageTransientExResultsQueueSize);
			PerformanceCounterManager.storagePermanentExResultsQueue = new FixedSizeQueueBool(PerformanceCounterManager.storagePermanentExResultsQueueSize);
		}

		internal static void InitializePerformanceCounters()
		{
			try
			{
				OwaSingleCounters.CurrentUsers.RawValue = 0L;
				OwaSingleCounters.CurrentUniqueUsers.RawValue = 0L;
				OwaSingleCounters.TotalUsers.RawValue = 0L;
				OwaSingleCounters.TotalUniqueUsers.RawValue = 0L;
				OwaSingleCounters.PeakUserCount.RawValue = 0L;
				OwaSingleCounters.CurrentUsersLight.RawValue = 0L;
				OwaSingleCounters.CurrentUniqueUsersLight.RawValue = 0L;
				OwaSingleCounters.TotalUsersLight.RawValue = 0L;
				OwaSingleCounters.TotalUniqueUsersLight.RawValue = 0L;
				OwaSingleCounters.PeakUserCountLight.RawValue = 0L;
				OwaSingleCounters.CurrentUsersPremium.RawValue = 0L;
				OwaSingleCounters.CurrentUniqueUsersPremium.RawValue = 0L;
				OwaSingleCounters.TotalUsersPremium.RawValue = 0L;
				OwaSingleCounters.TotalUniqueUsersPremium.RawValue = 0L;
				OwaSingleCounters.PeakUserCountPremium.RawValue = 0L;
				OwaSingleCounters.AttachmentsUploaded.RawValue = 0L;
				OwaSingleCounters.ItemsCreated.RawValue = 0L;
				OwaSingleCounters.ItemsDeleted.RawValue = 0L;
				OwaSingleCounters.ItemsUpdated.RawValue = 0L;
				OwaSingleCounters.MailViewsLoaded.RawValue = 0L;
				OwaSingleCounters.MailViewRefreshes.RawValue = 0L;
				OwaSingleCounters.MessagesSent.RawValue = 0L;
				OwaSingleCounters.IRMMessagesSent.RawValue = 0L;
				OwaSingleCounters.AverageResponseTime.RawValue = 0L;
				PerformanceCounterManager.responseTimeAverage = 0.0;
				OwaSingleCounters.IMAverageSignOnTime.RawValue = 0L;
				PerformanceCounterManager.imSignOnTimeAverage = 0.0;
				OwaSingleCounters.TotalSessionsEndedByLogoff.RawValue = 0L;
				OwaSingleCounters.TotalSessionsEndedByTimeout.RawValue = 0L;
				OwaSingleCounters.CalendarViewsLoaded.RawValue = 0L;
				OwaSingleCounters.CalendarViewsRefreshed.RawValue = 0L;
				OwaSingleCounters.AverageSearchTime.RawValue = 0L;
				PerformanceCounterManager.searchTimeAverage = 0.0;
				OwaSingleCounters.TotalSearches.RawValue = 0L;
				OwaSingleCounters.TotalRequests.RawValue = 0L;
				OwaSingleCounters.TotalRequestsFailed.RawValue = 0L;
				OwaSingleCounters.SearchesTimedOut.RawValue = 0L;
				OwaSingleCounters.TotalSpellchecks.RawValue = 0L;
				OwaSingleCounters.AverageSpellcheckTime.RawValue = 0L;
				OwaSingleCounters.InvalidCanaryRequests.RawValue = 0L;
				OwaSingleCounters.PID.RawValue = (long)Process.GetCurrentProcess().Id;
				OwaSingleCounters.NamesChecked.RawValue = 0L;
				OwaSingleCounters.PasswordChanges.RawValue = 0L;
				OwaSingleCounters.CurrentProxiedUsers.RawValue = 0L;
				OwaSingleCounters.ProxiedUserRequests.RawValue = 0L;
				OwaSingleCounters.ProxiedResponseTimeAverage.RawValue = 0L;
				PerformanceCounterManager.proxiedResponseTimeAverage = 0.0;
				OwaSingleCounters.ProxyRequestBytes.RawValue = 0L;
				OwaSingleCounters.ProxyResponseBytes.RawValue = 0L;
				OwaSingleCounters.WssBytes.RawValue = 0L;
				OwaSingleCounters.UncBytes.RawValue = 0L;
				OwaSingleCounters.WssRequests.RawValue = 0L;
				OwaSingleCounters.UncRequests.RawValue = 0L;
				OwaSingleCounters.ASQueries.RawValue = 0L;
				OwaSingleCounters.ASQueriesFailurePercent.RawValue = 0L;
				OwaSingleCounters.StoreLogonFailurePercent.RawValue = 0L;
				OwaSingleCounters.CASIntraSiteRedirectionLatertoEarlierVersion.RawValue = 0L;
				OwaSingleCounters.CASIntraSiteRedirectionEarliertoLaterVersion.RawValue = 0L;
				OwaSingleCounters.CASCrossSiteRedirectionLatertoEarlierVersion.RawValue = 0L;
				OwaSingleCounters.CASCrossSiteRedirectionEarliertoLaterVersion.RawValue = 0L;
				OwaSingleCounters.ActiveMailboxSubscriptions.RawValue = 0L;
				OwaSingleCounters.TotalMailboxNotifications.RawValue = 0L;
				OwaSingleCounters.TotalUserContextReInitializationRequests.RawValue = 0L;
				OwaSingleCounters.MailboxOfflineExceptionFailuresPercent.RawValue = 0L;
				OwaSingleCounters.ConnectionFailedTransientExceptionPercent.RawValue = 0L;
				OwaSingleCounters.StorageTransientExceptionPercent.RawValue = 0L;
				OwaSingleCounters.StoragePermanentExceptionPercent.RawValue = 0L;
				OwaSingleCounters.IMCurrentUsers.RawValue = 0L;
				OwaSingleCounters.IMTotalInstantMessagesReceived.RawValue = 0L;
				OwaSingleCounters.IMTotalInstantMessagesSent.RawValue = 0L;
				OwaSingleCounters.IMTotalLogonFailures.RawValue = 0L;
				OwaSingleCounters.IMTotalMessageDeliveryFailures.RawValue = 0L;
				OwaSingleCounters.IMTotalPresenceQueries.RawValue = 0L;
				OwaSingleCounters.IMTotalUsers.RawValue = 0L;
				OwaSingleCounters.IMLogonFailuresPercent.RawValue = 0L;
				OwaSingleCounters.IMSentMessageDeliveryFailuresPercent.RawValue = 0L;
				OwaSingleCounters.OwaToEwsRequestFailureRate.RawValue = 0L;
				OwaSingleCounters.RequestTimeouts.RawValue = 0L;
				OwaSingleCounters.SenderPhotosTotalLDAPCalls.RawValue = 0L;
				OwaSingleCounters.SenderPhotosTotalLDAPCallsWithPicture.RawValue = 0L;
				OwaSingleCounters.SenderPhotosNegativeCacheCount.RawValue = 0L;
				OwaSingleCounters.SenderPhotosDataFromNegativeCacheCount.RawValue = 0L;
				OwaSingleCounters.AggregatedUserConfigurationPartsRebuilt.RawValue = 0L;
				OwaSingleCounters.AggregatedUserConfigurationPartsRequested.RawValue = 0L;
				OwaSingleCounters.AggregatedUserConfigurationPartsRead.RawValue = 0L;
				OwaSingleCounters.SessionDataCacheBuildsStarted.RawValue = 0L;
				OwaSingleCounters.SessionDataCacheBuildsCompleted.RawValue = 0L;
				OwaSingleCounters.SessionDataCacheWaitedForPreload.RawValue = 0L;
				OwaSingleCounters.SessionDataCacheUsed.RawValue = 0L;
				OwaSingleCounters.SessionDataCacheWaitTimeout.RawValue = 0L;
				PerformanceCounterManager.ArePerfCountersEnabled = true;
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<string, string>(0L, "Failed to initialize perfmon counters, perf data will not be available. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
			}
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.CurrentUniqueUsers.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.CurrentUniqueUsersLight.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.CurrentUniqueUsersPremium.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.CurrentUsers.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.CurrentUsersLight.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.CurrentUsersPremium.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.CurrentProxiedUsers.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterValues.Add(OwaSingleCounters.IMCurrentUsers.CounterName, 0L);
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.CurrentUniqueUsers.CounterName, new object());
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.CurrentUniqueUsersLight.CounterName, new object());
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.CurrentUniqueUsersPremium.CounterName, new object());
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.CurrentUsers.CounterName, new object());
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.CurrentUsersLight.CounterName, new object());
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.CurrentUsersPremium.CounterName, new object());
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.CurrentProxiedUsers.CounterName, new object());
			PerformanceCounterManager.currentUserCounterLocks.Add(OwaSingleCounters.IMCurrentUsers.CounterName, new object());
			ThrottlingPerfCounterWrapper.Initialize(BudgetType.Owa);
		}

		public static UniqueUserData GetUniqueUserData(string userName)
		{
			UniqueUserData result;
			lock (PerformanceCounterManager.uniqueUserTable)
			{
				UniqueUserData uniqueUserData = null;
				if (!PerformanceCounterManager.uniqueUserTable.TryGetValue(userName, out uniqueUserData))
				{
					result = null;
				}
				else
				{
					result = uniqueUserData;
				}
			}
			return result;
		}

		public static void UpdateSearchTimePerformanceCounter(long newValue)
		{
			PerformanceCounterManager.UpdateMovingAveragePerformanceCounter(OwaSingleCounters.AverageSearchTime, newValue, ref PerformanceCounterManager.searchTimeAverage, PerformanceCounterManager.searchTimeAverageLock);
		}

		public static void UpdateSpellcheckTimePerformanceCounter(long newValue)
		{
			PerformanceCounterManager.UpdateMovingAveragePerformanceCounter(OwaSingleCounters.AverageSpellcheckTime, newValue, ref PerformanceCounterManager.spellcheckTimeAverage, PerformanceCounterManager.spellcheckTimeAverageLock);
		}

		public static void UpdateResponseTimePerformanceCounter(long newValue, bool isProxy)
		{
			PerformanceCounterManager.UpdateMovingAveragePerformanceCounter(OwaSingleCounters.AverageResponseTime, newValue, ref PerformanceCounterManager.responseTimeAverage, PerformanceCounterManager.responseTimeAverageLock);
			if (isProxy)
			{
				PerformanceCounterManager.UpdateMovingAveragePerformanceCounter(OwaSingleCounters.ProxiedResponseTimeAverage, newValue, ref PerformanceCounterManager.proxiedResponseTimeAverage, PerformanceCounterManager.proxiedResponseTimeAverageLock);
			}
		}

		public static void UpdateImSignOnTimePerformanceCounter(long newValue)
		{
			PerformanceCounterManager.UpdateMovingAveragePerformanceCounter(OwaSingleCounters.IMAverageSignOnTime, newValue, ref PerformanceCounterManager.imSignOnTimeAverage, PerformanceCounterManager.imSignOnTimeAverageLock);
		}

		private static void UpdateMovingAveragePerformanceCounter(ExPerformanceCounter performanceCounter, long newValue, ref double averageValue, object lockObject)
		{
			lock (lockObject)
			{
				averageValue = (1.0 - PerformanceCounterManager.averageMultiplier) * averageValue + PerformanceCounterManager.averageMultiplier * (double)newValue;
				performanceCounter.RawValue = (long)averageValue;
			}
		}

		public static void IncrementCurrentUsersCounterBy(ExPerformanceCounter performanceCounter, long incrementValue)
		{
			if (performanceCounter == null)
			{
				throw new ArgumentNullException("performanceCounter");
			}
			if (!PerformanceCounterManager.currentUserCounterLocks.ContainsKey(performanceCounter.CounterName))
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "The performance counter: \"{0}\" is not supported to be updated by method Globals.IncrementCurrentUsersCounterBy().", performanceCounter.CounterName);
				return;
			}
			object obj = PerformanceCounterManager.currentUserCounterLocks[performanceCounter.CounterName];
			lock (obj)
			{
				long num = PerformanceCounterManager.currentUserCounterValues[performanceCounter.CounterName];
				num += incrementValue;
				performanceCounter.RawValue = num;
				PerformanceCounterManager.currentUserCounterValues[performanceCounter.CounterName] = num;
			}
		}

		public static void AddAvailabilityServiceResult(bool result)
		{
			OwaSingleCounters.ASQueries.Increment();
			lock (PerformanceCounterManager.availabilityServiceResultsQueue)
			{
				PerformanceCounterManager.availabilityServiceResultsQueue.AddSample(result);
				OwaSingleCounters.ASQueriesFailurePercent.RawValue = (long)Math.Round(100.0 * (1.0 - PerformanceCounterManager.availabilityServiceResultsQueue.Mean));
			}
		}

		private static long AddResultToQueue(bool result, FixedSizeQueueBool queue, int queueSize)
		{
			long result2 = -1L;
			lock (queue)
			{
				queue.AddSample(result);
				if (queue.Count >= queueSize)
				{
					result2 = (long)Math.Round(100.0 * (1.0 - queue.Mean));
				}
			}
			return result2;
		}

		public static void AddStoreLogonResult(bool result)
		{
			long num = PerformanceCounterManager.AddResultToQueue(result, PerformanceCounterManager.storeLogonResultsQueue, 1);
			if (num != -1L)
			{
				OwaSingleCounters.StoreLogonFailurePercent.RawValue = num;
			}
		}

		public static void AddInstantMessagingLogonResult(bool result)
		{
			long num = PerformanceCounterManager.AddResultToQueue(result, PerformanceCounterManager.instantMessagingLogonResultsQueue, PerformanceCounterManager.instantMessagingLogonResultsQueueSize);
			if (num != -1L)
			{
				OwaSingleCounters.IMLogonFailuresPercent.RawValue = num;
			}
		}

		public static void AddSentInstantMessageResult(bool result)
		{
			long num = PerformanceCounterManager.AddResultToQueue(result, PerformanceCounterManager.sentInstantMessageResultsQueue, PerformanceCounterManager.sentInstantMessageResultsQueueSize);
			if (num != -1L)
			{
				OwaSingleCounters.IMSentMessageDeliveryFailuresPercent.RawValue = num;
			}
		}

		public static void AddEwsRequestResult(bool result)
		{
			lock (PerformanceCounterManager.ewsRequestResultQueue)
			{
				PerformanceCounterManager.ewsRequestResultQueue.AddSample(result);
				if (PerformanceCounterManager.ewsRequestResultQueue.Count > 50)
				{
					OwaSingleCounters.OwaToEwsRequestFailureRate.RawValue = (long)Math.Round(100.0 * (1.0 - PerformanceCounterManager.ewsRequestResultQueue.Mean));
				}
			}
		}

		public static void AddMailboxOfflineExResult(bool result)
		{
			long num = PerformanceCounterManager.AddResultToQueue(result, PerformanceCounterManager.mailboxOfflineExResultsQueue, PerformanceCounterManager.mailboxOfflineExResultsQueueSize);
			if (num != -1L)
			{
				OwaSingleCounters.MailboxOfflineExceptionFailuresPercent.RawValue = num;
			}
		}

		public static void AddConnectionFailedTransientExResult(bool result)
		{
			long num = PerformanceCounterManager.AddResultToQueue(result, PerformanceCounterManager.connectionFailedTransientExResultsQueue, PerformanceCounterManager.connectionFailedTransientExResultsQueueSize);
			if (num != -1L)
			{
				OwaSingleCounters.ConnectionFailedTransientExceptionPercent.RawValue = num;
			}
		}

		public static void AddStorageTransientExResult(bool result)
		{
			long num = PerformanceCounterManager.AddResultToQueue(result, PerformanceCounterManager.storageTransientExResultsQueue, PerformanceCounterManager.storageTransientExResultsQueueSize);
			if (num != -1L)
			{
				OwaSingleCounters.StorageTransientExceptionPercent.RawValue = num;
			}
		}

		public static void AddStoragePermanantExResult(bool result)
		{
			long num = PerformanceCounterManager.AddResultToQueue(result, PerformanceCounterManager.storagePermanentExResultsQueue, PerformanceCounterManager.storagePermanentExResultsQueueSize);
			if (num != -1L)
			{
				OwaSingleCounters.StoragePermanentExceptionPercent.RawValue = num;
			}
		}

		public static void ProcessUserContextReInitializationRequest()
		{
			if (PerformanceCounterManager.ArePerfCountersEnabled)
			{
				OwaSingleCounters.TotalUserContextReInitializationRequests.Increment();
			}
			long rawValue = OwaSingleCounters.TotalUserContextReInitializationRequests.RawValue;
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (utcNow.UtcTicks - PerformanceCounterManager.lastUserContextReInitializationIntervalStartTimeInTicks >= PerformanceCounterManager.userContextReInitializationCheckDuration.Ticks)
			{
				OwaSingleCounters.TotalUserContextReInitializationRequests.RawValue = 0L;
				Interlocked.Exchange(ref PerformanceCounterManager.lastUserContextReInitializationIntervalStartTimeInTicks, utcNow.UtcTicks);
			}
		}

		public static UniqueUserData GetUserData(string userName)
		{
			bool flag;
			return PerformanceCounterManager.GetUserData(userName, out flag);
		}

		public static UniqueUserData GetUserData(string userName, out bool isNewUser)
		{
			UniqueUserData uniqueUserData = null;
			lock (PerformanceCounterManager.uniqueUserTable)
			{
				if (!PerformanceCounterManager.uniqueUserTable.ContainsKey(userName))
				{
					uniqueUserData = new UniqueUserData();
					PerformanceCounterManager.uniqueUserTable.Add(userName, uniqueUserData);
					isNewUser = true;
				}
				else
				{
					uniqueUserData = PerformanceCounterManager.uniqueUserTable[userName];
					isNewUser = false;
				}
			}
			return uniqueUserData;
		}

		public static void IncrementUserPerfCounters(string userName, bool isProxy, bool isLightExperience)
		{
			if (isProxy)
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentProxiedUsers, 1L);
				return;
			}
			bool flag;
			UniqueUserData userData = PerformanceCounterManager.GetUserData(userName, out flag);
			if (flag)
			{
				OwaSingleCounters.TotalUniqueUsers.Increment();
			}
			if (userData.CurrentSessionCount == 0)
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUniqueUsers, 1L);
			}
			if (isLightExperience)
			{
				if (userData.CurrentLightSessionCount == 0)
				{
					PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUniqueUsersLight, 1L);
				}
				if (userData.IsFirstLightSession)
				{
					OwaSingleCounters.TotalUniqueUsersLight.Increment();
				}
			}
			else
			{
				if (userData.CurrentPremiumSessionCount == 0)
				{
					PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUniqueUsersPremium, 1L);
				}
				if (userData.IsFirstPremiumSession)
				{
					OwaSingleCounters.TotalUniqueUsersPremium.Increment();
				}
			}
			PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUsers, 1L);
			OwaSingleCounters.TotalUsers.Increment();
			if (OwaSingleCounters.CurrentUsers.RawValue > OwaSingleCounters.PeakUserCount.RawValue)
			{
				OwaSingleCounters.PeakUserCount.RawValue = OwaSingleCounters.CurrentUsers.RawValue;
			}
			if (isLightExperience)
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUsersLight, 1L);
				OwaSingleCounters.TotalUsersLight.Increment();
				if (OwaSingleCounters.CurrentUsersLight.RawValue > OwaSingleCounters.PeakUserCountLight.RawValue)
				{
					OwaSingleCounters.PeakUserCountLight.RawValue = OwaSingleCounters.CurrentUsersLight.RawValue;
					return;
				}
			}
			else
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUsersPremium, 1L);
				OwaSingleCounters.TotalUsersPremium.Increment();
				if (OwaSingleCounters.CurrentUsersPremium.RawValue > OwaSingleCounters.PeakUserCountPremium.RawValue)
				{
					OwaSingleCounters.PeakUserCountPremium.RawValue = OwaSingleCounters.CurrentUsersPremium.RawValue;
				}
			}
		}

		public static void DecrementUserPerfCounters(string userName, bool isProxy, bool isLightExperience)
		{
			if (isProxy)
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentProxiedUsers, -1L);
				return;
			}
			PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUsers, -1L);
			UniqueUserData userData = PerformanceCounterManager.GetUserData(userName);
			if (isLightExperience)
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUsersLight, -1L);
				if (userData.CurrentLightSessionCount == 0)
				{
					PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUniqueUsersLight, -1L);
				}
			}
			else
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUsersPremium, -1L);
				if (userData.CurrentPremiumSessionCount == 0)
				{
					PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUniqueUsersPremium, -1L);
				}
			}
			if (userData.CurrentSessionCount == 0)
			{
				PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.CurrentUniqueUsers, -1L);
			}
		}

		public static void UpdatePerfCounteronUserContextCreation(string userName, bool isProxy, bool isLightExperience, bool arePerfCountersEnabled)
		{
			if (arePerfCountersEnabled)
			{
				PerformanceCounterManager.IncrementUserPerfCounters(userName, isProxy, isLightExperience);
			}
			UniqueUserData userData = PerformanceCounterManager.GetUserData(userName);
			userData.IncreaseSessionCounter(isProxy, isLightExperience);
		}

		public static void UpdatePerfCounteronUserContextDeletion(string userName, bool isProxy, bool isLightExperience, bool arePerfCountersEnabled)
		{
			if (string.IsNullOrEmpty(userName))
			{
				ExTraceGlobals.CoreTracer.TraceError(0L, "UpdatePerfCounteronUserContextDeletion got null or empty value for parameter 'userName'.");
				return;
			}
			UniqueUserData uniqueUserData = PerformanceCounterManager.GetUniqueUserData(userName);
			uniqueUserData.DecreaseSessionCounter(isProxy, isLightExperience);
			if (arePerfCountersEnabled)
			{
				PerformanceCounterManager.DecrementUserPerfCounters(userName, isProxy, isLightExperience);
			}
		}

		private const int UserContextReInitializationCheckInterval = 60;

		private const int MaxUserContextReInitializationRequestsPerInterval = 1000;

		private static double averageMultiplier = 0.04;

		private static FixedSizeQueueBool availabilityServiceResultsQueue = new FixedSizeQueueBool(100);

		private static FixedSizeQueueBool storeLogonResultsQueue = new FixedSizeQueueBool(100);

		private static FixedSizeQueueBool instantMessagingLogonResultsQueue;

		private static FixedSizeQueueBool sentInstantMessageResultsQueue;

		private static int instantMessagingLogonResultsQueueSize = 100;

		private static int sentInstantMessageResultsQueueSize = 100;

		private static FixedSizeQueueBool ewsRequestResultQueue = new FixedSizeQueueBool(100);

		private static FixedSizeQueueBool mailboxOfflineExResultsQueue;

		private static FixedSizeQueueBool connectionFailedTransientExResultsQueue;

		private static FixedSizeQueueBool storageTransientExResultsQueue;

		private static FixedSizeQueueBool storagePermanentExResultsQueue;

		private static int mailboxOfflineExResultsQueueSize = 1024;

		private static int connectionFailedTransientExResultsQueueSize = 1024;

		private static int storageTransientExResultsQueueSize = 1024;

		private static int storagePermanentExResultsQueueSize = 1024;

		private static object responseTimeAverageLock = new object();

		private static double responseTimeAverage;

		private static object imSignOnTimeAverageLock = new object();

		private static double imSignOnTimeAverage;

		private static object proxiedResponseTimeAverageLock = new object();

		private static double proxiedResponseTimeAverage;

		private static double searchTimeAverage;

		private static object searchTimeAverageLock = new object();

		private static double spellcheckTimeAverage;

		private static object spellcheckTimeAverageLock = new object();

		private static Dictionary<string, long> currentUserCounterValues = new Dictionary<string, long>();

		private static Dictionary<string, object> currentUserCounterLocks = new Dictionary<string, object>();

		private static TimeSpan userContextReInitializationCheckDuration = new TimeSpan(0, 0, 3600);

		private static long lastUserContextReInitializationIntervalStartTimeInTicks = ExDateTime.UtcNow.UtcTicks;

		private static Dictionary<string, UniqueUserData> uniqueUserTable = new Dictionary<string, UniqueUserData>(StringComparer.OrdinalIgnoreCase);
	}
}
