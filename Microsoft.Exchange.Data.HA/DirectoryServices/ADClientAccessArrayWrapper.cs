using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADClientAccessArrayWrapper : ADObjectWrapperBase, IADClientAccessArray, IADObjectCommon
	{
		private ADClientAccessArrayWrapper(ClientAccessArray caArray) : base(caArray)
		{
			this.ExchangeLegacyDN = (string)caArray[ClientAccessArraySchema.ExchangeLegacyDN];
		}

		public static ADClientAccessArrayWrapper CreateWrapper(ClientAccessArray caArray)
		{
			if (caArray == null)
			{
				return null;
			}
			return new ADClientAccessArrayWrapper(caArray);
		}

		public string ExchangeLegacyDN { get; private set; }
	}
}
