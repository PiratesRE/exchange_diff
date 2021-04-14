using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Ex12MultivalueEntryIdNoMoveStampStrategy : Ex12MultivalueEntryIdStrategy
	{
		internal Ex12MultivalueEntryIdNoMoveStampStrategy(StorePropertyDefinition property, LocationEntryIdStrategy.GetLocationPropertyBagDelegate getLocationPropertyBag, int index) : base(property, getLocationPropertyBag, index)
		{
		}

		internal override void SetEntryId(DefaultFolderContext context, byte[] entryId)
		{
			byte[][] entryIds = this.GetLocationPropertyBag(context).TryGetProperty(this.Property) as byte[][];
			byte[][] propertyValue = Ex12MultivalueEntryIdStrategy.CreateMultiValuedPropertyValue(entryIds, entryId, this.index, 4);
			base.SetEntryValueInternal(context, propertyValue);
		}
	}
}
