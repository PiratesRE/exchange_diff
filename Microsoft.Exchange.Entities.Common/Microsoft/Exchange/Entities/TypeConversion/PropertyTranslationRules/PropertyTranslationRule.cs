using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules
{
	internal class PropertyTranslationRule<TLeft, TRight, TLeftProperty, TLeftValue, TRightValue> : IStorageTranslationRule<!0, !1>, IPropertyValueCollectionTranslationRule<TLeft, TLeftProperty, TRight>, ITranslationRule<TLeft, TRight>
	{
		public PropertyTranslationRule(IPropertyAccessor<TLeft, TLeftValue> leftAccessor, IPropertyAccessor<TRight, TRightValue> rightAccessor, IConverter<TLeftValue, TRightValue> leftToRightConverter = null, IConverter<TRightValue, TLeftValue> rightToLeftConverter = null)
		{
			this.leftToRightConverter = leftToRightConverter;
			this.rightToLeftConverter = rightToLeftConverter;
			this.LeftAccessor = leftAccessor;
			this.RightAccessor = rightAccessor;
			IStoragePropertyAccessor<TLeft, TLeftValue> storagePropertyAccessor = leftAccessor as IStoragePropertyAccessor<TLeft, TLeftValue>;
			if (storagePropertyAccessor == null)
			{
				IStoragePropertyAccessor<TRight, TRightValue> storagePropertyAccessor2 = rightAccessor as IStoragePropertyAccessor<TRight, TRightValue>;
				if (storagePropertyAccessor2 != null)
				{
					EntityPropertyAccessorBase<TLeft, TLeftValue> entityPropertyAccessorBase = leftAccessor as EntityPropertyAccessorBase<TLeft, TLeftValue>;
					if (entityPropertyAccessorBase != null)
					{
						this.storageDependencies = storagePropertyAccessor2.Dependencies;
						this.storagePropertyGroup = storagePropertyAccessor2.PropertyChangeMetadataGroup;
						this.entityProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
						{
							entityPropertyAccessorBase.PropertyDefinition
						};
						return;
					}
				}
			}
			else
			{
				EntityPropertyAccessorBase<TRight, TRightValue> entityPropertyAccessorBase2 = rightAccessor as EntityPropertyAccessorBase<TRight, TRightValue>;
				if (entityPropertyAccessorBase2 != null)
				{
					this.storageDependencies = storagePropertyAccessor.Dependencies;
					this.storagePropertyGroup = storagePropertyAccessor.PropertyChangeMetadataGroup;
					this.entityProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
					{
						entityPropertyAccessorBase2.PropertyDefinition
					};
				}
			}
		}

		public IPropertyAccessor<TLeft, TLeftValue> LeftAccessor { get; private set; }

		public IPropertyAccessor<TRight, TRightValue> RightAccessor { get; private set; }

		IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> IStorageTranslationRule<!0, !1>.StorageDependencies
		{
			get
			{
				return this.storageDependencies;
			}
		}

		PropertyChangeMetadata.PropertyGroup IStorageTranslationRule<!0, !1>.StoragePropertyGroup
		{
			get
			{
				return this.storagePropertyGroup;
			}
		}

		IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> IStorageTranslationRule<!0, !1>.EntityProperties
		{
			get
			{
				return this.entityProperties;
			}
		}

		public virtual IConverter<TLeftValue, TRightValue> GetLeftToRightConverter(TLeft left, TRight right)
		{
			return this.leftToRightConverter;
		}

		public virtual IConverter<TRightValue, TLeftValue> GetRightToLeftConverter(TLeft left, TRight right)
		{
			return this.rightToLeftConverter;
		}

		public void FromLeftToRightType(TLeft left, TRight right)
		{
			PropertyTranslationRule<TLeft, TRight, TLeftProperty, TLeftValue, TRightValue>.PerformTranslation<TLeft, TRight, TLeftValue, TRightValue>(left, this.LeftAccessor, right, this.RightAccessor, this.GetLeftToRightConverter(left, right));
		}

		public void FromRightToLeftType(TLeft left, TRight right)
		{
			PropertyTranslationRule<TLeft, TRight, TLeftProperty, TLeftValue, TRightValue>.PerformTranslation<TRight, TLeft, TRightValue, TLeftValue>(right, this.RightAccessor, left, this.LeftAccessor, this.GetRightToLeftConverter(left, right));
		}

		public void FromPropertyValues(IDictionary<TLeftProperty, int> propertyIndices, IList values, TRight right)
		{
			IPropertyValueCollectionAccessor<TLeft, TLeftProperty, TLeftValue> accessor;
			IConverter<TLeftValue, TRightValue> converter;
			if (this.CanTranslateFromPropertyValues<TLeftProperty>(out accessor, out converter))
			{
				PropertyTranslationRule<TLeft, TRight, TLeftProperty, TLeftValue, TRightValue>.PerformTranslation<TRight, TLeftValue, TRightValue>(delegate(out TLeftValue leftValue)
				{
					return accessor.TryGetValue(propertyIndices, values, out leftValue);
				}, right, this.RightAccessor, converter);
			}
		}

		protected static void PerformTranslation<TSource, TDestination, TSourceValue, TDestinationValue>(TSource source, IPropertyAccessor<TSource, TSourceValue> sourceAccessor, TDestination destination, IPropertyAccessor<TDestination, TDestinationValue> destinationAccessor, IConverter<TSourceValue, TDestinationValue> converter)
		{
			PropertyTranslationRule<TLeft, TRight, TLeftProperty, TLeftValue, TRightValue>.PerformTranslation<TDestination, TSourceValue, TDestinationValue>(delegate(out TSourceValue sourceValue)
			{
				return sourceAccessor.TryGetValue(source, out sourceValue);
			}, destination, destinationAccessor, converter);
		}

		protected static void PerformTranslation<TDestination, TSourceValue, TDestinationValue>(PropertyTranslationRule<TLeft, TRight, TLeftProperty, TLeftValue, TRightValue>.TryGetValueFunc<TSourceValue> tryGetSourceValue, TDestination destination, IPropertyAccessor<TDestination, TDestinationValue> destinationAccessor, IConverter<TSourceValue, TDestinationValue> converter)
		{
			if (tryGetSourceValue == null)
			{
				throw new ArgumentNullException("tryGetSourceValue");
			}
			TSourceValue value;
			if (converter != null && !destinationAccessor.Readonly && tryGetSourceValue(out value))
			{
				TDestinationValue value2 = converter.Convert(value);
				destinationAccessor.Set(destination, value2);
			}
		}

		private bool CanTranslateFromPropertyValues<TProperty>(out IPropertyValueCollectionAccessor<TLeft, TProperty, TLeftValue> accessor, out IConverter<TLeftValue, TRightValue> converter)
		{
			accessor = (this.LeftAccessor as IPropertyValueCollectionAccessor<TLeft, TProperty, TLeftValue>);
			converter = this.leftToRightConverter;
			return accessor != null && converter != null;
		}

		private readonly IConverter<TLeftValue, TRightValue> leftToRightConverter;

		private readonly IConverter<TRightValue, TLeftValue> rightToLeftConverter;

		private readonly IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> storageDependencies;

		private readonly PropertyChangeMetadata.PropertyGroup storagePropertyGroup;

		private readonly Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[] entityProperties;

		public delegate bool TryGetValueFunc<TValue>(out TValue value);
	}
}
