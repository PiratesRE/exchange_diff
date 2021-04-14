using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IStoreSimpleQueryTarget<T> : IStoreQueryTargetBase<T>
	{
		IEnumerable<T> GetRows(object[] parameters);
	}
}
