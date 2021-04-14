using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class SubFilter : QueryFilter
	{
		public SubFilter(SubFilterProperties subFilterProperty, QueryFilter filter)
		{
			this.subFilterProperty = subFilterProperty;
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

		public SubFilterProperties SubFilterProperty
		{
			get
			{
				return this.subFilterProperty;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.subFilterProperty.ToString());
			sb.Append("(");
			this.filter.ToString(sb);
			sb.Append("))");
		}

		public override bool Equals(object obj)
		{
			SubFilter subFilter = obj as SubFilter;
			return subFilter != null && subFilter.GetType() == base.GetType() && this.subFilterProperty == subFilter.subFilterProperty && this.filter.Equals(subFilter.filter);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.filter.GetHashCode();
		}

		protected override int GetSize()
		{
			int size = base.GetSize();
			return size + this.filter.Size;
		}

		private readonly QueryFilter filter;

		private readonly SubFilterProperties subFilterProperty;
	}
}
