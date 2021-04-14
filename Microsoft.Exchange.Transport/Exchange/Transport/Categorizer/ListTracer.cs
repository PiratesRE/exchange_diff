using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ListTracer<T>
	{
		public ListTracer(IList<T> source)
		{
			this.list = source;
		}

		public override string ToString()
		{
			if (this.list == null)
			{
				return "<null>";
			}
			StringBuilder stringBuilder = new StringBuilder("List length=");
			stringBuilder.Append(this.list.Count);
			stringBuilder.Append(" { ");
			int num = 0;
			foreach (T t in this.list)
			{
				if (num++ > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(t.ToString());
			}
			stringBuilder.Append(" }");
			return stringBuilder.ToString();
		}

		private IList<T> list;
	}
}
