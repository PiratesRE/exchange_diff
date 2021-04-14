using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class SubjectPortionProperty : SmartPropertyDefinition
	{
		internal SubjectPortionProperty(string displayName, NativeStorePropertyDefinition nativeProperty) : base(displayName, typeof(string), PropertyFlags.None, Array<PropertyDefinitionConstraint>.Empty, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiSubject, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.NormalizedSubjectInternal, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.SubjectPrefixInternal, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(nativeProperty, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedToReadForWrite)
		})
		{
			this.nativeProperty = nativeProperty;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(this.nativeProperty);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			string text = value as string;
			if (text == null)
			{
				throw new ArgumentNullException("value");
			}
			SubjectProperty.ModifySubjectProperty(propertyBag, this.nativeProperty, text);
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(this.nativeProperty);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.nativeProperty);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, this.nativeProperty);
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(TextFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ExistsFilter));
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
			return this.nativeProperty;
		}

		private readonly NativeStorePropertyDefinition nativeProperty;
	}
}
