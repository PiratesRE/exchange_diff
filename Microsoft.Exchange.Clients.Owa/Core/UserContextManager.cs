using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class UserContextManager
	{
		private UserContextManager()
		{
		}

		public static UserContext GetUserContext()
		{
			OwaContext owaContext = OwaContext.Current;
			if (owaContext == null)
			{
				return null;
			}
			return owaContext.UserContext;
		}

		internal static void InsertIntoCache(UserContext userContext)
		{
			UserContextManager.UserContextCacheWrapper userContextCacheWrapper = (UserContextManager.UserContextCacheWrapper)HttpRuntime.Cache.Get(userContext.Key.ToString());
			if (userContextCacheWrapper == null)
			{
				userContextCacheWrapper = new UserContextManager.UserContextCacheWrapper();
			}
			userContextCacheWrapper.UserContext = userContext;
			HttpRuntime.Cache.Insert(userContext.Key.ToString(), userContextCacheWrapper, null, DateTime.MaxValue, new TimeSpan(userContext.CalculateTimeout() * 10000L), CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(UserContextManager.UserContextRemovedCallback));
		}

		internal static UserContext TryGetUserContextFromCache(UserContextKey userContextKey)
		{
			ExTraceGlobals.UserContextCallTracer.TraceDebug(0L, "UserContextManager.TryGetUserContextFromCache");
			UserContext userContext = null;
			try
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContextKey>(0L, "Attempting to fetch user context from the cache.  Key={0}", userContextKey);
				UserContextManager.UserContextCacheWrapper userContextCacheWrapper = (UserContextManager.UserContextCacheWrapper)HttpRuntime.Cache.Get(userContextKey.ToString());
				if (userContextCacheWrapper != null && userContextCacheWrapper.UserContext != null)
				{
					userContext = userContextCacheWrapper.UserContext;
					ExTraceGlobals.UserContextTracer.TraceDebug<UserContextKey, UserContext>(0L, "User context found in cache. Key={0}, User context instance={1}", userContextKey, userContext);
					userContext.UpdateLastAccessedTime();
					userContext.Lock(true);
				}
				else
				{
					ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "An object for this user context ID value is not present in the cache (probably was expired), so we are going to reuse the user context ID value for the new session", userContextKey.UserContextId);
				}
			}
			finally
			{
				if (userContext != null && userContext.State != UserContextState.Active && userContext.LockedByCurrentThread())
				{
					userContext.Unlock();
					userContext = null;
				}
			}
			return userContext;
		}

		internal static void UserContextRemovedCallback(string key, object value, CacheItemRemovedReason reason)
		{
			try
			{
			}
			finally
			{
				try
				{
					UserContextManager.UserContextCacheWrapper userContextCacheWrapper = value as UserContextManager.UserContextCacheWrapper;
					if (userContextCacheWrapper.UserContext != null)
					{
						ExTraceGlobals.UserContextTracer.TraceDebug<string, UserContext, int>(0L, "Removing user context from cache, Key={0}, User context instance={1}, Reason={2}", key, userContextCacheWrapper.UserContext, (int)reason);
						UserContextManager.TerminateSession(userContextCacheWrapper.UserContext, reason);
					}
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception exception)
				{
					if (Globals.SendWatsonReports)
					{
						ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending watson report");
						ExWatson.SendReport(exception, ReportOptions.None, null);
					}
				}
			}
		}

		public static void TerminateSession(UserContext userContext, CacheItemRemovedReason abandonedReason)
		{
			ExTraceGlobals.UserContextCallTracer.TraceDebug(0L, "UserContextManager.TerminateSession");
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			userContext.TerminationStatus = UserContextTerminationStatus.TerminatePending;
			bool flag = false;
			try
			{
				try
				{
					userContext.Lock();
					flag = true;
				}
				catch (OwaLockTimeoutException)
				{
					ExTraceGlobals.UserContextTracer.TraceDebug<UserContext>(0L, "TerminateSession tried to grab a lock for an user context and it timed out. Mark it to be abandoned when unlocked. User context instance={0}", userContext);
					userContext.AbandonedReason = abandonedReason;
					return;
				}
				userContext.TerminationStatus = UserContextTerminationStatus.TerminateStarted;
				if (userContext.State != UserContextState.Abandoned)
				{
					try
					{
						UserContextManager.RecordUserContextDeletion(userContext.LogonIdentity, userContext);
						if (Globals.ArePerfCountersEnabled)
						{
							if (abandonedReason == CacheItemRemovedReason.Expired)
							{
								OwaSingleCounters.TotalSessionsEndedByTimeout.Increment();
							}
							else if (abandonedReason == CacheItemRemovedReason.Removed && userContext.State == UserContextState.MarkedForLogoff)
							{
								OwaSingleCounters.TotalSessionsEndedByLogoff.Increment();
							}
						}
					}
					finally
					{
						userContext.Dispose();
					}
				}
			}
			finally
			{
				if (userContext.LockedByCurrentThread() && flag)
				{
					userContext.State = UserContextState.Abandoned;
					userContext.AbandonedReason = abandonedReason;
					userContext.TerminationStatus = UserContextTerminationStatus.TerminateCompleted;
					userContext.Unlock();
				}
			}
		}

		public static long CachedUserContextsCount
		{
			get
			{
				return OwaSingleCounters.CurrentUsers.RawValue;
			}
		}

		internal static void RecordUserContextCreation(OwaIdentity logonIdentity, UserContext userContext)
		{
			if (logonIdentity == null)
			{
				throw new ArgumentNullException("logonIdentity");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			string userName = logonIdentity.UserSid.ToString();
			PerformanceCounterManager.UpdatePerfCounteronUserContextCreation(userName, userContext.IsProxy, userContext.IsBasicExperience, Globals.ArePerfCountersEnabled);
		}

		internal static void RecordUserContextDeletion(OwaIdentity logonIdentity, UserContext userContext)
		{
			string userName = logonIdentity.UserSid.ToString();
			PerformanceCounterManager.UpdatePerfCounteronUserContextDeletion(userName, userContext.IsProxy, userContext.IsBasicExperience, Globals.ArePerfCountersEnabled);
		}

		public static void TouchUserContext(UserContext userContext, Stopwatch stopWatch)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (stopWatch == null)
			{
				throw new ArgumentNullException("stopWatch");
			}
			int num = userContext.Configuration.SessionTimeout;
			if (num > UserContextManager.UserContextTouchThreshold)
			{
				num -= UserContextManager.UserContextTouchThreshold;
			}
			if (stopWatch.ElapsedMilliseconds > (long)(num * 60 * 1000))
			{
				userContext.Touch();
				stopWatch.Reset();
			}
		}

		internal static OwaRWLockWrapper GetUserContextKeyLock(string userContextKeyString)
		{
			UserContextManager.UserContextCacheWrapper userContextCacheWrapper = (UserContextManager.UserContextCacheWrapper)HttpRuntime.Cache.Get(userContextKeyString);
			if (userContextCacheWrapper == null)
			{
				lock (UserContextManager.lockObject)
				{
					userContextCacheWrapper = (UserContextManager.UserContextCacheWrapper)HttpRuntime.Cache.Get(userContextKeyString);
					if (userContextCacheWrapper == null)
					{
						userContextCacheWrapper = new UserContextManager.UserContextCacheWrapper
						{
							Lock = new OwaRWLockWrapper()
						};
						HttpRuntime.Cache.Insert(userContextKeyString, userContextCacheWrapper, null, DateTime.MaxValue, TimeSpan.FromMinutes(2.5), CacheItemPriority.NotRemovable, null);
					}
				}
			}
			return userContextCacheWrapper.Lock;
		}

		private static readonly int UserContextTouchThreshold = 5;

		private static object lockObject = new object();

		private class UserContextCacheWrapper
		{
			public UserContext UserContext { get; set; }

			public OwaRWLockWrapper Lock { get; set; }
		}
	}
}
