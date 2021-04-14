using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaMapiNotificationManager : DisposeTrackableBase, INotificationManager, IDisposable
	{
		internal OwaMapiNotificationManager(IMailboxContext userContext)
		{
			this.userContext = userContext;
			this.notificationHandlers = new List<MapiNotificationHandlerBase>();
			this.rowNotificationHandlerCache = new OwaMapiNotificationManager.RowNotificationHandlerCache(this.userContext);
		}

		public event EventHandler<EventArgs> RemoteKeepAliveEvent;

		public SearchNotificationHandler SearchNotificationHandler
		{
			get
			{
				return this.searchHandlerLoggedUser;
			}
		}

		public void SubscribeToHierarchyNotification(string subscriptionId)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					UserContext fullUserContext = this.GetFullUserContext("Hierarchy Notification");
					if (this.hierarchyHandlerLoggedUser == null)
					{
						try
						{
							this.userContext.LockAndReconnectMailboxSession(3000);
							this.hierarchyHandlerLoggedUser = HierachyNotificationHandlerFactory.CreateHandler(subscriptionId, fullUserContext);
							this.notificationHandlers.Add(this.hierarchyHandlerLoggedUser);
						}
						finally
						{
							if (this.userContext.MailboxSessionLockedByCurrentThread())
							{
								this.userContext.UnlockAndDisconnectMailboxSession();
							}
						}
						this.WireConnectionDroppedHandler(this.hierarchyHandlerLoggedUser);
					}
					this.hierarchyHandlerLoggedUser.Subscribe();
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<UserContextKey, string>((long)this.GetHashCode(), "[OwaMapiNotificationManager::SubscribeToHierarchyNotification] START userContextKey: {0} SubscriptionId: {1} Setting this.userContext.HasActiveHierarchySubscription = true", this.userContext.Key, subscriptionId);
					fullUserContext.HasActiveHierarchySubscription = true;
				}
			}
		}

		public void SubscribeToRowNotification(string subscriptionId, SubscriptionParameters parameters, ExTimeZone timeZone, CallContext callContext, bool remoteSubscription)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			if (parameters.FolderId == null)
			{
				throw new OwaInvalidOperationException("Folder Id must be specified when subscribing to row notifications");
			}
			if (subscriptionId == null)
			{
				throw new ArgumentNullException("subscriptionId");
			}
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<UserContextKey, string, string>((long)this.GetHashCode(), "[OwaMapiNotificationManager::SubscribeToRowNotification] START userContextKey: {0} SubscriptionId: {1} ChannelId: {2}", this.userContext.Key, subscriptionId, parameters.ChannelId);
					RowNotificationHandler rowNotificationHandler = null;
					this.rowNotificationHandlerCache.TryGetHandler(subscriptionId, out rowNotificationHandler);
					if (rowNotificationHandler == null)
					{
						StoreObjectId storeObjectId = StoreId.EwsIdToStoreObjectId(parameters.FolderId);
						if (storeObjectId == null)
						{
							throw new OwaInvalidOperationException("Invalid Folder Id. Could not be converted to a storeFolderId");
						}
						if (parameters.NotificationType == NotificationType.CalendarItemNotification)
						{
							rowNotificationHandler = new CalendarItemNotificationHandler(subscriptionId, parameters, storeObjectId, this.userContext, this.userContext.ExchangePrincipal.MailboxInfo.MailboxGuid, timeZone, remoteSubscription);
						}
						else if (parameters.NotificationType == NotificationType.PeopleIKnowNotification)
						{
							rowNotificationHandler = new PeopleIKnowRowNotificationHandler(subscriptionId, parameters, storeObjectId, this.userContext, this.userContext.ExchangePrincipal.MailboxInfo.MailboxGuid, timeZone, callContext.ClientCulture);
						}
						else if (parameters.IsConversation)
						{
							UserContext fullUserContext = this.GetFullUserContext("Conversation row notification");
							rowNotificationHandler = new ConversationRowNotificationHandler(subscriptionId, parameters, storeObjectId, this.userContext, this.userContext.ExchangePrincipal.MailboxInfo.MailboxGuid, timeZone, remoteSubscription, fullUserContext.FeaturesManager);
						}
						else
						{
							UserContext fullUserContext2 = this.GetFullUserContext("MessageItem row notification");
							rowNotificationHandler = new MessageItemRowNotificationHandler(subscriptionId, parameters, storeObjectId, this.userContext, this.userContext.ExchangePrincipal.MailboxInfo.MailboxGuid, timeZone, fullUserContext2.FeaturesManager);
						}
						try
						{
							ExTraceGlobals.NotificationsCallTracer.TraceDebug<UserContextKey, string, string>((long)this.GetHashCode(), "[OwaMapiNotificationManager::SubscribeToRowNotification] userContextKey: {0} New subscription for subscriptionId: {1} ChannelId: {2}", this.userContext.Key, subscriptionId, parameters.ChannelId);
							this.WireConnectionDroppedHandler(rowNotificationHandler);
							rowNotificationHandler.Subscribe();
							rowNotificationHandler.OnBeforeDisposed += this.BeforeDisposeRowNotificationHandler;
							this.rowNotificationHandlerCache.AddHandler(subscriptionId, rowNotificationHandler, parameters.ChannelId);
							rowNotificationHandler = null;
							goto IL_319;
						}
						finally
						{
							if (rowNotificationHandler != null)
							{
								try
								{
									this.userContext.LockAndReconnectMailboxSession(3000);
									rowNotificationHandler.Dispose();
									rowNotificationHandler = null;
								}
								catch (OwaLockTimeoutException ex)
								{
									ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)this.GetHashCode(), "[OwaMapiNotificationManager::SubscribeToRowNotification] User context lock timed out in attempt to dispose handler. Exception: {0}", ex.Message);
								}
								finally
								{
									if (this.userContext.MailboxSessionLockedByCurrentThread())
									{
										this.userContext.UnlockAndDisconnectMailboxSession();
									}
								}
							}
						}
					}
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[OwaMapiNotificationManager::SubscribeToRowNotification] userContextKey: {0} Reusing existing notification handler subscriptionId: {1} ChannelId: {2} Current RefCount: {3}. Setting MissedNotifications = false", new object[]
					{
						this.userContext.Key,
						subscriptionId,
						parameters.ChannelId,
						rowNotificationHandler.RefCount
					});
					rowNotificationHandler.MissedNotifications = false;
					if (rowNotificationHandler.NeedToReinitSubscriptions)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[OwaMapiNotificationManager::SubscribeToRowNotification] userContextKey: {0} Need to re-init subscriptionId: {1} ChannelId: {2} Refcount: {3}", new object[]
						{
							this.userContext.Key,
							subscriptionId,
							parameters.ChannelId,
							rowNotificationHandler.RefCount
						});
						rowNotificationHandler.Subscribe();
					}
					this.rowNotificationHandlerCache.AddHandler(subscriptionId, rowNotificationHandler, parameters.ChannelId);
				}
				IL_319:;
			}
		}

		public void SubscribeToReminderNotification(string subscriptionId)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					if (this.reminderHandlerLoggedUser == null)
					{
						this.reminderHandlerLoggedUser = new ReminderNotificationHandler(subscriptionId, this.userContext);
						this.notificationHandlers.Add(this.reminderHandlerLoggedUser);
						this.WireConnectionDroppedHandler(this.reminderHandlerLoggedUser);
					}
					this.reminderHandlerLoggedUser.Subscribe();
				}
			}
		}

		public void SubscribeToNewMailNotification(string subscriptionId, SubscriptionParameters parameters)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					if (this.newMailHandlerLoggedUser == null)
					{
						this.newMailHandlerLoggedUser = NewMailNotificationHandlerFactory.Create(subscriptionId, this.userContext, parameters);
						this.notificationHandlers.Add(this.newMailHandlerLoggedUser);
						this.WireConnectionDroppedHandler(this.newMailHandlerLoggedUser);
					}
					this.newMailHandlerLoggedUser.Subscribe();
				}
			}
		}

		public string SubscribeToUnseenItemNotification(string subscriptionId, UserMailboxLocator mailboxLocator, IRecipientSession adSession)
		{
			lock (this.syncRoot)
			{
				if (this.isDisposed)
				{
					throw new OwaInvalidOperationException("[OwaMapiNotificationManager::SubscribeToUnseenItemNotification] Subscribe failed because object OwaMapiNotificationManager is disposed.");
				}
				if (this.unseenItemHandler == null)
				{
					this.unseenItemHandler = new UnseenItemNotificationHandler(this.userContext, adSession);
					this.unseenItemHandler.Subscribe();
					this.notificationHandlers.Add(this.unseenItemHandler);
					this.WireConnectionDroppedHandler(this.unseenItemHandler);
				}
			}
			string result;
			try
			{
				this.userContext.LockAndReconnectMailboxSession(3000);
				result = this.unseenItemHandler.AddMemberSubscription(subscriptionId, mailboxLocator);
			}
			finally
			{
				if (this.userContext.MailboxSessionLockedByCurrentThread())
				{
					this.userContext.UnlockAndDisconnectMailboxSession();
				}
			}
			return result;
		}

		public void SubscribeToUnseenCountNotification(string subscriptionId, SubscriptionParameters parameters, IRecipientSession adSession)
		{
			throw new NotSupportedException("SubscribeToUnseenCountNotification is only supported through Broker not in OwaMapiNotificationManager.");
		}

		public void UnsubscribeToUnseenCountNotification(string subscriptionId, SubscriptionParameters parameters)
		{
			throw new NotSupportedException("UnSubscribeToUnseenCountNotification is only supported through Broker not in OwaMapiNotificationManager.");
		}

		public void SubscribeToGroupAssociationNotification(string subscriptionId, IRecipientSession adSession)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed && this.groupAssociationHandlerLoggedUser == null)
				{
					this.groupAssociationHandlerLoggedUser = new GroupAssociationNotificationHandler(subscriptionId, this.userContext, adSession);
					this.notificationHandlers.Add(this.groupAssociationHandlerLoggedUser);
					this.WireConnectionDroppedHandler(this.groupAssociationHandlerLoggedUser);
					this.groupAssociationHandlerLoggedUser.Subscribe();
				}
			}
		}

		public void SubscribeToSearchNotification()
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed && this.searchHandlerLoggedUser == null)
				{
					this.searchHandlerLoggedUser = new SearchNotificationHandler(this.userContext);
					this.notificationHandlers.Add(this.searchHandlerLoggedUser);
					this.WireConnectionDroppedHandler(this.searchHandlerLoggedUser);
					this.searchHandlerLoggedUser.Subscribe();
				}
			}
		}

		public void UnsubscribeForRowNotifications(string subscriptionId, SubscriptionParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[OwaMapiNotificationManager::UnsubscribeForRowNotifications] SubscriptionId: {0} ChannelId: {1}", subscriptionId, parameters.ChannelId);
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.rowNotificationHandlerCache.ReleaseHandler(subscriptionId, parameters.ChannelId);
				}
			}
		}

		public void ReleaseSubscription(string subscriptionId)
		{
			if (subscriptionId == null)
			{
				throw new ArgumentNullException("subscriptionId");
			}
			lock (this.syncRoot)
			{
				if (!this.isDisposed && this.unseenItemHandler != null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[OwaMapiNotificationManager::ReleaseSubscription] Removing UnseenItem subscription for SubscriptionId: {0}", subscriptionId);
					this.unseenItemHandler.RemoveSubscription(subscriptionId);
					if (!this.unseenItemHandler.HasNotifiers())
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[OwaMapiNotificationManager::ReleaseSubscription] Disposing UnseenItem handler since there are no more notifiers active.");
						this.unseenItemHandler.Dispose();
						this.unseenItemHandler = null;
					}
				}
			}
		}

		public void ReleaseSubscriptionsForChannelId(string channelId)
		{
			if (channelId == null)
			{
				throw new ArgumentNullException("channelId");
			}
			lock (this.syncRoot)
			{
				if (!this.isDisposed && this.rowNotificationHandlerCache != null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[OwaMapiNotificationManager::ReleaseSubscriptionsForChannelId] ChannelId: {0}", channelId);
					this.rowNotificationHandlerCache.ReleaseHandlersForChannelId(channelId);
				}
			}
		}

		public void RefreshSubscriptions(ExTimeZone timeZone)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					foreach (MapiNotificationHandlerBase mapiNotificationHandlerBase in this.notificationHandlers)
					{
						mapiNotificationHandlerBase.NeedToReinitSubscriptions = true;
						mapiNotificationHandlerBase.Subscribe();
					}
					this.rowNotificationHandlerCache.RefreshSubscriptions(timeZone);
				}
			}
		}

		public void CleanupSubscriptions()
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					foreach (MapiNotificationHandlerBase mapiNotificationHandlerBase in this.notificationHandlers)
					{
						mapiNotificationHandlerBase.DisposeSubscriptions();
					}
					this.rowNotificationHandlerCache.DisposeSubscriptions();
				}
			}
		}

		public void HandleConnectionDroppedNotification()
		{
			if (this.connectionDroppedNotificationHandler != null)
			{
				this.connectionDroppedNotificationHandler.HandleNotification(null);
			}
		}

		public void StartRemoteKeepAliveTimer()
		{
			bool flag = this.isDisposed;
			bool flag2 = this.remoteKeepAliveTimer == null;
			if (flag2)
			{
				lock (this.syncRoot)
				{
					flag = this.isDisposed;
					if (!flag)
					{
						flag2 = (this.remoteKeepAliveTimer == null);
						if (flag2)
						{
							this.remoteKeepAliveTimer = new Timer(new TimerCallback(this.RemoteKeepAliveTimerCallback), null, 120000, 120000);
						}
					}
				}
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "OwaMapiNotificationManager.StartRemoteKeepAliveTimer. isTimerNull: {0},  isDisposed: {1}.", flag2, flag);
		}

		private void RemoteKeepAliveTimerCallback(object state)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "OwaMapiNotificationManager.RemoteKeepAliveTimerCallback. Calling all registered handlers.");
			EventHandler<EventArgs> remoteKeepAliveEvent = this.RemoteKeepAliveEvent;
			if (remoteKeepAliveEvent != null)
			{
				remoteKeepAliveEvent(this, EventArgs.Empty);
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "OwaMapiNotificationManager.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing)
			{
				try
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						lock (this.syncRoot)
						{
							if (this.hierarchyHandlerLoggedUser != null)
							{
								this.hierarchyHandlerLoggedUser.Dispose();
								this.hierarchyHandlerLoggedUser = null;
							}
							if (this.reminderHandlerLoggedUser != null)
							{
								this.reminderHandlerLoggedUser.Dispose();
								this.reminderHandlerLoggedUser = null;
							}
							if (this.newMailHandlerLoggedUser != null)
							{
								this.newMailHandlerLoggedUser.Dispose();
								this.newMailHandlerLoggedUser = null;
							}
							if (this.unseenItemHandler != null)
							{
								this.unseenItemHandler.Dispose();
								this.unseenItemHandler = null;
							}
							if (this.groupAssociationHandlerLoggedUser != null)
							{
								this.groupAssociationHandlerLoggedUser.Dispose();
								this.groupAssociationHandlerLoggedUser = null;
							}
							if (this.searchHandlerLoggedUser != null)
							{
								this.searchHandlerLoggedUser.Dispose();
								this.searchHandlerLoggedUser = null;
							}
							if (this.rowNotificationHandlerCache != null)
							{
								ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[OwaMapiNotificationManager.Dispose]. Calling this.rowNotificationHandlerCache.Clear()");
								this.rowNotificationHandlerCache.Clear();
								this.rowNotificationHandlerCache = null;
							}
							if (this.connectionDroppedNotificationHandler != null)
							{
								this.connectionDroppedNotificationHandler.Dispose();
								this.connectionDroppedNotificationHandler = null;
							}
							if (this.notificationHandlers != null)
							{
								this.notificationHandlers.Clear();
								this.notificationHandlers = null;
							}
							if (this.remoteKeepAliveTimer != null)
							{
								this.remoteKeepAliveTimer.Dispose();
								this.remoteKeepAliveTimer = null;
							}
							this.isDisposed = true;
						}
					});
				}
				catch (GrayException ex)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "[OwaMapiNotificationManager.Dispose]. Unable to dispose object.  exception {0}", ex.Message);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaMapiNotificationManager>(this);
		}

		private void InitializeConnectionDroppedHandler()
		{
			if (this.connectionDroppedNotificationHandler == null)
			{
				this.connectionDroppedNotificationHandler = new ConnectionDroppedNotificationHandler(this.userContext);
				this.notificationHandlers.Add(this.connectionDroppedNotificationHandler);
				this.connectionDroppedNotificationHandler.Subscribe();
			}
		}

		private void WireConnectionDroppedHandler(MapiNotificationHandlerBase handler)
		{
			this.InitializeConnectionDroppedHandler();
			this.connectionDroppedNotificationHandler.OnConnectionDropped += handler.HandleConnectionDroppedNotification;
		}

		private void BeforeDisposeRowNotificationHandler(ConnectionDroppedNotificationHandler.ConnectionDroppedEventHandler connectionDroppedEventHandler)
		{
			if (connectionDroppedEventHandler == null)
			{
				throw new ArgumentNullException("connectionDroppedEventHandler");
			}
			if (this.connectionDroppedNotificationHandler != null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "OwaMapiNotificationManager.BeforeDisposeRowNotificationHandler. Removing connection dropped event handler");
				this.connectionDroppedNotificationHandler.OnConnectionDropped -= connectionDroppedEventHandler;
			}
		}

		private UserContext GetFullUserContext(string componentName)
		{
			UserContext userContext = this.userContext as UserContext;
			if (userContext == null)
			{
				throw new OwaInvalidOperationException(componentName + " must have a full user context to work");
			}
			return userContext;
		}

		private const int RemoteKeepAliveTimerIntervalInMilliSeconds = 120000;

		private IMailboxContext userContext;

		private HierarchyNotificationHandler hierarchyHandlerLoggedUser;

		private ReminderNotificationHandler reminderHandlerLoggedUser;

		private NewMailNotificationHandler newMailHandlerLoggedUser;

		private GroupAssociationNotificationHandler groupAssociationHandlerLoggedUser;

		private SearchNotificationHandler searchHandlerLoggedUser;

		private OwaMapiNotificationManager.RowNotificationHandlerCache rowNotificationHandlerCache;

		private ConnectionDroppedNotificationHandler connectionDroppedNotificationHandler;

		private UnseenItemNotificationHandler unseenItemHandler;

		private List<MapiNotificationHandlerBase> notificationHandlers;

		private object syncRoot = new object();

		private bool isDisposed;

		private Timer remoteKeepAliveTimer;

		internal class RowNotificationHandlerCache
		{
			internal RowNotificationHandlerCache(IMailboxContext userContext)
			{
				this.handlerCache = new Dictionary<string, RowNotificationHandler>();
				this.channelIdCache = new Dictionary<string, List<string>>();
				this.userContext = userContext;
			}

			internal Dictionary<string, RowNotificationHandler> HandlerCache
			{
				get
				{
					return this.handlerCache;
				}
			}

			internal Dictionary<string, List<string>> ChannelIdCache
			{
				get
				{
					return this.channelIdCache;
				}
			}

			internal bool TryGetHandler(string subscriptionId, out RowNotificationHandler handler)
			{
				if (subscriptionId == null)
				{
					throw new ArgumentNullException("subscriptionId");
				}
				if (this.handlerCache == null)
				{
					throw new OwaInvalidOperationException("this.handlerCache may not be null");
				}
				if (this.channelIdCache == null)
				{
					throw new OwaInvalidOperationException("this.channelIdCache may not be null");
				}
				handler = null;
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::TryGetHandler] TryGetHandle for SubscriptionId: {0}", subscriptionId);
				if (this.handlerCache.ContainsKey(subscriptionId))
				{
					handler = this.handlerCache[subscriptionId];
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[RowNotificationHandlerCache::TryGetHandler] Found handler for SubscriptionId: {0}. Current RefCount: {1}", subscriptionId, handler.RefCount);
					return true;
				}
				return false;
			}

			internal void AddHandler(string subscriptionId, RowNotificationHandler handler, string channelId)
			{
				if (subscriptionId == null)
				{
					throw new ArgumentNullException("subscriptionId");
				}
				if (handler == null)
				{
					throw new ArgumentNullException("handler");
				}
				if (this.handlerCache == null)
				{
					throw new OwaInvalidOperationException("this.handlerCache may not be null");
				}
				if (this.channelIdCache == null)
				{
					throw new OwaInvalidOperationException("this.channelIdCache may not be null");
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "[RowNotificationHandlerCache::AddHandler] Adding handler for SubscriptionId: {0}. ChannelId: {1}. Current RefCount: {2}", subscriptionId, channelId, handler.RefCount);
				if (!this.handlerCache.ContainsKey(subscriptionId))
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::AddHandler] Adding new handler for SubscriptionId: {0}. ChannelId: {1}. New RefCount: 1", subscriptionId, channelId);
					handler.RefCount = 1;
					this.handlerCache[subscriptionId] = handler;
					if (channelId != null)
					{
						this.TryAddSubscriptionIdToChannelIdCache(subscriptionId, channelId);
						return;
					}
				}
				else if (channelId != null)
				{
					bool flag = true;
					if (this.TryAddSubscriptionIdToChannelIdCache(subscriptionId, channelId))
					{
						handler.RefCount++;
						flag = false;
					}
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[RowNotificationHandlerCache::AddHandler] Is duplicate subscription request: '{0}' for existing unique view handler. SubscriptionId: {1}. ChannelId: {2}. RefCount: {3}", new object[]
					{
						flag,
						subscriptionId,
						channelId,
						handler.RefCount
					});
				}
			}

			internal void ReleaseHandler(string subscriptionId, string channelId)
			{
				if (subscriptionId == null)
				{
					throw new ArgumentNullException("subscriptionId");
				}
				if (this.handlerCache == null)
				{
					throw new OwaInvalidOperationException("this.handlerCache may not be null");
				}
				if (this.channelIdCache == null)
				{
					throw new OwaInvalidOperationException("this.channelIdCache may not be null");
				}
				RowNotificationHandler rowNotificationHandler = null;
				if (this.handlerCache.TryGetValue(subscriptionId, out rowNotificationHandler))
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[RowNotificationHandlerCache::ReleaseHandler] releasing handler for SubscriptionId: {0} ChannelId: {1} Old RefCount: {2} New RefCount: {3}", new object[]
					{
						rowNotificationHandler.SubscriptionId,
						channelId,
						rowNotificationHandler.RefCount,
						rowNotificationHandler.RefCount - 1
					});
					if (--rowNotificationHandler.RefCount == 0)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::ReleaseHandler] Disposing handler for SubscriptionId: {0} ChannelId: {1}", subscriptionId, channelId);
						this.handlerCache.Remove(subscriptionId);
						if (!this.userContext.MailboxSessionLockedByCurrentThread())
						{
							try
							{
								try
								{
									this.userContext.LockAndReconnectMailboxSession(3000);
									rowNotificationHandler.Dispose();
									rowNotificationHandler = null;
								}
								catch (OwaLockTimeoutException)
								{
									ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::ReleaseHandler] Disposing handler for SubscriptionId: {0} ChannelId: {1} Failed to acquire mbx lock", subscriptionId, channelId);
								}
								catch (StoragePermanentException ex)
								{
									ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "[RowNotificationHandlerCache::ReleaseHandler]. Unable to dispose object.  exception {0}", ex.Message);
								}
								catch (StorageTransientException ex2)
								{
									ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "[RowNotificationHandlerCache::ReleaseHandler]. Unable to dispose object.  exception {0}", ex2.Message);
								}
								goto IL_15C;
							}
							finally
							{
								this.userContext.UnlockAndDisconnectMailboxSession();
							}
						}
						rowNotificationHandler.Dispose();
						rowNotificationHandler = null;
					}
				}
				IL_15C:
				if (channelId == null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::ReleaseHandler] Returning without updating channel id lookup cache for non-unique view handler for SubscriptionId: {0}. ChannelId is null.", subscriptionId);
					return;
				}
				this.RemoveSubscriptionIdFromChannelIdCache(subscriptionId, channelId);
			}

			internal void ReleaseHandlersForChannelId(string channelId)
			{
				if (channelId == null)
				{
					throw new ArgumentNullException("channelId");
				}
				if (this.handlerCache == null)
				{
					throw new OwaInvalidOperationException("this.handlerCache may not be null");
				}
				if (this.channelIdCache == null)
				{
					throw new OwaInvalidOperationException("this.channelIdCache may not be null");
				}
				List<string> list = new List<string>();
				if (this.channelIdCache.ContainsKey(channelId))
				{
					list = this.channelIdCache[channelId];
					if (list == null)
					{
						throw new OwaInvalidOperationException("channelIdCache list of subscription ids for channel id: {0} may not be null");
					}
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<int, string>((long)this.GetHashCode(), "RowNotificationHandlerCache::ReleaseHandlersForChannelId - subscription list count is: {0} for channelId being removed: {1}", list.Count, channelId);
					this.channelIdCache.Remove(channelId);
					for (int i = 0; i < list.Count; i++)
					{
						this.ReleaseHandler(list[i], channelId);
					}
				}
			}

			internal void RefreshSubscriptions(ExTimeZone timeZone)
			{
				if (timeZone == null)
				{
					throw new ArgumentNullException("timeZone");
				}
				if (this.handlerCache == null)
				{
					throw new OwaInvalidOperationException("this.handlerCache may not be null");
				}
				if (this.channelIdCache == null)
				{
					throw new OwaInvalidOperationException("this.channelIdCache may not be null");
				}
				foreach (RowNotificationHandler rowNotificationHandler in this.handlerCache.Values)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::RefreshSubscriptions] Calling Subscribe after Resetting timeZone and setting NeedToReinitSubscriptions = true for SubscriptionId: {0}", rowNotificationHandler.SubscriptionId);
					rowNotificationHandler.TimeZone = timeZone;
					rowNotificationHandler.NeedToReinitSubscriptions = true;
					if (!Globals.Owa2ServerUnitTestsHook)
					{
						rowNotificationHandler.Subscribe();
					}
				}
			}

			internal void DisposeSubscriptions()
			{
				if (this.handlerCache == null)
				{
					throw new OwaInvalidOperationException("this.handlerCache may not be null");
				}
				foreach (RowNotificationHandler rowNotificationHandler in this.handlerCache.Values)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::DisposeSubsriptions] Calling Dispose after Resetting timeZone", rowNotificationHandler.SubscriptionId);
					if (!Globals.Owa2ServerUnitTestsHook)
					{
						rowNotificationHandler.DisposeSubscriptions();
					}
				}
			}

			internal void Clear()
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[RowNotificationHandlerCache::Clear] disposing all row notification handlers");
				try
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						if (this.handlerCache == null)
						{
							throw new OwaInvalidOperationException("this.handlerCache may not be null");
						}
						if (this.channelIdCache == null)
						{
							throw new OwaInvalidOperationException("this.channelIdCache may not be null");
						}
						foreach (string key in this.handlerCache.Keys)
						{
							RowNotificationHandler rowNotificationHandler = this.handlerCache[key];
							ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::Clear] Disposing handler for SubscriptionId: {0}", rowNotificationHandler.SubscriptionId);
							rowNotificationHandler.Dispose();
						}
						this.handlerCache = new Dictionary<string, RowNotificationHandler>();
						this.channelIdCache = new Dictionary<string, List<string>>();
					});
				}
				catch (GrayException ex)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "MapiNotificationHandlerBase.Dispose Unable to dispose object.  exception {0}", ex.Message);
				}
			}

			private bool TryAddSubscriptionIdToChannelIdCache(string subscriptionId, string channelId)
			{
				if (subscriptionId == null)
				{
					throw new OwaInvalidOperationException("subscriptionId may not be null");
				}
				if (channelId == null)
				{
					throw new OwaInvalidOperationException("channelId may not be null");
				}
				bool result = false;
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::TryAddSubscriptionIdToChannelIdCache] SubscriptionId: {0}. ChannelId: {1}.", subscriptionId, channelId);
				List<string> list = new List<string>();
				if (this.channelIdCache.ContainsKey(channelId))
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::TryAddSubscriptionIdToChannelIdCache] for pre-existing entry for SubscriptionId: {0}. ChannelId: {1}.", subscriptionId, channelId);
					list = this.channelIdCache[channelId];
				}
				if (!list.Contains(subscriptionId))
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::TryAddSubscriptionIdToChannelIdCache] Adding new subcription id: {0}. for channelId: {1}.", subscriptionId, channelId);
					list.Add(subscriptionId);
					result = true;
				}
				this.channelIdCache[channelId] = list;
				return result;
			}

			private void RemoveSubscriptionIdFromChannelIdCache(string subscriptionId, string channelId)
			{
				if (subscriptionId == null)
				{
					throw new OwaInvalidOperationException("subscriptionId may not be null");
				}
				if (channelId == null)
				{
					throw new OwaInvalidOperationException("channelId may not be null");
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::RemoveSubscriptionIdFromChannelIdCache] SubscriptionId: {0}. ChannelId: {1}.", subscriptionId, channelId);
				List<string> subIds = new List<string>();
				if (this.channelIdCache.ContainsKey(channelId))
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::RemoveSubscriptionIdFromChannelIdCache] Found entry for ChannelId: {0}. SubscriptionId: {1}.", channelId, subscriptionId);
					subIds = this.channelIdCache[channelId];
					this.RemoveSubscriptionIdFromList(subscriptionId, subIds, channelId);
				}
			}

			private void RemoveSubscriptionIdFromList(string subscriptionId, List<string> subIds, string channelId)
			{
				if (subscriptionId == null)
				{
					throw new OwaInvalidOperationException("subscriptionId may not be null");
				}
				if (subIds == null)
				{
					throw new ArgumentNullException("subIds");
				}
				if (channelId == null)
				{
					throw new OwaInvalidOperationException("channelId may not be null");
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "[RowNotificationHandlerCache::RemoveSubscriptionIdFromList] SubscriptionId: {0}. channelId: {1}. subscriptionId list count: {2}", subscriptionId, channelId, subIds.Count);
				if (subIds.Contains(subscriptionId))
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::RemoveSubscriptionIdFromList] Removing subscriptionId: {0} from channel id lookup cache list", subscriptionId);
					subIds.Remove(subscriptionId);
					if (subIds.Count == 0)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[RowNotificationHandlerCache::RemoveSubscriptionIdFromList] Removing channelId: {0} from channel id lookup cache", channelId);
						this.channelIdCache.Remove(channelId);
					}
				}
			}

			private Dictionary<string, RowNotificationHandler> handlerCache;

			private Dictionary<string, List<string>> channelIdCache;

			private IMailboxContext userContext;
		}
	}
}
