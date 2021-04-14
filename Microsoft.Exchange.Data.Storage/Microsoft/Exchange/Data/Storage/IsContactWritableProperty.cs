using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class IsContactWritableProperty : SmartPropertyDefinition
	{
		internal IsContactWritableProperty() : base("IsContactWritable", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, IsContactWritableProperty.PropertyDependencies)
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return string.IsNullOrEmpty(propertyBag.GetValueOrDefault<string>(InternalSchema.PartnerNetworkId, null));
		}

		private static readonly PropertyDependency[] PropertyDependencies = new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.PartnerNetworkId, PropertyDependencyType.NeedForRead)
		};
	}
}
