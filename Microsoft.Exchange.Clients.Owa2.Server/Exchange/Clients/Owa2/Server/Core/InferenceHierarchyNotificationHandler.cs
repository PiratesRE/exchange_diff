using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InferenceHierarchyNotificationHandler : HierarchyNotificationHandler
	{
		public InferenceHierarchyNotificationHandler(string subscriptionId, UserContext userContext, Guid mailboxGuid) : base(subscriptionId, userContext, mailboxGuid)
		{
		}

		protected override void InitSubscriptionInternal()
		{
			base.InitSubscriptionInternal();
			this.ResolveFilteredViewSearchFolderIds();
		}

		protected override bool IsAllowedSearchFolder(StoreObjectId folderId)
		{
			return this.clutterViewFolderId.Equals(folderId) || base.IsAllowedSearchFolder(folderId);
		}

		protected override HierarchyNotificationPayload GetPayloadInstance(StoreObjectId folderId)
		{
			if (this.clutterViewFolderId.Equals(folderId))
			{
				return new FilteredViewHierarchyNotificationPayload
				{
					Filter = ViewFilter.Clutter
				};
			}
			return new HierarchyNotificationPayload();
		}

		private void ResolveFilteredViewSearchFolderIds()
		{
			StoreId defaultFolderId = base.UserContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			this.clutterViewFolderId = OwaFilterState.GetLinkedFolderIdForFilteredView(base.UserContext.MailboxSession, defaultFolderId, OwaViewFilter.Clutter);
			if (this.clutterViewFolderId == null)
			{
				this.clutterViewFolderId = this.ResolveSearchFolderIdForFilteredView(defaultFolderId, OwaViewFilter.Clutter);
			}
		}

		private StoreObjectId ResolveSearchFolderIdForFilteredView(StoreId inboxFolderId, OwaViewFilter viewFilter)
		{
			StoreObjectId storeObjectId = null;
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "HierarchyNotificationHandler.ResolveSearchFolderIdForFilteredView Start. SubscriptionId: {0}", base.SubscriptionId);
			OwaSearchContext owaSearchContext = new OwaSearchContext();
			owaSearchContext.ViewFilter = viewFilter;
			owaSearchContext.FolderIdToSearch = inboxFolderId;
			StoreObjectId defaultFolderId = base.UserContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders);
			using (SearchFolder owaViewFilterSearchFolder = SearchUtil.GetOwaViewFilterSearchFolder(owaSearchContext, base.UserContext.MailboxSession, defaultFolderId, null, CallContext.Current))
			{
				if (owaViewFilterSearchFolder == null)
				{
					throw new ArgumentNullException(string.Format("HierarchyNotificationHandler.ResolveSearchFolderIdForFilteredView null searchFolder returned for subscriptionId: {0}. ViewFilter: {1}; Source folder id: {2}", base.SubscriptionId, viewFilter, inboxFolderId.ToString()));
				}
				storeObjectId = owaViewFilterSearchFolder.StoreObjectId;
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "HierarchyNotificationHandler.ResolveSearchFolderIdForFilteredView found filtered-view search folder subscriptionId: {0} . ViewFilter: {1}; Source folder id: {2}, Search folder id: {3}", new object[]
				{
					base.SubscriptionId,
					viewFilter,
					inboxFolderId.ToString(),
					storeObjectId.ToString()
				});
			}
			return storeObjectId;
		}

		private StoreObjectId clutterViewFolderId;
	}
}
