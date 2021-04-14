using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class ProtocolsContainer : Container
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ProtocolsContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ProtocolsContainer.mostDerivedClass;
			}
		}

		internal static readonly string DefaultName = "Protocols";

		private static ProtocolsContainerSchema schema = ObjectSchema.GetInstance<ProtocolsContainerSchema>();

		private static string mostDerivedClass = "protocolCfgSharedServer";
	}
}
