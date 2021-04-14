using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class RecipientToIndexProperty : SmartPropertyDefinition
	{
		internal RecipientToIndexProperty(string displayName, RecipientItemType? recipientItemType, NativeStorePropertyDefinition nativeStoreProperty) : base(displayName, typeof(List<RecipientToIndex>), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.GroupExpansionRecipients, PropertyDependencyType.NeedForRead)
		})
		{
			this.recipientItemType = recipientItemType;
			this.nativeStoreProperty = nativeStoreProperty;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return base.Capabilities | StorePropertyCapabilities.CanQuery;
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
			if (messageItem != null)
			{
				GroupExpansionRecipients groupExpansionRecipients = GroupExpansionRecipients.RetrieveFromStore(messageItem, InternalSchema.GroupExpansionRecipients);
				if (groupExpansionRecipients != null)
				{
					if (this.recipientItemType != null && this.recipientItemType != null)
					{
						RecipientItemType recipientType = this.recipientItemType.Value;
						return new List<RecipientToIndex>(from r in groupExpansionRecipients.Recipients
						where r.RecipientType == recipientType
						select r);
					}
					return groupExpansionRecipients.Recipients;
				}
			}
			return new PropertyError(this, PropertyErrorCode.GetCalculatedPropertyError);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, this.nativeStoreProperty);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.nativeStoreProperty);
		}

		private readonly RecipientItemType? recipientItemType;

		private readonly NativeStorePropertyDefinition nativeStoreProperty;
	}
}
