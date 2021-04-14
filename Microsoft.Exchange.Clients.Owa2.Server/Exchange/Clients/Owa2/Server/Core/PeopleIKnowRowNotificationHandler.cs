using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PeopleIKnowRowNotificationHandler : RowNotificationHandler
	{
		public PeopleIKnowRowNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, ExTimeZone timeZone, CultureInfo cultureInfo) : base(subscriptionId, parameters, folderId, userContext, mailboxGuid, timeZone, false)
		{
			this.cultureInfo = cultureInfo;
			OwsLogRegistry.Register("PeopleIKnowRowNotification", typeof(FindPeopleMetadata), new Type[0]);
		}

		protected PeopleIKnowRowNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, UserContext userContext, Guid mailboxGuid, RowNotifier notifier) : base(subscriptionId, parameters, folderId, userContext, mailboxGuid, notifier, false)
		{
		}

		protected override PropertyDefinition[] SubscriptionProperties
		{
			get
			{
				return BrowsePeopleInMailFolder.SenderAndUnreadProperties;
			}
		}

		protected override NotificationPayloadBase GetPayloadFromNotification(StoreObjectId folderId, QueryNotification notification)
		{
			ExTraceGlobals.PeopleIKnowNotificationsTracer.TraceDebug<string, QueryNotificationType>((long)this.GetHashCode(), "[PeopleIKnowRowNotificationHandler.GetPayloadFromNotification] SubscriptionId: {0}, NotificationEventType: {1}", base.SubscriptionId, notification.EventType);
			switch (notification.EventType)
			{
			case QueryNotificationType.RowAdded:
			case QueryNotificationType.RowModified:
				return this.GetRowNotificationPayload(notification);
			case QueryNotificationType.RowDeleted:
				return this.GetQueryResultChangedPayload(QueryNotificationType.QueryResultChanged);
			default:
				ExTraceGlobals.PeopleIKnowNotificationsTracer.TraceError<QueryNotificationType>((long)this.GetHashCode(), "[PeopleIKnowRowNotificationHandler.GetPayloadFromNotification] Unknown event type: {0}.", notification.EventType);
				return this.GetEmptyPayload();
			}
		}

		private PeopleIKnowRowNotificationPayload GetRowNotificationPayload(QueryNotification notification)
		{
			PeopleIKnowRowNotificationPayload emptyPayload = this.GetEmptyPayload();
			emptyPayload.EventType = notification.EventType;
			emptyPayload.PersonaEmailAdress = RowNotificationHandler.GetItemProperty<string>(notification, Array.IndexOf<PropertyDefinition>(this.SubscriptionProperties, MessageItemSchema.SenderSmtpAddress));
			emptyPayload.PersonaUnreadCount = RowNotificationHandler.GetItemProperty<int>(notification, Array.IndexOf<PropertyDefinition>(this.SubscriptionProperties, FolderSchema.UnreadCount));
			emptyPayload.Source = new MailboxLocation(base.MailboxGuid);
			return emptyPayload;
		}

		protected override QueryResult GetQueryResult(Folder folder)
		{
			ExTraceGlobals.PeopleIKnowNotificationsTracer.TraceDebug<string>((long)this.GetHashCode(), "[PeopleIKnowRowNotificationHandler.GetQueryResult] SubscriptionId: {0}", base.SubscriptionId);
			SendersInMailFolder sendersInMailFolder = new SendersInMailFolder(folder);
			return sendersInMailFolder.GetQueryResultGroupedBySender(BrowsePeopleInMailFolder.SenderAndUnreadProperties);
		}

		protected override void ProcessQueryResultChangedNotification()
		{
			ExTraceGlobals.PeopleIKnowNotificationsTracer.TraceDebug<string>((long)this.GetHashCode(), "[PeopleIKnowRowNotificationHandler.ProcessQueryResultChangedNotification] SubscriptionId: {0}", base.SubscriptionId);
			PeopleIKnowRowNotificationPayload queryResultChangedPayload = this.GetQueryResultChangedPayload(QueryNotificationType.QueryResultChanged);
			base.Notifier.AddFolderContentChangePayload(base.FolderId, queryResultChangedPayload);
		}

		protected override void ProcessReloadNotification()
		{
			ExTraceGlobals.PeopleIKnowNotificationsTracer.TraceDebug<string>((long)this.GetHashCode(), "[PeopleIKnowRowNotificationHandler.ProcessReloadNotification] SubscriptionId: {0}", base.SubscriptionId);
			PeopleIKnowRowNotificationPayload emptyPayload = this.GetEmptyPayload();
			emptyPayload.EventType = QueryNotificationType.Reload;
			base.Notifier.AddFolderContentChangePayload(base.FolderId, emptyPayload);
		}

		private PeopleIKnowRowNotificationPayload GetQueryResultChangedPayload(QueryNotificationType queryNotificationType)
		{
			PeopleIKnowRowNotificationPayload emptyPayload = this.GetEmptyPayload();
			emptyPayload.EventType = queryNotificationType;
			this.SetPayloadOnQueryResultChangedNotification(emptyPayload);
			return emptyPayload;
		}

		private PeopleIKnowRowNotificationPayload GetEmptyPayload()
		{
			return new PeopleIKnowRowNotificationPayload
			{
				SubscriptionId = base.SubscriptionId,
				Source = new MailboxLocation(base.MailboxGuid)
			};
		}

		protected virtual void SetPayloadOnQueryResultChangedNotification(PeopleIKnowRowNotificationPayload payload)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "PeopleIKnowRowNotification", delegate(MailboxSession mailboxSession, IRecipientSession adSession, RequestDetailsLogger logger)
			{
				FindPeopleParameters parameters = this.CreateFindPeopleParameters(logger);
				BrowsePeopleInMailFolder browsePeopleInMailFolder = new BrowsePeopleInMailFolder(parameters, mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.FromFavoriteSenders), NullTracer.Instance);
				FindPeopleResult findPeopleResult = browsePeopleInMailFolder.Execute();
				payload.Personas = findPeopleResult.PersonaList;
			});
		}

		private FindPeopleParameters CreateFindPeopleParameters(RequestDetailsLogger logger)
		{
			return new FindPeopleParameters
			{
				Paging = PeopleIKnowRowNotificationHandler.CreateIndexedPageView(0, 20),
				PersonaShape = Persona.DefaultPersonaShape,
				CultureInfo = this.cultureInfo,
				Logger = logger
			};
		}

		private static IndexedPageView CreateIndexedPageView(int offset, int maxEntries)
		{
			return new IndexedPageView
			{
				Origin = BasePagingType.PagingOrigin.Beginning,
				Offset = offset,
				MaxRows = maxEntries
			};
		}

		private const string LoggerEventId = "PeopleIKnowRowNotification";

		private readonly CultureInfo cultureInfo;
	}
}
