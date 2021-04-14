using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IColumn
	{
		int GetSize(ITWIR context);

		object GetValue(ITWIR context);
	}
}
