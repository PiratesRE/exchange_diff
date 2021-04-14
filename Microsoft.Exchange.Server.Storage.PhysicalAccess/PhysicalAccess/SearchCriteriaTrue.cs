using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaTrue : SearchCriteria
	{
		public override bool Evaluate(ITWIR twir, CompareInfo compareInfo)
		{
			return true;
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
			return Factory.CreateSearchCriteriaFalse();
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("TRUE");
		}

		public override string ToString()
		{
			return "TRUE";
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			return other is SearchCriteriaTrue;
		}
	}
}
