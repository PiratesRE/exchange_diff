using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDistributionList : IContactBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable, IRecipientBaseCollection<DistributionListMember>, IList<DistributionListMember>, ICollection<DistributionListMember>, IEnumerable<DistributionListMember>, IEnumerable
	{
		bool Remove(IDistributionListMember item);

		IEnumerator<IDistributionListMember> IGetEnumerator();
	}
}
