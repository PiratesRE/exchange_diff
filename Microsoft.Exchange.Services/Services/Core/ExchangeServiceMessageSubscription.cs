using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExchangeServiceMessageSubscription : ExchangeServiceSubscription
	{
		internal ExchangeServiceMessageSubscription(string subscriptionId) : base(subscriptionId)
		{
			this.mailboxId = new LazyMember<MailboxId>(() => new MailboxId(this.MailboxGuid));
		}

		internal Guid MailboxGuid { get; set; }

		internal Subscription Subscription { get; set; }

		internal Action<MessageNotification> Callback { get; set; }

		internal QueryResult QueryResult { get; set; }

		internal PropertyDefinition[] PropertyList { get; set; }

		internal override void HandleNotification(Notification notification)
		{
			MessageNotification messageNotification = null;
			if (notification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Received a null notification for subscriptionId: {0}", base.SubscriptionId);
				return;
			}
			if (notification is ConnectionDroppedNotification)
			{
				messageNotification = new MessageNotification();
				messageNotification.NotificationType = NotificationTypeType.Reload;
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Connection dropped, returning notification for reload");
			}
			else
			{
				QueryNotification queryNotification = notification as QueryNotification;
				if (queryNotification == null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Received a notification of an unknown type for subscriptionId: {0}", base.SubscriptionId);
					return;
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Received a {0} notification for subscriptionId: {1}", queryNotification.EventType.ToString(), base.SubscriptionId);
				switch (queryNotification.EventType)
				{
				case QueryNotificationType.RowAdded:
				case QueryNotificationType.RowModified:
					messageNotification = new MessageNotification();
					messageNotification.NotificationType = ((queryNotification.EventType == QueryNotificationType.RowAdded) ? NotificationTypeType.Create : NotificationTypeType.Update);
					messageNotification.Message = this.GetMessageFromNotification(queryNotification, this.PropertyList);
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Calling notification callback for message: {0}", messageNotification.Message.ItemId.Id);
					goto IL_1A1;
				case QueryNotificationType.RowDeleted:
					messageNotification = new MessageNotification();
					messageNotification.NotificationType = NotificationTypeType.Delete;
					messageNotification.Message = this.GetMessageFromNotification(queryNotification, queryNotification.PropertyDefinitions.ToArray<PropertyDefinition>());
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Notification for deletion");
					goto IL_1A1;
				case QueryNotificationType.Reload:
					messageNotification = new MessageNotification();
					messageNotification.NotificationType = NotificationTypeType.Reload;
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Notification for reload");
					goto IL_1A1;
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Unknown notification event type");
			}
			IL_1A1:
			if (messageNotification != null)
			{
				this.Callback(messageNotification);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceMessageSubscription.HandleNotification: Returned from callback");
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (this.Subscription != null)
			{
				this.Subscription.Dispose();
				this.Subscription = null;
			}
			if (this.QueryResult != null)
			{
				this.QueryResult.Dispose();
				this.QueryResult = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeServiceMessageSubscription>(this);
		}

		protected static bool IsPropertyDefined(QueryNotification notification, int index)
		{
			return index >= 0 && index < notification.Row.Length && notification.Row[index] != null && !(notification.Row[index] is PropertyError);
		}

		internal static SingleRecipientType CreateRecipientFromParticipant(Participant participant)
		{
			return new SingleRecipientType
			{
				Mailbox = new EmailAddressWrapper(),
				Mailbox = 
				{
					Name = participant.DisplayName,
					EmailAddress = participant.EmailAddress,
					RoutingType = participant.RoutingType,
					MailboxType = MailboxHelper.GetMailboxType(participant.Origin, participant.RoutingType).ToString()
				}
			};
		}

		protected static T GetItemProperty<T>(QueryNotification notification, int index)
		{
			return ExchangeServiceMessageSubscription.GetItemProperty<T>(notification, index, default(T));
		}

		protected static T GetItemProperty<T>(QueryNotification notification, int index, T defaultValue)
		{
			if (!ExchangeServiceMessageSubscription.IsPropertyDefined(notification, index))
			{
				return defaultValue;
			}
			object obj = notification.Row[index];
			if (!(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		protected string GetDateTimeProperty(QueryNotification notification, int index, ExTimeZone timeZone = null)
		{
			ExDateTime itemProperty = ExchangeServiceMessageSubscription.GetItemProperty<ExDateTime>(notification, index, ExDateTime.MinValue);
			if (ExDateTime.MinValue.Equals(itemProperty))
			{
				return null;
			}
			ExTimeZone exTimeZone = (timeZone == null || timeZone == ExTimeZone.UnspecifiedTimeZone) ? itemProperty.TimeZone : timeZone;
			if (exTimeZone == ExTimeZone.UtcTimeZone)
			{
				return ExDateTimeConverter.ToUtcXsdDateTime(itemProperty);
			}
			return ExDateTimeConverter.ToOffsetXsdDateTime(itemProperty, exTimeZone);
		}

		private ItemId StoreIdToEwsItemId(StoreId storeId)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, this.mailboxId.Member, null);
			return new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
		}

		protected string GetEwsId(StoreId storeId)
		{
			if (storeId == null)
			{
				return null;
			}
			return StoreId.StoreIdToEwsId(this.MailboxGuid, storeId);
		}

		private MessageType GetMessageFromNotification(QueryNotification notification, PropertyDefinition[] propertyList)
		{
			StoreId storeId = null;
			StoreObjectId storeObjectId = null;
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Id)))
			{
				storeId = ExchangeServiceMessageSubscription.GetItemProperty<StoreId>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Id));
				if (storeId != null)
				{
					storeObjectId = StoreId.GetStoreObjectId(storeId);
				}
			}
			MessageType messageType = (storeObjectId != null) ? MessageType.CreateFromStoreObjectType(storeObjectId.ObjectType) : new MessageType();
			if (storeId != null)
			{
				messageType.ItemId = this.StoreIdToEwsItemId(storeId);
			}
			messageType.InstanceKey = notification.Index;
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.ParentItemId)))
			{
				messageType.ParentFolderId = new FolderId(this.GetEwsId(ExchangeServiceMessageSubscription.GetItemProperty<StoreId>(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.ParentItemId))), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.ConversationId)))
			{
				messageType.ConversationId = new ItemId(IdConverter.ConversationIdToEwsId(this.MailboxGuid, ExchangeServiceMessageSubscription.GetItemProperty<ConversationId>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.ConversationId))), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Subject)))
			{
				messageType.Subject = ExchangeServiceMessageSubscription.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Subject));
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Importance)))
			{
				messageType.ImportanceString = ExchangeServiceMessageSubscription.GetItemProperty<Importance>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Importance), Importance.Normal).ToString();
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Sensitivity)))
			{
				messageType.SensitivityString = ExchangeServiceMessageSubscription.GetItemProperty<Sensitivity>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Sensitivity), Sensitivity.Normal).ToString();
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.ReceivedTime)))
			{
				messageType.DateTimeReceived = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.ReceivedTime), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.HasAttachment)))
			{
				messageType.HasAttachments = new bool?(ExchangeServiceMessageSubscription.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.HasAttachment)));
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.IsDraft)))
			{
				messageType.IsDraft = new bool?(ExchangeServiceMessageSubscription.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.IsDraft)));
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.ItemClass)))
			{
				messageType.ItemClass = ExchangeServiceMessageSubscription.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.ItemClass));
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.From)))
			{
				messageType.From = ExchangeServiceMessageSubscription.CreateRecipientFromParticipant((Participant)notification.Row[Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.From)]);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Sender)))
			{
				messageType.Sender = ExchangeServiceMessageSubscription.CreateRecipientFromParticipant((Participant)notification.Row[Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Sender)]);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.IsRead)))
			{
				messageType.IsRead = new bool?(ExchangeServiceMessageSubscription.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.IsRead)));
			}
			FlagType flagType = new FlagType();
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.CompleteDate)))
			{
				flagType.CompleteDate = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.CompleteDate), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.FlagStatus)))
			{
				flagType.FlagStatus = ExchangeServiceMessageSubscription.GetItemProperty<FlagStatus>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.FlagStatus), FlagStatus.NotFlagged);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, TaskSchema.StartDate)))
			{
				flagType.StartDate = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, TaskSchema.StartDate), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, TaskSchema.DueDate)))
			{
				flagType.DueDate = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, TaskSchema.DueDate), null);
			}
			messageType.Flag = flagType;
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.CreationTime)))
			{
				messageType.DateTimeCreated = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.CreationTime), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.LastModifiedTime)))
			{
				messageType.LastModifiedTime = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, StoreObjectSchema.LastModifiedTime), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.ReceivedOrRenewTime)))
			{
				messageType.ReceivedOrRenewTime = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.ReceivedOrRenewTime), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Categories)))
			{
				messageType.Categories = ExchangeServiceMessageSubscription.GetItemProperty<string[]>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Categories));
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Preview)))
			{
				messageType.Preview = ExchangeServiceMessageSubscription.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Preview), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Size)))
			{
				messageType.Size = ExchangeServiceMessageSubscription.GetItemProperty<int?>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.Size), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.NormalizedSubject)))
			{
				messageType.AddExtendedPropertyValue(new ExtendedPropertyType(ExchangeServiceMessageSubscription.normalizedSubject, ExchangeServiceMessageSubscription.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.NormalizedSubject), null)));
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.DisplayTo)))
			{
				messageType.DisplayTo = ExchangeServiceMessageSubscription.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.DisplayTo), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.IconIndex)))
			{
				IconIndex itemProperty = (IconIndex)ExchangeServiceMessageSubscription.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.IconIndex));
				if (itemProperty > (IconIndex)0)
				{
					messageType.IconIndexString = itemProperty.ToString();
				}
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.SentTime)))
			{
				messageType.DateTimeSent = this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, ItemSchema.SentTime), null);
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.LastVerbExecuted)))
			{
				messageType.AddExtendedPropertyValue(new ExtendedPropertyType(ExchangeServiceMessageSubscription.lastVerbExecuted, ExchangeServiceMessageSubscription.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.LastVerbExecuted)).ToString()));
			}
			if (ExchangeServiceMessageSubscription.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.LastVerbExecutionTime)))
			{
				messageType.AddExtendedPropertyValue(new ExtendedPropertyType(ExchangeServiceMessageSubscription.lastVerbExecutionTime, this.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(propertyList, MessageItemSchema.LastVerbExecutionTime), null)));
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[MessageItemExchangeServiceMessageSubscription.GetMessageItemFromNotification] End. SubscriptionId: {0}", base.SubscriptionId);
			return messageType;
		}

		private readonly LazyMember<MailboxId> mailboxId;

		private static ExtendedPropertyUri lastVerbExecuted = new ExtendedPropertyUri
		{
			PropertyTag = "0x1081",
			PropertyType = MapiPropertyType.Integer
		};

		private static ExtendedPropertyUri lastVerbExecutionTime = new ExtendedPropertyUri
		{
			PropertyTag = "0x1082",
			PropertyType = MapiPropertyType.SystemTime
		};

		private static ExtendedPropertyUri normalizedSubject = new ExtendedPropertyUri
		{
			PropertyTag = "0xe1d",
			PropertyType = MapiPropertyType.String
		};
	}
}
