using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetImItemList
	{
		public GetImItemList(ExtendedPropertyUri[] extendedProperties, UnifiedContactStoreUtilities unifiedContactStoreUtilities)
		{
			if (unifiedContactStoreUtilities == null)
			{
				throw new ArgumentNullException("unifiedContactStoreUtilities");
			}
			this.unifiedContactStoreUtilities = unifiedContactStoreUtilities;
			this.extendedProperties = extendedProperties;
		}

		public ImItemList Execute()
		{
			return this.unifiedContactStoreUtilities.GetImItemList(this.extendedProperties);
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly ExtendedPropertyUri[] extendedProperties;
	}
}
