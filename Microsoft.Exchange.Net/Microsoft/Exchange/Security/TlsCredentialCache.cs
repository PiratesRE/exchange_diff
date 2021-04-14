using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	internal static class TlsCredentialCache
	{
		internal static int MaxEntries
		{
			get
			{
				return TlsCredentialCache.maxEntries;
			}
		}

		internal static int MaxPerHost
		{
			get
			{
				return TlsCredentialCache.maxPerHost;
			}
		}

		internal static int Count
		{
			get
			{
				return TlsCredentialCache.cacheSize;
			}
		}

		internal static void Initialize(int maxEntries, int maxPerHost)
		{
			if (TlsCredentialCache.cache != null)
			{
				throw new InvalidOperationException();
			}
			TlsCredentialCache.maxEntries = maxEntries;
			TlsCredentialCache.maxPerHost = maxPerHost;
			TlsCredentialCache.cache = new Dictionary<TlsCredentialCache.CacheKey, TlsCredentialCache.CacheValue>(maxEntries / maxPerHost);
		}

		internal static void Shutdown()
		{
			Dictionary<TlsCredentialCache.CacheKey, TlsCredentialCache.CacheValue> dictionary;
			lock (TlsCredentialCache.syncRoot)
			{
				if (TlsCredentialCache.cache == null)
				{
					return;
				}
				dictionary = TlsCredentialCache.cache;
				TlsCredentialCache.cache = null;
				TlsCredentialCache.cacheSize = 0;
			}
			foreach (KeyValuePair<TlsCredentialCache.CacheKey, TlsCredentialCache.CacheValue> keyValuePair in dictionary)
			{
				keyValuePair.Value.DeleteAll();
			}
		}

		internal static SafeCredentialsHandle Find(X509Certificate cert, string targetName)
		{
			if (TlsCredentialCache.cache == null)
			{
				return null;
			}
			SafeCredentialsHandle result;
			lock (TlsCredentialCache.syncRoot)
			{
				if (TlsCredentialCache.cache == null)
				{
					result = null;
				}
				else
				{
					TlsCredentialCache.CacheKey key = new TlsCredentialCache.CacheKey(cert, targetName);
					TlsCredentialCache.CacheValue cacheValue;
					if (!TlsCredentialCache.cache.TryGetValue(key, out cacheValue))
					{
						result = null;
					}
					else
					{
						SafeCredentialsHandle safeCredentialsHandle = cacheValue.Pop();
						TlsCredentialCache.cacheSize--;
						if (cacheValue.Count == 0)
						{
							TlsCredentialCache.cache.Remove(key);
						}
						result = safeCredentialsHandle;
					}
				}
			}
			return result;
		}

		internal static void Add(X509Certificate cert, string targetName, SafeCredentialsHandle handle)
		{
			if (TlsCredentialCache.cache != null)
			{
				lock (TlsCredentialCache.syncRoot)
				{
					if (TlsCredentialCache.cache != null)
					{
						TlsCredentialCache.CacheKey key = new TlsCredentialCache.CacheKey(cert, targetName);
						TlsCredentialCache.CacheValue cacheValue;
						if (!TlsCredentialCache.cache.TryGetValue(key, out cacheValue))
						{
							cacheValue = new TlsCredentialCache.CacheValue();
							TlsCredentialCache.cache.Add(key, cacheValue);
						}
						if (cacheValue.Push(handle))
						{
							TlsCredentialCache.cacheSize++;
							if (TlsCredentialCache.cacheSize > TlsCredentialCache.maxEntries)
							{
								TlsCredentialCache.ExpireCache(10);
							}
						}
						return;
					}
				}
			}
			handle.Dispose();
		}

		internal static void Flush()
		{
			lock (TlsCredentialCache.syncRoot)
			{
				foreach (KeyValuePair<TlsCredentialCache.CacheKey, TlsCredentialCache.CacheValue> keyValuePair in TlsCredentialCache.cache)
				{
					keyValuePair.Value.DeleteAll();
				}
				TlsCredentialCache.cache.Clear();
				TlsCredentialCache.cacheSize = 0;
			}
		}

		private static void ExpireCache(int purgePercentage)
		{
			ExTraceGlobals.NetworkTracer.TraceDebug(0L, "Expiring TLS credential cache");
			List<TlsCredentialCache.CacheExpireEntry> list = new List<TlsCredentialCache.CacheExpireEntry>(TlsCredentialCache.cacheSize);
			foreach (KeyValuePair<TlsCredentialCache.CacheKey, TlsCredentialCache.CacheValue> keyValuePair in TlsCredentialCache.cache)
			{
				foreach (TlsCredentialCache.CachedCredential cachedCredential in keyValuePair.Value.Credentials)
				{
					TlsCredentialCache.CacheExpireEntry cacheExpireEntry = new TlsCredentialCache.CacheExpireEntry(keyValuePair.Key, cachedCredential.Created);
					int num = list.BinarySearch(cacheExpireEntry, cacheExpireEntry);
					if (num < 0)
					{
						num = ~num;
					}
					list.Insert(num, cacheExpireEntry);
				}
			}
			int num2 = TlsCredentialCache.cacheSize * purgePercentage / 100;
			foreach (TlsCredentialCache.CacheExpireEntry cacheExpireEntry2 in list)
			{
				if (num2-- <= 0)
				{
					break;
				}
				ExTraceGlobals.NetworkTracer.TraceDebug<string, DateTime>(0L, "Purging {0}, created {1}", cacheExpireEntry2.Key.TargetName, cacheExpireEntry2.Created);
				TlsCredentialCache.ExpireEntry(cacheExpireEntry2);
			}
		}

		private static void ExpireEntry(TlsCredentialCache.CacheExpireEntry entry)
		{
			TlsCredentialCache.CacheValue cacheValue = TlsCredentialCache.cache[entry.Key];
			cacheValue.DeleteOldest();
			TlsCredentialCache.cacheSize--;
			if (cacheValue.Count == 0)
			{
				TlsCredentialCache.cache.Remove(entry.Key);
			}
		}

		private const int PurgePercentage = 10;

		private static int maxEntries;

		private static int maxPerHost;

		private static int cacheSize;

		private static object syncRoot = new object();

		private static Dictionary<TlsCredentialCache.CacheKey, TlsCredentialCache.CacheValue> cache;

		private class CacheKey
		{
			internal CacheKey(X509Certificate certificate, string targetName)
			{
				this.certificate = certificate;
				this.targetName = targetName;
			}

			internal X509Certificate Certificate
			{
				get
				{
					return this.certificate;
				}
			}

			internal string TargetName
			{
				get
				{
					return this.targetName;
				}
			}

			public override int GetHashCode()
			{
				return ((this.certificate == null) ? 0 : this.certificate.GetHashCode()) ^ ((this.targetName == null) ? 0 : this.targetName.GetHashCode());
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				TlsCredentialCache.CacheKey cacheKey = obj as TlsCredentialCache.CacheKey;
				return cacheKey != null && this.targetName == cacheKey.targetName && this.certificate == cacheKey.certificate;
			}

			public override string ToString()
			{
				return this.targetName + " " + this.certificate;
			}

			private readonly X509Certificate certificate;

			private readonly string targetName;
		}

		private class CacheValue
		{
			internal int Count
			{
				get
				{
					return this.credentials.Count;
				}
			}

			internal List<TlsCredentialCache.CachedCredential> Credentials
			{
				get
				{
					return this.credentials;
				}
			}

			internal bool Push(SafeCredentialsHandle handle)
			{
				bool result = true;
				if (this.credentials.Count == TlsCredentialCache.MaxPerHost)
				{
					this.DeleteOldest();
					result = false;
				}
				this.credentials.Add(new TlsCredentialCache.CachedCredential(handle));
				return result;
			}

			internal SafeCredentialsHandle Pop()
			{
				if (this.credentials.Count == 0)
				{
					throw new InvalidOperationException();
				}
				SafeCredentialsHandle handle = this.credentials[this.credentials.Count - 1].Handle;
				this.credentials.RemoveAt(this.credentials.Count - 1);
				return handle;
			}

			internal void DeleteOldest()
			{
				if (this.credentials.Count == 0)
				{
					throw new InvalidOperationException();
				}
				this.credentials[0].Handle.Dispose();
				this.credentials.RemoveAt(0);
			}

			internal void DeleteAll()
			{
				foreach (TlsCredentialCache.CachedCredential cachedCredential in this.credentials)
				{
					cachedCredential.Handle.Dispose();
				}
				this.credentials.Clear();
			}

			private List<TlsCredentialCache.CachedCredential> credentials = new List<TlsCredentialCache.CachedCredential>(TlsCredentialCache.MaxPerHost);
		}

		private class CachedCredential
		{
			internal CachedCredential(SafeCredentialsHandle handle)
			{
				this.handle = handle;
			}

			internal SafeCredentialsHandle Handle
			{
				get
				{
					return this.handle;
				}
			}

			internal DateTime Created
			{
				get
				{
					return this.created;
				}
			}

			private readonly SafeCredentialsHandle handle;

			private readonly DateTime created = DateTime.UtcNow;
		}

		private class CacheExpireEntry : IComparer<TlsCredentialCache.CacheExpireEntry>
		{
			internal CacheExpireEntry(TlsCredentialCache.CacheKey key, DateTime created)
			{
				this.created = created;
				this.key = key;
			}

			internal TlsCredentialCache.CacheKey Key
			{
				get
				{
					return this.key;
				}
			}

			internal DateTime Created
			{
				get
				{
					return this.created;
				}
			}

			public int Compare(TlsCredentialCache.CacheExpireEntry x, TlsCredentialCache.CacheExpireEntry y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				if (x.created < y.created)
				{
					return -1;
				}
				if (!(x.created > y.created))
				{
					return 0;
				}
				return 1;
			}

			private readonly TlsCredentialCache.CacheKey key;

			private readonly DateTime created;
		}
	}
}
