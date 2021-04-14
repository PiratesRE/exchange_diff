using System;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IRowPropertyBag
	{
		object GetPropertyValue(Connection connection, StorePropTag propTag);
	}
}
