using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class CountFilter : QueryFilter
	{
		public CountFilter(uint count, QueryFilter filter)
		{
			this.count = count;
			this.filter = filter;
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.count);
			sb.Append(", (");
			if (this.filter != null)
			{
				this.filter.ToString(sb);
			}
			sb.Append("))");
		}

		public uint Count
		{
			get
			{
				return this.count;
			}
		}

		public QueryFilter Filter
		{
			get
			{
				return this.filter;
			}
		}

		private readonly uint count;

		private readonly QueryFilter filter;
	}
}
