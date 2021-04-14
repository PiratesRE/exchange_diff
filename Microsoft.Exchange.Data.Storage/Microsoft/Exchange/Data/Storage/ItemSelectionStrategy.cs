using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ItemSelectionStrategy : SelectionStrategy
	{
		public override StorePropertyDefinition[] RequiredProperties()
		{
			return ItemSelectionStrategy.DefaultRequiredProperties;
		}

		public override bool HasPriority(IStorePropertyBag item1, IStorePropertyBag item2)
		{
			return ItemSelectionStrategy.HasDefaultPriority(item1, item2);
		}

		public static bool HasDefaultPriority(IStorePropertyBag item1, IStorePropertyBag item2)
		{
			Util.ThrowOnNullArgument(item1, "item1");
			Util.ThrowOnNullArgument(item2, "item2");
			ExDateTime valueOrDefault = item1.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.MinValue);
			ExDateTime valueOrDefault2 = item2.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.MinValue);
			return valueOrDefault > valueOrDefault2;
		}

		public static SelectionStrategy CreateSingleSourceProperty(StorePropertyDefinition propertyDefinition)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			return new ItemSelectionStrategy.ItemSingleSourcePropertySelection(propertyDefinition);
		}

		protected ItemSelectionStrategy() : base(new StorePropertyDefinition[0])
		{
		}

		internal static readonly StorePropertyDefinition[] DefaultRequiredProperties = new StorePropertyDefinition[]
		{
			InternalSchema.CreationTime
		};

		internal class ItemSingleSourcePropertySelection : SelectionStrategy.SingleSourcePropertySelection
		{
			public ItemSingleSourcePropertySelection(StorePropertyDefinition sourceProperty) : base(sourceProperty)
			{
			}

			protected ItemSingleSourcePropertySelection(StorePropertyDefinition sourceProperty, params StorePropertyDefinition[] additionalDependencies) : base(sourceProperty, additionalDependencies)
			{
			}

			public override StorePropertyDefinition[] RequiredProperties()
			{
				return ItemSelectionStrategy.DefaultRequiredProperties;
			}

			public override bool HasPriority(IStorePropertyBag contact1, IStorePropertyBag contact2)
			{
				return ItemSelectionStrategy.HasDefaultPriority(contact1, contact2);
			}
		}
	}
}
