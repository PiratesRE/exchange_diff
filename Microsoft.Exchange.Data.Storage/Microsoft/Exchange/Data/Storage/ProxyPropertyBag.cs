using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ProxyPropertyBag : PropertyBag
	{
		protected ProxyPropertyBag(PropertyBag propertyBag)
		{
			this.propertyBag = propertyBag;
		}

		public override void Load(ICollection<PropertyDefinition> propsToLoad)
		{
			this.propertyBag.Load(propsToLoad);
		}

		public override bool IsDirty
		{
			get
			{
				return this.propertyBag.IsDirty;
			}
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.propertyBag).IsDirty(propertyDefinition);
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			((IDirectPropertyBag)this.propertyBag).SetValue(propertyDefinition, propertyValue);
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.propertyBag).GetValue(propertyDefinition);
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			((IDirectPropertyBag)this.propertyBag).Delete(propertyDefinition);
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.propertyBag).IsLoaded(propertyDefinition);
		}

		internal override ExTimeZone ExTimeZone
		{
			get
			{
				return this.propertyBag.ExTimeZone;
			}
			set
			{
				this.propertyBag.ExTimeZone = value;
			}
		}

		private readonly PropertyBag propertyBag;
	}
}
