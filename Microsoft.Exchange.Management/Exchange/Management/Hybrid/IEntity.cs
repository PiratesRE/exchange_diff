using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IEntity<T>
	{
		ADObjectId Identity { get; }

		bool Equals(T obj);

		T Clone(ADObjectId identity);

		void UpdateFrom(T obj);
	}
}
