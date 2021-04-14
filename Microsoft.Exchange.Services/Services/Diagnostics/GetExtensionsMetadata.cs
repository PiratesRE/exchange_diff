using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public enum GetExtensionsMetadata
	{
		[DisplayName("EXT", "GE")]
		GetExtensionsTime,
		[DisplayName("EXT", "GM")]
		GetMasterTableTime,
		[DisplayName("EXT", "GP")]
		GetProvidedExtensionsTime,
		[DisplayName("EXT", "AM")]
		AddMasterTableTime,
		[DisplayName("EXT", "CU")]
		CheckUpdatesTime,
		[DisplayName("EXT", "SU")]
		SaveMasterTableTime,
		[DisplayName("EXT", "OrgHost")]
		OrgMailboxEwsUrlHost,
		[DisplayName("EXT", "EWSReqId")]
		OrgMailboxEwsRequestId,
		[DisplayName("EXT", "GO")]
		GetOrgExtensionsTime,
		[DisplayName("EXT", "GET")]
		GetExtensionsTotalTime,
		[DisplayName("EXT", "CES")]
		CreateExchangeServiceTime,
		[DisplayName("EXT", "GCE")]
		GetClientExtensionTime,
		[DisplayName("EXT", "OAD")]
		OrgMailboxAdUserLookupTime,
		[DisplayName("EXT", "WSUrl")]
		WebServiceUrlLookupTime,
		[DisplayName("EXT", "CET")]
		CreateExtensionsTime,
		[DisplayName("EXT", "GMUT")]
		GetMarketplaceUrlTime
	}
}
