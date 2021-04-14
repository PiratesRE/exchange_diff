using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class Pop3Container : Container
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return Pop3Container.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Pop3Container.mostDerivedClass;
			}
		}

		internal static ADObjectId GetBaseContainer(ITopologyConfigurationSession dataSession)
		{
			ADObjectId relativePath = new ADObjectId("CN=Protocols");
			return dataSession.FindLocalServer().Id.GetDescendantId(relativePath);
		}

		private static Pop3ContainerSchema schema = ObjectSchema.GetInstance<Pop3ContainerSchema>();

		private static string mostDerivedClass = "msExchProtocolCfgPOPContainer";
	}
}
