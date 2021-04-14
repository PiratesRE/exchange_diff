using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExchangePrincipalPerformanceMarkers
	{
		public const string UpdateDatabaseLocationInfo = "UpdateDatabaseLocationInfo";

		public const string UpdateCrossPremiseStatus = "UpdateCrossPremiseStatus";

		public const string UpdateCrossPremiseStatusFindByExchangeGuidIncludingAlternate = "UpdateCrossPremiseStatusFindByExchangeGuidIncludingAlternate";

		public const string UpdateCrossPremiseStatusFindByLegacyExchangeDN = "UpdateCrossPremiseStatusFindByLegacyExchangeDN";

		public const string UpdateCrossPremiseStatusRemoteMailbox = "UpdateCrossPremiseStatusRemoteMailbox";

		public const string UpdateDelegationTokenRequest = "UpdateDelegationTokenRequest";

		public const string GetUserSKUCapability = "GetUserSKUCapability";

		public const string GetIsLicensingEnforcedInOrg = "GetIsLicensingEnforcedInOrg";
	}
}
