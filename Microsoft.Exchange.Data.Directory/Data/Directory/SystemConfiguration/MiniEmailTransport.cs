using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MiniEmailTransport : MiniObject
	{
		static MiniEmailTransport()
		{
			ADEmailTransport ademailTransport = new ADEmailTransport();
			MiniEmailTransport.implicitFilter = ademailTransport.ImplicitFilter;
			MiniEmailTransport.mostDerivedClass = ademailTransport.MostDerivedObjectClass;
			MiniEmailTransport.schema = ObjectSchema.GetInstance<MiniEmailTransportSchema>();
		}

		public bool IsPop3
		{
			get
			{
				return base.ObjectClass.Contains(Pop3AdConfiguration.MostDerivedClass);
			}
		}

		public bool IsImap4
		{
			get
			{
				return base.ObjectClass.Contains(Imap4AdConfiguration.MostDerivedClass);
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[ADEmailTransportSchema.Server];
			}
		}

		public MultiValuedProperty<IPBinding> UnencryptedOrTLSBindings
		{
			get
			{
				return (MultiValuedProperty<IPBinding>)this[PopImapAdConfigurationSchema.UnencryptedOrTLSBindings];
			}
		}

		public MultiValuedProperty<IPBinding> SSLBindings
		{
			get
			{
				return (MultiValuedProperty<IPBinding>)this[PopImapAdConfigurationSchema.SSLBindings];
			}
		}

		public MultiValuedProperty<ProtocolConnectionSettings> ExternalConnectionSettings
		{
			get
			{
				return (MultiValuedProperty<ProtocolConnectionSettings>)this[PopImapAdConfigurationSchema.ExternalConnectionSettings];
			}
		}

		public MultiValuedProperty<ProtocolConnectionSettings> InternalConnectionSettings
		{
			get
			{
				return (MultiValuedProperty<ProtocolConnectionSettings>)this[PopImapAdConfigurationSchema.InternalConnectionSettings];
			}
		}

		public LoginOptions LoginType
		{
			get
			{
				return (LoginOptions)this[PopImapAdConfigurationSchema.LoginType];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniEmailTransport.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MiniEmailTransport.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return MiniEmailTransport.implicitFilter;
			}
		}

		private static QueryFilter implicitFilter;

		private static ADObjectSchema schema;

		private static string mostDerivedClass;
	}
}
