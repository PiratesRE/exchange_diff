using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FolderContentChangeNotificationHandler : NotificationHandlerBase
	{
		private string MailboxSessionDisplayName
		{
			get
			{
				if (this.mailboxSessionDisplayName != null)
				{
					return this.mailboxSessionDisplayName;
				}
				return string.Empty;
			}
		}

		private ListViewContents2 ItemList
		{
			get
			{
				if (this.listView is GroupByList2)
				{
					return ((GroupByList2)this.listView).ItemList;
				}
				return this.listView;
			}
		}

		internal QueryResult QueryResult
		{
			get
			{
				QueryResult result;
				lock (this.syncRoot)
				{
					if (this.isDisposed || this.needReinitSubscriptions)
					{
						result = null;
					}
					else
					{
						result = this.result;
					}
				}
				return result;
			}
		}

		internal bool ShouldIgnoreNotification
		{
			get
			{
				return this.payload.ShouldIgnoreNotification(this.contextFolderId);
			}
		}

		private bool IsConversationView
		{
			get
			{
				return this.isConversationView;
			}
		}

		internal ExDateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		internal bool IssueDelayedLoadfresh
		{
			get
			{
				return this.issueDelayedLoadfresh;
			}
		}

		internal bool NeedReinitSubscriptions
		{
			get
			{
				return this.needReinitSubscriptions;
			}
		}

		internal bool AllowFolderRefreshNotification
		{
			get
			{
				return this.needReinitSubscriptions || this.missedNotifications;
			}
		}

		internal SortBy[] SortBy
		{
			get
			{
				return this.sortBy;
			}
		}

		internal FolderVirtualListViewFilter FolderFilter
		{
			get
			{
				return this.folderFilter;
			}
		}

		internal FolderContentChangeNotificationHandler(UserContext userContext, MailboxSession mailboxSession, OwaStoreObjectId contextFolderId, OwaStoreObjectId dataFolderId, QueryResult result, EmailPayload emailPayload, ListViewContents2 listView, PropertyDefinition[] subscriptionProperties, Dictionary<PropertyDefinition, int> propertyMap, SortBy[] sortBy, FolderVirtualListViewFilter folderFilter, bool isConversationView) : base(userContext, mailboxSession)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			this.listView = listView;
			this.subscriptionProperties = subscriptionProperties;
			this.contextFolderId = contextFolderId;
			this.dataFolderId = dataFolderId;
			this.result = result;
			this.payload = emailPayload;
			this.propertyMap = propertyMap;
			this.sortBy = sortBy;
			this.folderFilter = folderFilter;
			this.isConversationView = isConversationView;
			this.mailboxSessionDisplayName = mailboxSession.DisplayName;
			this.InitializeCachedObjectsThatNeedMailboxSession();
		}

		internal bool TrySubscribe(ConnectionDroppedNotificationHandler connectionDroppedNotificationHandler)
		{
			bool result;
			lock (this.syncRoot)
			{
				if (this.isDisposed)
				{
					throw new InvalidOperationException("Cannot call Subscribe on a Disposed object");
				}
				if (!this.userContext.LockedByCurrentThread())
				{
					throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscribing for folder content change notifications for user {0}", this.MailboxSessionDisplayName);
				try
				{
					this.InitSubscription();
				}
				catch (MapiExceptionObjectDisposed)
				{
					OwaMapiNotificationHandler.DisposeXSOObjects(this.result);
					return false;
				}
				catch (ObjectDisposedException)
				{
					return false;
				}
				catch (StoragePermanentException)
				{
					return false;
				}
				catch (StorageTransientException)
				{
					return false;
				}
				this.payload.AttachFolderContentChangeNotificationHandler(this.contextFolderId, this);
				connectionDroppedNotificationHandler.OnConnectionDropped += this.HandleConnectionDroppedNotification;
				result = true;
			}
			return result;
		}

		protected override void InitSubscription()
		{
			lock (this.syncRoot)
			{
				if (this.mapiSubscription == null)
				{
					this.mapiSubscription = Subscription.Create(this.result, new NotificationHandler(this.HandleNotification));
					if (Globals.ArePerfCountersEnabled)
					{
						OwaSingleCounters.ActiveMailboxSubscriptions.Increment();
					}
				}
			}
		}

		internal void RemoveSubscription(ConnectionDroppedNotificationHandler connectionDroppedNotificationHandler)
		{
			lock (this.syncRoot)
			{
				if (!this.userContext.LockedByCurrentThread())
				{
					throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
				}
				if (connectionDroppedNotificationHandler != null)
				{
					connectionDroppedNotificationHandler.OnConnectionDropped -= this.HandleConnectionDroppedNotification;
				}
				this.needReinitSubscriptions = true;
				this.payload.DetachFolderContentChangeNotificationHandler(this.contextFolderId);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Removed folder content change notification subscription for user {0}", this.MailboxSessionDisplayName);
			}
		}

		internal override void HandleNotification(Notification xsoNotification)
		{
			try
			{
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.TotalMailboxNotifications.Increment();
				}
				lock (this.syncRoot)
				{
					if (!this.isDisposed && !this.missedNotifications && !this.needReinitSubscriptions && !this.ShouldIgnoreNotification)
					{
						if (xsoNotification == null)
						{
							ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Received NULL notification");
						}
						else
						{
							QueryNotification queryNotification = (QueryNotification)xsoNotification;
							if (!this.ProcessErrorNotification(queryNotification))
							{
								switch (queryNotification.EventType)
								{
								case QueryNotificationType.QueryResultChanged:
								case QueryNotificationType.Reload:
									this.ProcessReloadNotification();
									break;
								case QueryNotificationType.RowAdded:
								case QueryNotificationType.RowDeleted:
								case QueryNotificationType.RowModified:
									this.payload.AddFolderContentChangePayload(this.contextFolderId, queryNotification);
									break;
								}
								this.payload.PickupData();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Unexpected exception in FolderContentChangeNotificationHandler:HandleNotification for user {0}. Exception: {1}", this.MailboxSessionDisplayName, ex.Message);
				this.missedNotifications = true;
			}
		}

		internal void ProcessNotification(TextWriter writer, QueryNotification notification, out bool successfullyProcessed)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (notification == null)
			{
				throw new ArgumentNullException("notification");
			}
			successfullyProcessed = true;
			if (this.isDisposed || this.missedNotifications || this.needReinitSubscriptions)
			{
				string message = string.Format("Ignoring folder content change notification for user {0}, isDisposed:{1}, missedNotifications:{2}, needReinitSubscriptions:{3}", new object[]
				{
					this.MailboxSessionDisplayName,
					this.isDisposed,
					this.missedNotifications,
					this.needReinitSubscriptions
				});
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), message);
				return;
			}
			try
			{
				QueryNotificationType eventType = notification.EventType;
				Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
				switch (eventType)
				{
				case QueryNotificationType.RowAdded:
					this.ProcessRowAddedNotification(writer, notification);
					break;
				case QueryNotificationType.RowDeleted:
					this.ProcessRowDeletedNotification(writer, notification);
					break;
				case QueryNotificationType.RowModified:
					this.ProcessRowModifiedNotification(writer, notification);
					break;
				default:
					throw new ArgumentException("Invalid queryNotificationType :" + eventType);
				}
			}
			catch (Exception ex)
			{
				if (!this.isDisposed && !this.missedNotifications && !this.needReinitSubscriptions)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in FolderContentChangeNotificationHandler:ProcessNotification. Exception: {0}", ex.Message);
					if (Globals.SendWatsonReports)
					{
						ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending watson report");
						ExWatson.AddExtraData(this.GetExtraWatsonData());
						ExWatson.SendReport(ex, ReportOptions.None, null);
					}
					successfullyProcessed = false;
					this.missedNotifications = true;
				}
			}
		}

		internal bool HasDataFolderChanged(OwaStoreObjectId inDataFolderId)
		{
			if (inDataFolderId == null)
			{
				throw new ArgumentNullException("inDataFolderId");
			}
			return !this.dataFolderId.Equals(inDataFolderId);
		}

		internal override void HandlePendingGetTimerCallback()
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					if (this.needReinitSubscriptions)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Sending refresh payload to the client for user {0}", this.MailboxSessionDisplayName);
						this.AddFolderRefreshPayload();
					}
				}
			}
		}

		internal override void DisposeInternal()
		{
			this.DisposeInternal(false);
		}

		internal override void DisposeInternal(bool doNotDisposeQueryResult)
		{
			base.DisposeInternal(doNotDisposeQueryResult);
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ActiveMailboxSubscriptions.Decrement();
			}
		}

		private void ProcessRowAddedNotification(TextWriter writer, QueryNotification notification)
		{
			this.CreateNotificationDataSource(notification);
			this.AddRowNotificationPrefix(writer, notification);
			writer.Write(",'");
			Utilities.JavascriptEncode(Convert.ToBase64String(notification.Prior), writer);
			writer.Write("','");
			this.AddRowData(writer, notification.EventType, ListViewContents2.ListViewRowType.RenderOnlyRow1);
			writer.Write("','");
			this.AddRowData(writer, notification.EventType, ListViewContents2.ListViewRowType.RenderOnlyRow2);
			writer.Write("','");
			this.AddRowProperties(writer, notification.EventType);
			writer.Write("'");
			this.AddRowNotificationSuffix(writer);
			this.listView.DataSource = null;
			this.ItemList.DataSource = null;
		}

		private void ProcessRowDeletedNotification(TextWriter writer, QueryNotification notification)
		{
			this.AddRowNotificationPrefix(writer, notification);
			this.AddRowNotificationSuffix(writer);
		}

		private void ProcessRowModifiedNotification(TextWriter writer, QueryNotification notification)
		{
			this.CreateNotificationDataSource(notification);
			this.AddRowNotificationPrefix(writer, notification);
			writer.Write(",'");
			Utilities.JavascriptEncode(Convert.ToBase64String(notification.Prior), writer);
			writer.Write("','");
			this.AddRowData(writer, notification.EventType, ListViewContents2.ListViewRowType.RenderOnlyRow1);
			writer.Write("','");
			this.AddRowData(writer, notification.EventType, ListViewContents2.ListViewRowType.RenderOnlyRow2);
			writer.Write("','");
			this.AddRowProperties(writer, notification.EventType);
			writer.Write("'");
			this.AddRowNotificationSuffix(writer);
			this.listView.DataSource = null;
			this.ItemList.DataSource = null;
		}

		private void ProcessReloadNotification()
		{
			this.AddFolderRefreshPayload();
		}

		private void AddRowNotificationPrefix(TextWriter writer, QueryNotification notification)
		{
			writer.Write("folderChangeNotification('");
			Utilities.JavascriptEncode(this.contextFolderId.StoreObjectId.ToBase64String(), writer);
			writer.Write("',['");
			QueryNotificationType eventType = notification.EventType;
			switch (eventType)
			{
			case QueryNotificationType.RowAdded:
			case QueryNotificationType.RowDeleted:
			case QueryNotificationType.RowModified:
				writer.Write((int)eventType);
				writer.Write("','");
				Utilities.JavascriptEncode(Convert.ToBase64String(notification.Index), writer);
				writer.Write("'");
				return;
			default:
				throw new ArgumentException("invalid value for notificationType");
			}
		}

		private void AddRowData(TextWriter writer, QueryNotificationType notificationType, ListViewContents2.ListViewRowType rowTypeToRender)
		{
			bool renderContainer = notificationType != QueryNotificationType.RowModified;
			bool showFlag = this.ItemList.ViewDescriptor.ContainsColumn(ColumnId.FlagDueDate);
			StringBuilder stringBuilder = new StringBuilder(1280);
			using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
			{
				this.ItemList.RenderRow(stringWriter, showFlag, rowTypeToRender, renderContainer);
			}
			Utilities.JavascriptEncode(stringBuilder.ToString(), writer, true);
			stringBuilder.Length = 0;
		}

		private void AddRowProperties(TextWriter writer, QueryNotificationType notificationType)
		{
			Utilities.JavascriptEncode(this.ItemList.DataSource.GetItemProperty<bool>(MessageItemSchema.IsRead, true) ? "1" : "0", writer);
			writer.Write("','");
			string s = string.Empty;
			if (this.listView is GroupByList2)
			{
				s = ((GroupByList2)this.listView).GetItemGroupByValueString();
			}
			Utilities.JavascriptEncode(s, writer);
			if (notificationType == QueryNotificationType.RowModified && !this.isConversationView)
			{
				writer.Write("','");
				Utilities.JavascriptEncode(this.ItemList.DataSource.GetItemProperty<string>(ItemSchema.Subject, string.Empty), writer);
			}
		}

		private void AddRowNotificationSuffix(TextWriter writer)
		{
			writer.Write("]);");
		}

		private void CreateNotificationDataSource(QueryNotification notification)
		{
			this.listView.DataSource = new ListViewNotificationDataSource(this.userContext, this.dataFolderId.StoreObjectId, this.isConversationView, this.propertyMap, this.sortBy, notification);
			this.ItemList.DataSource = this.listView.DataSource;
		}

		private bool ProcessErrorNotification(QueryNotification notification)
		{
			bool flag = false;
			if (notification.ErrorCode != 0)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Error in Notification: {0}", notification.ErrorCode);
				flag = true;
			}
			else if (notification.EventType == QueryNotificationType.Error)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Error in Notification, Type is QueryNotificationType.Error");
				flag = true;
			}
			else if ((notification.EventType == QueryNotificationType.RowAdded || notification.EventType == QueryNotificationType.RowModified) && notification.Row.Length < this.subscriptionProperties.Length)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<int, int>((long)this.GetHashCode(), "XSO gave an incomplete notification, expected row length {0}, actual row length {1}", notification.Row.Length, this.subscriptionProperties.Length);
				flag = true;
			}
			try
			{
				TimeGroupByList2 timeGroupByList = this.listView as TimeGroupByList2;
				if (timeGroupByList != null && !timeGroupByList.IsValid())
				{
					flag = true;
					this.needReinitSubscriptions = true;
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in FolderContentChangeNotificationHandler:ProcessErrorNotification. Exception: {0}", ex.ToString());
				if (Globals.SendWatsonReports)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending watson report");
					ExWatson.AddExtraData(this.GetExtraWatsonData());
					ExWatson.SendReport(ex);
				}
				flag = true;
				this.needReinitSubscriptions = true;
			}
			if (flag)
			{
				this.AddFolderRefreshPayload();
				this.payload.PickupData();
				return true;
			}
			return false;
		}

		private void AddFolderRefreshPayload()
		{
			this.payload.AddFolderRefreshPayload(this.contextFolderId);
			this.missedNotifications = true;
		}

		private void InitializeCachedObjectsThatNeedMailboxSession()
		{
			this.userContext.GetMasterCategoryList();
			if (this.isConversationView)
			{
				IList<StoreObjectId> excludedFolderIds = ((ConversationItemList2)this.ItemList).ExcludedFolderIds;
				StoreObjectId draftsFolderId = this.userContext.DraftsFolderId;
			}
		}

		private void ThrottleNotifications()
		{
			ExDateTime now = ExDateTime.Now;
			this.notificationCountSinceLastTimer++;
			if (now - this.throttleDurationStart >= this.throttleDuration)
			{
				if (this.notificationCountSinceLastTimer >= 300)
				{
					this.issueDelayedLoadfresh = true;
					return;
				}
				this.notificationCountSinceLastTimer = 0;
			}
		}

		private string GetExtraWatsonData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("OWA Version: ");
			stringBuilder.Append(Globals.ApplicationVersion);
			stringBuilder.AppendLine();
			if (this.listView is GroupByList2)
			{
				GroupByList2 groupByList = (GroupByList2)this.listView;
				stringBuilder.Append(string.Format("GroupByList: SortColumn: {0}, SortOrder: {1}", groupByList.GroupByColumn.Id, groupByList.SortOrder));
			}
			else
			{
				ItemList2 itemList = (ItemList2)this.ItemList;
				stringBuilder.Append(string.Format("NOT GroupByList: SortColumn: {0}, SortOrder: {1}", itemList.SortedColumn.Id, itemList.SortOrder));
			}
			stringBuilder.AppendLine();
			if (this.userContext != null && !Globals.DisableBreadcrumbs)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(this.userContext.DumpBreadcrumbs());
			}
			return stringBuilder.ToString();
		}

		private const int ThrottleDurationSeconds = 30;

		private const int ThrottleThreshold = 300;

		private OwaStoreObjectId contextFolderId;

		private OwaStoreObjectId dataFolderId;

		private EmailPayload payload;

		private ListViewContents2 listView;

		private bool isConversationView;

		private PropertyDefinition[] subscriptionProperties;

		private ExDateTime creationTime = ExDateTime.Now;

		private Dictionary<PropertyDefinition, int> propertyMap;

		private SortBy[] sortBy;

		private FolderVirtualListViewFilter folderFilter;

		private string mailboxSessionDisplayName;

		private TimeSpan throttleDuration = new TimeSpan(0, 0, 30);

		private bool issueDelayedLoadfresh;

		private ExDateTime throttleDurationStart = ExDateTime.Now;

		private int notificationCountSinceLastTimer;
	}
}
