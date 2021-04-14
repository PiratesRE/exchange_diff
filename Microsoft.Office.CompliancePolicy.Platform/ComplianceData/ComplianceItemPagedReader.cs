using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.ComplianceData
{
	public abstract class ComplianceItemPagedReader : IDisposable
	{
		public ComplianceItemPagedReader(int pageSize, QueryPredicate condition)
		{
			if (pageSize <= 0)
			{
				throw new ArgumentException("pageSize must be greater than 0");
			}
			this.PageSize = pageSize;
			this.Condition = condition;
		}

		public string Query { get; protected set; }

		public QueryPredicate Condition
		{
			get
			{
				return this.condition;
			}
			set
			{
				this.condition = value;
				this.Query = this.GenerateQuery();
			}
		}

		public int PageSize { get; private set; }

		public abstract string PageCookie { get; }

		public abstract IEnumerable<ComplianceItem> GetNextPage();

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected abstract string GenerateQuery();

		protected abstract void Dispose(bool isDisposing);

		private QueryPredicate condition;
	}
}
