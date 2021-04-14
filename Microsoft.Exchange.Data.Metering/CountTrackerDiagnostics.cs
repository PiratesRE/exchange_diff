using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Metering
{
	internal class CountTrackerDiagnostics<TEntityType, TCountType> : ICountTrackerDiagnostics<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public void SubscribeTo(MeteringEvent evt, ICountedEntity<TEntityType> entity, Action<ICountedEntity<TEntityType>> entityDelegate)
		{
			this.AddSubscriber(evt, new CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber(entityDelegate, entity));
		}

		public void SubscribeTo(MeteringEvent evt, TCountType? measure, Action<TCountType> measureDelegate)
		{
			this.AddSubscriber(evt, new CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber(measureDelegate, measure));
		}

		public void EntityAdded(ICountedEntity<TEntityType> entity)
		{
			this.RaiseEvent(MeteringEvent.EntityAdded, entity);
		}

		public void EntityRemoved(ICountedEntity<TEntityType> entity)
		{
			this.RaiseEvent(MeteringEvent.EntityRemoved, entity);
		}

		public void MeasureAdded(TCountType measure)
		{
			this.RaiseEvent(MeteringEvent.MeasureAdded, measure);
		}

		public void MeasureRemoved(TCountType measure)
		{
			this.RaiseEvent(MeteringEvent.MeasureRemoved, measure);
		}

		public void MeasurePromoted(TCountType measure)
		{
			this.RaiseEvent(MeteringEvent.MeasurePromoted, measure);
		}

		public void MeasureExpired(TCountType measure)
		{
			this.RaiseEvent(MeteringEvent.MeasureExpired, measure);
		}

		private void AddSubscriber(MeteringEvent evt, CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber subscriber)
		{
			lock (this.syncObject)
			{
				List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber> list;
				if (!this.listeners.TryGetValue(evt, out list))
				{
					list = new List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber>();
					this.listeners.Add(evt, list);
				}
				list.Add(subscriber);
			}
		}

		private void RaiseEvent(MeteringEvent evt, ICountedEntity<TEntityType> filter)
		{
			List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber> collection;
			if (this.TryGetSubscriber(evt, out collection))
			{
				List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber> list = new List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber>(collection);
				foreach (CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber subscriber in list)
				{
					subscriber.RaiseEvent(filter);
				}
			}
		}

		private void RaiseEvent(MeteringEvent evt, TCountType filter)
		{
			List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber> collection;
			if (this.TryGetSubscriber(evt, out collection))
			{
				List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber> list = new List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber>(collection);
				foreach (CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber subscriber in list)
				{
					subscriber.RaiseEvent(filter);
				}
			}
		}

		private bool TryGetSubscriber(MeteringEvent evt, out List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber> listener)
		{
			bool result;
			lock (this.syncObject)
			{
				result = this.listeners.TryGetValue(evt, out listener);
			}
			return result;
		}

		private Dictionary<MeteringEvent, List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber>> listeners = new Dictionary<MeteringEvent, List<CountTrackerDiagnostics<TEntityType, TCountType>.Subscriber>>();

		private object syncObject = new object();

		private class Subscriber
		{
			public Subscriber(Action<TCountType> measureDelegate, TCountType? measure)
			{
				this.measureDelegate = measureDelegate;
				this.measure = measure;
				this.measureBased = true;
			}

			public Subscriber(Action<ICountedEntity<TEntityType>> entityDelegate, ICountedEntity<TEntityType> entity)
			{
				this.entityDelegate = entityDelegate;
				this.entity = entity;
				this.measureBased = false;
			}

			public void RaiseEvent(ICountedEntity<TEntityType> countedEntity)
			{
				if (!this.measureBased && (this.entity == null || this.entity.Equals(countedEntity)))
				{
					this.entityDelegate(countedEntity);
				}
			}

			public void RaiseEvent(TCountType count)
			{
				if (this.measureBased)
				{
					if (this.measure != null)
					{
						TCountType value = this.measure.Value;
						if (!value.Equals(count))
						{
							return;
						}
					}
					this.measureDelegate(count);
				}
			}

			private readonly Action<ICountedEntity<TEntityType>> entityDelegate;

			private readonly ICountedEntity<TEntityType> entity;

			private readonly Action<TCountType> measureDelegate;

			private readonly TCountType? measure;

			private readonly bool measureBased;
		}
	}
}
