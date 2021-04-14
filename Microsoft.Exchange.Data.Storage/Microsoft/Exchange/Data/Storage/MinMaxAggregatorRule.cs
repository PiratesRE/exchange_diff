using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MinMaxAggregatorRule<T> : IAggregatorRule where T : IComparable<T>
	{
		public MinMaxAggregatorRule(PropertyDefinition propertyDefinition, bool isMin, T defaultValue)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Util.ThrowOnMismatchType<T>(propertyDefinition, "propertyDefinition");
			this.propertyDefinition = propertyDefinition;
			this.isMinCompare = isMin;
			this.result = defaultValue;
			this.defaultValue = defaultValue;
		}

		public void BeginAggregation()
		{
			this.aggregatorInitialized = false;
		}

		public void EndAggregation()
		{
			if (!this.aggregatorInitialized)
			{
				this.result = this.defaultValue;
			}
		}

		public void AddToAggregation(IStorePropertyBag propertyBag)
		{
			object obj = propertyBag.TryGetProperty(this.propertyDefinition);
			if (obj is T)
			{
				T other = (T)((object)obj);
				if (!this.aggregatorInitialized)
				{
					this.result = other;
					this.aggregatorInitialized = true;
					return;
				}
				int num = this.result.CompareTo(other);
				if (num > 0 && this.isMinCompare)
				{
					this.result = other;
					this.aggregatorInitialized = true;
					return;
				}
				if (num < 0 && !this.isMinCompare)
				{
					this.result = other;
					this.aggregatorInitialized = true;
				}
			}
		}

		public T Result
		{
			get
			{
				return this.result;
			}
		}

		private PropertyDefinition propertyDefinition;

		private bool isMinCompare;

		private bool aggregatorInitialized;

		private T result;

		private T defaultValue;
	}
}
