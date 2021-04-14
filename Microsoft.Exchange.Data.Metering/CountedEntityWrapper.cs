using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class CountedEntityWrapper<TEntityType, TCountType> : ICountedEntityWrapper<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public CountedEntityWrapper(ICountedEntity<TEntityType> entity, ICountTracker<TEntityType, TCountType> tracker)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("tracker", tracker);
			this.Entity = entity;
			this.tracker = tracker;
		}

		public ICountedEntity<TEntityType> Entity { get; private set; }

		public ICountedConfig GetConfig(TCountType measure)
		{
			return this.tracker.GetConfig(this.Entity, measure);
		}

		public void AddUsage(TCountType measure, ICountedConfig countConfig, int increment)
		{
			this.tracker.AddUsage(this.Entity, measure, countConfig, (long)increment);
		}

		public Task AddUsageAsync(TCountType measure, ICountedConfig countConfig, int increment)
		{
			return this.tracker.AddUsageAsync(this.Entity, measure, countConfig, (long)increment);
		}

		public bool TrySetUsage(TCountType measure, int value)
		{
			return this.tracker.TrySetUsage(this.Entity, measure, (long)value);
		}

		public Task<bool> SetUsageAsync(TCountType measure, int value)
		{
			return this.tracker.SetUsageAsync(this.Entity, measure, (long)value);
		}

		public ICount<TEntityType, TCountType> GetUsage(TCountType measure)
		{
			return this.tracker.GetUsage(this.Entity, measure);
		}

		public Task<ICount<TEntityType, TCountType>> GetUsageAsync(TCountType measure)
		{
			return this.tracker.GetUsageAsync(this.Entity, measure);
		}

		public IDictionary<TCountType, ICount<TEntityType, TCountType>> GetUsage(TCountType[] measures)
		{
			return this.tracker.GetUsage(this.Entity, measures);
		}

		public Task<IDictionary<TCountType, ICount<TEntityType, TCountType>>> GetUsageAsync(TCountType[] measures)
		{
			return this.tracker.GetUsageAsync(this.Entity, measures);
		}

		public IEnumerable<ICount<TEntityType, TCountType>> GetAllUsages()
		{
			return this.tracker.GetAllUsages(this.Entity);
		}

		public Task<IEnumerable<ICount<TEntityType, TCountType>>> GetAllUsagesAsync()
		{
			return this.tracker.GetAllUsagesAsync(this.Entity);
		}

		private readonly ICountTracker<TEntityType, TCountType> tracker;
	}
}
