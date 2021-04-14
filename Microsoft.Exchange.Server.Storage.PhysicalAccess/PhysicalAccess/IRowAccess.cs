using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IRowAccess
	{
		object GetPhysicalColumn(PhysicalColumn column);
	}
}
