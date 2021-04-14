using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SelectionStrategy
	{
		protected SelectionStrategy(params StorePropertyDefinition[] dependencies)
		{
			this.dependencies = PropertyDefinitionCollection.Merge<StorePropertyDefinition>(dependencies, this.RequiredProperties());
		}

		public StorePropertyDefinition[] Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		public abstract object GetValue(IStorePropertyBag item);

		public abstract bool IsSelectable(IStorePropertyBag source);

		public abstract bool HasPriority(IStorePropertyBag item1, IStorePropertyBag item2);

		public abstract StorePropertyDefinition[] RequiredProperties();

		private readonly StorePropertyDefinition[] dependencies;

		internal abstract class SingleSourcePropertySelection : SelectionStrategy
		{
			public SingleSourcePropertySelection(StorePropertyDefinition sourceProperty) : base(new StorePropertyDefinition[]
			{
				sourceProperty
			})
			{
				this.sourceProperty = sourceProperty;
			}

			protected SingleSourcePropertySelection(StorePropertyDefinition sourceProperty, params StorePropertyDefinition[] additionalDependencies) : base(PropertyDefinitionCollection.Merge<StorePropertyDefinition>(additionalDependencies, new StorePropertyDefinition[]
			{
				sourceProperty
			}))
			{
				this.sourceProperty = sourceProperty;
			}

			public override bool IsSelectable(IStorePropertyBag source)
			{
				Util.ThrowOnNullArgument(source, "source");
				object value = this.GetValue(source);
				return value != null && !(value is PropertyError);
			}

			public override object GetValue(IStorePropertyBag item)
			{
				Util.ThrowOnNullArgument(item, "item");
				return item.TryGetProperty(this.SourceProperty);
			}

			protected StorePropertyDefinition SourceProperty
			{
				get
				{
					return this.sourceProperty;
				}
			}

			private readonly StorePropertyDefinition sourceProperty;
		}
	}
}
