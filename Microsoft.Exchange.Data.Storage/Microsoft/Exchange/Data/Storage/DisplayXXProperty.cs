using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class DisplayXXProperty : AggregateRecipientProperty
	{
		internal DisplayXXProperty(string displayName) : this(displayName, null, null)
		{
		}

		internal DisplayXXProperty(string displayName, NativeStorePropertyDefinition storeComputedProperty, RecipientItemType? recipientItemType) : base(displayName, storeComputedProperty, InternalSchema.DisplayName)
		{
			this.recipientItemType = recipientItemType;
		}

		protected override bool IsRecipientIncluded(RecipientBase recipientBase)
		{
			return this.recipientItemType == null || this.recipientItemType == recipientBase.RecipientItemType;
		}

		private readonly RecipientItemType? recipientItemType = null;
	}
}
