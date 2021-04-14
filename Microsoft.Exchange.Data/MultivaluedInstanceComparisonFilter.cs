using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class MultivaluedInstanceComparisonFilter : ComparisonFilter
	{
		public MultivaluedInstanceComparisonFilter(ComparisonOperator comparisonOperator, PropertyDefinition property, object propertyValue) : base(comparisonOperator, property, propertyValue)
		{
		}
	}
}
