using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.MapiTasks;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class PublicFolderClientPermissionHelper
	{
		private static void AddEntry(DataTable table, ADObjectId identity, string name, MultiValuedProperty<PublicFolderAccessRight> accessRights)
		{
			DataRow dataRow = table.NewRow();
			dataRow["Identity"] = identity;
			dataRow["Name"] = name;
			dataRow["AccessRights"] = PublicFolderAccessRight.CalculatePublicFolderPermission(accessRights);
			table.Rows.Add(dataRow);
		}

		public static DataTable GenerateClientPermissionDataSource(object clientPermission)
		{
			AutomatedObjectPicker automatedObjectPicker = new AutomatedObjectPicker("PublicFolderClientPermissionConfigurable");
			DataTable dataTable = automatedObjectPicker.ObjectPickerProfile.DataTable.Clone();
			List<object> list = clientPermission as List<object>;
			if (list != null)
			{
				list.RemoveAll(delegate(object entry)
				{
					PublicFolderUserId user = (entry as PublicFolderClientPermissionEntry).User;
					return user.ActiveDirectoryIdentity == null && !user.IsAnonymous && !user.IsDefault;
				});
				foreach (object obj in list)
				{
					PublicFolderClientPermissionEntry publicFolderClientPermissionEntry = (PublicFolderClientPermissionEntry)obj;
					ADObjectId identity = PublicFolderClientPermissionHelper.ConvertUserToAdObjectId(publicFolderClientPermissionEntry.User);
					string name = (publicFolderClientPermissionEntry.User.ActiveDirectoryIdentity != null) ? publicFolderClientPermissionEntry.User.ActiveDirectoryIdentity.Name : publicFolderClientPermissionEntry.User.ToString();
					PublicFolderClientPermissionHelper.AddEntry(dataTable, identity, name, publicFolderClientPermissionEntry.AccessRights);
				}
				dataTable.DefaultView.Sort = "Identity asc";
				dataTable.AcceptChanges();
			}
			return dataTable;
		}

		internal static object ConvertAdObjectIdToUser(ADObjectId id)
		{
			object result = id;
			if (id == PublicFolderClientPermissionHelper.DefaultUserId)
			{
				result = PublicFolderUserId.DefaultUserId;
			}
			if (id == PublicFolderClientPermissionHelper.AnonymousUserId)
			{
				result = PublicFolderUserId.AnonymousUserId;
			}
			return result;
		}

		internal static ADObjectId ConvertUserToAdObjectId(PublicFolderUserId id)
		{
			ADObjectId result = id.ActiveDirectoryIdentity;
			if (id.IsDefault)
			{
				result = PublicFolderClientPermissionHelper.DefaultUserId;
			}
			if (id.IsAnonymous)
			{
				result = PublicFolderClientPermissionHelper.AnonymousUserId;
			}
			return result;
		}

		internal static readonly ADObjectId DefaultUserId = new ADObjectId(Guid.NewGuid());

		internal static readonly ADObjectId AnonymousUserId = new ADObjectId(Guid.NewGuid());
	}
}
