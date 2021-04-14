using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	internal interface IColumnResolver
	{
		Column Resolve(Column column);
	}
}
