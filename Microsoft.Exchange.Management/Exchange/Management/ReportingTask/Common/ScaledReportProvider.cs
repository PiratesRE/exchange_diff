using System;
using System.Linq;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class ScaledReportProvider<TReportObject> : ReportProvider<TReportObject> where TReportObject : ReportObject
	{
		public ScaledReportProvider(ITaskContext taskContext, IReportContextFactory reportContextFactory) : base(taskContext, reportContextFactory)
		{
		}

		protected override IQueryable<TReportObject> GetReportQuery(IReportContext reportContext)
		{
			IQueryable<TReportObject> queryable = base.GetReportQuery(reportContext);
			if (!DataMart.Instance.IsTableFunctionQueryDisabled)
			{
				base.LogSqlStatement(reportContext, queryable, 2);
				queryable = reportContext.GetScaledQuery<TReportObject>(queryable);
				QueryDecorator<TReportObject> queryDecorator = this.queryDecorators.Single((QueryDecorator<TReportObject> decorator) => decorator is OrderByDecorator<TReportObject, DateTime>);
				if (queryDecorator != null)
				{
					queryable = queryDecorator.GetQuery(queryable);
				}
			}
			return queryable;
		}
	}
}
