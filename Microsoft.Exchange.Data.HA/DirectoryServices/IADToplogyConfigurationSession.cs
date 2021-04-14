using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADToplogyConfigurationSession
	{
		bool UseConfigNC { get; set; }

		bool UseGlobalCatalog { get; set; }

		IADServer FindServerByName(string serverShortName);

		IADDatabaseAvailabilityGroup FindDagByServer(IADServer server);

		IADComputer FindComputerByHostName(string hostName);

		IEnumerable<IADDatabase> GetAllDatabases(IADServer server);

		IEnumerable<IADDatabaseCopy> GetAllDatabaseCopies(IADServer server);

		TADWrapperObject ReadADObject<TADWrapperObject>(ADObjectId objectId) where TADWrapperObject : class, IADObjectCommon;

		TADWrapperObject[] Find<TADWrapperObject>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults) where TADWrapperObject : class, IADObjectCommon;

		IADDatabase FindDatabaseByGuid(Guid dbGuid);

		IADServer ReadMiniServer(ADObjectId entryId);

		IADServer FindMiniServerByName(string serverName);

		bool TryFindByExchangeLegacyDN(string legacyExchangeDN, out IADMiniClientAccessServerOrArray miniClientAccessServerOrArray);

		IADMiniClientAccessServerOrArray ReadMiniClientAccessServerOrArray(ADObjectId entryId);

		IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByFqdn(string serverFqdn);

		IADSite GetLocalSite();
	}
}
