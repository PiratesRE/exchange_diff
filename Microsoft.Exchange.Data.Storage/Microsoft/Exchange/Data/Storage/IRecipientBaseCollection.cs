using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRecipientBaseCollection<ITEM_TYPE> : IList<ITEM_TYPE>, ICollection<ITEM_TYPE>, IEnumerable<ITEM_TYPE>, IEnumerable where ITEM_TYPE : IRecipientBase
	{
		ITEM_TYPE Add(Participant participant);

		ITEM_TYPE this[RecipientId id]
		{
			get;
		}

		void Remove(RecipientId id);
	}
}
