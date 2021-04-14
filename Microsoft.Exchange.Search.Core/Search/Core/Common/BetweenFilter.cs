using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class BetweenFilter : AndFilter
	{
		public BetweenFilter(ComparisonFilter leftFilter, ComparisonFilter rightFilter) : base(new QueryFilter[]
		{
			leftFilter,
			rightFilter
		})
		{
			if (leftFilter.ComparisonOperator != ComparisonOperator.GreaterThan && leftFilter.ComparisonOperator != ComparisonOperator.GreaterThanOrEqual)
			{
				throw new ExArgumentException(string.Format("Left comparison of BetweenFilter must be GreaterThan or GreaterThanOrEqual. Actual:{0}", leftFilter.ComparisonOperator));
			}
			if (rightFilter.ComparisonOperator != ComparisonOperator.LessThan && rightFilter.ComparisonOperator != ComparisonOperator.LessThanOrEqual)
			{
				throw new ExArgumentException(string.Format("Right comparison of BetweenFilter must be LessThan or LessThanOrEqual. Actual:{0}", leftFilter.ComparisonOperator));
			}
			if (rightFilter.Property != leftFilter.Property)
			{
				throw new ExArgumentException(string.Format("Left filter property {0} doesn't match right filter property {1}", leftFilter.Property.Name, rightFilter.Property.Name));
			}
			this.Property = rightFilter.Property;
			this.Left = leftFilter;
			this.Right = rightFilter;
		}

		public PropertyDefinition Property { get; private set; }

		public ComparisonFilter Left { get; private set; }

		public ComparisonFilter Right { get; private set; }
	}
}
