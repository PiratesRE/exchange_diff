using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class PublicFolderPermissionLevelListSource : EnumListSource
	{
		public PublicFolderPermissionLevelListSource() : base(PublicFolderPermissionLevelListSource.permissionRoleList, typeof(PublicFolderPermission))
		{
		}

		protected override string GetValueText(object objectValue)
		{
			return this.converter.Format(null, objectValue, null);
		}

		public static bool ContainsPermission(PublicFolderPermission value)
		{
			return PublicFolderPermissionLevelListSource.permissionRoleList.Contains(value);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static PublicFolderPermissionLevelListSource()
		{
			PublicFolderPermission[] array = new PublicFolderPermission[9];
			array[0] = (PublicFolderPermission.ReadItems | PublicFolderPermission.CreateItems | PublicFolderPermission.EditOwnedItems | PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.EditAllItems | PublicFolderPermission.DeleteAllItems | PublicFolderPermission.CreateSubfolders | PublicFolderPermission.FolderOwner | PublicFolderPermission.FolderContact | PublicFolderPermission.FolderVisible);
			array[1] = (PublicFolderPermission.ReadItems | PublicFolderPermission.CreateItems | PublicFolderPermission.EditOwnedItems | PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.EditAllItems | PublicFolderPermission.DeleteAllItems | PublicFolderPermission.CreateSubfolders | PublicFolderPermission.FolderVisible);
			array[2] = (PublicFolderPermission.ReadItems | PublicFolderPermission.CreateItems | PublicFolderPermission.EditOwnedItems | PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.EditAllItems | PublicFolderPermission.DeleteAllItems | PublicFolderPermission.FolderVisible);
			array[3] = (PublicFolderPermission.ReadItems | PublicFolderPermission.CreateItems | PublicFolderPermission.EditOwnedItems | PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.CreateSubfolders | PublicFolderPermission.FolderVisible);
			array[4] = (PublicFolderPermission.ReadItems | PublicFolderPermission.CreateItems | PublicFolderPermission.EditOwnedItems | PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.FolderVisible);
			array[5] = (PublicFolderPermission.ReadItems | PublicFolderPermission.CreateItems | PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.FolderVisible);
			array[6] = (PublicFolderPermission.ReadItems | PublicFolderPermission.FolderVisible);
			array[7] = (PublicFolderPermission.CreateItems | PublicFolderPermission.FolderVisible);
			PublicFolderPermissionLevelListSource.permissionRoleList = array;
		}

		private static PublicFolderPermission[] permissionRoleList;

		private ICustomTextConverter converter = new PublicFolderPermissionAsRoleCoverter();
	}
}
