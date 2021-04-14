using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PermissionsView : FolderBasedView
	{
		internal PermissionsView(Logon logon, ReferenceCount<CoreFolder> coreFolderReference, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle) : base(logon, coreFolderReference, tableFlags, View.Capabilities.Basic, ViewType.None, notificationHandler, returnNotificationHandle, null)
		{
		}

		protected override IQueryResult CreateQueryResult(NativeStorePropertyDefinition[] propertyDefinitions)
		{
			ModifyTableOptions modifyTableOptions = ModifyTableOptions.None;
			if ((byte)(base.TableFlags & TableFlags.Associated) == 2)
			{
				modifyTableOptions |= ModifyTableOptions.FreeBusyAware;
			}
			IQueryResult queryResult;
			using (IModifyTable permissionTable = base.CoreFolder.GetPermissionTable(modifyTableOptions))
			{
				queryResult = permissionTable.GetQueryResult(base.Filter, propertyDefinitions);
			}
			return queryResult;
		}

		protected override StoreId? ContainerFolderId
		{
			get
			{
				return null;
			}
		}

		protected override PropertyConverter PropertyConverter
		{
			get
			{
				return PropertyConverter.Permission;
			}
		}

		protected override void CheckPropertiesAllowed(PropertyTag[] propertyTags)
		{
		}
	}
}
