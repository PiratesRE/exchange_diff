using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADSessionFactory
	{
		IADToplogyConfigurationSession CreateIgnoreInvalidRootOrgSession(bool readOnly = true);

		IADToplogyConfigurationSession CreatePartiallyConsistentRootOrgSession(bool readOnly = true);

		IADToplogyConfigurationSession CreateFullyConsistentRootOrgSession(bool readOnly = true);

		IADRootOrganizationRecipientSession CreateIgnoreInvalidRootOrgRecipientSession();
	}
}
