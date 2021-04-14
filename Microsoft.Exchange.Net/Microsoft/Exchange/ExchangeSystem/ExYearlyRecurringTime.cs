using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	public abstract class ExYearlyRecurringTime : IComparable, IComparable<ExYearlyRecurringTime>
	{
		public ExYearlyRecurringTime()
		{
		}

		public abstract DateTime GetInstance(int year);

		public int CompareTo(ExYearlyRecurringTime value)
		{
			return Math.Sign(this.GetSortIndex() - value.GetSortIndex());
		}

		public int CompareTo(object value)
		{
			ExYearlyRecurringTime exYearlyRecurringTime = value as ExYearlyRecurringTime;
			if (value != null && exYearlyRecurringTime == null)
			{
				throw new ArgumentException();
			}
			return this.CompareTo(exYearlyRecurringTime);
		}

		internal int SortIndex
		{
			get
			{
				return this.GetSortIndex();
			}
		}

		protected abstract int GetSortIndex();

		internal abstract void Validate();

		public int Month
		{
			get
			{
				return this.month;
			}
			set
			{
				this.month = value;
			}
		}

		public int Hour
		{
			get
			{
				return this.hour;
			}
			set
			{
				this.hour = value;
			}
		}

		public int Minute
		{
			get
			{
				return this.minute;
			}
			set
			{
				this.minute = value;
			}
		}

		public int Second
		{
			get
			{
				return this.second;
			}
			set
			{
				this.second = value;
			}
		}

		public int Milliseconds
		{
			get
			{
				return this.milliseconds;
			}
			set
			{
				this.milliseconds = value;
			}
		}

		private int month;

		private int hour;

		private int minute;

		private int second;

		private int milliseconds;
	}
}
