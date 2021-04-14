using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CalendarItemNotificationHandler : RowNotificationHandler
	{
		public CalendarItemNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, ExTimeZone timeZone, bool remoteSubscription) : base(subscriptionId, parameters, folderId, userContext, mailboxGuid, timeZone, remoteSubscription)
		{
		}

		protected CalendarItemNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, RowNotifier notifier, bool remoteSubscription) : base(subscriptionId, parameters, folderId, userContext, mailboxGuid, notifier, remoteSubscription)
		{
		}

		protected override PropertyDefinition[] SubscriptionProperties
		{
			get
			{
				return this.querySubscriptionProperties;
			}
		}

		internal override void HandleNotificationInternal(Notification notification, MapiNotificationsLogEvent logEvent, object context)
		{
			lock (base.SyncRoot)
			{
				if (notification == null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Received NULL notification");
					logEvent.NullNotification = true;
				}
				else
				{
					QueryNotification queryNotification = (QueryNotification)notification;
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<QueryNotificationType, int>((long)this.GetHashCode(), "CalendarItemNotificationHandler received a notification. Type: {0}. Row length: {1}.", queryNotification.EventType, queryNotification.Row.Length);
					string itemProperty = RowNotificationHandler.GetItemProperty<string>(queryNotification, 38);
					if (ObjectClass.IsCalendarItemSeries(itemProperty))
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "CalendarItemNotificationHandler ignored notification for item of class: {0}.", itemProperty);
					}
					else if (base.ProcessErrorNotification(queryNotification))
					{
						logEvent.InvalidNotification = true;
					}
					else
					{
						base.Notifier.AddFolderContentChangePayload(base.FolderId, this.GetPayloadFromNotification(base.FolderId, queryNotification));
						base.Notifier.PickupData();
					}
				}
			}
		}

		protected override NotificationPayloadBase GetPayloadFromNotification(StoreObjectId folderId, QueryNotification notification)
		{
			CalendarItemNotificationPayload calendarItemNotificationPayload = new CalendarItemNotificationPayload();
			calendarItemNotificationPayload.FolderId = StoreId.StoreIdToEwsId(base.MailboxGuid, folderId);
			calendarItemNotificationPayload.SubscriptionId = base.SubscriptionId;
			calendarItemNotificationPayload.EventType = notification.EventType;
			calendarItemNotificationPayload.Source = new MailboxLocation(base.MailboxGuid);
			EwsCalendarItemType ewsCalendarItemType = new EwsCalendarItemType();
			ewsCalendarItemType.InstanceKey = notification.Index;
			calendarItemNotificationPayload.Item = ewsCalendarItemType;
			if (notification.EventType != QueryNotificationType.RowDeleted)
			{
				VersionedId itemProperty = RowNotificationHandler.GetItemProperty<VersionedId>(notification, 0);
				if (itemProperty == null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Invalid StoreId for calendar item notification.");
					calendarItemNotificationPayload.Reload = true;
					return calendarItemNotificationPayload;
				}
				ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(itemProperty, new MailboxId(base.MailboxGuid), null);
				ewsCalendarItemType.ItemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
				ewsCalendarItemType.Start = base.GetDateTimeProperty(notification, 2);
				ewsCalendarItemType.End = base.GetDateTimeProperty(notification, 3);
				ewsCalendarItemType.Subject = RowNotificationHandler.GetItemProperty<string>(notification, 4);
				ewsCalendarItemType.LegacyFreeBusyStatusString = BusyTypeConverter.ToString((BusyType)RowNotificationHandler.GetItemProperty<int>(notification, 5));
				ewsCalendarItemType.IsAllDayEvent = RowNotificationHandler.GetItemProperty<bool?>(notification, 6);
				ewsCalendarItemType.IsRecurring = new bool?(RowNotificationHandler.GetItemProperty<byte[]>(notification, 7) != null);
				if (RowNotificationHandler.IsPropertyDefined(notification, 8))
				{
					ewsCalendarItemType.Organizer = RowNotificationHandler.CreateRecipientFromParticipant((Participant)notification.Row[8]);
				}
				ewsCalendarItemType.IsMeeting = RowNotificationHandler.GetItemProperty<bool?>(notification, 9);
				ewsCalendarItemType.MyResponseTypeString = ResponseTypeConverter.ToString((ResponseType)RowNotificationHandler.GetItemProperty<int>(notification, 10));
				ewsCalendarItemType.SensitivityString = SensitivityConverter.ToString(RowNotificationHandler.GetItemProperty<Sensitivity>(notification, 11));
				ewsCalendarItemType.HasAttachments = RowNotificationHandler.GetItemProperty<bool?>(notification, 12);
				LocationSourceType itemProperty2 = (LocationSourceType)RowNotificationHandler.GetItemProperty<int>(notification, 26);
				ewsCalendarItemType.EnhancedLocation = ((itemProperty2 == LocationSourceType.None) ? new EnhancedLocationType
				{
					DisplayName = RowNotificationHandler.GetItemProperty<string>(notification, 13),
					PostalAddress = new Microsoft.Exchange.Services.Core.Types.PostalAddress()
				} : new EnhancedLocationType
				{
					DisplayName = RowNotificationHandler.GetItemProperty<string>(notification, 24),
					Annotation = RowNotificationHandler.GetItemProperty<string>(notification, 25),
					PostalAddress = new Microsoft.Exchange.Services.Core.Types.PostalAddress
					{
						LocationSource = itemProperty2,
						LocationUri = RowNotificationHandler.GetItemProperty<string>(notification, 27),
						Latitude = this.GetLocationPropertyValue(notification, 28),
						Longitude = this.GetLocationPropertyValue(notification, 29),
						Accuracy = this.GetLocationPropertyValue(notification, 30),
						Altitude = this.GetLocationPropertyValue(notification, 31),
						AltitudeAccuracy = this.GetLocationPropertyValue(notification, 32),
						Street = RowNotificationHandler.GetItemProperty<string>(notification, 33),
						City = RowNotificationHandler.GetItemProperty<string>(notification, 34),
						State = RowNotificationHandler.GetItemProperty<string>(notification, 35),
						Country = RowNotificationHandler.GetItemProperty<string>(notification, 36),
						PostalCode = RowNotificationHandler.GetItemProperty<string>(notification, 37)
					}
				});
				StoreId itemProperty3 = RowNotificationHandler.GetItemProperty<StoreId>(notification, 14);
				ewsCalendarItemType.ParentFolderId = IdConverter.GetFolderIdFromStoreId(itemProperty3, new MailboxId(base.MailboxGuid));
				byte[] itemProperty4 = RowNotificationHandler.GetItemProperty<byte[]>(notification, 15);
				if (itemProperty4 != null)
				{
					try
					{
						GlobalObjectId globalObjectId = new GlobalObjectId(itemProperty4);
						ewsCalendarItemType.UID = globalObjectId.Uid;
					}
					catch (CorruptDataException ex)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)this.GetHashCode(), "Exception setting the UID in CalendarItemNotificationHandler:GetPayloadFromNotification. Exception: {1}", ex.Message);
					}
				}
				ewsCalendarItemType.AppointmentState = new int?(RowNotificationHandler.GetItemProperty<int>(notification, 16));
				ewsCalendarItemType.IsCancelled = new bool?((ewsCalendarItemType.AppointmentState.Value & 4) == 4);
				ewsCalendarItemType.CalendarItemTypeString = CalendarItemTypeConverter.ToString(RowNotificationHandler.GetItemProperty<CalendarItemType>(notification, 17));
				ewsCalendarItemType.AppointmentReplyTime = base.GetDateTimeProperty(notification, 18);
				if (RowNotificationHandler.IsPropertyDefined(notification, 19))
				{
					ewsCalendarItemType.JoinOnlineMeetingUrl = RowNotificationHandler.GetItemProperty<string>(notification, 19);
				}
				ewsCalendarItemType.Categories = RowNotificationHandler.GetItemProperty<string[]>(notification, 20);
				ewsCalendarItemType.IsOrganizer = new bool?(RowNotificationHandler.GetItemProperty<bool>(notification, 23));
				ConversationId itemProperty5 = RowNotificationHandler.GetItemProperty<ConversationId>(notification, 21);
				string id = IdConverter.ConversationIdToEwsId(base.MailboxGuid, itemProperty5);
				ewsCalendarItemType.ConversationId = new ItemId(id, null);
				ewsCalendarItemType.IsResponseRequested = new bool?(RowNotificationHandler.GetItemProperty<bool>(notification, 22));
				try
				{
					base.UserContext.LockAndReconnectMailboxSession(3000);
					ewsCalendarItemType.EffectiveRights = EffectiveRightsProperty.GetFromEffectiveRights(EffectiveRights.Modify | EffectiveRights.Read | EffectiveRights.Delete, base.UserContext.MailboxSession);
				}
				finally
				{
					if (base.UserContext.MailboxSessionLockedByCurrentThread())
					{
						base.UserContext.UnlockAndDisconnectMailboxSession();
					}
				}
			}
			return calendarItemNotificationPayload;
		}

		protected override void ProcessReloadNotification()
		{
			base.Notifier.AddFolderContentChangePayload(base.FolderId, this.GetPayloadFromNotification(base.FolderId, null));
		}

		protected override void ProcessQueryResultChangedNotification()
		{
			this.ProcessReloadNotification();
		}

		private double? GetLocationPropertyValue(QueryNotification notification, int index)
		{
			double? itemProperty = RowNotificationHandler.GetItemProperty<double?>(notification, index);
			if (itemProperty == null || double.IsNaN(itemProperty.Value))
			{
				return null;
			}
			return itemProperty;
		}

		private PropertyDefinition[] querySubscriptionProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ChangeKey,
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime,
			ItemSchema.Subject,
			CalendarItemBaseSchema.FreeBusyStatus,
			CalendarItemBaseSchema.MapiIsAllDayEvent,
			CalendarItemBaseSchema.AppointmentRecurrenceBlob,
			ItemSchema.From,
			CalendarItemBaseSchema.IsMeeting,
			CalendarItemBaseSchema.ResponseType,
			ItemSchema.Sensitivity,
			ItemSchema.HasAttachment,
			CalendarItemBaseSchema.Location,
			StoreObjectSchema.ParentItemId,
			CalendarItemBaseSchema.GlobalObjectId,
			CalendarItemBaseSchema.AppointmentState,
			CalendarItemBaseSchema.CalendarItemType,
			CalendarItemBaseSchema.AppointmentReplyTime,
			CalendarItemBaseSchema.OnlineMeetingExternalLink,
			ItemSchema.Categories,
			ItemSchema.ConversationId,
			ItemSchema.IsResponseRequested,
			CalendarItemBaseSchema.IsOrganizer,
			CalendarItemBaseSchema.LocationDisplayName,
			CalendarItemBaseSchema.LocationAnnotation,
			CalendarItemBaseSchema.LocationSource,
			CalendarItemBaseSchema.LocationUri,
			CalendarItemBaseSchema.Latitude,
			CalendarItemBaseSchema.Longitude,
			CalendarItemBaseSchema.Accuracy,
			CalendarItemBaseSchema.Altitude,
			CalendarItemBaseSchema.AltitudeAccuracy,
			CalendarItemBaseSchema.LocationStreet,
			CalendarItemBaseSchema.LocationCity,
			CalendarItemBaseSchema.LocationState,
			CalendarItemBaseSchema.LocationCountry,
			CalendarItemBaseSchema.LocationPostalCode,
			StoreObjectSchema.ItemClass
		};
	}
}
