using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class CompositeFilter : QueryFilter
	{
		public CompositeFilter(bool ignoreWhenVerifyingMaxDepth, params QueryFilter[] filters)
		{
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			for (int i = 0; i < filters.Length; i++)
			{
				if (filters[i] == null)
				{
					throw new ArgumentNullException("filters[" + i + "]");
				}
			}
			this.filters = filters;
			this.ignoreWhenVerifyingMaxDepth = ignoreWhenVerifyingMaxDepth;
		}

		public ReadOnlyCollection<QueryFilter> Filters
		{
			get
			{
				if (this.filterCollection == null)
				{
					this.filterCollection = new ReadOnlyCollection<QueryFilter>(this.filters);
				}
				return this.filterCollection;
			}
		}

		public int FilterCount
		{
			get
			{
				return this.filters.Length;
			}
		}

		public override bool Equals(object obj)
		{
			CompositeFilter compositeFilter = obj as CompositeFilter;
			if (compositeFilter != null && compositeFilter.filters.Length == this.filters.Length && compositeFilter.GetType() == base.GetType() && compositeFilter.GetHashCode() == this.GetHashCode())
			{
				for (int i = 0; i < this.filters.Length; i++)
				{
					if (Array.IndexOf<QueryFilter>(compositeFilter.filters, this.filters[i]) == -1)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		protected QueryFilter[] CloneFiltersWithPropertyReplacement(IDictionary<PropertyDefinition, PropertyDefinition> propertyMap)
		{
			QueryFilter[] array = new QueryFilter[this.filters.Length];
			for (int i = 0; i < this.filters.Length; i++)
			{
				array[i] = this.filters[i].CloneWithPropertyReplacement(propertyMap);
			}
			return array;
		}

		public override int GetHashCode()
		{
			if (this.hashCode == null)
			{
				this.hashCode = new int?(base.GetType().GetHashCode());
				for (int i = 0; i < this.filters.Length; i++)
				{
					this.hashCode ^= this.filters[i].GetHashCode();
				}
			}
			return this.hashCode.Value;
		}

		public override IEnumerable<string> Keywords()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.filters.Length; i++)
			{
				list.AddRange(this.filters[i].Keywords());
			}
			return list;
		}

		internal override IEnumerable<PropertyDefinition> FilterProperties()
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			for (int i = 0; i < this.filters.Length; i++)
			{
				foreach (PropertyDefinition item in this.filters[i].FilterProperties())
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		public override string PropertyName
		{
			get
			{
				return this.Filters[0].PropertyName;
			}
		}

		protected override int GetSize()
		{
			int num = base.GetSize();
			int num2 = this.filters.Length;
			for (int i = 0; i < num2; i++)
			{
				num += this.filters[i].Size;
			}
			return num;
		}

		public bool IgnoreWhenVerifyingMaxDepth
		{
			get
			{
				return this.ignoreWhenVerifyingMaxDepth;
			}
		}

		private int? hashCode;

		private ReadOnlyCollection<QueryFilter> filterCollection;

		private bool ignoreWhenVerifyingMaxDepth;

		protected readonly QueryFilter[] filters;
	}
}
