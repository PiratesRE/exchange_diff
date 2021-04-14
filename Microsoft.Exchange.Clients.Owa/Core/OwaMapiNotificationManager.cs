using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaMapiNotificationManager : DisposeTrackableBase
	{
		internal OwaMapiNotificationManager(UserContext userContext)
		{
			this.userContext = userContext;
		}

		public void SubscribeForFolderChanges(OwaStoreObjectId folderId, MailboxSession sessionIn)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (sessionIn == mailboxSession)
			{
				if (this.omnhLoggedUser == null)
				{
					this.omnhLoggedUser = new OwaMapiNotificationHandler(this.userContext, mailboxSession, null);
					this.WireConnectionDroppedHandler(this.omnhLoggedUser);
				}
				this.omnhLoggedUser.SubscribeForFolderChanges();
				this.omnhLoggedUser.AddFolderChangeNotification(folderId);
				return;
			}
			if (Utilities.IsArchiveMailbox(sessionIn))
			{
				if (this.omnhArchives == null)
				{
					this.omnhArchives = new List<OwaMapiNotificationHandler>();
				}
				OwaMapiNotificationHandler owaMapiNotificationHandler = null;
				foreach (OwaMapiNotificationHandler owaMapiNotificationHandler2 in this.omnhArchives)
				{
					if (owaMapiNotificationHandler2.ArchiveMailboxSession == sessionIn)
					{
						owaMapiNotificationHandler = owaMapiNotificationHandler2;
						break;
					}
				}
				if (owaMapiNotificationHandler == null)
				{
					owaMapiNotificationHandler = new OwaMapiNotificationHandler(this.userContext, sessionIn, null);
					this.WireConnectionDroppedHandler(owaMapiNotificationHandler);
					this.omnhArchives.Add(owaMapiNotificationHandler);
				}
				owaMapiNotificationHandler.SubscribeForFolderChanges();
				owaMapiNotificationHandler.AddFolderChangeNotification(folderId);
				return;
			}
			if (this.omnhDelegates == null)
			{
				this.omnhDelegates = new List<OwaMapiNotificationHandler>();
			}
			OwaMapiNotificationHandler owaMapiNotificationHandler3 = null;
			foreach (OwaMapiNotificationHandler owaMapiNotificationHandler4 in this.omnhDelegates)
			{
				if (owaMapiNotificationHandler4.DelegateSessionHandle.Session == sessionIn)
				{
					owaMapiNotificationHandler3 = owaMapiNotificationHandler4;
					break;
				}
			}
			if (owaMapiNotificationHandler3 == null)
			{
				OwaStoreObjectIdSessionHandle delegateSessionHandle = new OwaStoreObjectIdSessionHandle(folderId, this.userContext);
				owaMapiNotificationHandler3 = new OwaMapiNotificationHandler(this.userContext, sessionIn, delegateSessionHandle);
				this.WireConnectionDroppedHandler(owaMapiNotificationHandler3);
				this.omnhDelegates.Add(owaMapiNotificationHandler3);
				this.ReleaseOldestSessionIfNecessary();
			}
			owaMapiNotificationHandler3.SubscribeForFolderChanges();
			owaMapiNotificationHandler3.AddFolderChangeNotification(folderId);
		}

		public void UnsubscribeFolderChanges(OwaStoreObjectId folderId, MailboxSession sessionIn)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (sessionIn == null || sessionIn == mailboxSession)
			{
				if (this.omnhLoggedUser != null)
				{
					this.omnhLoggedUser.SubscribeForFolderChanges();
					this.omnhLoggedUser.DeleteFolderChangeNotification(folderId);
					this.UnsubscribeFolderContentChanges(folderId);
					return;
				}
			}
			else if (Utilities.IsArchiveMailbox(sessionIn))
			{
				if (this.omnhArchives != null)
				{
					OwaMapiNotificationHandler owaMapiNotificationHandler = null;
					foreach (OwaMapiNotificationHandler owaMapiNotificationHandler2 in this.omnhArchives)
					{
						if (owaMapiNotificationHandler2.ArchiveMailboxSession == sessionIn)
						{
							owaMapiNotificationHandler = owaMapiNotificationHandler2;
							break;
						}
					}
					if (owaMapiNotificationHandler != null)
					{
						owaMapiNotificationHandler.SubscribeForFolderChanges();
						owaMapiNotificationHandler.DeleteFolderChangeNotification(folderId);
						return;
					}
				}
			}
			else if (this.omnhDelegates != null)
			{
				OwaMapiNotificationHandler owaMapiNotificationHandler3 = null;
				foreach (OwaMapiNotificationHandler owaMapiNotificationHandler4 in this.omnhDelegates)
				{
					if (owaMapiNotificationHandler4.DelegateSessionHandle.Session == sessionIn)
					{
						owaMapiNotificationHandler3 = owaMapiNotificationHandler4;
						break;
					}
				}
				if (owaMapiNotificationHandler3 != null)
				{
					owaMapiNotificationHandler3.SubscribeForFolderChanges();
					owaMapiNotificationHandler3.DeleteFolderChangeNotification(folderId);
				}
			}
		}

		public void RenewDelegateHandler(MailboxSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (this.omnhDelegates != null)
			{
				int i = 0;
				while (i < this.omnhDelegates.Count)
				{
					OwaMapiNotificationHandler owaMapiNotificationHandler = this.omnhDelegates[i];
					if (owaMapiNotificationHandler.DelegateSessionHandle.Session == session)
					{
						if (i < this.omnhDelegates.Count - 1)
						{
							this.omnhDelegates.Remove(owaMapiNotificationHandler);
							this.omnhDelegates.Add(owaMapiNotificationHandler);
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		public void SubscribeForFolderContentChanges(MailboxSession sessionIn, OwaStoreObjectId contextFolderId, OwaStoreObjectId dataFolderId, QueryResult queryResult, ListViewContents2 listView, PropertyDefinition[] subscriptionProperties, Dictionary<PropertyDefinition, int> propertyMap, SortBy[] sortBy, FolderVirtualListViewFilter folderFilter, bool isConversationView)
		{
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			this.SubscribeForFolderChanges(contextFolderId, sessionIn);
			if (queryResult == null)
			{
				return;
			}
			if (sessionIn == mailboxSession)
			{
				FolderContentChangeNotificationHandler folderContentChangeNotificationHandler = null;
				this.folderContentChangeNotificationHandlers.TryGetValue(contextFolderId, out folderContentChangeNotificationHandler);
				if (folderContentChangeNotificationHandler != null && folderContentChangeNotificationHandler.NeedReinitSubscriptions)
				{
					this.RemoveFolderContentChangeSubscription(contextFolderId);
					folderContentChangeNotificationHandler = null;
				}
				if (folderContentChangeNotificationHandler == null)
				{
					this.ClearOldFolderContentChangeSubscriptions();
					this.InitializeConnectionDroppedHandler();
					folderContentChangeNotificationHandler = new FolderContentChangeNotificationHandler(this.userContext, mailboxSession, contextFolderId, dataFolderId, queryResult, this.omnhLoggedUser.EmailPayload, listView, subscriptionProperties, propertyMap, sortBy, folderFilter, isConversationView);
					try
					{
						if (!folderContentChangeNotificationHandler.TrySubscribe(this.connectionDroppedNotificationHandler))
						{
							ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "Failed to create a folder content change subscription.");
							folderContentChangeNotificationHandler.Dispose();
							folderContentChangeNotificationHandler = null;
							return;
						}
						this.folderContentChangeNotificationHandlers[contextFolderId] = folderContentChangeNotificationHandler;
						folderContentChangeNotificationHandler = null;
						return;
					}
					finally
					{
						if (folderContentChangeNotificationHandler != null)
						{
							folderContentChangeNotificationHandler.Dispose();
							folderContentChangeNotificationHandler = null;
						}
					}
				}
				folderContentChangeNotificationHandler.MissedNotifications = false;
			}
		}

		public void UnsubscribeFolderContentChanges(OwaStoreObjectId folderId)
		{
			this.RemoveFolderContentChangeSubscription(folderId);
		}

		public bool HasDataFolderChanged(MailboxSession sessionIn, OwaStoreObjectId contextFolderId, OwaStoreObjectId dataFolderId)
		{
			FolderContentChangeNotificationHandler folderContentChangeNotificationHandler = this.GetFolderContentChangeNotificationHandler(contextFolderId);
			return folderContentChangeNotificationHandler != null && folderContentChangeNotificationHandler.HasDataFolderChanged(dataFolderId);
		}

		public FolderContentChangeNotificationHandler GetFolderContentChangeNotificationHandler(OwaStoreObjectId contextFolderId)
		{
			if (contextFolderId == null)
			{
				throw new ArgumentNullException("contextFolderId");
			}
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			FolderContentChangeNotificationHandler result = null;
			this.folderContentChangeNotificationHandlers.TryGetValue(contextFolderId, out result);
			return result;
		}

		private void ClearOldFolderContentChangeSubscriptions()
		{
			if (this.folderContentChangeNotificationHandlers.Count >= 2)
			{
				OwaStoreObjectId owaStoreObjectId = null;
				foreach (OwaStoreObjectId owaStoreObjectId2 in this.folderContentChangeNotificationHandlers.Keys)
				{
					if (owaStoreObjectId == null)
					{
						owaStoreObjectId = owaStoreObjectId2;
					}
					else if (this.folderContentChangeNotificationHandlers[owaStoreObjectId2].CreationTime < this.folderContentChangeNotificationHandlers[owaStoreObjectId].CreationTime)
					{
						owaStoreObjectId = owaStoreObjectId2;
					}
				}
				this.UnsubscribeFolderContentChanges(owaStoreObjectId);
			}
		}

		private void RemoveFolderContentChangeSubscription(OwaStoreObjectId folderId)
		{
			FolderContentChangeNotificationHandler folderContentChangeNotificationHandler = this.GetFolderContentChangeNotificationHandler(folderId);
			if (folderContentChangeNotificationHandler != null)
			{
				folderContentChangeNotificationHandler.RemoveSubscription(this.connectionDroppedNotificationHandler);
				folderContentChangeNotificationHandler.Dispose();
				this.folderContentChangeNotificationHandlers.Remove(folderId);
			}
		}

		private void RemoveAllFolderContentChangeSubscriptions()
		{
			foreach (OwaStoreObjectId contextFolderId in this.folderContentChangeNotificationHandlers.Keys)
			{
				FolderContentChangeNotificationHandler folderContentChangeNotificationHandler = this.GetFolderContentChangeNotificationHandler(contextFolderId);
				if (folderContentChangeNotificationHandler != null)
				{
					folderContentChangeNotificationHandler.RemoveSubscription(this.connectionDroppedNotificationHandler);
					folderContentChangeNotificationHandler.Dispose();
				}
			}
			this.folderContentChangeNotificationHandlers.Clear();
			this.folderContentChangeNotificationHandlers = null;
		}

		public QueryResult GetFolderQueryResult(OwaStoreObjectId folderId)
		{
			FolderContentChangeNotificationHandler folderContentChangeNotificationHandler = this.GetFolderContentChangeNotificationHandler(folderId);
			if (folderContentChangeNotificationHandler != null)
			{
				return folderContentChangeNotificationHandler.QueryResult;
			}
			return null;
		}

		public SortBy[] GetFolderSortBy(OwaStoreObjectId folderId)
		{
			FolderContentChangeNotificationHandler folderContentChangeNotificationHandler = this.GetFolderContentChangeNotificationHandler(folderId);
			if (folderContentChangeNotificationHandler != null)
			{
				return folderContentChangeNotificationHandler.SortBy;
			}
			return null;
		}

		public FolderVirtualListViewFilter GetFolderFilter(OwaStoreObjectId folderId)
		{
			FolderContentChangeNotificationHandler folderContentChangeNotificationHandler = this.GetFolderContentChangeNotificationHandler(folderId);
			if (folderContentChangeNotificationHandler != null)
			{
				return folderContentChangeNotificationHandler.FolderFilter;
			}
			return null;
		}

		public void SubscribeForFolderCounts(OwaStoreObjectId delegateFolderId, MailboxSession sessionIn)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (sessionIn == mailboxSession)
			{
				if (this.omnhLoggedUser == null)
				{
					this.omnhLoggedUser = new OwaMapiNotificationHandler(this.userContext, mailboxSession, null);
					this.WireConnectionDroppedHandler(this.omnhLoggedUser);
				}
				this.omnhLoggedUser.SubscribeForFolderCounts();
				return;
			}
			if (Utilities.IsArchiveMailbox(sessionIn))
			{
				if (this.omnhArchives == null)
				{
					this.omnhArchives = new List<OwaMapiNotificationHandler>();
				}
				OwaMapiNotificationHandler owaMapiNotificationHandler = null;
				foreach (OwaMapiNotificationHandler owaMapiNotificationHandler2 in this.omnhArchives)
				{
					if (owaMapiNotificationHandler2.ArchiveMailboxSession == sessionIn)
					{
						owaMapiNotificationHandler = owaMapiNotificationHandler2;
						break;
					}
				}
				if (owaMapiNotificationHandler == null)
				{
					owaMapiNotificationHandler = new OwaMapiNotificationHandler(this.userContext, sessionIn, null);
					this.WireConnectionDroppedHandler(owaMapiNotificationHandler);
					this.omnhArchives.Add(owaMapiNotificationHandler);
				}
				owaMapiNotificationHandler.SubscribeForFolderCounts();
				return;
			}
			if (this.omnhDelegates == null)
			{
				this.omnhDelegates = new List<OwaMapiNotificationHandler>();
			}
			OwaMapiNotificationHandler owaMapiNotificationHandler3 = null;
			foreach (OwaMapiNotificationHandler owaMapiNotificationHandler4 in this.omnhDelegates)
			{
				if (owaMapiNotificationHandler4.DelegateSessionHandle.Session == sessionIn)
				{
					owaMapiNotificationHandler3 = owaMapiNotificationHandler4;
					break;
				}
			}
			if (owaMapiNotificationHandler3 == null)
			{
				OwaStoreObjectIdSessionHandle delegateSessionHandle = new OwaStoreObjectIdSessionHandle(delegateFolderId, this.userContext);
				owaMapiNotificationHandler3 = new OwaMapiNotificationHandler(this.userContext, sessionIn, delegateSessionHandle);
				this.WireConnectionDroppedHandler(owaMapiNotificationHandler3);
				this.omnhDelegates.Add(owaMapiNotificationHandler3);
				this.ReleaseOldestSessionIfNecessary();
			}
			owaMapiNotificationHandler3.SubscribeForFolderCounts();
			owaMapiNotificationHandler3.AddFolderCountsNotification(delegateFolderId);
		}

		private void ReleaseOldestSessionIfNecessary()
		{
			if (this.omnhDelegates.Count <= 5)
			{
				return;
			}
			OwaMapiNotificationHandler owaMapiNotificationHandler = this.omnhDelegates[0];
			this.omnhDelegates.Remove(owaMapiNotificationHandler);
			owaMapiNotificationHandler.Dispose();
		}

		public void UnsubscribeFolderCounts(OwaStoreObjectId delegateFolderId, MailboxSession sessionIn)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (sessionIn == mailboxSession || Utilities.IsArchiveMailbox(sessionIn))
			{
				return;
			}
			if (this.omnhDelegates != null)
			{
				OwaMapiNotificationHandler owaMapiNotificationHandler = null;
				foreach (OwaMapiNotificationHandler owaMapiNotificationHandler2 in this.omnhDelegates)
				{
					if (owaMapiNotificationHandler2.DelegateSessionHandle.Session == sessionIn)
					{
						owaMapiNotificationHandler = owaMapiNotificationHandler2;
						break;
					}
				}
				if (owaMapiNotificationHandler != null)
				{
					owaMapiNotificationHandler.SubscribeForFolderCounts();
					owaMapiNotificationHandler.DeleteFolderCountsNotification(delegateFolderId);
				}
			}
		}

		public void SubscribeForNewMail()
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (mailboxSession.LogonType == LogonType.Delegated)
			{
				throw new OwaInvalidOperationException("Cannot call subscribe new mail for delegate logon type");
			}
			if (this.omnhLoggedUser == null)
			{
				this.omnhLoggedUser = new OwaMapiNotificationHandler(this.userContext, mailboxSession, null);
				this.WireConnectionDroppedHandler(this.omnhLoggedUser);
			}
			this.omnhLoggedUser.SubscribeForNewMail();
		}

		public void UnsubscribeNewMail()
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (mailboxSession.LogonType == LogonType.Delegated)
			{
				throw new OwaInvalidOperationException("Cannot call unsubscribe new mail for delegate logon type");
			}
			if (this.omnhLoggedUser != null)
			{
				this.omnhLoggedUser.UnsubscribeNewMail();
			}
		}

		public void SubscribeForReminders()
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (mailboxSession.LogonType == LogonType.Delegated)
			{
				throw new OwaInvalidOperationException("Cannot call subscribe reminders for delegate logon type");
			}
			if (this.omnhLoggedUser == null)
			{
				this.omnhLoggedUser = new OwaMapiNotificationHandler(this.userContext, mailboxSession, null);
				this.WireConnectionDroppedHandler(this.omnhLoggedUser);
			}
			this.omnhLoggedUser.SubscribeForReminderChanges();
		}

		public void SubscribeForSubscriptionChanges()
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (mailboxSession.LogonType == LogonType.Owner)
			{
				if (this.subscriptionNotificationHandler == null)
				{
					this.subscriptionNotificationHandler = new SubscriptionNotificationHandler(this.userContext, mailboxSession);
					this.WireConnectionDroppedHandler(this.subscriptionNotificationHandler);
				}
				this.subscriptionNotificationHandler.Subscribe();
			}
		}

		public object[][] GetReminderRows(ComparisonFilter filter, int maxRows)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			this.SubscribeForReminders();
			object[][] result = null;
			try
			{
				result = this.omnhLoggedUser.GetReminderRows(filter, maxRows);
			}
			catch (MapiExceptionObjectDisposed)
			{
				this.omnhLoggedUser.HandleConnectionDroppedNotification(null);
			}
			return result;
		}

		public void InitSearchNotifications(MailboxSession sessionIn, StoreObjectId searchFolderId, SearchFolder searchFolder, SearchFolderCriteria searchCriteria, string searchString)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			OwaMapiNotificationHandler owaMapiHandler = this.GetOwaMapiHandler(sessionIn);
			if (owaMapiHandler == null)
			{
				throw new OwaInvalidOperationException("Cannot find the mapi notification handler for this session");
			}
			owaMapiHandler.SubscribeForSearchPageNotify(searchFolderId, searchFolder, searchCriteria, searchString);
		}

		public void CancelSearchNotifications(MailboxSession sessionIn)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			OwaMapiNotificationHandler owaMapiHandler = this.GetOwaMapiHandler(sessionIn);
			if (owaMapiHandler == null)
			{
				throw new OwaInvalidOperationException("Cannot find the mapi notification handler for this session");
			}
			owaMapiHandler.CancelSearchPageNotify();
		}

		public void AddSearchFolderDeleteList(MailboxSession sessionIn, StoreObjectId folderId)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			OwaMapiNotificationHandler owaMapiHandler = this.GetOwaMapiHandler(sessionIn);
			if (owaMapiHandler == null)
			{
				throw new OwaInvalidOperationException("Cannot find the mapi notification handler for this session");
			}
			owaMapiHandler.AddSearchFolderDeleteList(folderId);
		}

		public bool IsSearchInProgress(MailboxSession sessionIn, StoreObjectId folderId)
		{
			bool result = false;
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			OwaMapiNotificationHandler owaMapiHandler = this.GetOwaMapiHandler(sessionIn);
			if (owaMapiHandler != null)
			{
				result = owaMapiHandler.IsSearchInProgress(folderId);
			}
			return result;
		}

		public bool HasCurrentSearchCompleted(MailboxSession sessionIn, StoreObjectId folderId, out bool wasFailNonContentIndexedSearchFlagSet)
		{
			bool result = false;
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			wasFailNonContentIndexedSearchFlagSet = false;
			OwaMapiNotificationHandler owaMapiHandler = this.GetOwaMapiHandler(sessionIn);
			if (owaMapiHandler != null)
			{
				result = owaMapiHandler.HasCurrentSearchCompleted(folderId, out wasFailNonContentIndexedSearchFlagSet);
			}
			return result;
		}

		public SearchPerformanceData GetSearchPerformanceData(MailboxSession sessionIn)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			OwaMapiNotificationHandler owaMapiHandler = this.GetOwaMapiHandler(sessionIn);
			if (owaMapiHandler != null)
			{
				return owaMapiHandler.SearchPerformanceData;
			}
			return null;
		}

		internal static bool IsNotificationEnabled(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return userContext.IsFeatureEnabled(Feature.Notifications) && !userContext.IsWebPartRequest;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "OwaMapiNotificationManager.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing)
			{
				if (this.subscriptionNotificationHandler != null)
				{
					this.subscriptionNotificationHandler.Dispose();
					this.subscriptionNotificationHandler = null;
				}
				if (this.omnhLoggedUser != null)
				{
					this.omnhLoggedUser.Dispose();
					this.omnhLoggedUser = null;
				}
				if (this.omnhArchives != null)
				{
					foreach (OwaMapiNotificationHandler owaMapiNotificationHandler in this.omnhArchives)
					{
						owaMapiNotificationHandler.Dispose();
					}
					this.omnhArchives.Clear();
					this.omnhArchives = null;
				}
				if (this.omnhDelegates != null)
				{
					foreach (OwaMapiNotificationHandler owaMapiNotificationHandler2 in this.omnhDelegates)
					{
						owaMapiNotificationHandler2.Dispose();
					}
					this.omnhDelegates.Clear();
					this.omnhDelegates = null;
				}
				this.RemoveAllFolderContentChangeSubscriptions();
				if (this.connectionDroppedNotificationHandler != null)
				{
					this.connectionDroppedNotificationHandler.Dispose();
					this.connectionDroppedNotificationHandler = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaMapiNotificationManager>(this);
		}

		private void WireConnectionDroppedHandler(OwaMapiNotificationHandler mapiNotificationHandler)
		{
			this.InitializeConnectionDroppedHandler();
			this.connectionDroppedNotificationHandler.OnConnectionDropped += mapiNotificationHandler.HandleConnectionDroppedNotification;
		}

		private void WireConnectionDroppedHandler(NotificationHandlerBase handler)
		{
			this.InitializeConnectionDroppedHandler();
			this.connectionDroppedNotificationHandler.OnConnectionDropped += handler.HandleConnectionDroppedNotification;
		}

		private void InitializeConnectionDroppedHandler()
		{
			if (this.connectionDroppedNotificationHandler == null)
			{
				this.connectionDroppedNotificationHandler = new ConnectionDroppedNotificationHandler(this.userContext, this.userContext.MailboxSession);
				this.connectionDroppedNotificationHandler.Subscribe();
			}
		}

		internal void HandleConnectionDroppedNotification()
		{
			if (this.connectionDroppedNotificationHandler != null)
			{
				this.connectionDroppedNotificationHandler.HandleNotification(null);
			}
		}

		private OwaMapiNotificationHandler GetOwaMapiHandler(MailboxSession sessionIn)
		{
			OwaMapiNotificationHandler result = null;
			MailboxSession mailboxSession = this.userContext.MailboxSession;
			if (sessionIn == mailboxSession)
			{
				if (this.omnhLoggedUser == null)
				{
					this.omnhLoggedUser = new OwaMapiNotificationHandler(this.userContext, sessionIn, null);
					this.WireConnectionDroppedHandler(this.omnhLoggedUser);
				}
				result = this.omnhLoggedUser;
			}
			else if (Utilities.IsArchiveMailbox(sessionIn))
			{
				if (this.omnhArchives == null)
				{
					this.omnhArchives = new List<OwaMapiNotificationHandler>();
				}
				OwaMapiNotificationHandler owaMapiNotificationHandler = null;
				foreach (OwaMapiNotificationHandler owaMapiNotificationHandler2 in this.omnhArchives)
				{
					if (owaMapiNotificationHandler2.ArchiveMailboxSession == sessionIn)
					{
						owaMapiNotificationHandler = owaMapiNotificationHandler2;
						break;
					}
				}
				if (owaMapiNotificationHandler == null)
				{
					owaMapiNotificationHandler = new OwaMapiNotificationHandler(this.userContext, sessionIn, null);
					this.WireConnectionDroppedHandler(owaMapiNotificationHandler);
					this.omnhArchives.Add(owaMapiNotificationHandler);
				}
				result = owaMapiNotificationHandler;
			}
			return result;
		}

		private const int MAXFOLDERCOUNTCHANGESUBSCRIPTIONS = 2;

		private UserContext userContext;

		private OwaMapiNotificationHandler omnhLoggedUser;

		private SubscriptionNotificationHandler subscriptionNotificationHandler;

		private Dictionary<OwaStoreObjectId, FolderContentChangeNotificationHandler> folderContentChangeNotificationHandlers = new Dictionary<OwaStoreObjectId, FolderContentChangeNotificationHandler>();

		private ConnectionDroppedNotificationHandler connectionDroppedNotificationHandler;

		private List<OwaMapiNotificationHandler> omnhDelegates;

		private List<OwaMapiNotificationHandler> omnhArchives;
	}
}
