using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	public class EnumerableTracer<T>
	{
		public EnumerableTracer(IEnumerable<T> data) : this(data, -1)
		{
		}

		public EnumerableTracer(IEnumerable<T> data, int limit)
		{
			this.data = data;
			this.limit = limit;
		}

		public override string ToString()
		{
			if (this.data != null && this.limit != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = this.limit;
				foreach (T t in this.data)
				{
					if (num == 0)
					{
						stringBuilder.Append(", ...");
						break;
					}
					if (num != this.limit)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(t.ToString());
					num--;
				}
				return stringBuilder.ToString();
			}
			return "<null>";
		}

		private IEnumerable<T> data;

		private int limit;
	}
}
