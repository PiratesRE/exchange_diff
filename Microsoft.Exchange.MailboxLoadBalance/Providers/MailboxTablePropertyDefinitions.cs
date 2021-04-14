using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxLoadBalance.Providers
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxTablePropertyDefinitions
	{
		public static readonly MapiPropertyDefinition ItemsPendingUpgrade = new MapiPropertyDefinition("ItemsPendingUpgrade", typeof(int), PropTag.ItemsPendingUpgrade, MapiPropertyDefinitionFlags.ReadOnly, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition CreationTime = MapiPropertyDefinitions.CreationTime;

		public static readonly MapiPropertyDefinition MailboxGuid = MapiPropertyDefinitions.MailboxGuid;

		public static readonly MapiPropertyDefinition PersistableTenantPartitionHint = MapiPropertyDefinitions.PersistableTenantPartitionHint;

		public static readonly MapiPropertyDefinition MessageTableTotalSize = MapiPropertyDefinitions.MessageTableTotalSize;

		public static readonly MapiPropertyDefinition AttachmentTableTotalSize = MapiPropertyDefinitions.AttachmentTableTotalSize;

		public static readonly MapiPropertyDefinition OtherTablesTotalSize = MapiPropertyDefinitions.OtherTablesTotalSize;

		public static readonly MapiPropertyDefinition IsQuarantined = MapiPropertyDefinitions.IsQuarantined;

		public static readonly MapiPropertyDefinition MailboxMiscFlags = MapiPropertyDefinitions.MailboxMiscFlags;

		public static readonly MapiPropertyDefinition TotalItemSize = MapiPropertyDefinitions.TotalItemSize;

		public static readonly MapiPropertyDefinition TotalDeletedItemSize = MapiPropertyDefinitions.TotalDeletedItemSize;

		public static readonly MapiPropertyDefinition StoreMailboxType = MailboxStatisticsSchema.StoreMailboxType;

		public static readonly MapiPropertyDefinition ItemCount = MailboxStatisticsSchema.ItemCount;

		public static readonly MapiPropertyDefinition DeletedItemCount = MailboxStatisticsSchema.DeletedItemCount;

		public static readonly MapiPropertyDefinition LastLogonTime = MailboxStatisticsSchema.LastLogonTime;

		public static readonly MapiPropertyDefinition DisconnectDate = MailboxStatisticsSchema.DisconnectDate;

		internal static readonly PropTag[] MailboxTablePropertiesToLoad = (from prop in new MapiPropertyDefinition[]
		{
			MailboxTablePropertyDefinitions.MailboxGuid,
			MailboxTablePropertyDefinitions.PersistableTenantPartitionHint,
			MailboxTablePropertyDefinitions.MessageTableTotalSize,
			MailboxTablePropertyDefinitions.AttachmentTableTotalSize,
			MailboxTablePropertyDefinitions.OtherTablesTotalSize,
			MailboxTablePropertyDefinitions.IsQuarantined,
			MailboxTablePropertyDefinitions.MailboxMiscFlags,
			MailboxTablePropertyDefinitions.TotalItemSize,
			MailboxTablePropertyDefinitions.TotalDeletedItemSize,
			MailboxTablePropertyDefinitions.StoreMailboxType,
			MailboxTablePropertyDefinitions.ItemCount,
			MailboxTablePropertyDefinitions.DeletedItemCount,
			MailboxTablePropertyDefinitions.LastLogonTime,
			MailboxTablePropertyDefinitions.DisconnectDate,
			MailboxTablePropertyDefinitions.CreationTime,
			MailboxTablePropertyDefinitions.ItemsPendingUpgrade
		}
		select prop.PropertyTag).ToArray<PropTag>();
	}
}
