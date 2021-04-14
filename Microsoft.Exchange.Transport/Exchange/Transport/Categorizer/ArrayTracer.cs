using System;
using System.Text;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ArrayTracer<T>
	{
		public ArrayTracer(T[] array)
		{
			this.array = array;
		}

		public override string ToString()
		{
			if (this.array == null)
			{
				return "<null array>";
			}
			StringBuilder stringBuilder = new StringBuilder("Array length=");
			stringBuilder.Append(this.array.Length);
			stringBuilder.Append(" { ");
			for (int i = 0; i < this.array.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(this.DumpElement(this.array[i]));
			}
			stringBuilder.Append(" }");
			return stringBuilder.ToString();
		}

		protected virtual string DumpElement(T element)
		{
			return element.ToString();
		}

		private T[] array;
	}
}
