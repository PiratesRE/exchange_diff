using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnionAggregatorRule<T> : IAggregatorRule
	{
		public UnionAggregatorRule(PropertyDefinition propertyDefinition, params SortBy[] sortByFields)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Util.ThrowOnMismatchType<T>(propertyDefinition, "propertyDefinition");
			this.propertyDefinition = propertyDefinition;
			this.sortByFields = sortByFields;
		}

		public void BeginAggregation()
		{
			this.result.Clear();
			this.propertyBags.Clear();
		}

		public void EndAggregation()
		{
			if (this.sortByFields != null)
			{
				this.propertyBags.Sort(new Comparison<IStorePropertyBag>(this.ComparePropertyBags));
			}
			Set<T> set = new Set<T>();
			for (int i = 0; i < this.propertyBags.Count; i++)
			{
				object obj = this.propertyBags[i].TryGetProperty(this.propertyDefinition);
				if (obj is T)
				{
					T t = (T)((object)obj);
					if (!set.Contains(t))
					{
						this.result.Add(t);
						set.Add(t);
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

		private int ComparePropertyBags(IStorePropertyBag leftPropertyBag, IStorePropertyBag rightPropertyBag)
		{
			int i = 0;
			while (i < this.sortByFields.Length)
			{
				int num = Util.CompareValues(leftPropertyBag.TryGetProperty(this.sortByFields[i].ColumnDefinition), rightPropertyBag.TryGetProperty(this.sortByFields[i].ColumnDefinition));
				if (num != 0)
				{
					if (this.sortByFields[i].SortOrder == SortOrder.Ascending)
					{
						return num;
					}
					return -1 * num;
				}
				else
				{
					i++;
				}
			}
			return 0;
		}

		private SortBy[] sortByFields;

		private List<T> result = new List<T>();

		private List<IStorePropertyBag> propertyBags = new List<IStorePropertyBag>();

		private PropertyDefinition propertyDefinition;
	}
}
