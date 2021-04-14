using System;
using System.Collections;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal interface IIdMapping
	{
		bool IsDirty { get; }

		IDictionaryEnumerator MailboxIdIdEnumerator { get; }

		IDictionaryEnumerator SyncIdIdEnumerator { get; }

		ISyncItemId this[string syncId]
		{
			get;
		}

		string this[ISyncItemId mailboxId]
		{
			get;
		}

		bool Contains(ISyncItemId mailboxId);

		bool Contains(string syncId);

		void Delete(params ISyncItemId[] mailboxIds);

		void Delete(params string[] syncIds);
	}
}
