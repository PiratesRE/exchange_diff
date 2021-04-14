using System;
using System.Collections;

namespace System
{
	internal interface IWellKnownStringEqualityComparer
	{
		IEqualityComparer GetRandomizedEqualityComparer();

		IEqualityComparer GetEqualityComparerForSerialization();
	}
}
