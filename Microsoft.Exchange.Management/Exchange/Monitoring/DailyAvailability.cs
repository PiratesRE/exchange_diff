using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class DailyAvailability : IComparable
	{
		protected DailyAvailability(DateTime date)
		{
			this.date = date;
		}

		public DateTime Date
		{
			get
			{
				return this.date;
			}
		}

		public double AvailabilityPercentage
		{
			get
			{
				return this.availabilityPercentage;
			}
			internal set
			{
				this.availabilityPercentage = value;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj is DailyAvailability)
			{
				DailyAvailability dailyAvailability = (DailyAvailability)obj;
				return this.Date.CompareTo(dailyAvailability.Date);
			}
			throw new ArgumentException(Strings.ExceptionIncomparableType(obj.GetType()), "obj");
		}

		public override bool Equals(object obj)
		{
			return this.CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			return this.Date.GetHashCode();
		}

		private readonly DateTime date;

		private double availabilityPercentage;
	}
}
