using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Data.Metering
{
	internal class CountTracker<TEntityType, TCountType> : ICountTracker<TEntityType, TCountType>, IDisposable where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public CountTracker(ICountTrackerConfig config, ICountTrackerDiagnostics<TEntityType, TCountType> perfCounters, Trace tracer) : this(config, perfCounters, tracer, () => DateTime.UtcNow)
		{
		}

		public CountTracker(ICountTrackerConfig config, ICountTrackerDiagnostics<TEntityType, TCountType> perfCounters, Trace tracer, Func<DateTime> timeProvider)
		{
			if (!typeof(TEntityType).IsEnum)
			{
				throw new ArgumentException("TEntityType must be an enum");
			}
			if (!typeof(TCountType).IsEnum)
			{
				throw new ArgumentException("TCountType must be an enum");
			}
			if (typeof(TEntityType).IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("TEntityType cannot be a flags enum");
			}
			if (typeof(TCountType).IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("TCountType cannot be a flags enum");
			}
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("perfCounters", perfCounters);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			this.config = config;
			this.perfCounters = perfCounters;
			this.tracer = tracer;
			this.timeProvider = timeProvider;
			this.lastUnpromotedEmptiedTime = this.timeProvider();
			this.timer = new GuardedTimer(new TimerCallback(this.TimerCallback), null, TimeSpan.FromSeconds(5.0));
		}

		public void AddUsage(ICountedEntity<TEntityType> entity, TCountType measure, ICountedConfig countConfig, long increment)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("countConfig", countConfig);
			CountTracker<TEntityType, TCountType>.EntityValue entityValue;
			if (this.entities.TryGetValue(entity.Name, out entityValue))
			{
				entityValue.AddMeasure(measure, countConfig, increment);
				return;
			}
			if (!countConfig.IsPromotable)
			{
				this.AddEntityAndMeasure(entity, measure, countConfig, increment);
				return;
			}
			if (this.entities.Any((KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> e) => e.Value.Entity.GroupName.Equals(entity.GroupName)))
			{
				this.AddEntityAndMeasure(entity, measure, countConfig, increment);
				return;
			}
			CountTracker<TEntityType, TCountType>.EntityKey entityKey = new CountTracker<TEntityType, TCountType>.EntityKey(entity.GroupName, measure);
			long num = this.unpromotedMeasures.AddOrUpdate(entityKey, (CountTracker<TEntityType, TCountType>.EntityKey k) => increment, (CountTracker<TEntityType, TCountType>.EntityKey k, long v) => v + increment);
			if (num > (long)countConfig.MinActivityThreshold)
			{
				this.AddEntityAndMeasure(entity, measure, countConfig, increment);
				this.unpromotedMeasures.TryRemove(entityKey, out num);
				this.tracer.TraceDebug<IEntityName<TEntityType>, TCountType, int>((long)this.GetHashCode(), "Promoted entity {0} and measure {1} for crossing threshold {2}", entityKey.Entity, entityKey.Measure, countConfig.MinActivityThreshold);
				this.perfCounters.MeasurePromoted(measure);
			}
		}

		public Task AddUsageAsync(ICountedEntity<TEntityType> entity, TCountType measure, ICountedConfig countConfig, long increment)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("countConfig", countConfig);
			return Task.Factory.StartNew(delegate()
			{
				this.AddUsage(entity, measure, countConfig, increment);
			});
		}

		public bool TrySetUsage(ICountedEntity<TEntityType> entity, TCountType measure, long value)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			CountTracker<TEntityType, TCountType>.EntityValue entityValue;
			return this.entities.TryGetValue(entity.Name, out entityValue) && entityValue.TrySetMeasure(measure, value);
		}

		public Task<bool> SetUsageAsync(ICountedEntity<TEntityType> entity, TCountType measure, long value)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			return Task<bool>.Factory.StartNew(() => this.TrySetUsage(entity, measure, value));
		}

		public ICount<TEntityType, TCountType> GetUsage(ICountedEntity<TEntityType> entity, TCountType measure)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			CountTracker<TEntityType, TCountType>.EntityValue entityValue;
			if (this.entities.TryGetValue(entity.Name, out entityValue))
			{
				ICount<TEntityType, TCountType> usage = entityValue.GetUsage(measure);
				if (usage != null)
				{
					return usage;
				}
			}
			return new EmptyCount<TEntityType, TCountType>(entity, measure);
		}

		public Task<ICount<TEntityType, TCountType>> GetUsageAsync(ICountedEntity<TEntityType> entity, TCountType measure)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			return Task<ICount<TEntityType, TCountType>>.Factory.StartNew(() => this.GetUsage(entity, measure));
		}

		public IDictionary<TCountType, ICount<TEntityType, TCountType>> GetUsage(ICountedEntity<TEntityType> entity, TCountType[] measures)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("measures", measures);
			CountTracker<TEntityType, TCountType>.EntityValue entityValue;
			if (this.entities.TryGetValue(entity.Name, out entityValue))
			{
				IDictionary<TCountType, ICount<TEntityType, TCountType>> dictionary = new Dictionary<TCountType, ICount<TEntityType, TCountType>>();
				foreach (TCountType tcountType in measures)
				{
					ICount<TEntityType, TCountType> usage = entityValue.GetUsage(tcountType);
					if (usage != null)
					{
						dictionary.Add(tcountType, usage);
					}
				}
				return dictionary;
			}
			return measures.ToDictionary((TCountType countType) => countType, (TCountType countType) => new EmptyCount<TEntityType, TCountType>(entity, countType));
		}

		public Task<IDictionary<TCountType, ICount<TEntityType, TCountType>>> GetUsageAsync(ICountedEntity<TEntityType> entity, TCountType[] measures)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("measures", measures);
			return Task<IDictionary<TCountType, ICount<TEntityType, TCountType>>>.Factory.StartNew(() => this.GetUsage(entity, measures));
		}

		public IEnumerable<ICount<TEntityType, TCountType>> GetAllUsages(ICountedEntity<TEntityType> entity)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			CountTracker<TEntityType, TCountType>.EntityValue source;
			if (this.entities.TryGetValue(entity.Name, out source))
			{
				return from c in source
				select new CountWrapper<TEntityType, TCountType>(c);
			}
			return null;
		}

		public Task<IEnumerable<ICount<TEntityType, TCountType>>> GetAllUsagesAsync(ICountedEntity<TEntityType> entity)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			return Task<IEnumerable<ICount<TEntityType, TCountType>>>.Factory.StartNew(() => this.GetAllUsages(entity));
		}

		public bool TryGetEntityObject(ICountedEntity<TEntityType> entity, out ICountedEntityWrapper<TEntityType, TCountType> wrapper)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			if (this.entities.ContainsKey(entity.Name))
			{
				wrapper = new CountedEntityWrapper<TEntityType, TCountType>(entity, this);
				return true;
			}
			wrapper = null;
			return false;
		}

		public ICountedConfig GetConfig(ICountedEntity<TEntityType> entity, TCountType measure)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			CountTracker<TEntityType, TCountType>.EntityValue entityValue;
			if (this.entities.TryGetValue(entity.Name, out entityValue))
			{
				ICount<TEntityType, TCountType> usage = entityValue.GetUsage(measure);
				if (usage != null)
				{
					return usage.Config;
				}
			}
			return null;
		}

		public IEnumerable<ICount<TEntityType, TCountType>> Filter(Func<ICount<TEntityType, TCountType>, bool> isMatch)
		{
			ArgumentValidator.ThrowIfNull("isMatch", isMatch);
			List<ICount<TEntityType, TCountType>> list = new List<ICount<TEntityType, TCountType>>();
			foreach (CountTracker<TEntityType, TCountType>.EntityValue entityValue in this.entities.Values)
			{
				list.AddRange(from c in entityValue.Filter(isMatch)
				select new CountWrapper<TEntityType, TCountType>(c));
			}
			return list;
		}

		public Task<IEnumerable<ICount<TEntityType, TCountType>>> FilterAsync(Func<ICount<TEntityType, TCountType>, bool> isMatch)
		{
			ArgumentValidator.ThrowIfNull("isMatch", isMatch);
			return Task<IEnumerable<ICount<TEntityType, TCountType>>>.Factory.StartNew(() => this.Filter(isMatch));
		}

		public IEnumerable<ICountedEntityValue<TEntityType, TCountType>> Filter(Func<ICountedEntityValue<TEntityType, TCountType>, bool> isMatch)
		{
			ArgumentValidator.ThrowIfNull("isMatch", isMatch);
			return from e in this.entities
			where isMatch(e.Value)
			select e.Value;
		}

		public Task<IEnumerable<ICountedEntityValue<TEntityType, TCountType>>> FilterAsync(Func<ICountedEntityValue<TEntityType, TCountType>, bool> isMatch)
		{
			ArgumentValidator.ThrowIfNull("isMatch", isMatch);
			return Task<IEnumerable<ICountedEntityValue<TEntityType, TCountType>>>.Factory.StartNew(() => this.Filter(isMatch));
		}

		public void GetDiagnosticInfo(string argument, XElement element)
		{
			element.SetAttributeValue("UnpromotedCount", this.unpromotedMeasures.Count);
			element.SetAttributeValue("PromotedCount", this.entities.Count);
			element.SetAttributeValue("LastPromotionTime", this.lastUnpromotedEmptiedTime.ToString("yyyy-MM-ddTHH:mm:ss"));
			element.SetAttributeValue("NeedsCleanup", this.needsEntityCleanup);
			if (argument != null && argument.IndexOf("verbose", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				XElement xelement = new XElement("Entities");
				IEnumerable<CountTracker<TEntityType, TCountType>.EntityValue> enumerable = (from e in this.entities
				orderby e.Value.LastAccesTime descending
				select e.Value).Take(50);
				foreach (CountTracker<TEntityType, TCountType>.EntityValue entityValue in enumerable)
				{
					if (!entityValue.IsEmpty)
					{
						xelement.Add(entityValue.GetDiagnosticInfo());
					}
				}
				element.Add(xelement);
			}
		}

		public XElement GetDiagnosticInfo(IEntityName<TEntityType> entity)
		{
			IOrderedEnumerable<CountTracker<TEntityType, TCountType>.EntityValue> orderedEnumerable = from p in this.entities
			where p.Key.Equals(entity) || p.Value.Entity.GroupName.Equals(entity)
			select p.Value into e
			orderby e.LastAccesTime descending
			select e;
			XElement xelement = new XElement("Entities");
			xelement.SetAttributeValue("Name", entity.ToString());
			foreach (CountTracker<TEntityType, TCountType>.EntityValue entityValue in orderedEnumerable)
			{
				xelement.Add(entityValue.GetDiagnosticInfo());
			}
			return xelement;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void TimedUpdate()
		{
			DateTime dateTime = this.timeProvider();
			if (dateTime - this.lastUnpromotedEmptiedTime > this.config.PromotionInterval)
			{
				this.unpromotedMeasures.Clear();
				this.lastUnpromotedEmptiedTime = dateTime;
			}
			CountConfig.CleanupCachedConfig(dateTime, this.config.IdleCachedConfigCleanupInterval);
			foreach (CountTracker<TEntityType, TCountType>.EntityValue entityValue in this.entities.Values)
			{
				entityValue.TimedUpdate();
			}
			this.RemoveEmptyEntities(dateTime);
			if (this.needsEntityCleanup)
			{
				this.RemoveExcessEntitiesPerGroup(dateTime);
				this.RemoveExcessEntities(dateTime);
				this.needsEntityCleanup = false;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.timer != null)
			{
				this.timer.Dispose(false);
				this.timer = null;
			}
		}

		private void AddEntityAndMeasure(ICountedEntity<TEntityType> entity, TCountType measure, ICountedConfig countConfig, long increment)
		{
			CountTracker<TEntityType, TCountType>.EntityValue entityValue = this.AddEntity(entity);
			entityValue.AddMeasure(measure, countConfig, increment);
		}

		private CountTracker<TEntityType, TCountType>.EntityValue AddEntity(ICountedEntity<TEntityType> entity)
		{
			CountTracker<TEntityType, TCountType>.EntityValue result = this.entities.AddOrUpdate(entity.Name, delegate(IEntityName<TEntityType> e)
			{
				this.perfCounters.EntityAdded(entity);
				return new CountTracker<TEntityType, TCountType>.EntityValue(entity, this.perfCounters, this.timeProvider);
			}, (IEntityName<TEntityType> e, CountTracker<TEntityType, TCountType>.EntityValue v) => v);
			this.needsEntityCleanup = (this.entities.Count > this.config.MaxEntityCount);
			return result;
		}

		private void TimerCallback(object state)
		{
			this.TimedUpdate();
		}

		private void RemoveEmptyEntities(DateTime now)
		{
			foreach (KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> keyValuePair in from p in this.entities
			where p.Value.IsEmpty
			select p)
			{
				if (!(keyValuePair.Value.LastAccesTime > now))
				{
					CountTracker<TEntityType, TCountType>.EntityValue value;
					this.entities.TryRemove(keyValuePair.Key, out value);
					if (!value.IsEmpty)
					{
						this.entities.AddOrUpdate(keyValuePair.Key, value, (IEntityName<TEntityType> k, CountTracker<TEntityType, TCountType>.EntityValue v) => v.Merge(value));
					}
				}
			}
		}

		private void RemoveExcessEntitiesPerGroup(DateTime now)
		{
			int num = 0;
			foreach (IGrouping<IEntityName<TEntityType>, KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue>> grouping in from p in this.entities
			group p by p.Value.Entity.GroupName into p
			where p.Count<KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue>>() > this.config.MaxEntitiesPerGroup
			select p into g
			orderby g.Count<KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue>>() descending
			select g)
			{
				int num2 = grouping.Count<KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue>>();
				IEnumerable<KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue>> enumerable = grouping.Where((KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> p) => p.Value.All((Count<TEntityType, TCountType> c) => c.IsRemovable)).OrderBy((KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> p) => p.Value.LastAccesTime).ThenBy((KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> p) => p.Value.Sum((Count<TEntityType, TCountType> c) => c.Total)).Take(num2 - this.config.MaxEntitiesPerGroup + (int)(0.05 * (double)this.config.MaxEntitiesPerGroup));
				foreach (KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> keyValuePair in enumerable)
				{
					if (!(keyValuePair.Value.LastAccesTime > now))
					{
						CountTracker<TEntityType, TCountType>.EntityValue entityValue;
						this.entities.TryRemove(keyValuePair.Key, out entityValue);
						this.perfCounters.EntityRemoved(entityValue.Entity);
						num++;
					}
				}
				this.tracer.TraceDebug<int, IEntityName<TEntityType>>(0L, "Removing {0} low-use entities for group {1}", num2 - this.config.MaxEntitiesPerGroup, grouping.Key);
			}
			if (num > 0)
			{
				this.tracer.TraceDebug<int>(0L, "Removed {0} entities for exceeded perGroup count", num);
			}
		}

		private void RemoveExcessEntities(DateTime now)
		{
			int num = 0;
			if (this.entities.Count > this.config.MaxEntityCount)
			{
				IEnumerable<KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue>> enumerable = (from p in this.entities
				where p.Value.All((Count<TEntityType, TCountType> c) => c.IsRemovable)
				select p into e
				orderby e.Value.LastAccesTime
				select e).ThenBy((KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> v) => v.Value.Sum((Count<TEntityType, TCountType> c) => c.Total)).Take(this.entities.Count - this.config.MaxEntityCount + (int)(0.05 * (double)this.config.MaxEntityCount));
				foreach (KeyValuePair<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> keyValuePair in enumerable)
				{
					if (!(keyValuePair.Value.LastAccesTime > now))
					{
						CountTracker<TEntityType, TCountType>.EntityValue entityValue;
						this.entities.TryRemove(keyValuePair.Key, out entityValue);
						this.perfCounters.EntityRemoved(entityValue.Entity);
						num++;
					}
				}
			}
			this.tracer.TraceDebug<int, int>(0L, "Removed {0} entities for exceeded maxEntity count {1}", num, this.config.MaxEntityCount);
		}

		private readonly ICountTrackerConfig config;

		private readonly ICountTrackerDiagnostics<TEntityType, TCountType> perfCounters;

		private readonly Trace tracer;

		private readonly ConcurrentDictionary<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue> entities = new ConcurrentDictionary<IEntityName<TEntityType>, CountTracker<TEntityType, TCountType>.EntityValue>();

		private readonly ConcurrentDictionary<CountTracker<TEntityType, TCountType>.EntityKey, long> unpromotedMeasures = new ConcurrentDictionary<CountTracker<TEntityType, TCountType>.EntityKey, long>();

		private Func<DateTime> timeProvider;

		private GuardedTimer timer;

		private bool needsEntityCleanup;

		private DateTime lastUnpromotedEmptiedTime;

		private class EntityKey
		{
			public EntityKey(IEntityName<TEntityType> entity, TCountType measure)
			{
				this.entity = entity;
				this.measure = measure;
			}

			public IEntityName<TEntityType> Entity
			{
				get
				{
					return this.entity;
				}
			}

			public TCountType Measure
			{
				get
				{
					return this.measure;
				}
			}

			public bool Equals(CountTracker<TEntityType, TCountType>.EntityKey key)
			{
				if (object.ReferenceEquals(null, key))
				{
					return false;
				}
				TCountType tcountType = this.measure;
				return tcountType.Equals(key.measure) && this.entity.Equals(key.entity);
			}

			public override bool Equals(object obj)
			{
				return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is CountTracker<TEntityType, TCountType>.EntityKey && this.Equals((CountTracker<TEntityType, TCountType>.EntityKey)obj)));
			}

			public override int GetHashCode()
			{
				int hashCode = this.entity.GetHashCode();
				TCountType tcountType = this.measure;
				return hashCode ^ tcountType.GetHashCode();
			}

			private readonly IEntityName<TEntityType> entity;

			private readonly TCountType measure;
		}

		private class EntityValue : IEnumerable<Count<TEntityType, TCountType>>, IEnumerable, ICountedEntityValue<TEntityType, TCountType>
		{
			static EntityValue()
			{
				Array values = Enum.GetValues(typeof(TCountType));
				int num = 0;
				foreach (object obj in values)
				{
					TCountType key = (TCountType)((object)obj);
					CountTracker<TEntityType, TCountType>.EntityValue.countTypeToIndexMap.Add(key, num++);
				}
			}

			public EntityValue(ICountedEntity<TEntityType> entity, ICountTrackerDiagnostics<TEntityType, TCountType> perfCounters, Func<DateTime> timeProvider)
			{
				this.entity = entity;
				this.perfCounters = perfCounters;
				this.timeProvider = timeProvider;
				this.measures = new Count<TEntityType, TCountType>[CountTracker<TEntityType, TCountType>.EntityValue.countTypeToIndexMap.Count];
				this.lastAccessTime = this.timeProvider();
			}

			private EntityValue(ICountedEntity<TEntityType> entity, ICountTrackerDiagnostics<TEntityType, TCountType> perfCounters, Count<TEntityType, TCountType>[] measures, Func<DateTime> timeProvider) : this(entity, perfCounters, timeProvider)
			{
				this.measures = measures;
			}

			public ICountedEntity<TEntityType> Entity
			{
				get
				{
					return this.entity;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.measures.All((Count<TEntityType, TCountType> c) => c == null || c.IsEmpty);
				}
			}

			public DateTime LastAccesTime
			{
				get
				{
					return this.lastAccessTime;
				}
			}

			public void AddMeasure(TCountType measure, ICountedConfig config, long value)
			{
				int num = this.ConvertMeasureToIndex(measure);
				Count<TEntityType, TCountType> count = this.measures[num];
				if (count == null)
				{
					count = this.CreateNewCount(measure, config, num);
				}
				count.AddValue(value);
				this.lastAccessTime = this.timeProvider();
			}

			public bool TrySetMeasure(TCountType measure, long value)
			{
				int num = this.ConvertMeasureToIndex(measure);
				Count<TEntityType, TCountType> count = this.measures[num];
				if (count == null)
				{
					return false;
				}
				this.lastAccessTime = this.timeProvider();
				return count.TrySetValue(value);
			}

			public ICount<TEntityType, TCountType> GetUsage(TCountType measure)
			{
				int num = this.ConvertMeasureToIndex(measure);
				this.lastAccessTime = this.timeProvider();
				Count<TEntityType, TCountType> count = this.measures[num];
				if (count != null)
				{
					return new CountWrapper<TEntityType, TCountType>(count);
				}
				return null;
			}

			public IEnumerable<Count<TEntityType, TCountType>> Filter(Func<ICount<TEntityType, TCountType>, bool> isMatch)
			{
				IEnumerable<Count<TEntityType, TCountType>> enumerable = from measure in this.measures
				where measure != null && isMatch(measure)
				select measure;
				if (enumerable.Any<Count<TEntityType, TCountType>>())
				{
					this.lastAccessTime = this.timeProvider();
				}
				return enumerable;
			}

			public bool HasUsage(TCountType measure)
			{
				int num = this.ConvertMeasureToIndex(measure);
				return this.measures[num] != null;
			}

			public void TimedUpdate()
			{
				this.Expire();
				foreach (Count<TEntityType, TCountType> count in from m in this
				where m != null && m.NeedsUpdate
				select m)
				{
					count.TimedUpdate();
				}
			}

			public IEnumerator<Count<TEntityType, TCountType>> GetEnumerator()
			{
				this.lastAccessTime = this.timeProvider();
				return (from count in this.measures
				where count != null
				select count).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public CountTracker<TEntityType, TCountType>.EntityValue Merge(CountTracker<TEntityType, TCountType>.EntityValue value)
			{
				if (!this.entity.Equals(value.entity))
				{
					throw new InvalidOperationException("Can't merge EntityValue objects for different entities");
				}
				if (this.IsEmpty)
				{
					return value;
				}
				if (value.IsEmpty)
				{
					return this;
				}
				CountTracker<TEntityType, TCountType>.EntityValue result;
				lock (this.syncObject)
				{
					Count<TEntityType, TCountType>[] array = new Count<TEntityType, TCountType>[CountTracker<TEntityType, TCountType>.EntityValue.countTypeToIndexMap.Count];
					for (int i = 0; i < this.measures.Length; i++)
					{
						if (this.measures[i] != null && value.measures[i] == null)
						{
							array[i] = this.measures[i];
						}
						else if (this.measures[i] == null && value.measures[i] != null)
						{
							array[i] = value.measures[i];
						}
						else if (this.measures[i] != null && value.measures[i] != null)
						{
							array[i] = this.measures[i].Merge(value.measures[i]);
						}
					}
					result = new CountTracker<TEntityType, TCountType>.EntityValue(this.Entity, this.perfCounters, array, this.timeProvider);
				}
				return result;
			}

			public XElement GetDiagnosticInfo()
			{
				XElement xelement = new XElement("Entity");
				xelement.SetAttributeValue("Name", this.Entity.ToString());
				foreach (Count<TEntityType, TCountType> count in this)
				{
					XElement diagnosticInfo = count.GetDiagnosticInfo();
					if (diagnosticInfo != null)
					{
						xelement.Add(diagnosticInfo);
					}
				}
				return xelement;
			}

			private int ConvertMeasureToIndex(TCountType measure)
			{
				int num;
				if (!CountTracker<TEntityType, TCountType>.EntityValue.countTypeToIndexMap.TryGetValue(measure, out num))
				{
					throw new ArgumentException("Unrecognized value of TCountType", "measure");
				}
				if (num < 0 || num > this.measures.Length)
				{
					throw new InvalidOperationException("Returned an invalid index from the map");
				}
				return num;
			}

			private Count<TEntityType, TCountType> CreateNewCount(TCountType measure, ICountedConfig config, int measureIndex)
			{
				Count<TEntityType, TCountType> count;
				lock (this.syncObject)
				{
					if (this.measures[measureIndex] == null)
					{
						count = CountFactory.CreateCount<TEntityType, TCountType>(this.entity, measure, config, this.timeProvider);
						this.measures[measureIndex] = count;
					}
					else
					{
						count = this.measures[measureIndex];
					}
				}
				this.perfCounters.MeasureAdded(measure);
				return count;
			}

			private void Expire()
			{
				for (int i = 0; i < this.measures.Length; i++)
				{
					Count<TEntityType, TCountType> count = this.measures[i];
					if (count != null && count.ShouldExpire)
					{
						this.perfCounters.MeasureExpired(count.Measure);
						this.measures[i] = null;
					}
				}
			}

			private static Dictionary<TCountType, int> countTypeToIndexMap = new Dictionary<TCountType, int>();

			private readonly ICountedEntity<TEntityType> entity;

			private readonly ICountTrackerDiagnostics<TEntityType, TCountType> perfCounters;

			private readonly Count<TEntityType, TCountType>[] measures;

			private DateTime lastAccessTime;

			private Func<DateTime> timeProvider;

			private object syncObject = new object();
		}
	}
}
