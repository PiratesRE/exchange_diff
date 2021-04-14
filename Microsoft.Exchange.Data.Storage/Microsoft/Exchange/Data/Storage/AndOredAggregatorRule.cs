using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AndOredAggregatorRule : IAggregatorRule
	{
		internal AndOredAggregatorRule(PropertyDefinition propertyDefinition, bool isAndOperation, bool defaultValue)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Util.ThrowOnMismatchType<bool>(propertyDefinition, "propertyDefinition");
			this.propertyDefinition = propertyDefinition;
			this.isAndOperation = isAndOperation;
			this.defaultValue = defaultValue;
			this.result = defaultValue;
		}

		public void BeginAggregation()
		{
			this.isInitialized = false;
		}

		public void EndAggregation()
		{
			if (!this.isInitialized)
			{
				this.result = this.defaultValue;
			}
		}

		public void AddToAggregation(IStorePropertyBag propertyBag)
		{
			object obj = propertyBag.TryGetProperty(this.propertyDefinition);
			if (obj is bool)
			{
				bool flag = (bool)obj;
				if (this.isInitialized)
				{
					if (this.isAndOperation)
					{
						this.result = (this.result && flag);
						return;
					}
					this.result = (this.result || flag);
					return;
				}
				else
				{
					this.result = flag;
					this.isInitialized = true;
				}
			}
		}

		public bool Result
		{
			get
			{
				return this.result;
			}
		}

		private PropertyDefinition propertyDefinition;

		private bool isAndOperation;

		private bool isInitialized;

		private bool defaultValue;

		private bool result;
	}
}
