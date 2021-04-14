using System;

namespace Microsoft.Exchange.EseRepl
{
	internal interface IPool<T> where T : class, IPoolableObject
	{
		bool TryReturnObject(T o);

		T TryGetObject();
	}
}
