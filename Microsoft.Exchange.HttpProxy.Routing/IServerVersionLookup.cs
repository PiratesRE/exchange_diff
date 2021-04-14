using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public interface IServerVersionLookup
	{
		int? LookupVersion(string server);
	}
}
