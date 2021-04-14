using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	internal class WhereDecorator<TReportObject> : QueryDecorator<TReportObject> where TReportObject : class
	{
		public WhereDecorator(ITaskContext taskContext) : base(taskContext)
		{
		}

		public Expression<Func<TReportObject, bool>> Predicate { get; set; }

		public override IQueryable<TReportObject> GetQuery(IQueryable<TReportObject> query)
		{
			if (this.Predicate == null)
			{
				return query;
			}
			query = query.Where(this.Predicate);
			return query;
		}
	}
}
