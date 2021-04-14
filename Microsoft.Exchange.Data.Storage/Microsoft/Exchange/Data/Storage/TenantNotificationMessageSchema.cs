using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TenantNotificationMessageSchema : MessageItemSchema
	{
		public new static TenantNotificationMessageSchema Instance
		{
			get
			{
				if (TenantNotificationMessageSchema.instance == null)
				{
					TenantNotificationMessageSchema.instance = new TenantNotificationMessageSchema();
				}
				return TenantNotificationMessageSchema.instance;
			}
		}

		private static TenantNotificationMessageSchema instance;

		public static readonly StorePropertyDefinition MonitoringEventInstanceId = InternalSchema.MonitoringEventInstanceId;

		public static readonly StorePropertyDefinition MonitoringEventSource = InternalSchema.MonitoringEventSource;

		public static readonly StorePropertyDefinition MonitoringEventCategoryId = InternalSchema.MonitoringEventCategoryId;

		public static readonly StorePropertyDefinition MonitoringEventTimeUtc = InternalSchema.MonitoringEventTimeUtc;

		public static readonly StorePropertyDefinition MonitoringInsertionStrings = InternalSchema.MonitoringInsertionStrings;

		public static readonly StorePropertyDefinition MonitoringUniqueId = InternalSchema.MonitoringUniqueId;

		public static readonly StorePropertyDefinition MonitoringNotificationEmailSent = InternalSchema.MonitoringNotificationEmailSent;

		public static readonly StorePropertyDefinition MonitoringCreationTimeUtc = InternalSchema.MonitoringCreationTimeUtc;

		public static readonly StorePropertyDefinition MonitoringEventEntryType = InternalSchema.MonitoringEventEntryType;

		public static readonly PropertyDefinition MonitoringCountOfNotificationsSentInPast24Hours = InternalSchema.MonitoringCountOfNotificationsSentInPast24Hours;

		public static readonly PropertyDefinition MonitoringNotificationRecipients = InternalSchema.MonitoringNotificationRecipients;

		public static readonly PropertyDefinition MonitoringHashCodeForDuplicateDetection = InternalSchema.MonitoringHashCodeForDuplicateDetection;

		public static readonly PropertyDefinition MonitoringNotificationMessageIds = InternalSchema.MonitoringNotificationMessageIds;

		public static readonly PropertyDefinition MonitoringEventPeriodicKey = InternalSchema.MonitoringEventPeriodicKey;
	}
}
