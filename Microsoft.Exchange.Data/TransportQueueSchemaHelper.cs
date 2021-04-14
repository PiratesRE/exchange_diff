using System;
using System.Reflection;

namespace Microsoft.Exchange.Data
{
	internal class TransportQueueSchemaHelper
	{
		internal static SimpleProviderPropertyDefinition CreatePropertyDefinition(string name, Type type)
		{
			return TransportQueueSchemaHelper.CreatePropertyDefinition(name, ExchangeObjectVersion.Exchange2010, type, PropertyDefinitionFlags.None, type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : ((type == typeof(string)) ? string.Empty : null), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition CreatePropertyDefinition(string name, Type type, object defaultValue, PropertyDefinitionFlags flags = PropertyDefinitionFlags.PersistDefaultValue)
		{
			return TransportQueueSchemaHelper.CreatePropertyDefinition(name, ExchangeObjectVersion.Exchange2010, type, flags, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition CreatePropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints)
		{
			return new SimpleProviderPropertyDefinition(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints);
		}
	}
}
