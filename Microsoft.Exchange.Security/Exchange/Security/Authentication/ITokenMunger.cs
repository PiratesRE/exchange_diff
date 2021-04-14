using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal interface ITokenMunger
	{
		ClientSecurityContext MungeToken(ClientSecurityContext clientSecurityContext, OrganizationId tenantOrganizationId);

		bool TryMungeToken(ClientSecurityContext clientSecurityContext, OrganizationId tenantOrganizationId, SecurityIdentifier slaveAccountSid, out ClientSecurityContext mungedClientSecurityContext);
	}
}
