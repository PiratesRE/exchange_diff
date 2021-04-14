using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class CacheEntry<K, T>
	{
		private CacheEntry(TimeoutType timeoutType, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime, K key, T value, RemoveItemDelegate<K, T> callback)
		{
			if (absoluteLiveTime.TotalMilliseconds <= 0.0)
			{
				throw new ArgumentException("absoluteLiveTime must represent a positive (and non-zero) time interval.");
			}
			if (slidingLiveTime.TotalMilliseconds <= 0.0)
			{
				throw new ArgumentException("slidingLiveTime must represent a positive (and non-zero) time interval.");
			}
			this.key = key;
			this.value = value;
			this.callback = callback;
			this.timeoutType = timeoutType;
			this.InShouldRemoveCycle = false;
			this.absoluteExpirationTime = ((absoluteLiveTime == TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.UtcNow.Add(absoluteLiveTime));
			if (this.timeoutType == TimeoutType.Sliding)
			{
				this.slidingLiveTime = slidingLiveTime;
				this.nextExpirationTime = DateTime.UtcNow.Add(this.slidingLiveTime);
			}
			else
			{
				this.slidingLiveTime = absoluteLiveTime;
				this.nextExpirationTime = this.absoluteExpirationTime;
			}
			this.touchedExpirationTime = this.nextExpirationTime;
		}

		private CacheEntry(DateTime absoluteExpirationTime, K key, T value, RemoveItemDelegate<K, T> callback)
		{
			TimeSpan timeSpan = (absoluteExpirationTime == DateTime.MaxValue) ? TimeSpan.MaxValue : (absoluteExpirationTime - DateTime.UtcNow);
			if (timeSpan.TotalMilliseconds < 0.0)
			{
				throw new ArgumentException("absoluteExpirationTime is in the past.");
			}
			this.absoluteExpirationTime = absoluteExpirationTime;
			this.callback = callback;
			this.key = key;
			this.value = value;
			this.nextExpirationTime = absoluteExpirationTime;
			this.touchedExpirationTime = this.nextExpirationTime;
			this.slidingLiveTime = timeSpan;
			this.timeoutType = TimeoutType.Absolute;
			this.InShouldRemoveCycle = false;
		}

		internal T Value
		{
			get
			{
				return this.value;
			}
		}

		internal K Key
		{
			get
			{
				return this.key;
			}
		}

		internal bool InShouldRemoveCycle
		{
			get
			{
				return this.inShouldRemoveCycle;
			}
			set
			{
				this.inShouldRemoveCycle = value;
			}
		}

		internal DateTime AbsoluteExpirationTime
		{
			get
			{
				return this.absoluteExpirationTime;
			}
		}

		internal DateTime NextExpirationTime
		{
			get
			{
				return this.nextExpirationTime;
			}
		}

		internal TimeSpan SlidingLiveTime
		{
			get
			{
				return this.slidingLiveTime;
			}
		}

		internal DateTime TouchedExpirationTime
		{
			get
			{
				return this.touchedExpirationTime;
			}
		}

		internal RemoveItemDelegate<K, T> Callback
		{
			get
			{
				return this.callback;
			}
		}

		internal TimeoutType TimeoutType
		{
			get
			{
				return this.timeoutType;
			}
		}

		internal CacheEntry<K, T> Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		internal CacheEntry<K, T> Previous
		{
			get
			{
				return this.previous;
			}
			set
			{
				this.previous = value;
			}
		}

		public static CacheEntry<K, T> CreateAbsolute(DateTime expireExact, K key, T value, RemoveItemDelegate<K, T> callback)
		{
			return new CacheEntry<K, T>(expireExact, key, value, callback);
		}

		public static CacheEntry<K, T> CreateAbsolute(TimeSpan absoluteLiveTime, K key, T value, RemoveItemDelegate<K, T> callback)
		{
			return new CacheEntry<K, T>(TimeoutType.Absolute, absoluteLiveTime, absoluteLiveTime, key, value, callback);
		}

		public static CacheEntry<K, T> CreateSliding(TimeSpan slidingLiveTime, K key, T value, RemoveItemDelegate<K, T> callback)
		{
			return new CacheEntry<K, T>(TimeoutType.Sliding, slidingLiveTime, TimeSpan.MaxValue, key, value, callback);
		}

		public static CacheEntry<K, T> CreateLimitedSliding(TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime, K key, T value, RemoveItemDelegate<K, T> callback)
		{
			if (slidingLiveTime.TotalMilliseconds > absoluteLiveTime.TotalMilliseconds)
			{
				throw new ArgumentException("absoluteLiveTime must be greater than slidingLiveTime");
			}
			return new CacheEntry<K, T>(TimeoutType.Sliding, slidingLiveTime, absoluteLiveTime, key, value, callback);
		}

		internal void Extend()
		{
			this.nextExpirationTime = this.GetLimitedExtensionTime();
			this.touchedExpirationTime = this.nextExpirationTime;
		}

		internal void UnrestrictedExtend()
		{
			this.nextExpirationTime = this.GetExtensionTime();
			this.touchedExpirationTime = this.nextExpirationTime;
		}

		internal void ExtendToTouchTime()
		{
			if (this.timeoutType == TimeoutType.Sliding)
			{
				this.nextExpirationTime = this.touchedExpirationTime;
			}
		}

		internal void Touch()
		{
			if (this.timeoutType == TimeoutType.Sliding)
			{
				this.touchedExpirationTime = this.GetLimitedExtensionTime();
			}
		}

		private DateTime GetLimitedExtensionTime()
		{
			if (this.nextExpirationTime == this.absoluteExpirationTime)
			{
				return this.nextExpirationTime;
			}
			DateTime dateTime = DateTime.UtcNow.Add(this.slidingLiveTime);
			if (!(dateTime < this.absoluteExpirationTime))
			{
				return this.absoluteExpirationTime;
			}
			return dateTime;
		}

		private DateTime GetExtensionTime()
		{
			return DateTime.UtcNow.Add(this.slidingLiveTime);
		}

		private readonly K key;

		private readonly T value;

		private readonly DateTime absoluteExpirationTime;

		private readonly TimeoutType timeoutType;

		private readonly TimeSpan slidingLiveTime;

		private readonly RemoveItemDelegate<K, T> callback;

		private DateTime nextExpirationTime;

		private DateTime touchedExpirationTime;

		private CacheEntry<K, T> next;

		private CacheEntry<K, T> previous;

		private volatile bool inShouldRemoveCycle;
	}
}
