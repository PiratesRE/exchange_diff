using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal sealed class AmbiguousNameResolutionFilter : QueryFilter
	{
		public AmbiguousNameResolutionFilter(string valueToMatch)
		{
			if (valueToMatch == null)
			{
				throw new ArgumentNullException("valueToMatch");
			}
			this.valueToMatch = valueToMatch;
		}

		public string ValueToMatch
		{
			get
			{
				return this.valueToMatch;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(ANR=");
			sb.Append(this.valueToMatch);
			sb.Append(")");
		}

		public override bool Equals(object obj)
		{
			AmbiguousNameResolutionFilter ambiguousNameResolutionFilter = obj as AmbiguousNameResolutionFilter;
			return ambiguousNameResolutionFilter != null && ambiguousNameResolutionFilter.GetType() == base.GetType() && this.valueToMatch.Equals(ambiguousNameResolutionFilter.valueToMatch);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.valueToMatch.GetHashCode();
		}

		private readonly string valueToMatch;
	}
}
