using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class IdProperty : SmartPropertyDefinition
	{
		protected internal IdProperty(string displayName, Type valueType, PropertyFlags flags, PropertyDefinitionConstraint[] constraints, params PropertyDependency[] dependencies) : base(displayName, valueType, flags, constraints, dependencies)
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyStore)
		{
			StoreObjectId storeObjectId = null;
			ICoreObject coreObject = propertyStore.Context.CoreObject;
			if (coreObject != null)
			{
				storeObjectId = coreObject.InternalStoreObjectId;
			}
			if (storeObjectId == null)
			{
				byte[] array = propertyStore.GetValue(InternalSchema.EntryId) as byte[];
				if (array != null)
				{
					storeObjectId = StoreObjectId.FromProviderSpecificId(array, this.GetStoreObjectType(propertyStore));
				}
				else
				{
					QueryResultPropertyBag queryResultPropertyBag = ((PropertyBag)propertyStore) as QueryResultPropertyBag;
					if (queryResultPropertyBag != null && (!((IDirectPropertyBag)queryResultPropertyBag).IsLoaded(InternalSchema.RowType) || object.Equals(queryResultPropertyBag.TryGetProperty(InternalSchema.RowType), 1)))
					{
						ExDiagnostics.FailFast(string.Format("EntryId: \"{0}\" in view", queryResultPropertyBag.TryGetProperty(InternalSchema.EntryId)), false);
					}
				}
			}
			if (storeObjectId == null)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			VersionedId versionedId = new VersionedId(storeObjectId, this.GetChangeKey(propertyStore));
			if (!this.IsCompatibleId(versionedId, coreObject))
			{
				return new PropertyError(this, PropertyErrorCode.NotSupported);
			}
			return versionedId;
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return IdProperty.SmartIdFilterToNativeIdFilter(filter, this, InternalSchema.EntryId);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			ComparisonFilter comparisonFilter = (ComparisonFilter)IdProperty.NativeIdFilterToSmartIdFilter(filter, this, InternalSchema.EntryId);
			if (comparisonFilter != null && !this.IsCompatibleId((StoreObjectId)comparisonFilter.PropertyValue, null))
			{
				return null;
			}
			return comparisonFilter;
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return InternalSchema.EntryId;
		}

		internal static QueryFilter SmartIdFilterToNativeIdFilter(SinglePropertyFilter filter, SmartPropertyDefinition smartProperty, PropertyDefinition nativeProperty)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null || !comparisonFilter.Property.Equals(smartProperty))
			{
				throw smartProperty.CreateInvalidFilterConversionException(filter);
			}
			if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && comparisonFilter.ComparisonOperator != ComparisonOperator.NotEqual)
			{
				throw smartProperty.CreateInvalidFilterConversionException(filter);
			}
			StoreId id = (StoreId)comparisonFilter.PropertyValue;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, nativeProperty, StoreId.GetStoreObjectId(id).ProviderLevelItemId);
		}

		internal static QueryFilter NativeIdFilterToSmartIdFilter(QueryFilter filter, SmartPropertyDefinition smartProperty, PropertyDefinition nativeProperty)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null || !comparisonFilter.Property.Equals(nativeProperty))
			{
				return null;
			}
			if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && comparisonFilter.ComparisonOperator != ComparisonOperator.NotEqual)
			{
				throw new CorruptDataException(ServerStrings.ExComparisonOperatorNotSupportedForProperty(comparisonFilter.ComparisonOperator.ToString(), smartProperty.Name));
			}
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, smartProperty, StoreObjectId.FromProviderSpecificId((byte[])comparisonFilter.PropertyValue, StoreObjectType.Unknown));
		}

		internal StoreObjectType GetStoreObjectType(PropertyBag propertyBag)
		{
			return this.GetStoreObjectType((PropertyBag.BasicPropertyStore)propertyBag);
		}

		protected abstract StoreObjectType GetStoreObjectType(PropertyBag.BasicPropertyStore propertyBag);

		protected virtual byte[] GetChangeKey(PropertyBag.BasicPropertyStore propertyBag)
		{
			return (propertyBag.GetValue(InternalSchema.ChangeKey) as byte[]) ?? Array<byte>.Empty;
		}

		protected abstract bool IsCompatibleId(StoreId id, ICoreObject coreObject);
	}
}
