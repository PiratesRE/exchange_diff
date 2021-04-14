using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaNot : SearchCriteria
	{
		protected SearchCriteriaNot(SearchCriteria criteria)
		{
			this.criteria = criteria;
		}

		public SearchCriteria Criteria
		{
			get
			{
				return this.criteria;
			}
		}

		public override bool Evaluate(ITWIR twir, CompareInfo compareInfo)
		{
			return !this.criteria.Evaluate(twir, compareInfo);
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state, bool explodeCompositeColumns)
		{
			this.criteria.EnumerateColumns(callback, state, explodeCompositeColumns);
		}

		protected override SearchCriteria InspectAndFixChildren(SearchCriteria.InspectAndFixCriteriaDelegate callback, CompareInfo compareInfo, bool simplifyNegation)
		{
			SearchCriteria searchCriteria = this.criteria.InspectAndFix(callback, compareInfo, simplifyNegation);
			if (simplifyNegation)
			{
				if (!object.ReferenceEquals(searchCriteria, this.criteria) || searchCriteria.CanBeNegated)
				{
					return searchCriteria.Negate();
				}
			}
			else if (!object.ReferenceEquals(searchCriteria, this.criteria))
			{
				return Factory.CreateSearchCriteriaNot(searchCriteria);
			}
			return this;
		}

		internal override bool CanBeNegated
		{
			get
			{
				return true;
			}
		}

		internal override SearchCriteria Negate()
		{
			return this.criteria;
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("NOT(");
			this.criteria.AppendToString(sb, formatOptions);
			sb.Append(")");
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			SearchCriteriaNot searchCriteriaNot = other as SearchCriteriaNot;
			return searchCriteriaNot != null && (object.ReferenceEquals(this.criteria, searchCriteriaNot.criteria) || (this.criteria != null && this.criteria.Equals(searchCriteriaNot.criteria)));
		}

		private readonly SearchCriteria criteria;
	}
}
