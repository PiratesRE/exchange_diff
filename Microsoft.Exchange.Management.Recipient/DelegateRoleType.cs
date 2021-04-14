using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public enum DelegateRoleType
	{
		[LocDescription(Strings.IDs.DelegateRoleTypeOrgAdmin)]
		OrgAdmin,
		[LocDescription(Strings.IDs.DelegateRoleTypeRecipientAdmin)]
		RecipientAdmin,
		[LocDescription(Strings.IDs.DelegateRoleTypeServerAdmin)]
		ServerAdmin,
		[LocDescription(Strings.IDs.DelegateRoleTypeViewOnlyAdmin)]
		ViewOnlyAdmin,
		[LocDescription(Strings.IDs.DelegateRoleTypePublicFolderAdmin)]
		PublicFolderAdmin
	}
}
