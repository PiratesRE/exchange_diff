using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	public interface IFlightSettingRepository
	{
		string GetHostNameFromVdir(ADObjectId serverSiteId, string protocol);
	}
}
