using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationDataQueue
	{
		public NotificationDataQueue(int maxNotifications, Func<string> traceContextDelegate)
		{
			this.maxNotifications = maxNotifications;
			this.traceContextDelegate = traceContextDelegate;
		}

		internal bool IsEmpty
		{
			get
			{
				return this.notificationList.Count == 0;
			}
		}

		private string TraceContext
		{
			get
			{
				if (this.traceContext == null && this.traceContextDelegate != null)
				{
					this.traceContext = this.traceContextDelegate();
				}
				else
				{
					this.traceContext = string.Empty;
				}
				return this.traceContext;
			}
		}

		internal void Enqueue(NotificationData data)
		{
			lock (this.notificationListLock)
			{
				if (this.notificationList.Count >= this.maxNotifications)
				{
					this.CollapseTableNotifications();
				}
				if (!this.TryFilterNotificationData(data))
				{
					this.AddNotificationData(data);
				}
			}
		}

		internal bool Peek(out NotificationData data)
		{
			bool result;
			lock (this.notificationListLock)
			{
				if (this.notificationList.Count > 0)
				{
					data = this.notificationList[0];
					result = true;
				}
				else
				{
					data = null;
					result = false;
				}
			}
			return result;
		}

		internal bool Dequeue(out NotificationData data)
		{
			data = null;
			if (this.notificationList.Count == 0)
			{
				return false;
			}
			lock (this.notificationListLock)
			{
				if (this.notificationList.Count == 0)
				{
					return false;
				}
				data = this.notificationList[0];
				this.notificationList.RemoveAt(0);
				this.TraceQueueEvent("Dequeue", string.Empty, data);
			}
			return true;
		}

		internal NotificationData[] ToArray()
		{
			return this.notificationList.ToArray();
		}

		private static bool CompareByteArrays(byte[] array1, byte[] array2)
		{
			if (array1 != null && array2 != null && array1.Length == array2.Length)
			{
				for (int i = 0; i < array1.Length; i++)
				{
					if (array1[i] != array2[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static NotificationData CreateTableChangedNotification(NotificationData data, QueryNotification queryNotification)
		{
			return new NotificationData(data.NotificationHandleValue, data.Logon, NotificationDataQueue.queryResultChangedNotification, data.RootFolderId, data.View, data.String8Encoding);
		}

		private static NotificationData CreateTableRowAddedNotification(NotificationData data, QueryNotification queryNotification)
		{
			return new NotificationData(data.NotificationHandleValue, data.Logon, queryNotification.CreateRowAddedNotification(), data.RootFolderId, data.View, data.String8Encoding);
		}

		private void TraceQueueEvent(string eventName, string eventDescription, NotificationData data)
		{
			if (ExTraceGlobals.NotificationHandlerTracer.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				ExTraceGlobals.NotificationHandlerTracer.TracePerformance(Activity.TraceId, "{0}: <<{1}>> Handle = {2}. NotificationType = {3}. CurrentQueueSize = {4}, MaxQueueSize = {5}. {6}", new object[]
				{
					this.TraceContext,
					eventName,
					data.NotificationHandleValue,
					data.Notification.Type,
					this.notificationList.Count,
					this.maxNotifications,
					eventDescription
				});
			}
		}

		private void CollapseTableNotifications()
		{
			for (int i = 0; i < this.notificationList.Count; i++)
			{
				NotificationData notificationData = this.notificationList[i];
				if (notificationData != null && notificationData.Notification != null && notificationData.Notification.Type == NotificationType.Query)
				{
					QueryNotification queryNotification = notificationData.Notification as QueryNotification;
					if (queryNotification != null && queryNotification.EventType != QueryNotificationType.QueryResultChanged)
					{
						this.PurgeAllNotifications(notificationData.Logon, notificationData.NotificationHandleValue, i + 1);
						this.notificationList[i] = NotificationDataQueue.CreateTableChangedNotification(notificationData, queryNotification);
					}
				}
			}
		}

		private void PurgeAllNotifications(Logon logon, ServerObjectHandle handle, int startIndex = 0)
		{
			if (this.notificationList.Count > 0)
			{
				for (int i = this.notificationList.Count - 1; i >= startIndex; i--)
				{
					NotificationData notificationData = this.notificationList[i];
					if (notificationData.Logon == logon && notificationData.NotificationHandleValue == handle)
					{
						this.notificationList.RemoveAt(i);
					}
				}
			}
		}

		private bool TryCombineTableNotification(NotificationData data, QueryNotification queryNotification)
		{
			if (queryNotification.EventType == QueryNotificationType.RowModified || queryNotification.EventType == QueryNotificationType.RowDeleted)
			{
				for (int i = this.notificationList.Count - 1; i >= 0; i--)
				{
					NotificationData notificationData = this.notificationList[i];
					if (notificationData != null && notificationData.Notification != null && notificationData.Notification.Type == NotificationType.Query && data.Logon == notificationData.Logon && data.NotificationHandleValue == notificationData.NotificationHandleValue)
					{
						QueryNotification queryNotification2 = notificationData.Notification as QueryNotification;
						if (queryNotification2 != null && (queryNotification2.EventType == QueryNotificationType.RowAdded || queryNotification2.EventType == QueryNotificationType.RowModified))
						{
							if (NotificationDataQueue.CompareByteArrays(queryNotification.Index, queryNotification2.Index))
							{
								if (queryNotification.EventType == QueryNotificationType.RowModified)
								{
									if (queryNotification2.EventType == QueryNotificationType.RowAdded)
									{
										this.notificationList[i] = NotificationDataQueue.CreateTableRowAddedNotification(data, queryNotification);
									}
									else
									{
										this.notificationList[i] = data;
									}
									return true;
								}
								this.notificationList.RemoveAt(i);
								if (queryNotification2.EventType == QueryNotificationType.RowAdded)
								{
									return true;
								}
							}
							else if (NotificationDataQueue.CompareByteArrays(queryNotification.Prior, queryNotification2.Index) || NotificationDataQueue.CompareByteArrays(queryNotification.Index, queryNotification2.Prior))
							{
								return false;
							}
						}
					}
				}
			}
			return false;
		}

		private bool IsTableChangedAlreadyQueued(Logon logon, ServerObjectHandle handle)
		{
			for (int i = 0; i < this.notificationList.Count; i++)
			{
				NotificationData notificationData = this.notificationList[i];
				if (notificationData != null && notificationData.Notification != null && notificationData.Notification.Type == NotificationType.Query && notificationData.Logon == logon && notificationData.NotificationHandleValue == handle)
				{
					QueryNotification queryNotification = notificationData.Notification as QueryNotification;
					if (queryNotification != null && queryNotification.EventType == QueryNotificationType.QueryResultChanged)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool TryFilterNotificationData(NotificationData data)
		{
			if (this.notificationList.Count > 0 && data != null && data.Notification != null && data.Notification.Type == NotificationType.Query)
			{
				QueryNotification queryNotification = data.Notification as QueryNotification;
				if (queryNotification != null)
				{
					if (queryNotification.EventType == QueryNotificationType.QueryResultChanged)
					{
						this.PurgeAllNotifications(data.Logon, data.NotificationHandleValue, 0);
						this.TraceQueueEvent("Purged", "TableChanged event, purging all notifications for this table.", data);
						return false;
					}
					if (this.IsTableChangedAlreadyQueued(data.Logon, data.NotificationHandleValue))
					{
						this.TraceQueueEvent("Filtered", "TableChanged already queued for this table.", data);
						return true;
					}
					if (this.TryCombineTableNotification(data, queryNotification))
					{
						this.TraceQueueEvent("Filtered", "Combined with previous notification for this table.", data);
						return true;
					}
				}
			}
			return false;
		}

		private void AddNotificationData(NotificationData data)
		{
			if (this.notificationList.Count >= this.maxNotifications)
			{
				NotificationData data2 = this.notificationList[0];
				this.notificationList.RemoveAt(0);
				this.TraceQueueEvent("Dropped", "Queue full notification dropped.", data2);
			}
			this.notificationList.Add(data);
			this.TraceQueueEvent("Enqueue", string.Empty, data);
		}

		private static readonly QueryNotification queryResultChangedNotification = QueryNotification.CreateQueryResultChangedNotification();

		private readonly List<NotificationData> notificationList = new List<NotificationData>();

		private readonly int maxNotifications;

		private readonly object notificationListLock = new object();

		private readonly Func<string> traceContextDelegate;

		private string traceContext;
	}
}
