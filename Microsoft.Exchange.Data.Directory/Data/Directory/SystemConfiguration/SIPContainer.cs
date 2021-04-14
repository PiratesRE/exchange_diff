using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class SIPContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SIPContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchProtocolCfgSIPContainer";
			}
		}

		internal static ADObjectId GetBaseContainer(ITopologyConfigurationSession dataSession)
		{
			ADObjectId relativePath = new ADObjectId("CN=Protocols");
			return dataSession.FindLocalServer().Id.GetDescendantId(relativePath);
		}

		private const string MostDerivedClass = "msExchProtocolCfgSIPContainer";

		private static SIPContainerSchema schema = ObjectSchema.GetInstance<SIPContainerSchema>();
	}
}
