using System;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	public class ArrayTracer<T>
	{
		public ArrayTracer(T[] array) : this(array, int.MaxValue)
		{
		}

		public ArrayTracer(T[] array, int limit)
		{
			this.array = array;
			this.limit = limit;
		}

		public override string ToString()
		{
			if (this.array != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<array length=" + this.array.Length + ">");
				for (int i = 0; i < this.array.Length; i++)
				{
					if (i >= this.limit)
					{
						stringBuilder.Append(" ...");
						break;
					}
					stringBuilder.Append(string.Concat(new object[]
					{
						" [",
						i,
						"]=",
						this.array[i].ToString(),
						";"
					}));
				}
				return stringBuilder.ToString();
			}
			return "<null>";
		}

		private T[] array;

		private int limit;
	}
}
