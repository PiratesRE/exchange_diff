using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class MailboxListRestrictionFinalCleanup : IMailboxListRestriction
	{
		public SearchCriteria Filter(Context context)
		{
			return Factory.CreateSearchCriteriaCompare(DatabaseSchema.MailboxTable(context.Database).Status, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(5));
		}

		public Index Index(MailboxTable mailboxTable)
		{
			return mailboxTable.MailboxTablePK;
		}
	}
}
