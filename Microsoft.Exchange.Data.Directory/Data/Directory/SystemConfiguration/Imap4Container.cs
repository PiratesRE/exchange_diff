using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class Imap4Container : Container
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return Imap4Container.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Imap4Container.mostDerivedClass;
			}
		}

		internal static ADObjectId GetBaseContainer(ITopologyConfigurationSession dataSession)
		{
			ADObjectId relativePath = new ADObjectId("CN=Protocols");
			return dataSession.FindLocalServer().Id.GetDescendantId(relativePath);
		}

		private static Imap4ContainerSchema schema = ObjectSchema.GetInstance<Imap4ContainerSchema>();

		private static string mostDerivedClass = "msExchProtocolCfgIMAPContainer";
	}
}
