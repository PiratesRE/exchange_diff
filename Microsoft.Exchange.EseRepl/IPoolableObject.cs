using System;

namespace Microsoft.Exchange.EseRepl
{
	internal interface IPoolableObject
	{
		bool Preallocated { get; }
	}
}
