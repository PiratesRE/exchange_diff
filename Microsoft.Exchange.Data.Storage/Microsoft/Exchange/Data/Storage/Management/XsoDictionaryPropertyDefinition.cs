using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class XsoDictionaryPropertyDefinition : SimpleProviderPropertyDefinition
	{
		public string UserConfigurationName { get; private set; }

		public XsoDictionaryPropertyDefinition(string configurationName, string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : base(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints)
		{
			this.UserConfigurationName = configurationName;
		}
	}
}
