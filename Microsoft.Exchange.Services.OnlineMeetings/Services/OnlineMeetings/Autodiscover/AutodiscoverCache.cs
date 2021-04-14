using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal class AutodiscoverCache
	{
		internal static ConcurrentDictionary<string, string> DomainCache
		{
			get
			{
				return AutodiscoverCache.perDomainEndpointCache;
			}
		}

		internal static ConcurrentDictionary<string, AutodiscoverCacheEntry> UserCache
		{
			get
			{
				return AutodiscoverCache.perUserUcwaUrlCache;
			}
		}

		public static bool ContainsDomain(string domain)
		{
			string text;
			return AutodiscoverCache.DomainCache.TryGetValue(domain, out text);
		}

		public static string GetValueForDomain(string domain)
		{
			string result;
			if (AutodiscoverCache.DomainCache.TryGetValue(domain, out result))
			{
				return result;
			}
			return string.Empty;
		}

		public static void UpdateDomain(string domain, string endpoint)
		{
			AutodiscoverCache.DomainCache.AddOrUpdate(domain, endpoint, (string key, string value) => endpoint);
		}

		public static bool InvalidateDomain(string domain)
		{
			bool result;
			lock (AutodiscoverCache.lockObject)
			{
				string arg;
				if (AutodiscoverCache.DomainCache.TryRemove(domain, out arg))
				{
					ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string, string>(0, 0L, "[AutodiscoverCache][InvalidateDomain] Removed entry from per domain cache: key '{0}' with value '{1}'", domain, arg);
					string[] array = new string[AutodiscoverCache.UserCache.Keys.Count];
					AutodiscoverCache.UserCache.Keys.CopyTo(array, 0);
					foreach (string sipAddress in array)
					{
						if (string.Compare(domain, AutodiscoverCache.GetDomain(sipAddress), StringComparison.CurrentCultureIgnoreCase) == 0)
						{
							AutodiscoverCache.InvalidateUserImpl(sipAddress);
						}
					}
					result = true;
				}
				else
				{
					ExTraceGlobals.OnlineMeetingTracer.TraceDebug<string>(0, 0L, "[AutodiscoverCache][InvalidateDomain] Unable to find entry for '{0}' in per domain cache", domain);
					result = false;
				}
			}
			return result;
		}

		public static bool ContainsUser(string user)
		{
			string text;
			return AutodiscoverCache.TryGetUserUcwaUserUrl(user, out text);
		}

		public static string GetValueForUser(string user)
		{
			string result;
			AutodiscoverCache.TryGetUserUcwaUserUrl(user, out result);
			return result;
		}

		private static bool TryGetUserUcwaUserUrl(string user, out string ucwaUrl)
		{
			ucwaUrl = string.Empty;
			AutodiscoverCacheEntry autodiscoverCacheEntry;
			if (!AutodiscoverCache.UserCache.TryGetValue(user, out autodiscoverCacheEntry))
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceDebug<string>(0, 0L, "[AutodiscoverCache][TryGetUserUcwaUserUrl] Unable to find entry for '{0}' in per user cache", user);
				return false;
			}
			if (autodiscoverCacheEntry.IsValid)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string>(0, 0L, "[AutodiscoverCache][TryGetUserUcwaUserUrl] Cache entry for user '{0}' is valid", user);
				ucwaUrl = autodiscoverCacheEntry.UcwaUrl;
				return true;
			}
			ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string, int, string>(0, 0L, "[AutodiscoverCache][TryGetUserUcwaUserUrl] Cache entry for user '{0}' not valid. FailureCount: {1}, ExpirationDate: {2}", user, autodiscoverCacheEntry.FailureCount, (autodiscoverCacheEntry.Expiration != null) ? autodiscoverCacheEntry.Expiration.Value.ToString() : "none");
			return false;
		}

		public static void UpdateUser(string sipAddress, string ucwaUrl)
		{
			bool flag = false;
			try
			{
				object obj;
				Monitor.Enter(obj = AutodiscoverCache.lockObject, ref flag);
				AutodiscoverCacheEntry entry;
				if (string.IsNullOrEmpty(ucwaUrl))
				{
					ExDateTime value2 = ExDateTime.Now.AddDays(1.0);
					ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string, string>(0, 0L, "[AutodiscoverCache][UpdateUser] Updating entry in per user cache for user '{0}' with no ucwaUrl and expiration date of '{1}'", sipAddress, value2.ToString());
					entry = new AutodiscoverCacheEntry(sipAddress, ucwaUrl, new ExDateTime?(value2));
				}
				else
				{
					ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string, string>(0, 0L, "[AutodiscoverCache][UpdateUser] Updating entry in per user cache for user '{0}' with ucwaUrl '{1}' with no expiration", sipAddress, ucwaUrl);
					entry = new AutodiscoverCacheEntry(sipAddress, ucwaUrl, null);
				}
				AutodiscoverCache.UserCache.AddOrUpdate(sipAddress, entry, (string key, AutodiscoverCacheEntry value) => entry);
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
		}

		public static bool InvalidateUser(string sipAddress)
		{
			bool result;
			lock (AutodiscoverCache.lockObject)
			{
				result = AutodiscoverCache.InvalidateUserImpl(sipAddress);
			}
			return result;
		}

		public static void IncrementFailureCount(string sipAddress)
		{
			lock (AutodiscoverCache.lockObject)
			{
				AutodiscoverCacheEntry autodiscoverCacheEntry;
				if (AutodiscoverCache.UserCache.TryGetValue(sipAddress, out autodiscoverCacheEntry))
				{
					autodiscoverCacheEntry.IncrementFailureCount();
				}
			}
		}

		private static bool InvalidateUserImpl(string sipAddress)
		{
			AutodiscoverCacheEntry autodiscoverCacheEntry;
			if (AutodiscoverCache.UserCache.TryRemove(sipAddress, out autodiscoverCacheEntry))
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string>(0, 0L, "[AutodiscoverCache][InvalidateUserImpl] Removing entry from per user cache: key '{0}'", sipAddress);
				return true;
			}
			ExTraceGlobals.OnlineMeetingTracer.TraceDebug<string>(0, 0L, "[AutodiscoverCache][InvalidateUserImpl] Unable to find entry for '{0}' in per user cache", sipAddress);
			return false;
		}

		private static string GetDomain(string sipAddress)
		{
			if (string.IsNullOrWhiteSpace(sipAddress))
			{
				return string.Empty;
			}
			int num = sipAddress.LastIndexOf("@");
			if (num <= 0)
			{
				return string.Empty;
			}
			return sipAddress.Substring(num + 1);
		}

		private const string AutodiscoverCacheName = "[AutodiscoverCache]";

		private static readonly ConcurrentDictionary<string, string> perDomainEndpointCache = new ConcurrentDictionary<string, string>();

		private static readonly ConcurrentDictionary<string, AutodiscoverCacheEntry> perUserUcwaUrlCache = new ConcurrentDictionary<string, AutodiscoverCacheEntry>();

		private static readonly object lockObject = new object();
	}
}
