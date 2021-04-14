using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class SearchPayload : IPendingRequestNotifier
	{
		internal SearchPayload(UserContext userContext, MailboxSession mailboxSession, OwaMapiNotificationHandler omnhParent)
		{
			this.searchFolderRefreshList = new List<OwaStoreObjectId>();
			this.userContext = userContext;
			this.mailboxSessionDisplayName = string.Copy(mailboxSession.DisplayName);
			this.omnhParent = omnhParent;
		}

		public event DataAvailableEventHandler DataAvailable;

		public bool ShouldThrottle
		{
			get
			{
				return false;
			}
		}

		public string ReadDataAndResetState()
		{
			string result = null;
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[SearchPayload.ReadDataAndResetState] Mailbox: {0}", this.mailboxSessionDisplayName);
			lock (this)
			{
				this.containsDataToPickup = false;
				StringBuilder stringBuilder = null;
				if (this.searchFolderRefreshList.Count > 0)
				{
					stringBuilder = new StringBuilder();
					using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
					{
						foreach (OwaStoreObjectId owaStoreObjectId in this.searchFolderRefreshList)
						{
							stringWriter.Write("searchNotification(\"");
							Utilities.JavascriptEncode(owaStoreObjectId.ToBase64String(), stringWriter);
							stringWriter.Write("\");");
						}
					}
				}
				result = ((stringBuilder != null) ? stringBuilder.ToString() : string.Empty);
				this.searchFolderRefreshList.Clear();
				this.UpdateSearchPerformanceData();
			}
			return result;
		}

		public void PickupData()
		{
			bool flag = false;
			lock (this)
			{
				flag = (this.searchFolderRefreshList.Count > 0 && !this.containsDataToPickup);
				if (flag)
				{
					this.containsDataToPickup = true;
				}
			}
			if (flag)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[SearchPayload.PickupData] DataAvailable method called after search notification. Mailbox: {0}", this.mailboxSessionDisplayName);
				this.DataAvailable(this, new EventArgs());
				return;
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[SearchPayload.PickupData] No need to call DataAvailable method. Mailbox: {0}", this.mailboxSessionDisplayName);
		}

		public void ConnectionAliveTimer()
		{
		}

		internal void AddSearchFolderRefreshPayload(OwaStoreObjectId folderId, SearchNotificationType searchNotificationType)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<SearchNotificationType>((long)this.GetHashCode(), "[SearchPayload.AddSearchFolderRefreshPayload] Adding search notification type {0}", searchNotificationType);
			lock (this)
			{
				this.searchNotificationType |= searchNotificationType;
				if (!this.searchFolderRefreshList.Contains(folderId))
				{
					this.searchFolderRefreshList.Add(folderId);
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

		private void UpdateSearchPerformanceData()
		{
			if (this.omnhParent != null && this.searchNotificationType != SearchNotificationType.None)
			{
				SearchPerformanceData searchPerformanceData = this.omnhParent.SearchPerformanceData;
				if (searchPerformanceData != null)
				{
					searchPerformanceData.NotificationPickedUpByPendingGet(this.searchNotificationType);
					this.searchNotificationType = SearchNotificationType.None;
				}
			}
		}

		private List<OwaStoreObjectId> searchFolderRefreshList;

		private bool containsDataToPickup;

		private UserContext userContext;

		private OwaMapiNotificationHandler omnhParent;

		private string mailboxSessionDisplayName;

		private SearchNotificationType searchNotificationType;
	}
}
