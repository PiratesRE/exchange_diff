using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NearFilter : QueryFilter
	{
		public NearFilter(uint distance, bool ordered, AndFilter filter)
		{
			this.distance = distance;
			this.ordered = ordered;
			this.filter = filter;
		}

		public override bool Equals(object obj)
		{
			NearFilter nearFilter = obj as NearFilter;
			return nearFilter != null && nearFilter.GetType() == base.GetType() && nearFilter.Distance == this.Distance && nearFilter.Ordered == this.Ordered && nearFilter.Filter.Equals(this.Filter);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.Distance.GetHashCode() ^ this.Ordered.GetHashCode() ^ this.filter.GetHashCode();
		}

		public override IEnumerable<string> Keywords()
		{
			return this.filter.Keywords();
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.distance);
			sb.Append(", ");
			sb.Append(this.ordered);
			sb.Append(", (");
			if (this.filter != null)
			{
				this.filter.ToString(sb);
			}
			sb.Append("))");
		}

		protected override int GetSize()
		{
			int size = base.GetSize();
			return size + this.filter.Size;
		}

		public uint Distance
		{
			get
			{
				return this.distance;
			}
		}

		public bool Ordered
		{
			get
			{
				return this.ordered;
			}
		}

		public AndFilter Filter
		{
			get
			{
				return this.filter;
			}
		}

		private readonly uint distance;

		private readonly bool ordered;

		private readonly AndFilter filter;
	}
}
