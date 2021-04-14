using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IUnseenItemsReader : IDisposable
	{
		void LoadLastNItemReceiveDates(IMailboxSession mailboxSession);

		int GetUnseenItemCount(ExDateTime lastVisitedDate);
	}
}
