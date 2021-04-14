using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
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
	internal class ConversationRowNotificationHandler : RowNotificationHandler
	{
		static ConversationRowNotificationHandler()
		{
			Shape[] shapes = new Shape[]
			{
				ConversationShape.CreateShape()
			};
			ResponseShape responseShape = WellKnownShapes.ResponseShapes[WellKnownShapeName.ConversationUberListView];
			ConversationRowNotificationHandler.defaultConversationViewQuerySubscriptionProperties = RowNotificationHandler.GetPropertyDefinitionsForResponseShape(shapes, responseShape, new PropertyDefinition[]
			{
				ConversationItemSchema.ConversationPreview
			});
		}

		public ConversationRowNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, ExTimeZone timeZone, bool remoteSubscription, IFeaturesManager featuresManager) : base(subscriptionId, parameters, folderId, userContext, mailboxGuid, timeZone, remoteSubscription)
		{
			this.conversationViewQuerySubscriptionProperties = ConversationRowNotificationHandler.GetSubscriptionProperties(parameters.ConversationShapeName, featuresManager);
			SimulatedWebRequestContext.Execute(userContext, "ConversationNotificationDraftFolderId", delegate(MailboxSession mailboxSession, IRecipientSession adSession, RequestDetailsLogger logger)
			{
				this.draftFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
			});
		}

		protected override PropertyDefinition[] SubscriptionProperties
		{
			get
			{
				return this.conversationViewQuerySubscriptionProperties;
			}
		}

		protected override NotificationPayloadBase GetPayloadFromNotification(StoreObjectId folderId, QueryNotification notification)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[ConversationRowNotificationHandler.GetPayloadFromNotification] SubscriptionId: {0}", base.SubscriptionId);
			RowNotificationPayload rowNotificationPayload = new RowNotificationPayload();
			rowNotificationPayload.Source = new MailboxLocation(base.MailboxGuid);
			base.GetPartialPayloadFromNotification(rowNotificationPayload, notification);
			rowNotificationPayload.Conversation = this.GetConversationFromNotification(notification);
			return rowNotificationPayload;
		}

		protected override QueryResult GetQueryResult(Folder folder)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[ConversationRowNotificationHandler.GetQueryResult] SubscriptionId: {0}", base.SubscriptionId);
			QueryFilter queryFilter = null;
			SortBy[] sortColumns = base.SortBy;
			if (SearchUtil.IsComplexClutterFilteredView(base.ViewFilter, base.ClutterFilter))
			{
				queryFilter = SearchUtil.GetViewQueryForComplexClutterFilteredView(base.ClutterFilter, true);
			}
			else if (!string.IsNullOrEmpty(base.FromFilter))
			{
				queryFilter = PeopleIKnowQuery.GetConversationQueryFilter(base.FromFilter);
				sortColumns = PeopleIKnowQuery.GetConversationQuerySortBy(base.SortBy);
			}
			return folder.ConversationItemQuery(queryFilter, sortColumns, this.SubscriptionProperties);
		}

		protected override void ProcessQueryResultChangedNotification()
		{
			base.Notifier.AddQueryResultChangedPayload(base.FolderId, base.SubscriptionId);
		}

		private static PropertyDefinition[] GetSubscriptionProperties(string requestedConversationShapeName, IFeaturesManager featuresManager)
		{
			if (string.IsNullOrEmpty(requestedConversationShapeName))
			{
				requestedConversationShapeName = WellKnownShapeName.ConversationUberListView.ToString();
			}
			ConversationResponseShape clientResponseShape = new ConversationResponseShape(ShapeEnum.IdOnly, new PropertyPath[0]);
			ConversationResponseShape responseShape = Global.ResponseShapeResolver.GetResponseShape<ConversationResponseShape>(requestedConversationShapeName, clientResponseShape, featuresManager);
			if (responseShape == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)requestedConversationShapeName.GetHashCode(), "[ConversationRowNotificationHandler.GetSubscriptionProperties] Unable to resolve requestedConversationShapeName: {0}", requestedConversationShapeName);
				return ConversationRowNotificationHandler.defaultConversationViewQuerySubscriptionProperties;
			}
			Shape[] shapes = new Shape[]
			{
				ConversationShape.CreateShape()
			};
			PropertyDefinition[] specialConversationProperties = ConversationRowNotificationHandler.GetSpecialConversationProperties(responseShape);
			return RowNotificationHandler.GetPropertyDefinitionsForResponseShape(shapes, responseShape, specialConversationProperties);
		}

		private static PropertyDefinition[] GetSpecialConversationProperties(ConversationResponseShape itemShape)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (PropertyPath propertyPath in itemShape.AdditionalProperties)
			{
				PropertyUri propertyUri = propertyPath as PropertyUri;
				if (propertyUri != null && propertyUri.Uri == PropertyUriEnum.ConversationPreview)
				{
					hashSet.Add(ConversationItemSchema.ConversationPreview);
					break;
				}
			}
			return hashSet.ToArray<PropertyDefinition>();
		}

		private ConversationType GetConversationFromNotification(QueryNotification notification)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[ConversationRowNotificationHandler.GetConversationFromNotification] Start. SubscriptionId: {0}", base.SubscriptionId);
			ConversationType conv = new ConversationType();
			conv.InstanceKey = notification.Index;
			if (notification.EventType != QueryNotificationType.RowDeleted)
			{
				conv.ConversationId = new ItemId(IdConverter.ConversationIdToEwsId(base.MailboxGuid, RowNotificationHandler.GetItemProperty<ConversationId>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationId))), null);
				conv.ConversationTopic = RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationTopic));
				conv.UniqueRecipients = RowNotificationHandler.GetItemProperty<string[]>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationMVTo));
				conv.UniqueSenders = RowNotificationHandler.GetItemProperty<string[]>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationMVFrom));
				conv.LastDeliveryTime = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationLastDeliveryTime));
				conv.LastDeliveryOrRenewTime = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationLastDeliveryOrRenewTime));
				conv.Categories = RowNotificationHandler.GetItemProperty<string[]>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationCategories));
				FlagType flagType = new FlagType();
				if (RowNotificationHandler.IsPropertyDefined(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationFlagStatus)))
				{
					flagType.FlagStatus = (FlagStatus)RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationFlagStatus), 0);
				}
				conv.FlagStatus = flagType.FlagStatus;
				conv.HasAttachments = new bool?(RowNotificationHandler.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationHasAttach)));
				conv.HasIrm = new bool?(RowNotificationHandler.GetItemProperty<bool>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationHasIrm)));
				conv.MessageCount = new int?(RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationMessageCount)));
				conv.GlobalMessageCount = new int?(RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationGlobalMessageCount)));
				conv.UnreadCount = new int?(RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationUnreadMessageCount)));
				conv.GlobalUnreadCount = new int?(RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationGlobalUnreadMessageCount)));
				conv.Size = new int?(RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationMessageSize)));
				conv.ItemClasses = RowNotificationHandler.GetItemProperty<string[]>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationMessageClasses));
				conv.ImportanceString = ((ImportanceType)RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationImportance), 1)).ToString();
				StoreId[] itemProperty = RowNotificationHandler.GetItemProperty<StoreId[]>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationItemIds), new StoreId[0]);
				conv.ItemIds = Array.ConvertAll<StoreId, ItemId>(itemProperty, (StoreId s) => new ItemId(base.GetEwsId(s), null));
				StoreId[] itemProperty2 = RowNotificationHandler.GetItemProperty<StoreId[]>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationGlobalItemIds), new StoreId[0]);
				conv.GlobalItemIds = Array.ConvertAll<StoreId, ItemId>(itemProperty2, (StoreId s) => new ItemId(base.GetEwsId(s), null));
				conv.DraftStoreIds = from storeId in itemProperty2
				where DraftItemIdsProperty.IsItemInDraftsFolder(storeId, this.draftFolderId)
				select StoreId.GetStoreObjectId(storeId);
				if (conv.DraftStoreIds.FirstOrDefault<StoreId>() != null)
				{
					SimulatedWebRequestContext.Execute(base.UserContext, "ConversationNotificationDraftItemIds", delegate(MailboxSession mailboxSession, IRecipientSession adSession, RequestDetailsLogger logger)
					{
						NormalQueryView.PrepareDraftItemIds(mailboxSession, new ConversationType[]
						{
							conv
						});
					});
				}
				conv.LastModifiedTime = base.GetDateTimeProperty(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, StoreObjectSchema.LastModifiedTime));
				conv.Preview = RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationPreview));
				conv.MailboxScopeString = MailboxSearchLocation.PrimaryOnly.ToString();
				IconIndex itemProperty3 = (IconIndex)RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationReplyForwardState));
				if (itemProperty3 > (IconIndex)0)
				{
					conv.IconIndexString = itemProperty3.ToString();
				}
				itemProperty3 = (IconIndex)RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.conversationViewQuerySubscriptionProperties, ConversationItemSchema.ConversationGlobalReplyForwardState));
				if (itemProperty3 > (IconIndex)0)
				{
					conv.GlobalIconIndexString = itemProperty3.ToString();
				}
				this.LoadConversationFeedPropertiesIfRequested(notification, conv);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[ConversationRowNotificationHandler.GetConversationFromNotification] End. SubscriptionId: {0}", base.SubscriptionId);
			}
			return conv;
		}

		private void LoadConversationFeedPropertiesIfRequested(QueryNotification notification, ConversationType conv)
		{
			bool flag = false;
			int index = Array.IndexOf<PropertyDefinition>(this.SubscriptionProperties, ConversationItemSchema.ConversationInitialMemberDocumentId);
			int index2 = Array.IndexOf<PropertyDefinition>(this.SubscriptionProperties, ConversationItemSchema.ConversationMemberDocumentIds);
			int? initialMemberDocumentId = null;
			int[] memberDocumentIds = null;
			if (RowNotificationHandler.IsPropertyDefined(notification, index))
			{
				initialMemberDocumentId = RowNotificationHandler.GetItemProperty<int?>(notification, index);
				flag = true;
			}
			if (RowNotificationHandler.IsPropertyDefined(notification, index2))
			{
				memberDocumentIds = RowNotificationHandler.GetItemProperty<int[]>(notification, index2);
				flag = true;
			}
			if (flag)
			{
				SimulatedWebRequestContext.Execute(base.UserContext, "LoadConversationFeedPropertiesIfRequested", delegate(MailboxSession mailboxSession, IRecipientSession adSession, RequestDetailsLogger logger)
				{
					ConversationFeedLoader conversationFeedLoader = new ConversationFeedLoader(mailboxSession, this.TimeZone);
					conversationFeedLoader.LoadConversationFeedItems(conv, initialMemberDocumentId, memberDocumentIds);
				});
			}
		}

		private const WellKnownShapeName defaultConversationViewShapeName = WellKnownShapeName.ConversationUberListView;

		private static PropertyDefinition[] defaultConversationViewQuerySubscriptionProperties;

		private StoreObjectId draftFolderId;

		private readonly PropertyDefinition[] conversationViewQuerySubscriptionProperties;
	}
}
