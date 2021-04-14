using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarFolderSchema : FolderSchema
	{
		public new static CalendarFolderSchema Instance
		{
			get
			{
				if (CalendarFolderSchema.instance == null)
				{
					CalendarFolderSchema.instance = new CalendarFolderSchema();
				}
				return CalendarFolderSchema.instance;
			}
		}

		public const int CurrentCalendarFolderVersion = 1;

		public static readonly StorePropertyDefinition FreeBusySecurityDescriptor = InternalSchema.FreeBusySecurityDescriptor;

		public static readonly StorePropertyDefinition ConsumerCalendarGuid = InternalSchema.ConsumerCalendarGuid;

		public static readonly StorePropertyDefinition ConsumerCalendarOwnerId = InternalSchema.ConsumerCalendarOwnerId;

		public static readonly StorePropertyDefinition ConsumerCalendarPrivateFreeBusyId = InternalSchema.ConsumerCalendarPrivateFreeBusyId;

		public static readonly StorePropertyDefinition ConsumerCalendarPrivateDetailId = InternalSchema.ConsumerCalendarPrivateDetailId;

		public static readonly StorePropertyDefinition ConsumerCalendarPublishVisibility = InternalSchema.ConsumerCalendarPublishVisibility;

		public static readonly StorePropertyDefinition ConsumerCalendarSharingInvitations = InternalSchema.ConsumerCalendarSharingInvitations;

		public static readonly StorePropertyDefinition ConsumerCalendarPermissionLevel = InternalSchema.ConsumerCalendarPermissionLevel;

		public static readonly StorePropertyDefinition ConsumerCalendarSynchronizationState = InternalSchema.ConsumerCalendarSynchronizationState;

		[Autoload]
		public static readonly StorePropertyDefinition CharmId = InternalSchema.CharmId;

		public static readonly StorePropertyDefinition[] ConsumerCalendarProperties = new StorePropertyDefinition[]
		{
			CalendarFolderSchema.ConsumerCalendarGuid,
			CalendarFolderSchema.ConsumerCalendarOwnerId,
			CalendarFolderSchema.ConsumerCalendarPrivateFreeBusyId,
			CalendarFolderSchema.ConsumerCalendarPrivateDetailId,
			CalendarFolderSchema.ConsumerCalendarPublishVisibility,
			CalendarFolderSchema.ConsumerCalendarSharingInvitations,
			CalendarFolderSchema.ConsumerCalendarPermissionLevel,
			CalendarFolderSchema.ConsumerCalendarSynchronizationState
		};

		private static CalendarFolderSchema instance = null;
	}
}
