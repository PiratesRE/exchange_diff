using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public abstract class VersionedXmlConfigurationObject : XsoMailboxConfigurationObject
	{
		internal abstract string UserConfigurationName { get; }

		internal abstract ProviderPropertyDefinition RawVersionedXmlPropertyDefinition { get; }
	}
}
