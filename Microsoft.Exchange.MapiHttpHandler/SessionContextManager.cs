using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MapiHttpHandler;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SessionContextManager
	{
		public static SessionContextActivity CreateSessionContextActivity(SessionContextIdentifier sessionContextIdentifier, TimeSpan idleTimeout, string userAuthIdentifier, string userName, string userPrincipalName, string userSecurityIdentifier, string organization, string mailboxIdentifier)
		{
			SessionContextManager.EnsureBackgroundMonitorStarted();
			if (string.IsNullOrWhiteSpace(userAuthIdentifier))
			{
				throw ProtocolException.FromResponseCode((LID)55388, "User authentication identifier not defined.", ResponseCode.AnonymousNotAllowed, null);
			}
			UserContextActivity userContextActivity = null;
			SessionContextActivity result;
			try
			{
				try
				{
					SessionContextManager.userContextsLock.EnterWriteLock();
					UserContext userContext;
					if (SessionContextManager.userContexts.TryGetValue(userAuthIdentifier, out userContext))
					{
						userContextActivity = new UserContextActivity(userContext);
					}
				}
				finally
				{
					try
					{
						SessionContextManager.userContextsLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				if (userContextActivity == null)
				{
					try
					{
						SessionContextManager.userContextsLock.EnterWriteLock();
						UserContext userContext2;
						if (!SessionContextManager.userContexts.TryGetValue(userAuthIdentifier, out userContext2))
						{
							userContext2 = new UserContext(userName, userPrincipalName, userSecurityIdentifier, userAuthIdentifier, organization);
							SessionContextManager.userContexts[userAuthIdentifier] = userContext2;
						}
						userContextActivity = new UserContextActivity(userContext2);
					}
					finally
					{
						try
						{
							SessionContextManager.userContextsLock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
				result = userContextActivity.UserContext.CreateSessionContextActivity(mailboxIdentifier, sessionContextIdentifier, idleTimeout);
			}
			finally
			{
				Util.DisposeIfPresent(userContextActivity);
			}
			return result;
		}

		public static bool TryGetSessionContextActivity(string cookie, string userAuthIdentifier, TimeSpan idleTimeout, out SessionContextActivity sessionContextActivity, out Exception failureException)
		{
			sessionContextActivity = null;
			failureException = null;
			SessionContextManager.EnsureBackgroundMonitorStarted();
			if (string.IsNullOrWhiteSpace(userAuthIdentifier))
			{
				failureException = ProtocolException.FromResponseCode((LID)61984, "User authentication identifier not defined.", ResponseCode.AnonymousNotAllowed, null);
				return false;
			}
			if (string.IsNullOrWhiteSpace(cookie))
			{
				failureException = ProtocolException.FromResponseCode((LID)52736, "Cookie isn't valid.", ResponseCode.InvalidContextCookie, null);
				return false;
			}
			long id;
			if (!SessionContextIdentifier.TryGetIdFromCookie(cookie, out id, out failureException))
			{
				return false;
			}
			UserContextActivity userContextActivity = null;
			bool result;
			try
			{
				try
				{
					SessionContextManager.userContextsLock.EnterReadLock();
					UserContext userContext;
					if (!SessionContextManager.userContexts.TryGetValue(userAuthIdentifier, out userContext))
					{
						failureException = ProtocolException.FromResponseCode((LID)40480, "Unable to find user context based on user authentication identifier.", ResponseCode.ContextNotFound, null);
						return false;
					}
					userContextActivity = new UserContextActivity(userContext);
				}
				finally
				{
					try
					{
						SessionContextManager.userContextsLock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				result = userContextActivity.UserContext.TryGetSessionContextActivity(id, idleTimeout, out sessionContextActivity, out failureException);
			}
			finally
			{
				Util.DisposeIfPresent(userContextActivity);
			}
			return result;
		}

		public static SessionContextActivity GetSessionContextActivity(string cookie, string userAuthIdentifier, TimeSpan idleTimeout)
		{
			SessionContextManager.EnsureBackgroundMonitorStarted();
			SessionContextActivity result = null;
			Exception ex = null;
			if (!SessionContextManager.TryGetSessionContextActivity(cookie, userAuthIdentifier, idleTimeout, out result, out ex))
			{
				throw ex;
			}
			return result;
		}

		public static bool TryGetSessionContextInfo(string userAuthIdentifier, out SessionContextInfo[] sessionContextInfoArray)
		{
			sessionContextInfoArray = null;
			SessionContextManager.EnsureBackgroundMonitorStarted();
			if (string.IsNullOrWhiteSpace(userAuthIdentifier))
			{
				return false;
			}
			UserContextActivity userContextActivity = null;
			bool result;
			try
			{
				try
				{
					SessionContextManager.userContextsLock.EnterReadLock();
					UserContext userContext;
					if (!SessionContextManager.userContexts.TryGetValue(userAuthIdentifier, out userContext))
					{
						return false;
					}
					userContextActivity = new UserContextActivity(userContext);
				}
				finally
				{
					try
					{
						SessionContextManager.userContextsLock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				result = userContextActivity.UserContext.TryGetSessionContextInfo(out sessionContextInfoArray);
			}
			finally
			{
				Util.DisposeIfPresent(userContextActivity);
			}
			return result;
		}

		public static void RundownContextHandle(object contextHandle)
		{
			SessionContextManager.EnsureBackgroundMonitorStarted();
			if (MapiHttpHandler.IsValidContextHandle(contextHandle))
			{
				lock (SessionContextManager.contextHandlesToRundownLock)
				{
					SessionContextManager.contextHandlesToRundown.Add(contextHandle);
				}
			}
		}

		public static void WakeupIdleContextMonitor(ExDateTime wakeupTime)
		{
			if (wakeupTime + TimeSpan.FromSeconds(5.0) < SessionContextManager.nextExpiration)
			{
				SessionContextManager.monitorThreadWakeupEvent.Set();
			}
		}

		private static void IdleContextMonitorThread()
		{
			List<UserContext> list = new List<UserContext>();
			List<UserContext> list2 = new List<UserContext>();
			List<SessionContext> list3 = new List<SessionContext>();
			TimeSpan timeSpan = SessionContextManager.monitorThreadScanPeriod;
			ExDateTime exDateTime = ExDateTime.UtcNow;
			for (;;)
			{
				bool flag = false;
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (utcNow < exDateTime || utcNow > exDateTime + TimeSpan.FromMinutes(10.0))
				{
					flag = true;
					exDateTime = utcNow;
				}
				ExTraceGlobals.SessionContextTracer.TraceInformation(63904, 0L, "IdleContextMoniterThread: processing");
				list.Clear();
				list2.Clear();
				list3.Clear();
				ExDateTime exDateTime2 = ExDateTime.MaxValue;
				try
				{
					SessionContextManager.userContextsLock.EnterReadLock();
					foreach (KeyValuePair<string, UserContext> keyValuePair in SessionContextManager.userContexts)
					{
						ExDateTime exDateTime3;
						keyValuePair.Value.GatherExpiredSessionContexts(list3, out exDateTime3);
						if (exDateTime3 < exDateTime2)
						{
							exDateTime2 = exDateTime3;
						}
						if (flag && !keyValuePair.Value.IsActive)
						{
							list.Add(keyValuePair.Value);
						}
					}
				}
				finally
				{
					try
					{
						SessionContextManager.userContextsLock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				if (list.Count > 0)
				{
					try
					{
						SessionContextManager.userContextsLock.EnterWriteLock();
						foreach (UserContext userContext in list)
						{
							if (!userContext.IsActive)
							{
								if (ExTraceGlobals.SessionContextTracer.IsTraceEnabled(TraceType.InfoTrace))
								{
									list2.Add(userContext);
								}
								SessionContextManager.userContexts.Remove(userContext.UserAuthIdentifier);
							}
						}
					}
					finally
					{
						try
						{
							SessionContextManager.userContextsLock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
				list.Clear();
				lock (SessionContextManager.contextHandlesToRundownLock)
				{
					foreach (SessionContext sessionContext in list3)
					{
						object contextHandle = sessionContext.ContextHandle;
						if (MapiHttpHandler.IsValidContextHandle(contextHandle))
						{
							SessionContextManager.contextHandlesToRundown.Add(contextHandle);
						}
					}
				}
				if (ExTraceGlobals.SessionContextTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					foreach (SessionContext sessionContext2 in list3)
					{
						Trace sessionContextTracer = ExTraceGlobals.SessionContextTracer;
						int lid = 39328;
						long id = 0L;
						string formatString = "UserContext: [{0},{1}] SessionContext [{2}] created [{3}] is expired; ContextHandle [{4}] rundown";
						object[] array = new object[5];
						object[] array2 = array;
						int num = 0;
						string text;
						if ((text = sessionContext2.UserContext.UserPrincipalName) == null)
						{
							text = (sessionContext2.UserContext.UserName ?? string.Empty);
						}
						array2[num] = text;
						array[1] = sessionContext2.UserContext.UserAuthIdentifier;
						array[2] = sessionContext2.Id;
						array[3] = sessionContext2.CreationTime;
						array[4] = sessionContext2.ContextHandle;
						sessionContextTracer.TraceInformation(lid, id, formatString, array);
					}
					foreach (UserContext userContext2 in list2)
					{
						ExTraceGlobals.SessionContextTracer.TraceInformation<string, string>(55712, 0L, "UserContext: [{0},{1}] inactive; removed", userContext2.UserPrincipalName, userContext2.UserAuthIdentifier);
					}
				}
				list3.Clear();
				list2.Clear();
				List<object> list4 = new List<object>();
				List<object> list5;
				lock (SessionContextManager.contextHandlesToRundownLock)
				{
					list5 = SessionContextManager.contextHandlesToRundown;
					SessionContextManager.contextHandlesToRundown = list4;
				}
				List<object> list6 = null;
				foreach (object obj3 in list5)
				{
					if (!MapiHttpHandler.TryContextHandleRundown(obj3))
					{
						ExTraceGlobals.SessionContextTracer.TraceInformation(43424, 0L, "ContextHandle: [{0}] failed to rundown; re-queued", new object[]
						{
							obj3
						});
						if (list6 == null)
						{
							list6 = new List<object>();
						}
						list6.Add(obj3);
					}
					else
					{
						ExTraceGlobals.SessionContextTracer.TraceInformation(59808, 0L, "ContextHandle: [{0}] successfully rundown", new object[]
						{
							obj3
						});
					}
				}
				if (list6 != null && list6.Count > 0)
				{
					lock (SessionContextManager.contextHandlesToRundownLock)
					{
						SessionContextManager.contextHandlesToRundown.AddRange(list6);
					}
				}
				ExTraceGlobals.SessionContextTracer.TraceInformation(35232, 0L, "IdleContextMoniterThread: sleeping");
				utcNow = ExDateTime.UtcNow;
				if (utcNow >= exDateTime2)
				{
					timeSpan = TimeSpan.FromSeconds(5.0);
				}
				else
				{
					timeSpan = utcNow - exDateTime2;
					if (timeSpan < TimeSpan.FromSeconds(5.0))
					{
						timeSpan = TimeSpan.FromSeconds(5.0);
					}
					else if (timeSpan > TimeSpan.FromSeconds(60.0))
					{
						timeSpan = TimeSpan.FromSeconds(60.0);
					}
				}
				SessionContextManager.nextExpiration = utcNow + timeSpan;
				SessionContextManager.monitorThreadWakeupEvent.WaitOne(timeSpan);
			}
		}

		private static void EnsureBackgroundMonitorStarted()
		{
			if (!SessionContextManager.isMonitorStarted)
			{
				lock (SessionContextManager.monitorStartedLock)
				{
					if (!SessionContextManager.isMonitorStarted)
					{
						new Thread(new ThreadStart(SessionContextManager.IdleContextMonitorThread))
						{
							IsBackground = true,
							Name = "Idle SessionContext Monitor"
						}.Start();
						SessionContextManager.isMonitorStarted = true;
					}
				}
			}
		}

		private static readonly Dictionary<string, UserContext> userContexts = new Dictionary<string, UserContext>();

		private static readonly ReaderWriterLockSlim userContextsLock = new ReaderWriterLockSlim();

		private static readonly object monitorStartedLock = new object();

		private static readonly object contextHandlesToRundownLock = new object();

		private static readonly AutoResetEvent monitorThreadWakeupEvent = new AutoResetEvent(false);

		private static readonly TimeSpan monitorThreadScanPeriod = TimeSpan.FromSeconds(15.0);

		private static List<object> contextHandlesToRundown = new List<object>();

		private static bool isMonitorStarted = false;

		private static ExDateTime nextExpiration = ExDateTime.UtcNow;
	}
}
