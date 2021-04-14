using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SearchNotificationHandler : MapiNotificationHandlerBase, IOwaCallback
	{
		public SearchNotificationHandler(IMailboxContext userContext) : base(userContext, false)
		{
			this.searchNotifier = new SearchNotifier(userContext);
			this.searchNotifier.RegisterWithPendingRequestNotifier();
		}

		public void ProcessCallback(object owaContext)
		{
			OwaSearchContext owaSearchContext = owaContext as OwaSearchContext;
			if (owaSearchContext != null)
			{
				lock (base.SyncRoot)
				{
					base.MissedNotifications = false;
					base.NeedToReinitSubscriptions = false;
					this.SubscribeForSearchComplete(owaSearchContext);
				}
			}
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
		}

		internal override void HandleNotificationInternal(Notification notif, MapiNotificationsLogEvent logEvent, object context)
		{
			if (notif == null)
			{
				return;
			}
			if ((notif.Type & NotificationType.SearchComplete) != NotificationType.SearchComplete)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "notification is not for search complete");
				return;
			}
			OwaSearchContext localSearchContext = context as OwaSearchContext;
			if (localSearchContext == null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "notification has  not passed in context data");
				throw new ArgumentNullException("context");
			}
			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				lock (this.SyncRoot)
				{
					if (localSearchContext != this.currentSearchContext)
					{
						ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "Not sending search completed notification because the currentSearchContext and the localSearchContext are different");
						return;
					}
				}
				SearchNotificationPayload payload = SearchNotificationHandler.SearchPayloadCreator.CreatePayLoad(this.UserContext, localSearchContext);
				lock (this.SyncRoot)
				{
					if (localSearchContext != this.currentSearchContext)
					{
						ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "Payload data calculated. Not sending the notification to the client because another search has been triggered");
					}
					else
					{
						this.searchNotifier.Payload = payload;
						this.searchNotifier.PickupData();
					}
				}
			}, null);
		}

		protected override void InitSubscriptionInternal()
		{
		}

		private void SubscribeForSearchComplete(OwaSearchContext searchContext)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "SearchNotificationHandler.SubscribeForSearchComplete Start");
			if (base.IsDisposed)
			{
				return;
			}
			try
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(searchContext.SearchFolderId);
				base.UserContext.LockAndReconnectMailboxSession(3000);
				if (base.Subscription != null)
				{
					MapiNotificationHandlerBase.DisposeXSOObjects(base.Subscription, base.UserContext);
					base.Subscription = null;
				}
				this.currentSearchContext = searchContext;
				base.Subscription = Subscription.Create(base.UserContext.MailboxSession, this.GetDefaultNotificationHandler(this.currentSearchContext), NotificationType.SearchComplete, storeObjectId);
			}
			catch (OwaLockTimeoutException ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceError<string>((long)this.GetHashCode(), "User context lock timed out in SubscribeForSearchComplete. Exception: {0}", ex.Message);
			}
			finally
			{
				if (base.UserContext.MailboxSessionLockedByCurrentThread())
				{
					base.UserContext.UnlockAndDisconnectMailboxSession();
				}
			}
		}

		protected NotificationHandler GetDefaultNotificationHandler(object context)
		{
			return new NotificationHandler(new SearchNotificationHandler.SubscriptionContextHolder(this, context).HandleNotification);
		}

		private SearchNotifier searchNotifier;

		private OwaSearchContext currentSearchContext;

		private static class SearchPayloadCreator
		{
			public static SearchNotificationPayload CreatePayLoad(IMailboxContext userContext, OwaSearchContext searchContext)
			{
				SearchNotificationPayload payload = new SearchNotificationPayload();
				payload.ClientId = searchContext.ClientSearchFolderIdentity;
				payload.IsComplete = true;
				payload.Source = MailboxLocation.FromMailboxContext(userContext);
				SearchNotificationHandler.SearchPayloadCreator.SetHighlightTerms(payload, searchContext.HighlightTerms);
				try
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						SearchNotificationHandler.SearchPayloadCreator.FillItemDataInPayload(userContext, searchContext, payload);
					});
				}
				catch (GrayException arg)
				{
					payload.ServerSearchResultsRowCount = -1;
					ExTraceGlobals.NotificationsCallTracer.TraceError<GrayException>(0L, "MapiNotificationHandlerBase.CreatePayLoad Unable to create payload with data for search results.  exception {0}", arg);
				}
				return payload;
			}

			private static void FillItemDataInPayload(IMailboxContext userContext, OwaSearchContext searchContext, SearchNotificationPayload payload)
			{
				try
				{
					userContext.LockAndReconnectMailboxSession(3000);
					int serverSearchResultsRowCount = 0;
					if (searchContext.SearchContextType == SearchContextType.ItemSearch)
					{
						payload.MessageItems = SearchFolderItemDataRetriever.GetItemDataFromSearchFolder(searchContext, userContext.MailboxSession, out serverSearchResultsRowCount);
					}
					else if (searchContext.SearchContextType == SearchContextType.ConversationSearch)
					{
						payload.Conversations = SearchFolderConversationRetriever.GetConversationDataFromSearchFolder(searchContext, userContext.MailboxSession, out serverSearchResultsRowCount);
					}
					payload.ServerSearchResultsRowCount = serverSearchResultsRowCount;
				}
				catch (OwaLockTimeoutException ex)
				{
					ExTraceGlobals.CoreCallTracer.TraceError<string>(0L, "User context lock timed out in FillItemDataInPayload. Exception: {0}", ex.Message);
				}
				finally
				{
					if (userContext.MailboxSessionLockedByCurrentThread())
					{
						userContext.UnlockAndDisconnectMailboxSession();
					}
				}
			}

			private static void SetHighlightTerms(SearchNotificationPayload payload, KeyValuePair<string, string>[] searchContextHighlightTerms)
			{
				if (searchContextHighlightTerms == null || searchContextHighlightTerms.Length <= 0)
				{
					return;
				}
				HighlightTermType[] array = new HighlightTermType[searchContextHighlightTerms.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new HighlightTermType
					{
						Scope = searchContextHighlightTerms[i].Key,
						Value = searchContextHighlightTerms[i].Value
					};
				}
				payload.HighlightTerms = array;
			}
		}

		private class SubscriptionContextHolder
		{
			public SubscriptionContextHolder(MapiNotificationHandlerBase parent, object context)
			{
				this.parent = parent;
				this.context = context;
			}

			internal void HandleNotification(Notification notification)
			{
				this.parent.HandleNotification(notification, this.context);
			}

			private readonly object context;

			private readonly MapiNotificationHandlerBase parent;
		}
	}
}
