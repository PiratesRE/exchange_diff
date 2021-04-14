using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OscFolderContactSourcesEnumerator : IEnumerable<OscNetworkMoniker>, IEnumerable
	{
		public OscFolderContactSourcesEnumerator(IStorePropertyBag item)
		{
			Util.ThrowOnNullArgument(item, "item");
			this.item = item;
		}

		public IEnumerator<OscNetworkMoniker> GetEnumerator()
		{
			return (from moniker in this.GetNetworkMonikers()
			where !string.IsNullOrWhiteSpace(moniker)
			select new OscNetworkMoniker(moniker)).GetEnumerator();
		}

		private string[] GetNetworkMonikers()
		{
			object obj = this.item.TryGetProperty(MessageItemSchema.OscContactSources);
			if (obj == null || obj is PropertyError)
			{
				return Array<string>.Empty;
			}
			return (string[])obj;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private readonly IStorePropertyBag item;
	}
}
