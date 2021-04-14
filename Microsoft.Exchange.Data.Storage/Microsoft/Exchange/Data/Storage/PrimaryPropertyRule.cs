using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PrimaryPropertyRule : PropertyRule
	{
		public PrimaryPropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, NativeStorePropertyDefinition primaryProperty, NativeStorePropertyDefinition secondaryProperty) : base(name, onSetWriteEnforceLocationIdentifier, new PropertyReference[]
		{
			new PropertyReference(primaryProperty, PropertyAccess.Read),
			new PropertyReference(secondaryProperty, PropertyAccess.Write)
		})
		{
			if (primaryProperty.Type != secondaryProperty.Type)
			{
				throw new ArgumentException("properties should be same type for PrimaryPropertyRule.");
			}
			this.primary = primaryProperty;
			this.secondary = secondaryProperty;
		}

		protected override bool WriteEnforceRule(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if (propertyBag.IsPropertyDirty(this.primary))
			{
				propertyBag.SetOrDeleteProperty(this.secondary, propertyBag.TryGetProperty(this.primary));
				result = true;
			}
			return result;
		}

		private readonly PropertyDefinition primary;

		private readonly PropertyDefinition secondary;
	}
}
