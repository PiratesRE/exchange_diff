using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFindMiniServer
	{
		IADToplogyConfigurationSession AdSession { get; }

		void Clear();

		IADServer FindMiniServerByFqdn(string serverFqdn);

		IADServer FindMiniServerByShortName(string shortName);

		IADServer FindMiniServerByShortNameEx(string shortName, out Exception ex);

		IADServer ReadMiniServerByObjectId(ADObjectId serverId);
	}
}
