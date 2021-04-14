using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class EmailPayload : IPendingRequestNotifier
	{
		internal EmailPayload(UserContext userContext, MailboxSession mailboxSession, OwaMapiNotificationHandler omnhParent)
		{
			this.folderCountTable = new Dictionary<OwaStoreObjectId, ItemCountPair>();
			this.folderRefreshList = new List<OwaStoreObjectId>();
			this.payloadStringReminderChanges = new StringBuilder(256);
			this.payloadStringNewMail = new StringBuilder(256);
			this.payloadStringRefreshAll = new StringBuilder(256);
			this.payloadStringQuota = new StringBuilder(256);
			this.payloadStringReminderNotify = new StringBuilder(256);
			this.folderContentChangeNotifications = new Dictionary<OwaStoreObjectId, EmailPayload.FCNHState>();
			this.userContext = userContext;
			this.mailboxSessionDisplayName = string.Copy(mailboxSession.DisplayName);
			this.omnhParent = omnhParent;
			this.connectionAliveTimerCount = 1;
		}

		public event DataAvailableEventHandler DataAvailable;

		public bool ShouldThrottle
		{
			get
			{
				return true;
			}
		}

		public string ReadDataAndResetState()
		{
			string result = null;
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "EmailPayload.ReadDataAndResetState. Mailbox: {0}", this.mailboxSessionDisplayName);
			lock (this)
			{
				this.containsDataToPickup = false;
				if (this.payloadStringRefreshAll.Length > 0)
				{
					result = this.payloadStringRefreshAll.ToString();
				}
				else
				{
					StringBuilder stringBuilder = null;
					StringBuilder stringBuilder2 = null;
					if (this.folderCountTable.Count > 0)
					{
						stringBuilder = new StringBuilder();
						using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
						{
							foreach (KeyValuePair<OwaStoreObjectId, ItemCountPair> keyValuePair in this.folderCountTable)
							{
								OwaStoreObjectId key = keyValuePair.Key;
								ItemCountPair value = keyValuePair.Value;
								stringWriter.Write("updMapiCnt(\"");
								Utilities.JavascriptEncode(key.ToBase64String(), stringWriter);
								stringWriter.Write(string.Concat(new object[]
								{
									"\",",
									value.ItemCount,
									",",
									value.UnreadItemCount,
									");"
								}));
							}
						}
					}
					stringBuilder2 = new StringBuilder();
					if (this.folderRefreshList.Count > 0)
					{
						using (StringWriter stringWriter2 = new StringWriter(stringBuilder2, CultureInfo.InvariantCulture))
						{
							foreach (OwaStoreObjectId owaStoreObjectId in this.folderRefreshList)
							{
								if (!this.folderContentChangeNotifications.ContainsKey(owaStoreObjectId) || this.folderContentChangeNotifications[owaStoreObjectId].NotificationHandler == null || this.folderContentChangeNotifications[owaStoreObjectId].NotificationHandler.AllowFolderRefreshNotification)
								{
									stringWriter2.Write("stMapiDrty(\"");
									Utilities.JavascriptEncode(owaStoreObjectId.ToBase64String(), stringWriter2);
									stringWriter2.Write("\");");
								}
							}
						}
					}
					int num = 0;
					if (this.folderContentChangeNotifications.Count > 0)
					{
						foreach (EmailPayload.FCNHState fcnhstate in this.folderContentChangeNotifications.Values)
						{
							num += fcnhstate.Queue.Count;
						}
					}
					StringBuilder stringBuilder3 = null;
					bool flag2 = true;
					if (num > 0)
					{
						stringBuilder3 = new StringBuilder(1280 * num);
						using (StringWriter stringWriter3 = new StringWriter(stringBuilder3, CultureInfo.InvariantCulture))
						{
							foreach (KeyValuePair<OwaStoreObjectId, EmailPayload.FCNHState> keyValuePair2 in this.folderContentChangeNotifications)
							{
								OwaStoreObjectId key2 = keyValuePair2.Key;
								Queue<QueryNotification> queue = keyValuePair2.Value.Queue;
								FolderContentChangeNotificationHandler notificationHandler = keyValuePair2.Value.NotificationHandler;
								foreach (QueryNotification notification in queue)
								{
									notificationHandler.ProcessNotification(stringWriter3, notification, out flag2);
								}
							}
						}
					}
					result = string.Concat(new string[]
					{
						(stringBuilder != null) ? stringBuilder.ToString() : string.Empty,
						(stringBuilder2 != null) ? stringBuilder2.ToString() : string.Empty,
						this.payloadStringReminderChanges.ToString(),
						this.payloadStringNewMail.ToString(),
						this.payloadStringQuota.ToString(),
						this.payloadStringReminderNotify.ToString(),
						(stringBuilder3 != null) ? stringBuilder3.ToString() : string.Empty
					});
				}
				this.Clear(true);
			}
			return result;
		}

		public void PickupData()
		{
			bool flag = false;
			lock (this)
			{
				flag = ((this.folderCountTable.Count > 0 || this.folderRefreshList.Count > 0 || this.payloadStringNewMail.Length > 0 || this.payloadStringQuota.Length > 0 || this.payloadStringReminderChanges.Length > 0 || this.payloadStringReminderNotify.Length > 0 || this.payloadStringRefreshAll.Length > 0 || this.AreThereFolderContentChangeNotifications()) && !this.containsDataToPickup);
				if (flag)
				{
					this.containsDataToPickup = true;
				}
			}
			if (flag)
			{
				this.DataAvailable(this, new EventArgs());
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "EmailPayload.PickupData. DataAvailable method called. Mailbox: {0}", this.mailboxSessionDisplayName);
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "EmailPayload.PickupData. No need to call DataAvailable method. Mailbox: {0}", this.mailboxSessionDisplayName);
		}

		public void Clear(bool clearRefreshPayload)
		{
			this.folderCountTable.Clear();
			this.folderRefreshList.Clear();
			this.payloadStringReminderChanges.Remove(0, this.payloadStringReminderChanges.Length);
			this.payloadStringNewMail.Remove(0, this.payloadStringNewMail.Length);
			this.payloadStringQuota.Remove(0, this.payloadStringQuota.Length);
			this.payloadStringReminderNotify.Remove(0, this.payloadStringReminderNotify.Length);
			this.ClearAllFolderContentChangeNotifications();
			if (clearRefreshPayload)
			{
				this.payloadStringRefreshAll.Remove(0, this.payloadStringRefreshAll.Length);
			}
		}

		public void ConnectionAliveTimer()
		{
			int num = Interlocked.Increment(ref this.connectionAliveTimerCount);
			bool clearSearchFolderDeleteList = false;
			if (num % 5 == 0)
			{
				if (num % 15 == 0)
				{
					clearSearchFolderDeleteList = true;
				}
				this.omnhParent.HandlePendingGetTimerCallback(clearSearchFolderDeleteList);
			}
		}

		public void AddFolderRefreshPayload(OwaStoreObjectId folderId)
		{
			this.AddFolderRefreshPayload(folderId, true);
		}

		public void AddFolderRefreshPayload(OwaStoreObjectId folderId, bool forceRefresh)
		{
			lock (this)
			{
				if (this.payloadStringRefreshAll.Length == 0)
				{
					if (!this.folderRefreshList.Contains(folderId) && (forceRefresh || !this.folderContentChangeNotifications.ContainsKey(folderId) || this.folderContentChangeNotifications[folderId].NotificationHandler == null || this.folderContentChangeNotifications[folderId].NotificationHandler.AllowFolderRefreshNotification))
					{
						this.folderRefreshList.Add(folderId);
						this.ClearFolderContentChangePayload(folderId);
					}
					if (this.folderRefreshList.Count > 20)
					{
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_MessagingPayloadNotBeingPickedup, string.Empty, new object[]
						{
							this.mailboxSessionDisplayName
						});
						this.AddRefreshPayload();
					}
				}
			}
		}

		public void AttachFolderContentChangeNotificationHandler(OwaStoreObjectId folderId, FolderContentChangeNotificationHandler notificationHandler)
		{
			lock (this)
			{
				if (this.folderContentChangeNotifications.ContainsKey(folderId))
				{
					throw new OwaInvalidOperationException("There is already an active notification handler for this folder. That should not be the case");
				}
				this.folderContentChangeNotifications[folderId] = new EmailPayload.FCNHState(notificationHandler);
			}
		}

		public void DetachFolderContentChangeNotificationHandler(OwaStoreObjectId folderId)
		{
			lock (this)
			{
				if (this.folderContentChangeNotifications.ContainsKey(folderId))
				{
					this.ClearFolderContentChangePayload(folderId);
					this.folderContentChangeNotifications[folderId].NotificationHandler = null;
					this.folderContentChangeNotifications[folderId].Queue = null;
					this.folderContentChangeNotifications.Remove(folderId);
				}
			}
		}

		public bool ShouldIgnoreNotification(OwaStoreObjectId folderId)
		{
			lock (this)
			{
				if (this.folderRefreshList.Contains(folderId))
				{
					return true;
				}
				Queue<QueryNotification> folderContentChangePayloadQueue = this.GetFolderContentChangePayloadQueue(folderId);
				if (folderContentChangePayloadQueue != null && folderContentChangePayloadQueue.Count >= 40)
				{
					this.AddFolderRefreshPayload(folderId);
					return true;
				}
			}
			return false;
		}

		public void AddFolderContentChangePayload(OwaStoreObjectId folderId, QueryNotification notification)
		{
			lock (this)
			{
				if (!this.folderRefreshList.Contains(folderId))
				{
					Queue<QueryNotification> folderContentChangePayloadQueue = this.GetFolderContentChangePayloadQueue(folderId);
					if (folderContentChangePayloadQueue != null && folderContentChangePayloadQueue.Count >= 40)
					{
						this.AddFolderRefreshPayload(folderId);
					}
					else
					{
						folderContentChangePayloadQueue.Enqueue(notification);
					}
				}
			}
		}

		private void ClearFolderContentChangePayload(OwaStoreObjectId folderId)
		{
			lock (this)
			{
				Queue<QueryNotification> folderContentChangePayloadQueue = this.GetFolderContentChangePayloadQueue(folderId);
				if (folderContentChangePayloadQueue != null)
				{
					folderContentChangePayloadQueue.Clear();
				}
			}
		}

		private Queue<QueryNotification> GetFolderContentChangePayloadQueue(OwaStoreObjectId folderId)
		{
			if (this.folderContentChangeNotifications.ContainsKey(folderId))
			{
				return this.folderContentChangeNotifications[folderId].Queue;
			}
			return null;
		}

		private void ClearAllFolderContentChangeNotifications()
		{
			if (this.folderContentChangeNotifications.Count > 0)
			{
				foreach (EmailPayload.FCNHState fcnhstate in this.folderContentChangeNotifications.Values)
				{
					fcnhstate.Queue.Clear();
				}
			}
		}

		private bool AreThereFolderContentChangeNotifications()
		{
			if (this.folderContentChangeNotifications.Count > 0)
			{
				foreach (EmailPayload.FCNHState fcnhstate in this.folderContentChangeNotifications.Values)
				{
					if (fcnhstate.Queue.Count > 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public void AddNewMailPayload(StringBuilder newMailBuilder)
		{
			lock (this)
			{
				if (this.payloadStringRefreshAll.Length == 0)
				{
					this.payloadStringNewMail = newMailBuilder;
				}
			}
		}

		public void AddRemindersPayload(StringBuilder remindersBuilder)
		{
			lock (this)
			{
				if (this.payloadStringRefreshAll.Length == 0)
				{
					this.payloadStringReminderChanges = remindersBuilder;
				}
			}
		}

		public void AddQuotaPayload(string mailboxQuotaHtml)
		{
			lock (this)
			{
				if (this.payloadStringRefreshAll.Length == 0)
				{
					StringBuilder sb = new StringBuilder();
					using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
					{
						stringWriter.Write("updateMailboxUsage(\"");
						Utilities.JavascriptEncode(mailboxQuotaHtml, stringWriter);
						stringWriter.Write("\"); ");
					}
					this.payloadStringQuota = sb;
				}
			}
		}

		public void AddReminderNotifyPayload(int minutesOffset)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				stringWriter.Write("rmNotfy(");
				stringWriter.Write(minutesOffset);
				stringWriter.Write(", 0, \"\");");
			}
			lock (this)
			{
				if (this.payloadStringRefreshAll.Length == 0)
				{
					this.payloadStringReminderNotify = sb;
				}
			}
		}

		public void AddRefreshPayload()
		{
			StringBuilder sb = null;
			lock (this)
			{
				this.Clear(false);
				if (this.payloadStringRefreshAll.Length == 0)
				{
					sb = new StringBuilder();
					using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
					{
						stringWriter.Write("stMapiRfrshAll();");
					}
					this.payloadStringRefreshAll = sb;
				}
			}
		}

		internal void RegisterWithPendingRequestNotifier()
		{
			if (this.userContext != null && this.userContext.PendingRequestManager != null)
			{
				this.userContext.PendingRequestManager.AddPendingRequestNotifier(this);
			}
		}

		internal void AddFolderCountPayload(OwaStoreObjectId folderId, long itemCount, long itemUnreadCount)
		{
			lock (this)
			{
				if (this.payloadStringRefreshAll.Length == 0)
				{
					this.folderCountTable[folderId] = new ItemCountPair(itemCount, itemUnreadCount);
					if (this.folderCountTable.Count > 200)
					{
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_MessagingPayloadNotBeingPickedup, string.Empty, new object[]
						{
							this.mailboxSessionDisplayName
						});
						this.AddRefreshPayload();
					}
				}
			}
		}

		private const int MaxFolderCountTableSize = 200;

		private const int MaxFolderChangeListSize = 20;

		private const int DefaultPayloadStringSize = 256;

		private const int DefaultFolderContentChangePayloadStringSize = 1280;

		private const int MaxFolderContentNotificationQueueSize = 40;

		private Dictionary<OwaStoreObjectId, ItemCountPair> folderCountTable;

		private bool containsDataToPickup;

		private List<OwaStoreObjectId> folderRefreshList;

		private StringBuilder payloadStringReminderChanges;

		private StringBuilder payloadStringNewMail;

		private StringBuilder payloadStringQuota;

		private StringBuilder payloadStringReminderNotify;

		private StringBuilder payloadStringRefreshAll;

		private Dictionary<OwaStoreObjectId, EmailPayload.FCNHState> folderContentChangeNotifications;

		private UserContext userContext;

		private OwaMapiNotificationHandler omnhParent;

		private int connectionAliveTimerCount;

		private string mailboxSessionDisplayName;

		internal class FCNHState
		{
			internal FCNHState(FolderContentChangeNotificationHandler notificationHandler)
			{
				this.NotificationHandler = notificationHandler;
				this.Queue = new Queue<QueryNotification>();
			}

			internal FolderContentChangeNotificationHandler NotificationHandler;

			internal Queue<QueryNotification> Queue;
		}
	}
}
