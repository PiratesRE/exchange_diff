using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal class DefaultItemSynchronizer : ItemSynchronizer
	{
		public DefaultItemSynchronizer(LocalFolder localFolder) : base(localFolder)
		{
		}

		public override void Sync(ItemType item, MailboxSession mailboxSession, ExchangeService exchangeService)
		{
		}

		protected override Item Bind(MailboxSession mailboxSession, StoreId storeId)
		{
			return null;
		}
	}
}
