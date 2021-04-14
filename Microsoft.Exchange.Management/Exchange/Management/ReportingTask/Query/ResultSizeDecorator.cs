using System;
using System.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	internal class ResultSizeDecorator<TReportObject> : QueryDecorator<TReportObject> where TReportObject : class
	{
		public ResultSizeDecorator(ITaskContext taskContext) : base(taskContext)
		{
			this.ResultSize = null;
		}

		public Unlimited<int>? ResultSize { get; set; }

		public Unlimited<int> DefaultResultSize
		{
			get
			{
				return this.defaultResultSize;
			}
			set
			{
				this.defaultResultSize = value;
			}
		}

		public Unlimited<int> TargetResultSize
		{
			get
			{
				Unlimited<int>? resultSize = this.ResultSize;
				if (resultSize == null)
				{
					return this.DefaultResultSize;
				}
				return resultSize.GetValueOrDefault();
			}
		}

		public override QueryOrder QueryOrder
		{
			get
			{
				return QueryOrder.Top;
			}
		}

		public bool IsResultSizeReached(long totalCount)
		{
			return this.ResultSize != null && !this.ResultSize.Value.IsUnlimited && (long)this.ResultSize.Value.Value < totalCount;
		}

		public bool IsDefaultResultSizeReached(long totalCount)
		{
			return this.ResultSize == null && !this.DefaultResultSize.IsUnlimited && (long)this.DefaultResultSize.Value < totalCount;
		}

		public bool IsTargetResultSizeReached(long totalCount)
		{
			return !this.TargetResultSize.IsUnlimited && (long)this.TargetResultSize.Value < totalCount;
		}

		public override IQueryable<TReportObject> GetQuery(IQueryable<TReportObject> query)
		{
			if (!this.TargetResultSize.IsUnlimited)
			{
				int num = this.TargetResultSize.Value;
				if (num > 0)
				{
					num++;
				}
				query = query.Take(num);
			}
			return query;
		}

		public override void Validate()
		{
			if (!this.TargetResultSize.IsUnlimited && this.TargetResultSize.Value <= 0)
			{
				base.TaskContext.WriteError(new ReportingException(Strings.ErrorInvalidResultSize), ExchangeErrorCategory.Client, null);
			}
		}

		private Unlimited<int> defaultResultSize = new Unlimited<int>(DataMart.Instance.DefaultReportResultSize);
	}
}
