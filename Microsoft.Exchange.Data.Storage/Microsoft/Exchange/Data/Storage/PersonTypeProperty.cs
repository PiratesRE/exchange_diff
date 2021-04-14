using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class PersonTypeProperty : SmartPropertyDefinition
	{
		internal PersonTypeProperty() : base("PersonType", typeof(PersonType), PropertyFlags.None, PropertyDefinitionConstraint.None, PersonTypeProperty.PropertyDependencies)
		{
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.None;
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object obj = PersonTypeProperty.ExceptionHandlingGetValueOrDefault<object>(propertyBag, InternalSchema.InternalPersonType, null);
			if (obj is int)
			{
				return (PersonType)obj;
			}
			bool flag = PersonTypeProperty.ExceptionHandlingGetValueOrDefault<bool>(propertyBag, InternalSchema.IsDistributionListContact, false);
			if (flag)
			{
				return PersonType.DistributionList;
			}
			string itemClass = PersonTypeProperty.ExceptionHandlingGetValueOrDefault<string>(propertyBag, InternalSchema.ItemClass, string.Empty);
			if (ObjectClass.IsDistributionList(itemClass))
			{
				return PersonType.DistributionList;
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (!(value is PersonType))
			{
				throw new ArgumentException("value");
			}
			propertyBag.SetValue(InternalSchema.InternalPersonType, (int)value);
		}

		private static T ExceptionHandlingGetValueOrDefault<T>(PropertyBag.BasicPropertyStore propertyBag, AtomicStorePropertyDefinition propertyDefinition, T defaultValue)
		{
			T result;
			try
			{
				result = propertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
			}
			catch (NotInBagPropertyErrorException)
			{
				result = defaultValue;
			}
			return result;
		}

		private static readonly PropertyDependency[] PropertyDependencies = new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.InternalPersonType, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.IsDistributionListContact, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead)
		};
	}
}
