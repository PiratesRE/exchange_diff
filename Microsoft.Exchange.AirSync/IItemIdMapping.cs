using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal interface IItemIdMapping : IIdMapping
	{
		string Add(ISyncItemId mailboxItemId);

		void Add(ISyncItemId itemId, string syncId);
	}
}
