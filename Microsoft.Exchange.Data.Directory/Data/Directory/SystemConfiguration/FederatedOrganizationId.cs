using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class FederatedOrganizationId : ADConfigurationObject
	{
		internal static string AddHybridConfigurationWellKnownSubDomain(string domain)
		{
			if (!string.IsNullOrEmpty(domain) && !FederatedOrganizationId.ContainsHybridConfigurationWellKnownSubDomain(domain) && !Globals.IsDatacenter)
			{
				return FederatedOrganizationId.HybridConfigurationWellKnownSubDomain + "." + domain;
			}
			return domain;
		}

		internal static bool ContainsHybridConfigurationWellKnownSubDomain(string domain)
		{
			return domain != null && domain.StartsWith(FederatedOrganizationId.HybridConfigurationWellKnownSubDomain + ".", StringComparison.OrdinalIgnoreCase);
		}

		internal static string RemoveHybridConfigurationWellKnownSubDomain(string domain)
		{
			string result = domain;
			if (FederatedOrganizationId.ContainsHybridConfigurationWellKnownSubDomain(domain))
			{
				result = domain.Substring(FederatedOrganizationId.HybridConfigurationWellKnownSubDomain.Length + 1);
			}
			return result;
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		public SmtpDomain AccountNamespace
		{
			get
			{
				SmtpDomain smtpDomain = (SmtpDomain)this[FederatedOrganizationIdSchema.AccountNamespace];
				if (smtpDomain == null)
				{
					return null;
				}
				return new SmtpDomain(FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(smtpDomain.Domain));
			}
			internal set
			{
				this[FederatedOrganizationIdSchema.AccountNamespace] = value;
			}
		}

		public SmtpDomain AccountNamespaceWithWellKnownSubDomain
		{
			get
			{
				if (!Globals.IsDatacenter)
				{
					return (SmtpDomain)this[FederatedOrganizationIdSchema.AccountNamespace];
				}
				return this.AccountNamespace;
			}
			internal set
			{
				if (!Globals.IsDatacenter)
				{
					this[FederatedOrganizationIdSchema.AccountNamespace] = value;
					return;
				}
				this[FederatedOrganizationIdSchema.AccountNamespace] = new SmtpDomain(FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(value.ToString()));
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[FederatedOrganizationIdSchema.Enabled];
			}
			set
			{
				this[FederatedOrganizationIdSchema.Enabled] = value;
			}
		}

		public SmtpAddress OrganizationContact
		{
			get
			{
				return (SmtpAddress)this[FederatedOrganizationIdSchema.OrganizationContact];
			}
			set
			{
				this[FederatedOrganizationIdSchema.OrganizationContact] = value;
			}
		}

		public ADObjectId DelegationTrustLink
		{
			get
			{
				return this[FederatedOrganizationIdSchema.DelegationTrustLink] as ADObjectId;
			}
			internal set
			{
				this[FederatedOrganizationIdSchema.DelegationTrustLink] = value;
			}
		}

		public ADObjectId ClientTrustLink
		{
			get
			{
				return this[FederatedOrganizationIdSchema.ClientTrustLink] as ADObjectId;
			}
			internal set
			{
				this[FederatedOrganizationIdSchema.ClientTrustLink] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AcceptedDomainsBackLink
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[FederatedOrganizationIdSchema.AcceptedDomainsBackLink];
			}
		}

		public ADObjectId DefaultSharingPolicyLink
		{
			get
			{
				return this[FederatedOrganizationIdSchema.DefaultSharingPolicyLink] as ADObjectId;
			}
			internal set
			{
				this[FederatedOrganizationIdSchema.DefaultSharingPolicyLink] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return FederatedOrganizationId.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchFedOrgId";
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		public const string ContainerName = "Federation";

		internal const string TaskNoun = "FederatedOrganizationIdentifier";

		internal const string LdapName = "msExchFedOrgId";

		internal static readonly ADObjectId Container = new ADObjectId(string.Format("CN={0}", "Federation"));

		internal static readonly string HybridConfigurationWellKnownSubDomain = "FYDIBOHF25SPDLT";

		private static readonly FederatedOrganizationIdSchema SchemaObject = ObjectSchema.GetInstance<FederatedOrganizationIdSchema>();
	}
}
