using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NumberedRecurrenceRange : RecurrenceRange
	{
		public int NumberOfOccurrences
		{
			get
			{
				return this.numberOfOccurrences;
			}
			private set
			{
				if (!NumberedRecurrenceRange.IsValidNumberedRecurrenceRange(value))
				{
					throw new ArgumentException(ServerStrings.ExInvalidNumberOfOccurrences, "NumberOfOccurrences");
				}
				this.numberOfOccurrences = value;
			}
		}

		public NumberedRecurrenceRange(ExDateTime startDate, int numberOfOccurrences)
		{
			this.StartDate = startDate;
			this.NumberOfOccurrences = numberOfOccurrences;
		}

		public override bool Equals(RecurrenceRange value)
		{
			if (!(value is NumberedRecurrenceRange))
			{
				return false;
			}
			NumberedRecurrenceRange numberedRecurrenceRange = (NumberedRecurrenceRange)value;
			return numberedRecurrenceRange.NumberOfOccurrences == this.numberOfOccurrences && base.Equals(value);
		}

		public override string ToString()
		{
			return string.Format("Starts {0}, occurs {1} times", this.StartDate.ToString(DateTimeFormatInfo.InvariantInfo), this.NumberOfOccurrences);
		}

		internal static bool IsValidNumberedRecurrenceRange(int numberedOccurrence)
		{
			return numberedOccurrence >= 1 && numberedOccurrence <= StorageLimits.Instance.RecurrenceMaximumNumberedOccurrences;
		}

		private int numberOfOccurrences;
	}
}
