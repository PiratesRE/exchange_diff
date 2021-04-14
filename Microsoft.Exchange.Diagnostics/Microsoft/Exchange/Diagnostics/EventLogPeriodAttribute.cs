using System;

namespace Microsoft.Exchange.Diagnostics
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class EventLogPeriodAttribute : Attribute
	{
		public EventLogPeriodAttribute()
		{
			this.period = string.Empty;
		}

		public string Period
		{
			get
			{
				return this.period;
			}
			set
			{
				this.period = value;
			}
		}

		private string period;
	}
}
