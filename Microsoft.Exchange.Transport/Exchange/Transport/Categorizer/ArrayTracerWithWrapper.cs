using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ArrayTracerWithWrapper<T, W> : ArrayTracer<T> where W : ITraceWrapper<T>, new()
	{
		public ArrayTracerWithWrapper(T[] array) : base(array)
		{
		}

		protected override string DumpElement(T element)
		{
			W w = (default(W) == null) ? Activator.CreateInstance<W>() : default(W);
			w.Element = element;
			return w.ToString();
		}
	}
}
