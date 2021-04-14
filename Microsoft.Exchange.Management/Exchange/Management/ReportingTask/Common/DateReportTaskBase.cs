using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	public abstract class DateReportTaskBase<TReportObject> : ReportingTaskBase<TReportObject> where TReportObject : ReportObject, IDateColumn
	{
		protected DateReportTaskBase()
		{
			this.dateDecorator = new DateDecorator<TReportObject>(base.TaskContext);
			base.AddQueryDecorator(this.dateDecorator);
			base.AddQueryDecorator(new OrderByDecorator<TReportObject, DateTime>(base.TaskContext)
			{
				OrderBy = ((TReportObject report) => report.Date),
				Descending = true
			});
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDate
		{
			get
			{
				return this.dateDecorator.StartDate;
			}
			set
			{
				this.dateDecorator.StartDate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? EndDate
		{
			get
			{
				return this.dateDecorator.EndDate;
			}
			set
			{
				this.dateDecorator.EndDate = value;
			}
		}

		private readonly DateDecorator<TReportObject> dateDecorator;
	}
}
