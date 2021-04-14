using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallInfoCache
	{
		internal UMCallInfoEx this[Guid key]
		{
			get
			{
				CallInfoCache.CacheEntry cacheEntry = null;
				lock (this.cache)
				{
					this.cache.TryGetValue(key, out cacheEntry);
				}
				if (cacheEntry == null)
				{
					return null;
				}
				return cacheEntry.CallInfo;
			}
			set
			{
				lock (this.cache)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this, "Adding a new entry to the CallInfoCache: {0}={1}/{2}.", new object[]
					{
						key,
						value.CallState,
						value.EventCause
					});
					this.cache[key] = new CallInfoCache.CacheEntry(key, value, Constants.CallInfoExpirationTime, new TimerCallback(this.RemoveCacheEntry));
				}
			}
		}

		private void RemoveCacheEntry(object state)
		{
			CallInfoCache.CacheEntry cacheEntry = state as CallInfoCache.CacheEntry;
			lock (this.cache)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this, "Removing {0} from CallInfoCache.", new object[]
				{
					cacheEntry.Key
				});
				this.cache.Remove(cacheEntry.Key);
				cacheEntry.Dispose();
			}
		}

		private Dictionary<Guid, CallInfoCache.CacheEntry> cache = new Dictionary<Guid, CallInfoCache.CacheEntry>();

		private class CacheEntry : DisposableBase
		{
			internal CacheEntry(Guid key, UMCallInfoEx callInfo, TimeSpan expirationTime, TimerCallback cacheEntryExpired)
			{
				this.key = key;
				this.callInfo = callInfo;
				this.expirationTimer = new Timer(cacheEntryExpired, this, expirationTime, TimeSpan.Zero);
			}

			public Guid Key
			{
				get
				{
					return this.key;
				}
			}

			public UMCallInfoEx CallInfo
			{
				get
				{
					return this.callInfo;
				}
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing && this.expirationTimer != null)
				{
					this.expirationTimer.Dispose();
					this.expirationTimer = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<CallInfoCache.CacheEntry>(this);
			}

			private Guid key;

			private UMCallInfoEx callInfo;

			private Timer expirationTimer;
		}
	}
}
