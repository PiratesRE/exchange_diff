using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DefaultValuePropertyRule : PropertyRule
	{
		public DefaultValuePropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, NativeStorePropertyDefinition propertyToSet, object defaultPropertyValue) : base(name, onSetWriteEnforceLocationIdentifier, new PropertyReference[]
		{
			new PropertyReference(propertyToSet, PropertyAccess.ReadWrite)
		})
		{
			this.propertyToSet = propertyToSet;
			if (defaultPropertyValue == null)
			{
				throw new ArgumentNullException("defaultPropertyValue");
			}
			this.defaultValue = defaultPropertyValue;
		}

		protected sealed override bool WriteEnforceRule(ICorePropertyBag propertyBag)
		{
			bool result = false;
			object propertyValue = propertyBag.TryGetProperty(this.propertyToSet);
			if (PropertyError.IsPropertyNotFound(propertyValue))
			{
				propertyBag.SetOrDeleteProperty(this.propertyToSet, this.defaultValue);
				result = true;
			}
			return result;
		}

		private readonly PropertyDefinition propertyToSet;

		private readonly object defaultValue;
	}
}
