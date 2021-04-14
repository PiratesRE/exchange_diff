using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MessageItemRowNotificationHandler : RowNotificationHandler
	{
		static MessageItemRowNotificationHandler()
		{
			Shape[] shapes = new Shape[]
			{
				ItemShape.CreateShape(),
				MessageShape.CreateShape(),
				TaskShape.CreateShape()
			};
			ResponseShape responseShape = WellKnownShapes.ResponseShapes[WellKnownShapeName.MailListItem];
			MessageItemRowNotificationHandler.defaultSubscriptionProperties = RowNotificationHandler.GetPropertyDefinitionsForResponseShape(shapes, responseShape, new PropertyDefinition[0]);
			MessageItemRowNotificationHandler.normalizedSubjectPropertyDefinition = WellKnownProperties.NormalizedSubject.ToPropertyDefinition();
			MessageItemRowNotificationHandler.lastVerbExecutedPropertyDefinition = WellKnownProperties.LastVerbExecuted.ToPropertyDefinition();
			MessageItemRowNotificationHandler.lastVerbExecutionTimePropertyDefinition = WellKnownProperties.LastVerbExecutionTime.ToPropertyDefinition();
		}

		public MessageItemRowNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, ExTimeZone timeZone, IFeaturesManager featuresManager = null) : base(subscriptionId, parameters, folderId, userContext, mailboxGuid, timeZone, false)
		{
			this.mailboxId = new MailboxId(mailboxGuid);
			this.subscriptionProperties = MessageItemRowNotificationHandler.GetSubscriptionProperties(featuresManager);
		}

		protected MessageItemRowNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, RowNotifier notifier, IFeaturesManager featuresManager = null) : base(subscriptionId, parameters, folderId, userContext, mailboxGuid, notifier, false)
		{
			this.mailboxId = new MailboxId(mailboxGuid);
			this.subscriptionProperties = MessageItemRowNotificationHandler.GetSubscriptionProperties(featuresManager);
		}

		protected override PropertyDefinition[] SubscriptionProperties
		{
			get
			{
				return this.subscriptionProperties;
			}
		}

		protected override NotificationPayloadBase GetPayloadFromNotification(StoreObjectId folderId, QueryNotification notification)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[MessageItemRowNotificationHandler.GetPayloadFromNotification] Start. SubscriptionId: {0}", base.SubscriptionId);
			RowNotificationPayload rowNotificationPayload = new RowNotificationPayload();
			rowNotificationPayload.Source = new MailboxLocation(base.MailboxGuid);
			base.GetPartialPayloadFromNotification(rowNotificationPayload, notification);
			rowNotificationPayload.Item = this.GetMessageItemFromNotification(notification);
			return rowNotificationPayload;
		}

		protected override QueryResult GetQueryResult(Folder folder)
		{
			bool flag = SearchUtil.IsComplexClutterFilteredView(base.ViewFilter, base.ClutterFilter);
			if (base.ViewFilter == ViewFilter.TaskOverdue || flag)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "MessageItemRowNotificationHandler.GetQueryResult Start. subscription {0}", base.SubscriptionId);
				QueryFilter queryFilter = flag ? SearchUtil.GetViewQueryForComplexClutterFilteredView(base.ClutterFilter, false) : SearchUtil.GetViewQueryFilter(base.ViewFilter);
				return folder.ItemQuery(ItemQueryType.None, queryFilter, base.SortBy, this.SubscriptionProperties);
			}
			return base.GetQueryResult(folder);
		}

		protected override void ProcessQueryResultChangedNotification()
		{
			base.Notifier.AddQueryResultChangedPayload(base.FolderId, base.SubscriptionId);
		}

		private static PropertyDefinition[] GetSubscriptionProperties(IFeaturesManager featuresManager)
		{
			string text = WellKnownShapeName.MailListItem.ToString();
			ItemResponseShape itemResponseShape = new ItemResponseShape();
			itemResponseShape.BaseShape = ShapeEnum.IdOnly;
			ItemResponseShape responseShape = Global.ResponseShapeResolver.GetResponseShape<ItemResponseShape>(text, itemResponseShape, featuresManager);
			if (responseShape == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)text.GetHashCode(), "[MessageItemRowNotificationHandler.GetSubscriptionProperties] Unable to resolve shapeName: {0} with features manager", text);
				return MessageItemRowNotificationHandler.defaultSubscriptionProperties;
			}
			Shape[] shapes = new Shape[]
			{
				ItemShape.CreateShape(),
				MessageShape.CreateShape(),
				TaskShape.CreateShape()
			};
			return RowNotificationHandler.GetPropertyDefinitionsForResponseShape(shapes, responseShape, new PropertyDefinition[0]);
		}

		private ItemId StoreIdToEwsItemId(StoreId storeId)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, this.mailboxId, null);
			return new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
		}

		private ItemType GetMessageItemFromNotification(QueryNotification notification)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[MessageItemRowNotificationHandler.GetMessageItemFromNotification] Start. SubscriptionId: {0}", base.SubscriptionId);
			ItemType itemType;
			if (notification.EventType == QueryNotificationType.RowDeleted)
			{
				itemType = new MessageType
				{
					InstanceKey = notification.Index
				};
			}
			else
			{
				StoreId itemProperty = RowNotificationHandler.GetItemProperty<StoreId>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Id));
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(itemProperty);
				itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
				itemType.InstanceKey = notification.Index;
				itemType.ItemId = this.StoreIdToEwsItemId(itemProperty);
				itemType.ParentFolderId = new FolderId(base.GetEwsId(RowNotificationHandler.GetItemProperty<StoreId>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, StoreObjectSchema.ParentItemId))), null);
				itemType.ConversationId = new ItemId(IdConverter.ConversationIdToEwsId(base.MailboxGuid, RowNotificationHandler.GetItemProperty<ConversationId>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.ConversationId))), null);
				itemType.Subject = RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Subject));
				itemType.ImportanceString = RowNotificationHandler.GetItemProperty<Importance>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Importance), Importance.Normal).ToString();
				itemType.SensitivityString = RowNotificationHandler.GetItemProperty<Sensitivity>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Sensitivity), Sensitivity.Normal).ToString();
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.ReceivedTime)))
				{
					itemType.DateTimeReceived = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.ReceivedTime));
				}
				itemType.HasAttachments = new bool?(RowNotificationHandler.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.HasAttachment)));
				itemType.IsDraft = new bool?(RowNotificationHandler.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, MessageItemSchema.IsDraft)));
				itemType.ItemClass = RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, StoreObjectSchema.ItemClass));
				MessageType messageType = itemType as MessageType;
				if (messageType != null)
				{
					if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.From)))
					{
						messageType.From = RowNotificationHandler.CreateRecipientFromParticipant((Participant)notification.Row[Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.From)]);
					}
					if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Sender)))
					{
						messageType.Sender = RowNotificationHandler.CreateRecipientFromParticipant((Participant)notification.Row[Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Sender)]);
					}
					messageType.IsRead = new bool?(RowNotificationHandler.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, MessageItemSchema.IsRead)));
				}
				FlagType flagType = new FlagType();
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.CompleteDate)))
				{
					flagType.CompleteDate = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.CompleteDate));
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.FlagStatus)))
				{
					flagType.FlagStatus = RowNotificationHandler.GetItemProperty<FlagStatus>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.FlagStatus), FlagStatus.NotFlagged);
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, TaskSchema.StartDate)))
				{
					flagType.StartDate = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, TaskSchema.StartDate));
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, TaskSchema.DueDate)))
				{
					flagType.DueDate = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, TaskSchema.DueDate));
				}
				itemType.Flag = flagType;
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, StoreObjectSchema.CreationTime)))
				{
					itemType.DateTimeCreated = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, StoreObjectSchema.CreationTime));
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, StoreObjectSchema.LastModifiedTime)))
				{
					itemType.LastModifiedTime = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, StoreObjectSchema.LastModifiedTime));
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.ReceivedOrRenewTime)))
				{
					itemType.ReceivedOrRenewTime = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.ReceivedOrRenewTime));
				}
				itemType.Categories = RowNotificationHandler.GetItemProperty<string[]>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Categories));
				itemType.Preview = RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Preview), null);
				itemType.Size = RowNotificationHandler.GetItemProperty<int?>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.Size), null);
				itemType.AddExtendedPropertyValue(new ExtendedPropertyType(WellKnownProperties.NormalizedSubject, RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, MessageItemRowNotificationHandler.normalizedSubjectPropertyDefinition), null)));
				itemType.DisplayTo = RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.DisplayTo), null);
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.IconIndex)))
				{
					IconIndex itemProperty2 = (IconIndex)RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.IconIndex));
					if (itemProperty2 > (IconIndex)0)
					{
						itemType.IconIndexString = itemProperty2.ToString();
					}
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.SentTime)))
				{
					itemType.DateTimeSent = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, ItemSchema.SentTime));
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, MessageItemRowNotificationHandler.lastVerbExecutedPropertyDefinition)))
				{
					itemType.AddExtendedPropertyValue(new ExtendedPropertyType(WellKnownProperties.LastVerbExecuted, RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, MessageItemRowNotificationHandler.lastVerbExecutedPropertyDefinition)).ToString()));
				}
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, MessageItemRowNotificationHandler.lastVerbExecutionTimePropertyDefinition)))
				{
					itemType.AddExtendedPropertyValue(new ExtendedPropertyType(WellKnownProperties.LastVerbExecutionTime, base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.subscriptionProperties, MessageItemRowNotificationHandler.lastVerbExecutionTimePropertyDefinition))));
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[MessageItemRowNotificationHandler.GetMessageItemFromNotification] End. SubscriptionId: {0}", base.SubscriptionId);
			}
			return itemType;
		}

		private static PropertyDefinition[] defaultSubscriptionProperties;

		private static PropertyDefinition normalizedSubjectPropertyDefinition;

		private static PropertyDefinition lastVerbExecutedPropertyDefinition;

		private static PropertyDefinition lastVerbExecutionTimePropertyDefinition;

		private readonly PropertyDefinition[] subscriptionProperties;

		private readonly MailboxId mailboxId;
	}
}
