using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDirectoryAccessor : IADUserFinder
	{
		bool TryGetOrganizationContentConversionProperties(OrganizationId organizationId, out OrganizationContentConversionProperties organizationContentConversionProperties);

		SmtpAddress GetOrganizationFederatedMailboxIdentity(IConfigurationSession configurationSession);

		OrganizationRelationship GetOrganizationRelationship(OrganizationId organizationId, string domain);

		int? GetPrimaryGroupId(IRecipientSession recipientSession, SecurityIdentifier userSid);

		bool IsLicensingEnforcedInOrg(OrganizationId organizationId);

		bool IsTenantAccessBlocked(OrganizationId organizationId);

		Server GetLocalServer();
	}
}
