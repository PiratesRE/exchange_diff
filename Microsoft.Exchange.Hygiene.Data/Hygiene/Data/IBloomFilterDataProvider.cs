using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	public interface IBloomFilterDataProvider
	{
		bool Check<T>(QueryFilter queryFilter);
	}
}
