using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DateDuration : ICloneable
	{
		public DateDuration(DateTime? startDate, DateTime? endDate)
		{
			if (startDate != null)
			{
				startDate = new DateTime?((startDate.Value.Kind == DateTimeKind.Unspecified) ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Local) : startDate.Value.Date);
			}
			if (endDate != null)
			{
				endDate = new DateTime?((endDate.Value.Kind == DateTimeKind.Unspecified) ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Local) : endDate.Value.Date);
				endDate = new DateTime?(endDate.Value.Date + new TimeSpan(23, 59, 59));
			}
			if (startDate != null && endDate != null && endDate.Value.CompareTo(startDate.Value) < 0)
			{
				throw new StrongTypeFormatException(Strings.DateDurationOutOfRangeErrorMessage, string.Empty);
			}
			this.startDate = startDate;
			this.endDate = endDate;
		}

		public bool IsWithDate()
		{
			return this.StartDate != null || this.EndDate != null;
		}

		public DateTime? StartDate
		{
			get
			{
				return this.startDate;
			}
		}

		public DateTime? EndDate
		{
			get
			{
				return this.endDate;
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			if (this.StartDate != null)
			{
				text = string.Format("{0} {1}", Strings.DateDurationAfter, this.StartDate.Value.ToLongDateString());
			}
			string text2 = string.Empty;
			if (this.EndDate != null)
			{
				text2 = string.Format("{0} {1}", Strings.DateDurationBefore, this.EndDate.Value.ToLongDateString());
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				return string.Format("{0} {1} {2}", text, (this.StartDate == null) ? string.Empty : Strings.DateDurationAnd, text2);
			}
			return text + text2;
		}

		public object Clone()
		{
			return new DateDuration(this.StartDate, this.EndDate);
		}

		private DateTime? startDate;

		private DateTime? endDate;
	}
}
