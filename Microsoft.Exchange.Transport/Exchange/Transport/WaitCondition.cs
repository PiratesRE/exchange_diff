using System;

namespace Microsoft.Exchange.Transport
{
	internal abstract class WaitCondition : IComparable
	{
		public abstract int CompareTo(object obj);
	}
}
