using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal static class MailboxSessionCache
	{
		internal static void Start()
		{
			if (GlobalSettings.MailboxSessionCacheTimeout > TimeSpan.Zero)
			{
				lock (MailboxSessionCache.synchronizationObject)
				{
					if (MailboxSessionCache.mailboxSessionCache != null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, MailboxSessionCache.mailboxSessionCache, "MailboxSessionCache is already started.");
					}
					else
					{
						MailboxSessionCache.mailboxSessionCache = new MruDictionaryCache<Guid, SecurityContextAndSession>(GlobalSettings.MailboxSessionCacheInitialSize, GlobalSettings.MailboxSessionCacheMaxSize, (int)GlobalSettings.MailboxSessionCacheTimeout.TotalMinutes);
					}
				}
			}
		}

		internal static void Stop()
		{
			lock (MailboxSessionCache.synchronizationObject)
			{
				if (MailboxSessionCache.mailboxSessionCache != null)
				{
					MailboxSessionCache.mailboxSessionCache.Dispose();
					MailboxSessionCache.mailboxSessionCache = null;
				}
			}
		}

		public static int Count
		{
			get
			{
				int result;
				lock (MailboxSessionCache.synchronizationObject)
				{
					result = ((MailboxSessionCache.mailboxSessionCache == null) ? 0 : MailboxSessionCache.mailboxSessionCache.Count);
				}
				return result;
			}
		}

		public static List<MruCacheDiagnosticEntryInfo> GetCacheEntries()
		{
			List<MruCacheDiagnosticEntryInfo> result;
			lock (MailboxSessionCache.synchronizationObject)
			{
				if (MailboxSessionCache.mailboxSessionCache != null)
				{
					result = MailboxSessionCache.mailboxSessionCache.GetDiagnosticsInfo((SecurityContextAndSession securityContextAndSession) => securityContextAndSession.MailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static float GetCacheEfficiency()
		{
			return MailboxSessionCache.cacheEfficiency.GetValue();
		}

		public static void IncrementDiscardedSessions()
		{
			MailboxSessionCache.discardedSessions.Add(1U);
		}

		public static int DiscardedSessions
		{
			get
			{
				return (int)MailboxSessionCache.discardedSessions.GetValue();
			}
		}

		public static bool TryGetAndRemoveValue(Guid token, out SecurityContextAndSession data)
		{
			bool result;
			lock (MailboxSessionCache.synchronizationObject)
			{
				if (MailboxSessionCache.mailboxSessionCache != null)
				{
					bool flag2 = MailboxSessionCache.mailboxSessionCache.TryGetAndRemoveValue(token, out data);
					MailboxSessionCache.cacheEfficiency.Add(flag2 ? 1U : 0U);
					result = flag2;
				}
				else
				{
					data = null;
					result = false;
				}
			}
			return result;
		}

		public static bool ContainsKey(Guid token)
		{
			bool result;
			lock (MailboxSessionCache.synchronizationObject)
			{
				if (MailboxSessionCache.mailboxSessionCache != null)
				{
					result = MailboxSessionCache.mailboxSessionCache.ContainsKey(token);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public static bool AddOrReplace(Guid token, SecurityContextAndSession data)
		{
			lock (MailboxSessionCache.synchronizationObject)
			{
				if (MailboxSessionCache.mailboxSessionCache != null)
				{
					MailboxSessionCache.mailboxSessionCache[token] = data;
					return true;
				}
			}
			return false;
		}

		private const uint Hit = 1U;

		private const uint Miss = 0U;

		private static FixedTimeAverage cacheEfficiency = new FixedTimeAverage(10000, 60, Environment.TickCount, TimeSpan.FromHours(1.0));

		private static FixedTimeSum discardedSessions = new FixedTimeSum(10000, 60);

		private static object synchronizationObject = new object();

		private static MruDictionaryCache<Guid, SecurityContextAndSession> mailboxSessionCache;
	}
}
