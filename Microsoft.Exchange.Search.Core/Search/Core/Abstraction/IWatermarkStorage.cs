using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IWatermarkStorage : IDisposable
	{
		VersionInfo GetVersionInfo();

		long GetNotificationsWatermark();

		ICollection<MailboxCrawlerState> GetMailboxesForCrawling();

		ICollection<MailboxState> GetMailboxesForDeleting();

		IAsyncResult BeginSetVersionInfo(VersionInfo version, AsyncCallback callback, object state);

		void EndSetVersionInfo(IAsyncResult asyncResult);

		IAsyncResult BeginSetNotificationsWatermark(long watermark, AsyncCallback callback, object state);

		void EndSetNotificationsWatermark(IAsyncResult asyncResult);

		IAsyncResult BeginSetMailboxCrawlerState(MailboxCrawlerState mailboxState, AsyncCallback callback, object state);

		void EndSetMailboxCrawlerState(IAsyncResult asyncResult);

		IAsyncResult BeginSetMailboxDeletionPending(MailboxState mailboxState, AsyncCallback callback, object state);

		void EndSetMailboxDeletionPending(IAsyncResult asyncResult);

		void ResetWatermarkCache();

		void RefreshCachedCrawlerWatermarks();

		bool HasCrawlerWatermarks();
	}
}
