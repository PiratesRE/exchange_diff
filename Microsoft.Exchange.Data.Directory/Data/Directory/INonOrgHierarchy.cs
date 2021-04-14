using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface INonOrgHierarchy
	{
		OrganizationId OrgHierarchyToIgnore { get; set; }
	}
}
