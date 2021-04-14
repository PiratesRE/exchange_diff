using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	public interface IDirectory
	{
		ADSite[] GetADSites();

		ClientAccessArray[] GetClientAccessArrays();

		Server[] GetServers();
	}
}
