using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MiniReceiveConnector : MiniObject
	{
		static MiniReceiveConnector()
		{
			ReceiveConnector receiveConnector = new ReceiveConnector();
			MiniReceiveConnector.mostDerivedClass = receiveConnector.MostDerivedObjectClass;
			MiniReceiveConnector.schema = ObjectSchema.GetInstance<MiniReceiveConnectorSchema>();
		}

		public bool IsSmtp
		{
			get
			{
				return base.ObjectClass.Contains("msExchSmtpReceiveConnector");
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[ReceiveConnectorSchema.Server];
			}
		}

		public AuthMechanisms AuthMechanism
		{
			get
			{
				return (AuthMechanisms)this[ReceiveConnectorSchema.SecurityFlags];
			}
		}

		public MultiValuedProperty<IPBinding> Bindings
		{
			get
			{
				return (MultiValuedProperty<IPBinding>)this[ReceiveConnectorSchema.Bindings];
			}
		}

		public bool AdvertiseClientSettings
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.AdvertiseClientSettings];
			}
		}

		public Fqdn ServiceDiscoveryFqdn
		{
			get
			{
				return (Fqdn)this[ReceiveConnectorSchema.ServiceDiscoveryFqdn];
			}
		}

		public Fqdn Fqdn
		{
			get
			{
				return (Fqdn)this[ReceiveConnectorSchema.Fqdn];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniReceiveConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MiniReceiveConnector.mostDerivedClass;
			}
		}

		private static ADObjectSchema schema;

		private static string mostDerivedClass;
	}
}
