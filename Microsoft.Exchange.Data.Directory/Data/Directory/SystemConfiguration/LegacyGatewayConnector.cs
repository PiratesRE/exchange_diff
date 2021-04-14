using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class LegacyGatewayConnector : MailGateway
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return LegacyGatewayConnector.schema;
			}
		}

		private static LegacyGatewayConnectorSchema schema = ObjectSchema.GetInstance<LegacyGatewayConnectorSchema>();
	}
}
