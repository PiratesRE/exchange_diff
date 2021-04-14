using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal static class SyncRequestWasHangingCache
	{
		public static string BuildDiagnosticsString(string request, string cache, string lookup)
		{
			return string.Format("{0}-{1}-{2}", request, cache, lookup);
		}

		public static int Count
		{
			get
			{
				return SyncRequestWasHangingCache.syncRequests.Count;
			}
		}

		public static bool TryGet(Guid mailboxGuid, DeviceIdentity deviceIdentity, out bool value)
		{
			if (GlobalSettings.DisableCaching)
			{
				value = false;
				return false;
			}
			if (!SyncRequestWasHangingCache.syncRequests.TryGetValue(SyncRequestWasHangingCache.BuildKey(mailboxGuid, deviceIdentity), out value))
			{
				value = false;
				return false;
			}
			return true;
		}

		public static void Set(Guid mailboxGuid, DeviceIdentity deviceIdentity, bool value)
		{
			SyncRequestWasHangingCache.syncRequests[SyncRequestWasHangingCache.BuildKey(mailboxGuid, deviceIdentity)] = value;
		}

		public static bool Remove(Guid mailboxGuid, DeviceIdentity deviceIdentity)
		{
			return SyncRequestWasHangingCache.syncRequests.Remove(SyncRequestWasHangingCache.BuildKey(mailboxGuid, deviceIdentity));
		}

		internal static string BuildKey(Guid mailboxGuid, DeviceIdentity deviceIdentity)
		{
			return string.Format("{0}~{1}", mailboxGuid.ToString(), deviceIdentity);
		}

		public const string RequestHanging = "H";

		public const string RequestNonHanging = "N";

		public const string RequestEmpty = "E";

		public const string CacheHanging = "H";

		public const string CacheNonHanging = "N";

		public const string CacheMiss = "M";

		public const string CacheNotNecessary = "-";

		public const string LookupMissing = "M";

		public const string LookupHanging = "H";

		public const string LookupNonHanging = "N";

		public const string LookupNotNecessary = "-";

		private static MruDictionaryCache<string, bool> syncRequests = new MruDictionaryCache<string, bool>(GlobalSettings.HangingSyncHintCacheSize, (int)GlobalSettings.HangingSyncHintCacheTimeout.TotalMinutes);
	}
}
