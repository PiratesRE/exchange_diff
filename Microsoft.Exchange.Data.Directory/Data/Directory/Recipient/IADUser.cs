using System;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADUser : IADMailboxRecipient, IADMailStorage, IADSecurityPrincipal, IADOrgPerson, IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag, IOriginatingChangeTimestamp, IFederatedIdentityParameters, IProvisioningCacheInvalidation
	{
		ADObjectId ActiveSyncMailboxPolicy { get; set; }

		MultiValuedProperty<Guid> AggregatedMailboxGuids { get; }

		ADObjectId ArchiveDatabase { get; set; }

		SmtpDomain ArchiveDomain { get; }

		Guid ArchiveGuid { get; }

		MultiValuedProperty<string> ArchiveName { get; set; }

		ArchiveState ArchiveState { get; }

		ArchiveStatusFlags ArchiveStatus { get; }

		IMailboxLocationCollection MailboxLocations { get; }

		NetID NetID { get; }

		ADObjectId OwaMailboxPolicy { get; set; }

		ADObjectId QueryBaseDN { get; set; }

		Capability? SKUCapability { get; set; }

		bool? SKUAssigned { get; set; }

		string UserPrincipalName { get; set; }

		SmtpAddress GetFederatedSmtpAddress();
	}
}
