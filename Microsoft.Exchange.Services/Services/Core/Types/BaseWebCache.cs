using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class BaseWebCache<K, T>
	{
		public BaseWebCache(string keyPrefix, SlidingOrAbsoluteTimeout slidingOrAbsoluteTimeout, int timeoutInMinutes) : this(keyPrefix, slidingOrAbsoluteTimeout, timeoutInMinutes, false)
		{
		}

		public BaseWebCache(string keyPrefix, SlidingOrAbsoluteTimeout slidingOrAbsoluteTimeout, int timeoutInMinutes, bool requireRemoveCallback)
		{
			this.keyPrefix = keyPrefix;
			this.slidingOrAbsoluteTimeout = slidingOrAbsoluteTimeout;
			this.timeoutInMinutes = timeoutInMinutes;
			this.requireRemoveCallback = requireRemoveCallback;
		}

		protected virtual bool ValidateAddition(K key, T value)
		{
			return true;
		}

		protected virtual string KeyToString(K key)
		{
			return key.ToString();
		}

		protected virtual string BuildKey(K key)
		{
			return this.keyPrefix + this.KeyToString(key);
		}

		public virtual bool Add(K key, T value)
		{
			return this.AddInternal(key, value, false);
		}

		public virtual bool ForceAdd(K key, T value)
		{
			return this.AddInternal(key, value, true);
		}

		private bool AddInternal(K key, T value, bool replace)
		{
			if (!this.ValidateAddition(key, value))
			{
				return false;
			}
			DateTime absoluteExpiration;
			TimeSpan slidingExpiration;
			if (this.slidingOrAbsoluteTimeout == SlidingOrAbsoluteTimeout.Absolute)
			{
				absoluteExpiration = ExDateTime.Now.AddMinutes((double)this.timeoutInMinutes).UniversalTime;
				slidingExpiration = Cache.NoSlidingExpiration;
			}
			else
			{
				absoluteExpiration = Cache.NoAbsoluteExpiration;
				slidingExpiration = TimeSpan.FromMinutes((double)this.timeoutInMinutes);
			}
			string key2 = this.BuildKey(key);
			if (!this.Contains(key))
			{
				HttpRuntime.Cache.Add(key2, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Low, this.requireRemoveCallback ? new CacheItemRemovedCallback(this.HandleRemove) : null);
				return true;
			}
			if (replace)
			{
				HttpRuntime.Cache.Insert(key2, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Low, this.requireRemoveCallback ? new CacheItemRemovedCallback(this.HandleRemove) : null);
				return true;
			}
			return false;
		}

		public virtual T Get(K key)
		{
			return (T)((object)HttpRuntime.Cache.Get(this.BuildKey(key)));
		}

		protected virtual void HandleRemove(string key, object value, CacheItemRemovedReason reason)
		{
		}

		public virtual bool Contains(K key)
		{
			return HttpRuntime.Cache.Get(this.BuildKey(key)) != null;
		}

		internal virtual void ClearCache()
		{
			List<string> list = new List<string>();
			foreach (object obj in HttpRuntime.Cache)
			{
				string text = ((DictionaryEntry)obj).Key as string;
				if (text != null && text.StartsWith(this.keyPrefix))
				{
					list.Add(text);
				}
			}
			foreach (string key in list)
			{
				HttpRuntime.Cache.Remove(key);
			}
		}

		private string keyPrefix;

		private SlidingOrAbsoluteTimeout slidingOrAbsoluteTimeout;

		private int timeoutInMinutes;

		private bool requireRemoveCallback;
	}
}
