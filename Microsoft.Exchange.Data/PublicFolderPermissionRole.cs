using System;

namespace Microsoft.Exchange.Data
{
	public enum PublicFolderPermissionRole
	{
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRoleOwner)]
		Owner = 2043,
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRolePublishingEditor)]
		PublishingEditor = 1275,
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRoleEditor)]
		Editor = 1147,
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRolePublishingAuthor)]
		PublishingAuthor = 1179,
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRoleAuthor)]
		Author = 1051,
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRoleNonEditingAuthor)]
		NonEditingAuthor = 1043,
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRoleReviewer)]
		Reviewer = 1025,
		[LocDescription(DataStrings.IDs.PublicFolderPermissionRoleContributor)]
		Contributor
	}
}
