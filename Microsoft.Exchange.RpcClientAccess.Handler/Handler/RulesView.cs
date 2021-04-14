using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RulesView : FolderBasedView
	{
		internal RulesView(Logon logon, ReferenceCount<CoreFolder> coreFolderReference, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle) : base(logon, coreFolderReference, tableFlags, View.Capabilities.CanRestrict, ViewType.None, notificationHandler, returnNotificationHandle, null)
		{
		}

		protected override IQueryResult CreateQueryResult(NativeStorePropertyDefinition[] propertyDefinitions)
		{
			IQueryResult queryResult;
			using (IModifyTable ruleTable = base.CoreFolder.GetRuleTable())
			{
				queryResult = ruleTable.GetQueryResult(base.Filter, propertyDefinitions);
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
				return PropertyConverter.Rule;
			}
		}

		protected override void CheckPropertiesAllowed(PropertyTag[] propertyTags)
		{
		}
	}
}
