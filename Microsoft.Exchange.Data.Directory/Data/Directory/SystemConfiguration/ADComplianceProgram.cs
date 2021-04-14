using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADComplianceProgram : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADComplianceProgram.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADComplianceProgram.mostDerivedClass;
			}
		}

		public string TransportRulesXml
		{
			get
			{
				return (string)this[ADComplianceProgramSchema.TransportRulesXml];
			}
			internal set
			{
				this[ADComplianceProgramSchema.TransportRulesXml] = value;
			}
		}

		public string PublisherName
		{
			get
			{
				return (string)this[ADComplianceProgramSchema.PublisherName];
			}
			internal set
			{
				this[ADComplianceProgramSchema.PublisherName] = value;
			}
		}

		public DlpPolicyState State
		{
			get
			{
				return (DlpPolicyState)this[ADComplianceProgramSchema.State];
			}
			internal set
			{
				this[ADComplianceProgramSchema.State] = value;
			}
		}

		public string Version
		{
			get
			{
				return (string)this[ADComplianceProgramSchema.Version];
			}
			internal set
			{
				this[ADComplianceProgramSchema.Version] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[ADComplianceProgramSchema.Description];
			}
			internal set
			{
				this[ADComplianceProgramSchema.Description] = value;
			}
		}

		public Guid ImmutableId
		{
			get
			{
				Guid guid = (Guid)this[ADComplianceProgramSchema.ImmutableId];
				if (guid == Guid.Empty)
				{
					guid = ((base.Id == null) ? Guid.Empty : base.Id.ObjectGuid);
				}
				return guid;
			}
			internal set
			{
				this[ADComplianceProgramSchema.ImmutableId] = value;
			}
		}

		public string[] Countries
		{
			get
			{
				return ((MultiValuedProperty<string>)this[ADComplianceProgramSchema.Countries]).ToArray();
			}
			internal set
			{
				MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
				for (int i = 0; i < value.Length; i++)
				{
					string item = value[i];
					multiValuedProperty.Add(item);
				}
				this[ADComplianceProgramSchema.Countries] = multiValuedProperty;
			}
		}

		public string[] Keywords
		{
			get
			{
				return ((MultiValuedProperty<string>)this[ADComplianceProgramSchema.Keywords]).ToArray();
			}
			internal set
			{
				MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
				for (int i = 0; i < value.Length; i++)
				{
					string item = value[i];
					multiValuedProperty.Add(item);
				}
				this[ADComplianceProgramSchema.Keywords] = multiValuedProperty;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static ADComplianceProgramSchema schema = ObjectSchema.GetInstance<ADComplianceProgramSchema>();

		private static string mostDerivedClass = "msExchMailflowPolicy";
	}
}
