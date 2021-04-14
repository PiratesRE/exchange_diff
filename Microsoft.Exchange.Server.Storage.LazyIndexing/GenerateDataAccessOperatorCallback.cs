using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public delegate SimpleQueryOperator.SimpleQueryOperatorDefinition GenerateDataAccessOperatorCallback(Context context, LogicalIndex logicalIndex, IList<Column> columnsToFetch, out LogicalIndex baseViewIndex);
}
