using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface ITWIR
	{
		int GetColumnSize(Column column);

		object GetColumnValue(Column column);

		int GetPhysicalColumnSize(PhysicalColumn column);

		object GetPhysicalColumnValue(PhysicalColumn column);

		int GetPropertyColumnSize(PropertyColumn column);

		object GetPropertyColumnValue(PropertyColumn column);
	}
}
