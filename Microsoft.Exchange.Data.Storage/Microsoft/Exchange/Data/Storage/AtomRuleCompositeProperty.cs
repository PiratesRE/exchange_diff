using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class AtomRuleCompositeProperty : SmartPropertyDefinition
	{
		protected AtomRuleCompositeProperty(string displayName, NativeStorePropertyDefinition compositeProperty, IList<NativeStorePropertyDefinition> atomAndRuleProperties) : base(displayName, typeof(string), PropertyFlags.None, PropertyDefinitionConstraint.None, AtomRuleCompositeProperty.GetDependencies(compositeProperty, atomAndRuleProperties))
		{
			this.CompositeProperty = compositeProperty;
			this.atomAndRuleProperties = new ReadOnlyCollection<NativeStorePropertyDefinition>(atomAndRuleProperties);
			this.RegisterFilterTranslation();
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return base.Capabilities | StorePropertyCapabilities.CanQuery | StorePropertyCapabilities.CanSortBy | StorePropertyCapabilities.CanGroupBy;
			}
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return this.CompositeProperty;
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(this.CompositeProperty);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (!propertyBag.CanIgnoreUnchangedProperties || (this.CompositeProperty.PropertyFlags & PropertyFlags.SetIfNotChanged) == PropertyFlags.SetIfNotChanged || !propertyBag.IsLoaded(this.CompositeProperty) || !object.Equals(this.InternalTryGetValue(propertyBag), value))
			{
				propertyBag.SetValueWithFixup(this.CompositeProperty, value);
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			string text = propertyBag.GetValueOrDefault<string>(this.CompositeProperty);
			if (!propertyBag.IsDirty(this.CompositeProperty) && this.IsAtomOrRulePropertyDirty(propertyBag))
			{
				text = this.GenerateCompositePropertyValue(propertyBag);
			}
			return text ?? new PropertyError(this, PropertyErrorCode.NotFound);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, this.CompositeProperty);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.CompositeProperty);
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ExistsFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(TextFilter));
		}

		internal void UpdateCompositePropertyValue(PropertyBag propertyBag)
		{
			PropertyBag.BasicPropertyStore propertyBag2 = (PropertyBag.BasicPropertyStore)propertyBag;
			if (!propertyBag.IsPropertyDirty(this.CompositeProperty) && this.IsAtomOrRulePropertyDirty(propertyBag2))
			{
				string text = this.GenerateCompositePropertyValue(propertyBag2);
				if (text != null)
				{
					propertyBag[this.CompositeProperty] = text;
					return;
				}
				propertyBag.Delete(this.CompositeProperty);
			}
		}

		protected abstract string GenerateCompositePropertyValue(PropertyBag.BasicPropertyStore propertyBag);

		protected bool IsAtomOrRulePropertyDirty(PropertyBag.BasicPropertyStore propertyBag)
		{
			foreach (NativeStorePropertyDefinition propertyDefinition in this.atomAndRuleProperties)
			{
				if (propertyBag.IsDirty(propertyDefinition))
				{
					return true;
				}
			}
			return false;
		}

		private static PropertyDependency[] GetDependencies(NativeStorePropertyDefinition compositeProperty, IList<NativeStorePropertyDefinition> atomOrRuleProperties)
		{
			List<PropertyDependency> list = new List<PropertyDependency>(atomOrRuleProperties.Count + 1);
			foreach (NativeStorePropertyDefinition property in atomOrRuleProperties)
			{
				list.Add(new PropertyDependency(property, PropertyDependencyType.AllRead));
			}
			list.Add(new PropertyDependency(compositeProperty, PropertyDependencyType.AllRead));
			return list.ToArray();
		}

		protected readonly NativeStorePropertyDefinition CompositeProperty;

		private readonly IList<NativeStorePropertyDefinition> atomAndRuleProperties;

		protected class FormattedSentenceContext : FormattedSentence.Context
		{
			public FormattedSentenceContext(PropertyBag.BasicPropertyStore propertyBag, Dictionary<string, NativeStorePropertyDefinition> placeholderCodeToPropDef)
			{
				this.propertyBag = propertyBag;
				this.placeholderCodeToPropDef = placeholderCodeToPropDef;
			}

			public override string ResolvePlaceholder(string code)
			{
				NativeStorePropertyDefinition propertyDefinition;
				if (!this.placeholderCodeToPropDef.TryGetValue(code, out propertyDefinition))
				{
					throw new FormatException("Invalid placeholder code: " + code);
				}
				return this.propertyBag.GetValue(propertyDefinition) as string;
			}

			private readonly PropertyBag.BasicPropertyStore propertyBag;

			private readonly Dictionary<string, NativeStorePropertyDefinition> placeholderCodeToPropDef;
		}
	}
}
