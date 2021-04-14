using System;
using System.Linq;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	public class QueryDecorator<TReportObject> where TReportObject : class
	{
		public QueryDecorator(ITaskContext taskContext)
		{
			this.TaskContext = taskContext;
		}

		public ITaskContext TaskContext { get; private set; }

		public bool IsPipeline { get; set; }

		public bool IsEnforced { get; set; }

		public virtual QueryOrder QueryOrder
		{
			get
			{
				return QueryOrder.Where;
			}
		}

		public virtual IQueryable<TReportObject> GetQuery(IQueryable<TReportObject> query)
		{
			return query;
		}

		public virtual void Validate()
		{
		}
	}
}
