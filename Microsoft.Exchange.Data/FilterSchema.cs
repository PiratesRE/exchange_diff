using System;

namespace Microsoft.Exchange.Data
{
	internal abstract class FilterSchema
	{
		public abstract string And { get; }

		public abstract string Or { get; }

		public abstract string Not { get; }

		public abstract string Like { get; }

		public abstract string QuotationMark { get; }

		public abstract bool SupportQuotedPrefix { get; }

		public abstract string GetRelationalOperator(ComparisonOperator op);

		public abstract string EscapeStringValue(object o);

		public abstract string GetExistsFilter(ExistsFilter filter);

		public abstract string GetFalseFilter();

		public abstract string GetPropertyName(string propertyName);
	}
}
