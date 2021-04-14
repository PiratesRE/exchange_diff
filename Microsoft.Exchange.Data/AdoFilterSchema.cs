using System;

namespace Microsoft.Exchange.Data
{
	internal class AdoFilterSchema : FilterSchema
	{
		public override string And
		{
			get
			{
				return "AND";
			}
		}

		public override string Or
		{
			get
			{
				return "OR";
			}
		}

		public override string Not
		{
			get
			{
				return "NOT";
			}
		}

		public override string Like
		{
			get
			{
				return " LIKE ";
			}
		}

		public override string QuotationMark
		{
			get
			{
				return "'";
			}
		}

		public override bool SupportQuotedPrefix
		{
			get
			{
				return true;
			}
		}

		public override string GetRelationalOperator(ComparisonOperator op)
		{
			switch (op)
			{
			case ComparisonOperator.Equal:
				return " = ";
			case ComparisonOperator.NotEqual:
				return " <> ";
			case ComparisonOperator.LessThan:
				return " < ";
			case ComparisonOperator.LessThanOrEqual:
				return " <= ";
			case ComparisonOperator.GreaterThan:
				return " > ";
			case ComparisonOperator.GreaterThanOrEqual:
				return " >= ";
			case ComparisonOperator.Like:
				return " LIKE ";
			default:
				return null;
			}
		}

		public override string EscapeStringValue(object o)
		{
			if (o == null)
			{
				return null;
			}
			IDnFormattable dnFormattable = o as IDnFormattable;
			if (dnFormattable != null)
			{
				return dnFormattable.ToDNString().Replace("'", "''");
			}
			return o.ToString().Replace("'", "''");
		}

		public override string GetExistsFilter(ExistsFilter filter)
		{
			return string.Format("(NOT({0} IS NULL))", filter.Property.Name);
		}

		public override string GetFalseFilter()
		{
			return "FALSE";
		}

		public override string GetPropertyName(string propertyName)
		{
			return propertyName;
		}
	}
}
