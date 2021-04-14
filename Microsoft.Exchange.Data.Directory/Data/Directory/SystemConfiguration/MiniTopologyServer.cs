using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MiniTopologyServer : MiniObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniTopologyServer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MiniTopologyServer.mostDerivedClass;
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this[MiniTopologyServerSchema.Fqdn];
			}
		}

		public int VersionNumber
		{
			get
			{
				return (int)this[MiniTopologyServerSchema.VersionNumber];
			}
		}

		public int MajorVersion
		{
			get
			{
				return (int)this[MiniTopologyServerSchema.MajorVersion];
			}
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[MiniTopologyServerSchema.AdminDisplayVersion];
			}
		}

		public bool IsE14OrLater
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsE14OrLater];
			}
		}

		public bool IsExchange2007OrLater
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsExchange2007OrLater];
			}
		}

		public bool IsE15OrLater
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsE15OrLater];
			}
		}

		public ADObjectId ServerSite
		{
			get
			{
				return (ADObjectId)this[MiniTopologyServerSchema.ServerSite];
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[MiniTopologyServerSchema.ExchangeLegacyDN];
			}
		}

		public ServerRole CurrentServerRole
		{
			get
			{
				return (ServerRole)this[MiniTopologyServerSchema.CurrentServerRole];
			}
		}

		public bool IsClientAccessServer
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsClientAccessServer];
			}
		}

		public bool IsCafeServer
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsCafeServer];
			}
		}

		public bool IsHubTransportServer
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsHubTransportServer];
			}
		}

		public bool IsEdgeServer
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsEdgeServer];
			}
		}

		public bool IsFrontendTransportServer
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsFrontendTransportServer];
			}
		}

		public bool IsMailboxServer
		{
			get
			{
				return (bool)this[MiniTopologyServerSchema.IsMailboxServer];
			}
		}

		public ADObjectId DatabaseAvailabilityGroup
		{
			get
			{
				return (ADObjectId)this[MiniTopologyServerSchema.DatabaseAvailabilityGroup];
			}
		}

		public MultiValuedProperty<string> ComponentStates
		{
			get
			{
				return (MultiValuedProperty<string>)this[MiniTopologyServerSchema.ComponentStates];
			}
		}

		public ADObjectId HomeRoutingGroup
		{
			get
			{
				return (ADObjectId)this[MiniTopologyServerSchema.HomeRoutingGroup];
			}
		}

		internal SmtpAddress? ExternalPostmasterAddress
		{
			get
			{
				return (SmtpAddress?)this[MiniTopologyServerSchema.ExternalPostmasterAddress];
			}
		}

		internal ITopologyConfigurationSession Session
		{
			get
			{
				return (ITopologyConfigurationSession)this.m_Session;
			}
		}

		internal void SetProperties(ADObject server)
		{
			ADPropertyBag adpropertyBag = new ADPropertyBag();
			adpropertyBag.SetIsReadOnly(false);
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				ADPropertyDefinition key = (ADPropertyDefinition)propertyDefinition;
				object value = server.propertyBag.Contains(key) ? server.propertyBag[key] : null;
				adpropertyBag.SetField(key, value);
			}
			MultiValuedProperty<string> multiValuedProperty = adpropertyBag[ADObjectSchema.ObjectClass] as MultiValuedProperty<string>;
			if (multiValuedProperty == null || multiValuedProperty.Count == 0)
			{
				multiValuedProperty = new MultiValuedProperty<string>(this.MostDerivedObjectClass);
				adpropertyBag.SetField(ADObjectSchema.ObjectClass, multiValuedProperty);
			}
			if (adpropertyBag[ADObjectSchema.WhenChangedUTC] == null)
			{
				DateTime utcNow = DateTime.UtcNow;
				adpropertyBag.SetField(ADObjectSchema.WhenChangedUTC, utcNow);
				adpropertyBag.SetField(ADObjectSchema.WhenCreatedUTC, utcNow);
			}
			adpropertyBag.SetIsReadOnly(true);
			this.propertyBag = adpropertyBag;
		}

		private static MiniTopologyServerSchema schema = ObjectSchema.GetInstance<MiniTopologyServerSchema>();

		private static string mostDerivedClass = "msExchExchangeServer";
	}
}
