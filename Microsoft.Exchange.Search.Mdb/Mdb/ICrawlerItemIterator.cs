using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Search.Mdb
{
	internal interface ICrawlerItemIterator<TSort> where TSort : struct, IComparable<TSort>
	{
		IEnumerable<MdbItemIdentity> GetItems(StoreSession session, TSort intervalStart, TSort intervalStop);
	}
}
