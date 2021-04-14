using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public delegate IEnumerable<IColumnValueBag> GetColumnValueBagsEnumeratorDelegate(IContextProvider contextProvider, IEnumerable<Column> requiredColumns, IInterruptControl interruptControl, out LogicalIndex baseViewIndex);
}
