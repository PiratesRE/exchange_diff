using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	internal class OrderByDecorator<TReportObject, TOrderByProperty> : QueryDecorator<TReportObject> where TReportObject : class
	{
		public OrderByDecorator(ITaskContext taskContext) : base(taskContext)
		{
		}

		public bool Descending { get; set; }

		public Expression<Func<TReportObject, TOrderByProperty>> OrderBy { get; set; }

		public override QueryOrder QueryOrder
		{
			get
			{
				return QueryOrder.OrderBy;
			}
		}

		public override IQueryable<TReportObject> GetQuery(IQueryable<TReportObject> query)
		{
			if (this.OrderBy == null)
			{
				return query;
			}
			if (this.Descending)
			{
				query = query.OrderByDescending(this.OrderBy);
			}
			else
			{
				query = query.OrderBy(this.OrderBy);
			}
			return query;
		}
	}
}
