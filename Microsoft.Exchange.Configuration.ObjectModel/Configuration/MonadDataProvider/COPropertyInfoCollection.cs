using System;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class COPropertyInfoCollection : KeyedCollection<string, COPropertyInfo>
	{
		protected override string GetKeyForItem(COPropertyInfo item)
		{
			return item.Name;
		}
	}
}
