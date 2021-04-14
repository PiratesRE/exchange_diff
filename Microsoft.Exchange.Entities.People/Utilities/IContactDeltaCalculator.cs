using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.People.Utilities
{
	public interface IContactDeltaCalculator<T, K>
	{
		ICollection<Tuple<K, object>> CalculateDelta(T objectSource, T objectTarget);
	}
}
