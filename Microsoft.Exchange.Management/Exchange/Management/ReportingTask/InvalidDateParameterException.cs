using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidDateParameterException : ReportingException
	{
		public InvalidDateParameterException(DateTime startDate, DateTime endDate) : base(Strings.InvalidDateParameterException(startDate, endDate))
		{
			this.startDate = startDate;
			this.endDate = endDate;
		}

		public InvalidDateParameterException(DateTime startDate, DateTime endDate, Exception innerException) : base(Strings.InvalidDateParameterException(startDate, endDate), innerException)
		{
			this.startDate = startDate;
			this.endDate = endDate;
		}

		protected InvalidDateParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.startDate = (DateTime)info.GetValue("startDate", typeof(DateTime));
			this.endDate = (DateTime)info.GetValue("endDate", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("startDate", this.startDate);
			info.AddValue("endDate", this.endDate);
		}

		public DateTime StartDate
		{
			get
			{
				return this.startDate;
			}
		}

		public DateTime EndDate
		{
			get
			{
				return this.endDate;
			}
		}

		private readonly DateTime startDate;

		private readonly DateTime endDate;
	}
}
