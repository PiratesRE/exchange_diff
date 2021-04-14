using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OrganizationIdConvertor : IOrganizationIdConvertor
	{
		public OrganizationId FromExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId)
		{
			return OrganizationId.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId);
		}

		public string ToExternalDirectoryOrganizationId(OrganizationId orgId)
		{
			return orgId.ToExternalDirectoryOrganizationId();
		}

		public static readonly OrganizationIdConvertor Default = new OrganizationIdConvertor();
	}
}
