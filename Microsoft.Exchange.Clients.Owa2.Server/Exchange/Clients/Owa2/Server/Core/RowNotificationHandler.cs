using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class RowNotificationHandler : MapiNotificationHandlerBase
	{
		public RowNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, ExTimeZone timeZone, bool remoteSubscription) : this(subscriptionId, parameters, folderId, userContext, mailboxGuid, new RowNotifier(subscriptionId, userContext, mailboxGuid), remoteSubscription)
		{
			this.timeZone = timeZone;
		}

		protected RowNotificationHandler(string subscriptionId, SubscriptionParameters parameters, StoreObjectId folderId, IMailboxContext userContext, Guid mailboxGuid, RowNotifier notifier, bool remoteSubscription) : base(subscriptionId, userContext, remoteSubscription)
		{
			this.mailboxGuid = mailboxGuid;
			this.notifier = notifier;
			this.notifier.RegisterWithPendingRequestNotifier();
			this.SetSubscriptionParameter(parameters);
			this.folderId = folderId;
			this.originalFolderId = folderId;
			this.originalFolderIdAsString = parameters.FolderId;
		}

		internal ExDateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		internal RowNotifier Notifier
		{
			get
			{
				return this.notifier;
			}
			set
			{
				this.notifier = value;
			}
		}

		internal SubscriptionParameters SubscriptionParameters { get; set; }

		internal StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		internal StoreId SearchFolderId
		{
			get
			{
				return this.searchFolderId;
			}
			set
			{
				this.searchFolderId = value;
			}
		}

		internal ExTimeZone TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
			}
		}

		internal int RefCount
		{
			get
			{
				return this.refcount;
			}
			set
			{
				this.refcount = value;
			}
		}

		protected abstract PropertyDefinition[] SubscriptionProperties { get; }

		protected Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		protected SortBy[] SortBy
		{
			get
			{
				return this.sortBy;
			}
		}

		protected ViewFilter ViewFilter
		{
			get
			{
				return this.viewFilter;
			}
		}

		protected ClutterFilter ClutterFilter
		{
			get
			{
				return this.clutterFilter;
			}
		}

		protected string FromFilter
		{
			get
			{
				return this.fromFilter;
			}
		}

		private void SetSubscriptionParameter(SubscriptionParameters parameters)
		{
			this.SubscriptionParameters = parameters;
			if (parameters.SortBy != null)
			{
				this.sortBy = SortResults.ToXsoSortBy(parameters.SortBy);
			}
			if (parameters.Filter != ViewFilter.All)
			{
				this.viewFilter = parameters.Filter;
			}
			if (parameters.ClutterFilter != ClutterFilter.All)
			{
				this.clutterFilter = parameters.ClutterFilter;
			}
			if (!string.IsNullOrEmpty(parameters.FromFilter))
			{
				this.fromFilter = parameters.FromFilter;
			}
		}

		internal override void HandleNotificationInternal(Notification notification, MapiNotificationsLogEvent logEvent, object context)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress, string>((long)this.GetHashCode(), "RowNotificationHandler.HandleNotification row notification received. User: {0}. SubscriptionId: {1}", base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
			lock (base.SyncRoot)
			{
				if (base.IsDisposed_Reentrant)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandlerBase.HandleNotificationInternal for {0}: Ignoring notification because we're disposed (reentrant).", base.GetType().Name);
				}
				else if (notification == null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress, string>((long)this.GetHashCode(), "RowNotificationHandler.HandleNotification: Received a null Row Notification object for user {0} SubscriptionId: {1}", base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
					logEvent.NullNotification = true;
				}
				else
				{
					QueryNotification queryNotification = (QueryNotification)notification;
					if (this.ProcessErrorNotification(queryNotification))
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress, string>((long)this.GetHashCode(), "RowNotificationHandler.HandleNotification: Received a invalid Row Notification object for user {0} SubscriptionId: {1}", base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
						logEvent.InvalidNotification = true;
					}
					else
					{
						QueryNotificationType eventType = queryNotification.EventType;
						switch (eventType)
						{
						case QueryNotificationType.QueryResultChanged:
							ExTraceGlobals.NotificationsCallTracer.TraceDebug<QueryNotificationType, SmtpAddress, string>((long)this.GetHashCode(), "RowNotificationHandler.HandleNotification: EventType {0}. Calling ProcessQueryResultChangedNotification. user {1} SubscriptionId: {2}", eventType, base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
							this.ProcessQueryResultChangedNotification();
							goto IL_1EE;
						case QueryNotificationType.RowAdded:
						case QueryNotificationType.RowDeleted:
						case QueryNotificationType.RowModified:
							ExTraceGlobals.NotificationsCallTracer.TraceDebug<QueryNotificationType, SmtpAddress, string>((long)this.GetHashCode(), "RowNotificationHandler.HandleNotification: EventType {0} for user {1} SubscriptionId: {2}", eventType, base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
							this.notifier.AddFolderContentChangePayload(this.folderId, this.GetPayloadFromNotification(this.folderId, queryNotification));
							goto IL_1EE;
						case QueryNotificationType.Reload:
							ExTraceGlobals.NotificationsCallTracer.TraceDebug<QueryNotificationType, SmtpAddress, string>((long)this.GetHashCode(), "RowNotificationHandler.HandleNotification: EventType {0}. Calling ProcessReloadNotification. user {1} SubscriptionId: {2}", eventType, base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
							this.ProcessReloadNotification();
							goto IL_1EE;
						}
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<QueryNotificationType, SmtpAddress, string>((long)this.GetHashCode(), "RowNotificationHandler.HandleNotification: EventType {0}. Ignoring. user {1} SubscriptionId: {2}", eventType, base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
						IL_1EE:
						this.notifier.PickupData();
					}
				}
			}
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.HandlePendingGetTimerCallback Start. SubscriptionId: {0}", base.SubscriptionId);
			lock (base.SyncRoot)
			{
				base.InitSubscription();
				if (base.MissedNotifications)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.HandlePendingGetTimerCallback this.MissedNotifications == true. SubscriptionId: {0}", base.SubscriptionId);
					base.NeedRefreshPayload = true;
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.HandlePendingGetTimerCallback setting this.MissedNotifications = false. SubscriptionId: {0}", base.SubscriptionId);
				base.MissedNotifications = false;
			}
			if (base.NeedRefreshPayload)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.HandlePendingGetTimerCallback NeedRefreshPayload.  SubscriptionId: {0}", base.SubscriptionId);
				this.AddFolderRefreshPayload();
				logEvent.RefreshPayloadSent = true;
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.HandlePendingGetTimerCallback setting this.NeedRefreshPayload = false, since we have called AddFolderRefresshPayload.  SubscriptionId: {0}", base.SubscriptionId);
				base.NeedRefreshPayload = false;
			}
			this.notifier.PickupData();
		}

		protected static T GetItemProperty<T>(QueryNotification notification, int index)
		{
			return RowNotificationHandler.GetItemProperty<T>(notification, index, default(T));
		}

		protected static T GetItemProperty<T>(QueryNotification notification, int index, T defaultValue)
		{
			if (!RowNotificationHandler.IsPropertyDefined(notification, index))
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

		protected static bool IsPropertyDefined(QueryNotification notification, int index)
		{
			return index >= 0 && index < notification.Row.Length && notification.Row[index] != null && !(notification.Row[index] is PropertyError);
		}

		protected static PropertyDefinition[] GetPropertyDefinitionsForResponseShape(IEnumerable<Shape> shapes, ResponseShape responseShape, params PropertyDefinition[] specialProperties)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			foreach (PropertyPath propertyPath in responseShape.AdditionalProperties)
			{
				ExtendedPropertyUri extendedPropertyUri = propertyPath as ExtendedPropertyUri;
				if (extendedPropertyUri != null)
				{
					list.Add(extendedPropertyUri.ToPropertyDefinition());
				}
				else
				{
					foreach (Shape shape in shapes)
					{
						PropertyInformation propertyInformation;
						if (shape.TryGetPropertyInformation(propertyPath, out propertyInformation))
						{
							list.AddRange(propertyInformation.GetPropertyDefinitions(null));
							break;
						}
					}
				}
			}
			if (specialProperties != null)
			{
				list.AddRange(specialProperties);
			}
			return list.ToArray();
		}

		protected abstract NotificationPayloadBase GetPayloadFromNotification(StoreObjectId folderId, QueryNotification notification);

		protected void GetPartialPayloadFromNotification(RowNotificationPayload payload, QueryNotification notification)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.GetPartialPayloadFromNotification Start. SubscriptionId: {0}", base.SubscriptionId);
			payload.SubscriptionId = base.SubscriptionId;
			payload.EventType = notification.EventType;
			payload.Prior = Convert.ToBase64String(notification.Prior);
			payload.FolderId = StoreId.StoreIdToEwsId(this.mailboxGuid, this.folderId);
		}

		protected override void InitSubscriptionInternal()
		{
			if (!base.UserContext.MailboxSessionLockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method RowNotificationHandler.InitSubscriptionInternal");
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.InitSubscriptionInternal Start. SubscriptionId: {0}", base.SubscriptionId);
			if (!SearchUtil.IsViewFilterNonQuery(this.viewFilter) || this.clutterFilter != ClutterFilter.All)
			{
				this.CreateSubscriptionForFilteredView();
			}
			else
			{
				if (this.folderId == null)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "RowNotificationHandler: OriginalStoreFolderId {0} OriginalStoreFolderIdAsString {1} FolderId {2}", new object[]
					{
						this.originalFolderId,
						this.originalFolderIdAsString,
						this.folderId
					});
					throw new OwaInvalidOperationException(message);
				}
				using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, this.folderId))
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.InitSubscriptionInternal create folder subscription {0}", base.SubscriptionId);
					this.CreateSubscription(folder);
				}
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.InitSubscriptionInternal End. subscription {0}", base.SubscriptionId);
		}

		protected virtual QueryResult GetQueryResult(Folder folder)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.GetQueryResult. subscription {0}", base.SubscriptionId);
			QueryFilter queryFilter = null;
			SortBy[] itemQuerySortBy = this.SortBy;
			if (!string.IsNullOrEmpty(this.FromFilter))
			{
				queryFilter = PeopleIKnowQuery.GetItemQueryFilter(this.FromFilter);
				itemQuerySortBy = PeopleIKnowQuery.GetItemQuerySortBy(this.SortBy);
			}
			return folder.ItemQuery(ItemQueryType.None, queryFilter, itemQuerySortBy, this.SubscriptionProperties);
		}

		protected virtual void ProcessReloadNotification()
		{
		}

		protected virtual void ProcessQueryResultChangedNotification()
		{
		}

		protected void AddFolderRefreshPayload()
		{
			ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)this.GetHashCode(), "RowNotificationHandler::AddFolderRefreshPayload - called for subscription: {0}", base.SubscriptionId);
			this.notifier.AddFolderRefreshPayload(this.folderId, base.SubscriptionId);
		}

		protected bool ProcessErrorNotification(QueryNotification notification)
		{
			bool flag = false;
			if (notification.ErrorCode != 0)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<int, string>((long)this.GetHashCode(), "[RowNotificationHandler::ProcessErrorNotification]. Error in Notification: {0}. Subscription: {1}", notification.ErrorCode, base.SubscriptionId);
				flag = true;
			}
			else if (notification.EventType == QueryNotificationType.Error)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)this.GetHashCode(), "[RowNotificationHandler::ProcessErrorNotification] Error in Notification, Type is QueryNotificationType.Error. Subscription: {0}", base.SubscriptionId);
				flag = true;
			}
			else if ((notification.EventType == QueryNotificationType.RowAdded || notification.EventType == QueryNotificationType.RowModified) && notification.Row.Length < this.SubscriptionProperties.Length)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<int, int, string>((long)this.GetHashCode(), "[RowNotificationHandler::ProcessErrorNotification] XSO gave an incomplete notification, expected row length {0}, actual row length {1}. Subscription: {2}", notification.Row.Length, this.SubscriptionProperties.Length, base.SubscriptionId);
				flag = true;
			}
			if (flag)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)this.GetHashCode(), "[RowNotificationHandler::ProcessErrorNotification] Notification has error, calling AddFolderRefreshPayload and returning true. Subscription: {0}", base.SubscriptionId);
				this.AddFolderRefreshPayload();
				this.notifier.PickupData();
				return true;
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandler::ProcessErrorNotification]. Returning false, no error to process. Subscription: {0}", base.SubscriptionId);
			return false;
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

		protected string GetDateTimeProperty(QueryNotification notification, int index)
		{
			ExDateTime itemProperty = RowNotificationHandler.GetItemProperty<ExDateTime>(notification, index, ExDateTime.MinValue);
			if (ExDateTime.MinValue.Equals(itemProperty))
			{
				return null;
			}
			return ExDateTimeConverter.ToOffsetXsdDateTime(itemProperty, this.timeZone);
		}

		protected string GetEwsId(StoreId storeId)
		{
			if (storeId == null)
			{
				return null;
			}
			return StoreId.StoreIdToEwsId(this.mailboxGuid, storeId);
		}

		private void CreateSubscriptionForFilteredView()
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.CreateSubscriptionForFilteredView Start. SubscriptionId: {0}", base.SubscriptionId);
			OwaSearchContext owaSearchContext = new OwaSearchContext();
			owaSearchContext.ViewFilter = (OwaViewFilter)SearchUtil.GetViewFilterForSearchFolder(this.viewFilter, this.clutterFilter);
			owaSearchContext.SearchFolderId = this.searchFolderId;
			owaSearchContext.FolderIdToSearch = this.folderId;
			owaSearchContext.RequestTimeZone = this.timeZone;
			owaSearchContext.FromFilter = this.fromFilter;
			StoreObjectId defaultFolderId = base.UserContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders);
			using (SearchFolder owaViewFilterSearchFolder = SearchUtil.GetOwaViewFilterSearchFolder(owaSearchContext, base.UserContext.MailboxSession, defaultFolderId, this.sortBy, CallContext.Current))
			{
				if (owaViewFilterSearchFolder == null)
				{
					throw new ArgumentNullException(string.Format("RowNotificationHandler::CreateSubscriptionForFilteredView - null searchFolder returned for subscriptionId: {0}. ViewFilter: {1}; Source folder id: {2}", base.SubscriptionId, this.viewFilter, this.SubscriptionParameters.FolderId.ToString()));
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, ViewFilter, string>((long)this.GetHashCode(), "RowNotificationHandler.CreateSubscriptionForFilteredView create filtered view subscriptionId: {0} . ViewFilter: {1}; Source folder id: {2}", base.SubscriptionId, this.viewFilter, this.SubscriptionParameters.FolderId);
				this.searchFolderId = owaSearchContext.SearchFolderId;
				this.CreateSubscription(owaViewFilterSearchFolder);
			}
		}

		private void CreateSubscription(Folder subscriptionfolder)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.CreateSubscription Start for subscription {0}", base.SubscriptionId);
			base.QueryResult = this.GetQueryResult(subscriptionfolder);
			base.QueryResult.GetRows(base.QueryResult.EstimatedRowCount);
			base.Subscription = Subscription.Create(base.QueryResult, new NotificationHandler(base.HandleNotification));
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RowNotificationHandler.CreateSubscription End for subscription {0}", base.SubscriptionId);
		}

		private readonly Guid mailboxGuid;

		private readonly StoreObjectId folderId;

		private StoreId searchFolderId;

		private ViewFilter viewFilter;

		private ClutterFilter clutterFilter;

		private string fromFilter;

		private SortBy[] sortBy;

		private RowNotifier notifier;

		private ExDateTime creationTime = ExDateTime.Now;

		private ExTimeZone timeZone;

		private int refcount;

		private readonly StoreObjectId originalFolderId;

		private readonly string originalFolderIdAsString;
	}
}
