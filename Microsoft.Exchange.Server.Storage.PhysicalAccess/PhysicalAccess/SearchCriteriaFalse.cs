using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaFalse : SearchCriteria
	{
		public override bool Evaluate(ITWIR twir, CompareInfo compareInfo)
		{
			return false;
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
			return Factory.CreateSearchCriteriaTrue();
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("FALSE");
		}

		public override string ToString()
		{
			return "FALSE";
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			return other is SearchCriteriaFalse;
		}
	}
}
