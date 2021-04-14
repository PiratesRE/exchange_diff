using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ConversionPropertyBag : PropertyBag
	{
		internal ConversionPropertyBag(IReadOnlyPropertyBag underlyingPropertyBag, SchemaConverter schemaConverter)
		{
			this.schemaConverter = schemaConverter;
			this.underlyingPropertyBag = underlyingPropertyBag;
			this.isReadOnly = !(this.underlyingPropertyBag is IPropertyBag);
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			SchemaConverter.Getter getter = this.schemaConverter.GetGetter(propertyDefinition);
			if (getter != null)
			{
				object obj = getter(this.underlyingPropertyBag);
				if (obj is PropertyErrorCode)
				{
					obj = new PropertyError(propertyDefinition, (PropertyErrorCode)obj);
				}
				return ExTimeZoneHelperForMigrationOnly.ToExDateTimeIfObjectIsDateTime(this.exTimeZone, obj);
			}
			return new PropertyError(propertyDefinition, PropertyErrorCode.NotSupported);
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			SchemaConverter.Setter setter = this.schemaConverter.GetSetter(propertyDefinition);
			if (!this.isReadOnly && setter != null)
			{
				setter((IPropertyBag)this.underlyingPropertyBag, ExTimeZoneHelperForMigrationOnly.ToUtcIfDateTime(propertyValue));
				return;
			}
			throw PropertyError.ToException(new PropertyError[]
			{
				new PropertyError(propertyDefinition, PropertyErrorCode.NotSupported)
			});
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			throw new NotSupportedException();
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			throw new NotSupportedException();
		}

		internal override ExTimeZone ExTimeZone
		{
			get
			{
				return this.exTimeZone;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ExTimeZone");
				}
				this.exTimeZone = value;
			}
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			throw new NotSupportedException();
		}

		public override bool IsDirty
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override void Load(ICollection<PropertyDefinition> propsToLoad)
		{
			throw new NotSupportedException();
		}

		private readonly bool isReadOnly;

		private readonly SchemaConverter schemaConverter;

		private ExTimeZone exTimeZone = ExTimeZone.UtcTimeZone;

		private readonly IReadOnlyPropertyBag underlyingPropertyBag;
	}
}
