using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	public enum MailboxFolderPermissionRole
	{
		[LocDescription(Strings.IDs.RoleOwner)]
		Owner = 2043,
		[LocDescription(Strings.IDs.RolePublishingEditor)]
		PublishingEditor = 1275,
		[LocDescription(Strings.IDs.RoleEditor)]
		Editor = 1147,
		[LocDescription(Strings.IDs.RolePublishingAuthor)]
		PublishingAuthor = 1179,
		[LocDescription(Strings.IDs.RoleAuthor)]
		Author = 1051,
		[LocDescription(Strings.IDs.RoleNonEditingAuthor)]
		NonEditingAuthor = 1043,
		[LocDescription(Strings.IDs.RoleReviewer)]
		Reviewer = 1025,
		[LocDescription(Strings.IDs.RoleContributor)]
		Contributor,
		[LocDescription(Strings.IDs.RoleAvailabilityOnly)]
		AvailabilityOnly = 2048,
		[LocDescription(Strings.IDs.RoleLimitedDetails)]
		LimitedDetails = 6144
	}
}
