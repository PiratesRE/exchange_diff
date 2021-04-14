using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaNear : SearchCriteria
	{
		protected SearchCriteriaNear(int distance, bool ordered, SearchCriteriaAnd criteria)
		{
			if (criteria == null || criteria.NestedCriteria == null)
			{
				throw new StoreException((LID)59728U, ErrorCodeValue.TooComplex, "NEAR requires a non-null AND criteria");
			}
			if (criteria.NestedCriteria.Length < 2)
			{
				throw new StoreException((LID)35152U, ErrorCodeValue.TooComplex, "NEAR requires at least two criteria");
			}
			bool foundRootAndCriteria = false;
			criteria.InspectAndFix(delegate(SearchCriteria criterion, CompareInfo compareInfo)
			{
				if (criterion is SearchCriteriaAnd && !foundRootAndCriteria)
				{
					foundRootAndCriteria = true;
					return criterion;
				}
				if (criterion is SearchCriteriaNear)
				{
					return null;
				}
				if (criterion is SearchCriteriaOr)
				{
					return criterion;
				}
				if (!(criterion is SearchCriteriaText))
				{
					throw new StoreException((LID)52904U, ErrorCodeValue.TooComplex, "NEAR only supports TEXT, NEAR and OR operators");
				}
				return null;
			}, null, false);
			this.distance = distance;
			this.ordered = ordered;
			this.criteria = criteria;
		}

		public int Distance
		{
			get
			{
				return this.distance;
			}
		}

		public bool Ordered
		{
			get
			{
				return this.ordered;
			}
		}

		public SearchCriteriaAnd Criteria
		{
			get
			{
				return this.criteria;
			}
		}

		public override bool Evaluate(ITWIR twir, CompareInfo compareInfo)
		{
			return this.criteria.Evaluate(twir, compareInfo);
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state, bool explodeCompositeColumns)
		{
			this.criteria.EnumerateColumns(callback, state, explodeCompositeColumns);
		}

		protected override SearchCriteria InspectAndFixChildren(SearchCriteria.InspectAndFixCriteriaDelegate callback, CompareInfo compareInfo, bool simplifyNegation)
		{
			SearchCriteria searchCriteria = this.criteria.InspectAndFix(callback, compareInfo, simplifyNegation);
			if (object.ReferenceEquals(searchCriteria, this.criteria))
			{
				return this;
			}
			if (searchCriteria is SearchCriteriaAnd)
			{
				return Factory.CreateSearchCriteriaNear(this.distance, this.ordered, (SearchCriteriaAnd)searchCriteria);
			}
			return searchCriteria;
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("NEAR(");
			for (int i = 0; i < this.criteria.NestedCriteria.Length; i++)
			{
				this.criteria.NestedCriteria[i].AppendToString(sb, formatOptions);
				sb.Append(", ");
			}
			sb.Append(this.distance);
			sb.Append(", ");
			sb.Append(this.ordered);
			sb.Append(")");
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			SearchCriteriaNear searchCriteriaNear = other as SearchCriteriaNear;
			return searchCriteriaNear != null && this.ordered == searchCriteriaNear.ordered && this.distance == searchCriteriaNear.distance && (object.ReferenceEquals(this.criteria, searchCriteriaNear.criteria) || (this.criteria != null && this.criteria.Equals(searchCriteriaNear.criteria)));
		}

		private readonly int distance;

		private readonly bool ordered;

		private readonly SearchCriteriaAnd criteria;
	}
}
