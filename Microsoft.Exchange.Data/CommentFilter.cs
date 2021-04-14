using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class CommentFilter : QueryFilter
	{
		public CommentFilter(PropertyDefinition[] properties, object[] values, QueryFilter filter)
		{
			if ((properties != null && values == null) || (properties == null && values != null))
			{
				throw new ArgumentException("Either of properties or values is Null while the other is not.");
			}
			if (properties != null && values != null && properties.Length != values.Length)
			{
				throw new ArgumentException("The lengths of properties and values do not match.");
			}
			this.properties = properties;
			this.values = values;
			this.filter = filter;
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append("[");
			if (this.properties != null)
			{
				for (int i = 0; i < this.properties.Length; i++)
				{
					if (i != 0)
					{
						sb.Append(", ");
					}
					sb.Append(this.properties[i].ToString());
					sb.Append("=");
					sb.Append(this.values[i].ToString());
				}
			}
			sb.Append("], (");
			if (this.filter != null)
			{
				this.filter.ToString(sb);
			}
			sb.Append("))");
		}

		public QueryFilter Filter
		{
			get
			{
				return this.filter;
			}
		}

		public PropertyDefinition[] Properties
		{
			get
			{
				return this.properties;
			}
		}

		public object[] Values
		{
			get
			{
				return this.values;
			}
		}

		private readonly PropertyDefinition[] properties;

		private readonly object[] values;

		private readonly QueryFilter filter;
	}
}
