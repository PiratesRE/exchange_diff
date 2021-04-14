using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class DefaultToInternetSendConnector : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return DefaultToInternetSendConnector.schema;
			}
		}

		public MultiValuedProperty<ADObjectId> SourceTransportServers
		{
			get
			{
				if (this.connector != null)
				{
					return this.connector.SourceTransportServers;
				}
				return null;
			}
		}

		public DefaultToInternetSendConnector()
		{
		}

		public DefaultToInternetSendConnector(SmtpSendConnectorConfig connector) : base(connector)
		{
			this.connector = connector;
		}

		private static DefaultToInternetSendConnectorSchema schema = ObjectSchema.GetInstance<DefaultToInternetSendConnectorSchema>();

		private SmtpSendConnectorConfig connector;
	}
}
