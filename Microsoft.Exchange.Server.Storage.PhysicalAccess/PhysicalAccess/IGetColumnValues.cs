using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	internal interface IGetColumnValues
	{
		object[] GetColumnValues(IEnumerable<Column> columns);
	}
}
