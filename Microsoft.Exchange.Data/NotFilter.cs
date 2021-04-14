using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NotFilter : QueryFilter
	{
		public NotFilter(QueryFilter filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			this.filter = filter;
		}

		public QueryFilter Filter
		{
			get
			{
				return this.filter;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(!(");
			this.filter.ToString(sb);
			sb.Append("))");
		}

		public override bool Equals(object obj)
		{
			NotFilter notFilter = obj as NotFilter;
			return notFilter != null && notFilter.GetType() == base.GetType() && notFilter.filter.Equals(this.filter);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.filter.GetHashCode();
		}

		public override string PropertyName
		{
			get
			{
				if (this.filter is SinglePropertyFilter)
				{
					SinglePropertyFilter singlePropertyFilter = this.filter as SinglePropertyFilter;
					return QueryFilter.ConvertPropertyName(singlePropertyFilter.Property.Name);
				}
				if (this.filter is CompositeFilter)
				{
					CompositeFilter compositeFilter = this.filter as CompositeFilter;
					return compositeFilter.PropertyName;
				}
				return base.PropertyName;
			}
		}

		public override IEnumerable<string> Keywords()
		{
			return this.filter.Keywords();
		}

		public override QueryFilter CloneWithPropertyReplacement(IDictionary<PropertyDefinition, PropertyDefinition> propertyMap)
		{
			return new NotFilter(this.filter.CloneWithPropertyReplacement(propertyMap));
		}

		internal override IEnumerable<PropertyDefinition> FilterProperties()
		{
			return this.Filter.FilterProperties();
		}

		protected override int GetSize()
		{
			int size = base.GetSize();
			return size + this.filter.Size;
		}

		private readonly QueryFilter filter;
	}
}
