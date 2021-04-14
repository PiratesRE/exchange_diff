using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	internal class DateDecorator<TReportObject> : QueryDecorator<TReportObject> where TReportObject : class, IDateColumn
	{
		public DateDecorator(ITaskContext taskContext) : base(taskContext)
		{
		}

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public override IQueryable<TReportObject> GetQuery(IQueryable<TReportObject> query)
		{
			if (this.StartDate != null)
			{
				query = from report in query
				where report.Date >= this.StartDate.Value
				select report;
			}
			if (this.EndDate != null)
			{
				query = from report in query
				where report.Date <= this.EndDate.Value
				select report;
			}
			return query;
		}

		public override void Validate()
		{
			if (this.StartDate != null && (this.StartDate < DateDecorator<TReportObject>.MinDateTime || this.StartDate > DateDecorator<TReportObject>.MaxDateTime))
			{
				base.TaskContext.WriteError(new InvalidDateValueException(this.StartDate.Value, DateDecorator<TReportObject>.MinDateTime, DateDecorator<TReportObject>.MaxDateTime), ExchangeErrorCategory.Client, null);
			}
			if (this.EndDate != null && (this.EndDate < DateDecorator<TReportObject>.MinDateTime || this.EndDate > DateDecorator<TReportObject>.MaxDateTime))
			{
				base.TaskContext.WriteError(new InvalidDateValueException(this.EndDate.Value, DateDecorator<TReportObject>.MinDateTime, DateDecorator<TReportObject>.MaxDateTime), ExchangeErrorCategory.Client, null);
			}
			if (this.StartDate != null && this.EndDate != null && this.StartDate > this.EndDate)
			{
				base.TaskContext.WriteError(new InvalidDateParameterException(this.StartDate.Value, this.EndDate.Value), ExchangeErrorCategory.Client, null);
			}
		}

		public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);

		public static readonly DateTime MaxDateTime = new DateTime(9999, 12, 31);
	}
}
