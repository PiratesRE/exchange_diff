using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidDateValueException : ReportingException
	{
		public InvalidDateValueException(DateTime date, DateTime minDate, DateTime maxDate) : base(Strings.InvalidDateValueException(date, minDate, maxDate))
		{
			this.date = date;
			this.minDate = minDate;
			this.maxDate = maxDate;
		}

		public InvalidDateValueException(DateTime date, DateTime minDate, DateTime maxDate, Exception innerException) : base(Strings.InvalidDateValueException(date, minDate, maxDate), innerException)
		{
			this.date = date;
			this.minDate = minDate;
			this.maxDate = maxDate;
		}

		protected InvalidDateValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.date = (DateTime)info.GetValue("date", typeof(DateTime));
			this.minDate = (DateTime)info.GetValue("minDate", typeof(DateTime));
			this.maxDate = (DateTime)info.GetValue("maxDate", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("date", this.date);
			info.AddValue("minDate", this.minDate);
			info.AddValue("maxDate", this.maxDate);
		}

		public DateTime Date
		{
			get
			{
				return this.date;
			}
		}

		public DateTime MinDate
		{
			get
			{
				return this.minDate;
			}
		}

		public DateTime MaxDate
		{
			get
			{
				return this.maxDate;
			}
		}

		private readonly DateTime date;

		private readonly DateTime minDate;

		private readonly DateTime maxDate;
	}
}
