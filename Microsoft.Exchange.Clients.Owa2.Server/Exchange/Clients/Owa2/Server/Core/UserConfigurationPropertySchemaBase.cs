using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class UserConfigurationPropertySchemaBase
	{
		internal int Count
		{
			get
			{
				return this.PropertyDefinitions.Length;
			}
		}

		internal abstract UserConfigurationPropertyDefinition[] PropertyDefinitions { get; }

		internal abstract UserConfigurationPropertyId PropertyDefinitionsBaseId { get; }

		internal UserConfigurationPropertyDefinition GetPropertyDefinition(UserConfigurationPropertyId id)
		{
			int index = id - this.PropertyDefinitionsBaseId;
			return this.GetPropertyDefinition(index);
		}

		internal UserConfigurationPropertyDefinition GetPropertyDefinition(int index)
		{
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<int, string>(0L, "Get UserConfigurationPropertyDefinition: index = '{0}', name = '{1}'", index, this.PropertyDefinitions[index].PropertyName);
			return this.PropertyDefinitions[index];
		}
	}
}
