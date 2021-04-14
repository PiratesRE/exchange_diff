using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Serialization;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public abstract class RecurrencePattern
	{
		public abstract RecurrencePatternType Type { get; }

		public int Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (value > RecurrencePattern.MaxInterval)
				{
					throw new ValueOutOfRangeException("Interval", value);
				}
				this.interval = value;
			}
		}

		public static readonly int MaxInterval = StorageLimits.Instance.RecurrenceMaximumInterval;

		private int interval;
	}
}
