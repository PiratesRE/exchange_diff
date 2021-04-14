using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class ADObjectComparer<T> : IEqualityComparer<T> where T : ADObject
	{
		bool IEqualityComparer<!0>.Equals(T x, T y)
		{
			return x.Id.Equals(y.Id);
		}

		int IEqualityComparer<!0>.GetHashCode(T obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
