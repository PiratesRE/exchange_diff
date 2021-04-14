using System;

namespace Microsoft.Exchange.Data
{
	internal interface IEnumerableFilter<T>
	{
		bool AcceptElement(T element);
	}
}
