using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IStoreDatabaseQueryTarget<T> : IStoreQueryTargetBase<T>
	{
		IEnumerable<T> GetRows(IConnectionProvider connectionProvider, object[] parameters);
	}
}
