using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IDatabaseExecutionContext : IExecutionContext
	{
		void OnBeforeTableAccess(Connection.OperationType operationType, Table table, IList<object> partitionValues);
	}
}
