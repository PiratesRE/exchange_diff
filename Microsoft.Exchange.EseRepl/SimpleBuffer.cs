using System;

namespace Microsoft.Exchange.EseRepl
{
	public class SimpleBuffer : IPoolableObject
	{
		public byte[] Buffer { get; private set; }

		public bool Preallocated { get; private set; }

		public SimpleBuffer(int size, bool preallocated)
		{
			this.Buffer = new byte[size];
			this.Preallocated = preallocated;
		}
	}
}
