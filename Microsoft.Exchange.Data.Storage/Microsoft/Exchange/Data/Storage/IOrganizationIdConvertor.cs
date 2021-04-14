using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IOrganizationIdConvertor
	{
		OrganizationId FromExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId);

		string ToExternalDirectoryOrganizationId(OrganizationId orgId);
	}
}
