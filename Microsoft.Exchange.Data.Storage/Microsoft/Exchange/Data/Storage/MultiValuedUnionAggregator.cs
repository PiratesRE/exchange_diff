using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MultiValuedUnionAggregator<T> : IAggregatorRule
	{
		public MultiValuedUnionAggregator(PropertyDefinition propertyDefinition)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			this.propertyDefinition = propertyDefinition;
			if (!typeof(ICollection<T>).GetTypeInfo().IsAssignableFrom(this.propertyDefinition.Type.GetTypeInfo()))
			{
				throw new ArgumentException("propertyDefinition");
			}
		}

		public void BeginAggregation()
		{
			this.result.Clear();
			this.propertyBags.Clear();
		}

		public void EndAggregation()
		{
			Set<T> set = new Set<T>();
			for (int i = 0; i < this.propertyBags.Count; i++)
			{
				object obj = this.propertyBags[i].TryGetProperty(this.propertyDefinition);
				ICollection<T> collection = obj as ICollection<T>;
				if (collection != null)
				{
					foreach (T t in collection)
					{
						if (!set.Contains(t))
						{
							this.result.Add(t);
							set.Add(t);
						}
					}
				}
			}
		}

		public void AddToAggregation(IStorePropertyBag propertyBag)
		{
			this.propertyBags.Add(propertyBag);
		}

		public List<T> Result
		{
			get
			{
				return this.result;
			}
		}

		private List<T> result = new List<T>();

		private List<IStorePropertyBag> propertyBags = new List<IStorePropertyBag>();

		private PropertyDefinition propertyDefinition;
	}
}
