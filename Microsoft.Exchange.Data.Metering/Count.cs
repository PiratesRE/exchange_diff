using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal abstract class Count<TEntityType, TCountType> : ICount<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public Count(ICountedEntity<TEntityType> entity, ICountedConfig config, TCountType measure) : this(entity, config, measure, () => DateTime.UtcNow)
		{
		}

		public Count(ICountedEntity<TEntityType> entity, ICountedConfig config, TCountType measure, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("measure", measure);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			this.Entity = entity;
			this.Config = config;
			this.Measure = measure;
			this.timeProvider = timeProvider;
			this.lastAccessTime = this.timeProvider();
			if (this.Config.TimeToLive != TimeSpan.Zero)
			{
				this.expirationTime = this.timeProvider().Add(this.Config.TimeToLive);
			}
		}

		public ICountedEntity<TEntityType> Entity { get; private set; }

		public ICountedConfig Config { get; private set; }

		public TCountType Measure { get; private set; }

		public bool IsPromoted
		{
			get
			{
				return true;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.updateQueue.IsEmpty && this.Total == 0L && (this.Trendline == null || this.Trendline.IsEmpty);
			}
		}

		public bool IsRemovable
		{
			get
			{
				return this.Config.IsRemovable || this.IsEmpty;
			}
		}

		public ITrendline Trend
		{
			get
			{
				return this.trendline;
			}
		}

		public abstract long Total { get; }

		public abstract long Average { get; }

		internal bool NeedsUpdate
		{
			get
			{
				return this.updateQueue.Count > 0;
			}
		}

		internal DateTime ExpirationTime
		{
			get
			{
				return this.expirationTime;
			}
		}

		internal bool ShouldExpire
		{
			get
			{
				DateTime dateTime = this.TimeProvider();
				return (this.Config.IdleTimeToLive != TimeSpan.Zero && dateTime - this.lastAccessTime > this.Config.IdleTimeToLive) || (this.expirationTime != DateTime.MaxValue && this.expirationTime < dateTime);
			}
		}

		protected Trendline Trendline
		{
			get
			{
				return this.trendline;
			}
			set
			{
				this.trendline = value;
			}
		}

		protected Func<DateTime> TimeProvider
		{
			get
			{
				return this.timeProvider;
			}
		}

		public void AddValue(long increment)
		{
			if (increment == 0L)
			{
				return;
			}
			bool flag = false;
			try
			{
				Monitor.TryEnter(this.syncObject, 0, ref flag);
				if (flag)
				{
					this.UpdateAccessTime();
					this.DrainUpdateQueue();
					this.InternalAddValue(increment);
				}
				else
				{
					this.updateQueue.Enqueue(increment);
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this.syncObject);
				}
			}
		}

		public bool TrySetValue(long value)
		{
			bool flag = false;
			bool result;
			try
			{
				Monitor.TryEnter(this.syncObject, ref flag);
				if (flag)
				{
					this.UpdateAccessTime();
					result = this.InternalSetValue(value);
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this.syncObject);
				}
			}
			return result;
		}

		public bool TryGetObject(string key, out object value)
		{
			value = null;
			if (this.properties == null)
			{
				return false;
			}
			this.UpdateAccessTime();
			return this.properties.TryGetValue(key, out value);
		}

		public void SetObject(string key, object value)
		{
			if (this.properties == null)
			{
				lock (this.propertiesSyncObject)
				{
					if (this.properties == null)
					{
						this.properties = new ConcurrentDictionary<string, object>();
					}
				}
			}
			this.UpdateAccessTime();
			this.properties[key] = value;
		}

		public void TimedUpdate()
		{
			if (!this.NeedsUpdate)
			{
				return;
			}
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					this.DrainUpdateQueue();
				}
			}
		}

		public Count<TEntityType, TCountType> Merge(Count<TEntityType, TCountType> count)
		{
			ArgumentValidator.ThrowIfNull("count", count);
			if (object.ReferenceEquals(this, count))
			{
				return this;
			}
			if (!object.ReferenceEquals(this.Config, count.Config))
			{
				throw new InvalidOperationException("cannot merge two counts with different config");
			}
			if (!this.Entity.Equals(count.Entity))
			{
				throw new InvalidOperationException("cannot merge two counts representing different entities");
			}
			TCountType measure = this.Measure;
			if (!measure.Equals(count.Measure))
			{
				throw new InvalidOperationException("cannot merge two counts representing different measures");
			}
			return this.InternalMerge(count);
		}

		public XElement GetDiagnosticInfo()
		{
			if (this.IsEmpty || this.ShouldExpire)
			{
				return null;
			}
			XElement xelement = new XElement("Count");
			xelement.SetAttributeValue("Name", this.Measure);
			xelement.SetAttributeValue("Value", this.Total);
			xelement.SetAttributeValue("Average", this.Average);
			xelement.SetAttributeValue("LastAccessTime", this.lastAccessTime.ToString("yyyy-MM-ddTHH:mm:ss"));
			xelement.SetAttributeValue("ExpirationTime", this.ExpirationTime.ToString("yyyy-MM-ddTHH:mm:ss"));
			xelement.SetAttributeValue("PropertyCount", (this.properties != null) ? this.properties.Count : 0);
			return xelement;
		}

		public override string ToString()
		{
			return string.Format("Count for Entity {0}, Measure:{1}", this.Entity, this.Measure);
		}

		protected abstract Count<TEntityType, TCountType> InternalMerge(Count<TEntityType, TCountType> count);

		protected abstract void InternalAddValue(long increment);

		protected abstract bool InternalSetValue(long value);

		protected void CopyPropertiesTo(Count<TEntityType, TCountType> other)
		{
			if (this.properties != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in this.properties)
				{
					other.SetObject(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		protected void UpdateAccessTime()
		{
			this.lastAccessTime = this.timeProvider();
		}

		private void DrainUpdateQueue()
		{
			if (!this.NeedsUpdate)
			{
				return;
			}
			long increment;
			while (this.updateQueue.TryDequeue(out increment))
			{
				this.InternalAddValue(increment);
			}
		}

		private readonly object syncObject = new object();

		private readonly DateTime expirationTime = DateTime.MaxValue;

		private Func<DateTime> timeProvider;

		private ConcurrentQueue<long> updateQueue = new ConcurrentQueue<long>();

		private Trendline trendline;

		private ConcurrentDictionary<string, object> properties;

		private object propertiesSyncObject = new object();

		private DateTime lastAccessTime;
	}
}
