using System;

namespace Microsoft.Exchange.Data
{
	internal class MonadFilterSchema : FilterSchema
	{
		public override string And
		{
			get
			{
				return "-and";
			}
		}

		public override string Or
		{
			get
			{
				return "-or";
			}
		}

		public override string Not
		{
			get
			{
				return "-not";
			}
		}

		public override string Like
		{
			get
			{
				return " -like ";
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
				return " -eq ";
			case ComparisonOperator.NotEqual:
				return " -ne ";
			case ComparisonOperator.LessThan:
				return " -lt ";
			case ComparisonOperator.LessThanOrEqual:
				return " -le ";
			case ComparisonOperator.GreaterThan:
				return " -gt ";
			case ComparisonOperator.GreaterThanOrEqual:
				return " -ge ";
			case ComparisonOperator.Like:
				return " -like ";
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
			return string.Format("{0} -ne $null", filter.Property.Name);
		}

		public override string GetFalseFilter()
		{
			return "$False";
		}

		public override string GetPropertyName(string propertyName)
		{
			return propertyName;
		}
	}
}
