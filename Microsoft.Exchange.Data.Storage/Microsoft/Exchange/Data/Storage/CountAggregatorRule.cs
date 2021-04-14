using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CountAggregatorRule<T> : IAggregatorRule
	{
		internal CountAggregatorRule(PropertyDefinition propertyDefinition, T value)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Util.ThrowOnMismatchType<T>(propertyDefinition, "propertyDefinition");
			this.propertyDefinition = propertyDefinition;
			this.compareValue = value;
			this.result = 0;
		}

		public void BeginAggregation()
		{
			this.result = 0;
		}

		public void EndAggregation()
		{
		}

		public void AddToAggregation(IStorePropertyBag propertyBag)
		{
			if (object.Equals(this.compareValue, propertyBag.TryGetProperty(this.propertyDefinition)))
			{
				this.result++;
			}
		}

		public int Result
		{
			get
			{
				return this.result;
			}
		}

		private int result;

		private PropertyDefinition propertyDefinition;

		private T compareValue;
	}
}
