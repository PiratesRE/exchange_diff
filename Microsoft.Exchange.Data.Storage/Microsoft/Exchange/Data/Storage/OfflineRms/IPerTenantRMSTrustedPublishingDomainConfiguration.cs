using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPerTenantRMSTrustedPublishingDomainConfiguration
	{
		Uri IntranetLicensingUrl { get; }

		Uri ExtranetLicensingUrl { get; }

		Uri IntranetCertificationUrl { get; }

		Uri ExtranetCertificationUrl { get; }

		string CompressedSLCCertChain { get; }

		Dictionary<string, PrivateKeyInformation> PrivateKeys { get; }

		IList<string> CompressedRMSTemplates { get; }

		IList<string> CompressedTrustedDomainChains { get; }
	}
}
