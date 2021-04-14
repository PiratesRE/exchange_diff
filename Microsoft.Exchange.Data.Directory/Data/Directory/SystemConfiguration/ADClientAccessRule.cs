using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADClientAccessRule : ADConfigurationObject
	{
		internal string RuleName
		{
			get
			{
				return (string)this[ADClientAccessRuleSchema.RuleName];
			}
			set
			{
				this[ADClientAccessRuleSchema.RuleName] = value;
			}
		}

		internal int InternalPriority
		{
			get
			{
				return (int)this[ADClientAccessRuleSchema.InternalPriority];
			}
			set
			{
				this[ADClientAccessRuleSchema.InternalPriority] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				return (int)this[ADClientAccessRuleSchema.Priority];
			}
			set
			{
				this[ADClientAccessRuleSchema.Priority] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[ADClientAccessRuleSchema.Enabled];
			}
			set
			{
				this[ADClientAccessRuleSchema.Enabled] = value;
			}
		}

		public bool DatacenterAdminsOnly
		{
			get
			{
				return (bool)this[ADClientAccessRuleSchema.DatacenterAdminsOnly];
			}
			set
			{
				this[ADClientAccessRuleSchema.DatacenterAdminsOnly] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ClientAccessRulesAction Action
		{
			get
			{
				return (ClientAccessRulesAction)this[ADClientAccessRuleSchema.Action];
			}
			set
			{
				this[ADClientAccessRuleSchema.Action] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> AnyOfClientIPAddressesOrRanges
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[ADClientAccessRuleSchema.AnyOfClientIPAddressesOrRanges];
			}
			set
			{
				this[ADClientAccessRuleSchema.AnyOfClientIPAddressesOrRanges] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> ExceptAnyOfClientIPAddressesOrRanges
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[ADClientAccessRuleSchema.ExceptAnyOfClientIPAddressesOrRanges];
			}
			set
			{
				this[ADClientAccessRuleSchema.ExceptAnyOfClientIPAddressesOrRanges] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IntRange> AnyOfSourceTcpPortNumbers
		{
			get
			{
				return (MultiValuedProperty<IntRange>)this[ADClientAccessRuleSchema.AnyOfSourceTcpPortNumbers];
			}
			set
			{
				this[ADClientAccessRuleSchema.AnyOfSourceTcpPortNumbers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IntRange> ExceptAnyOfSourceTcpPortNumbers
		{
			get
			{
				return (MultiValuedProperty<IntRange>)this[ADClientAccessRuleSchema.ExceptAnyOfSourceTcpPortNumbers];
			}
			set
			{
				this[ADClientAccessRuleSchema.ExceptAnyOfSourceTcpPortNumbers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UsernameMatchesAnyOfPatterns
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADClientAccessRuleSchema.UsernameMatchesAnyOfPatterns];
			}
			set
			{
				this[ADClientAccessRuleSchema.UsernameMatchesAnyOfPatterns] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExceptUsernameMatchesAnyOfPatterns
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADClientAccessRuleSchema.ExceptUsernameMatchesAnyOfPatterns];
			}
			set
			{
				this[ADClientAccessRuleSchema.ExceptUsernameMatchesAnyOfPatterns] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UserIsMemberOf
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADClientAccessRuleSchema.UserIsMemberOf];
			}
			set
			{
				this[ADClientAccessRuleSchema.UserIsMemberOf] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExceptUserIsMemberOf
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADClientAccessRuleSchema.ExceptUserIsMemberOf];
			}
			set
			{
				this[ADClientAccessRuleSchema.ExceptUserIsMemberOf] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessAuthenticationMethod> AnyOfAuthenticationTypes
		{
			get
			{
				return (MultiValuedProperty<ClientAccessAuthenticationMethod>)this[ADClientAccessRuleSchema.AnyOfAuthenticationTypes];
			}
			set
			{
				this[ADClientAccessRuleSchema.AnyOfAuthenticationTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessAuthenticationMethod> ExceptAnyOfAuthenticationTypes
		{
			get
			{
				return (MultiValuedProperty<ClientAccessAuthenticationMethod>)this[ADClientAccessRuleSchema.ExceptAnyOfAuthenticationTypes];
			}
			set
			{
				this[ADClientAccessRuleSchema.ExceptAnyOfAuthenticationTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessProtocol> AnyOfProtocols
		{
			get
			{
				return (MultiValuedProperty<ClientAccessProtocol>)this[ADClientAccessRuleSchema.AnyOfProtocols];
			}
			set
			{
				this[ADClientAccessRuleSchema.AnyOfProtocols] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessProtocol> ExceptAnyOfProtocols
		{
			get
			{
				return (MultiValuedProperty<ClientAccessProtocol>)this[ADClientAccessRuleSchema.ExceptAnyOfProtocols];
			}
			set
			{
				this[ADClientAccessRuleSchema.ExceptAnyOfProtocols] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UserRecipientFilter
		{
			get
			{
				return (string)this[ADClientAccessRuleSchema.UserRecipientFilter];
			}
			set
			{
				this[ADClientAccessRuleSchema.UserRecipientFilter] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADClientAccessRule.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADClientAccessRule.mostDerivedClass;
			}
		}

		private string Xml
		{
			get
			{
				return (string)this[ADClientAccessRuleSchema.Xml];
			}
			set
			{
				this[ADClientAccessRuleSchema.Xml] = value;
			}
		}

		private new OrganizationId OrganizationId
		{
			get
			{
				return base.OrganizationId;
			}
			set
			{
				base.OrganizationId = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return ADClientAccessRule.parentPath;
			}
		}

		internal ClientAccessRule GetClientAccessRule()
		{
			return ClientAccessRule.FromADProperties((string)this[ADClientAccessRuleSchema.Xml], this.Identity, base.Name, this.Priority, this.Enabled, this.DatacenterAdminsOnly, false);
		}

		internal bool HasAnyOfSpecificProtocolsPredicate(List<ClientAccessProtocol> protocols)
		{
			bool flag = false;
			if (this.AnyOfProtocols != null)
			{
				if (this.AnyOfProtocols.Except(protocols).Count<ClientAccessProtocol>() > 0)
				{
					return false;
				}
				flag = (flag || this.AnyOfProtocols.Intersect(protocols).Count<ClientAccessProtocol>() > 0);
			}
			if (this.ExceptAnyOfProtocols != null)
			{
				if (this.ExceptAnyOfProtocols.Except(protocols).Count<ClientAccessProtocol>() > 0)
				{
					return false;
				}
				flag = (flag || this.ExceptAnyOfProtocols.Intersect(protocols).Count<ClientAccessProtocol>() > 0);
			}
			return flag;
		}

		internal bool HasAuthenticationMethodPredicate(ClientAccessAuthenticationMethod authenticationMethod)
		{
			return (this.AnyOfAuthenticationTypes != null && this.AnyOfAuthenticationTypes.Contains(authenticationMethod)) || (this.ExceptAnyOfAuthenticationTypes != null && this.ExceptAnyOfAuthenticationTypes.Contains(authenticationMethod));
		}

		internal bool HasAnyAuthenticationMethodPredicate()
		{
			return (this.AnyOfAuthenticationTypes != null && this.AnyOfAuthenticationTypes.Count > 0) || (this.ExceptAnyOfAuthenticationTypes != null && this.ExceptAnyOfAuthenticationTypes.Count > 0);
		}

		internal bool ValidateUserRecipientFilterParsesWithSchema()
		{
			if (!string.IsNullOrEmpty(this.UserRecipientFilter))
			{
				new QueryParser(this.UserRecipientFilter, ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>(), QueryParser.Capabilities.All, null, new QueryParser.ConvertValueFromStringDelegate(QueryParserUtils.ConvertValueFromString));
			}
			return true;
		}

		private static ADClientAccessRuleSchema schema = ObjectSchema.GetInstance<ADClientAccessRuleSchema>();

		private static string mostDerivedClass = "msExchClientAccessRule";

		private static ADObjectId parentPath = new ADObjectId("CN=" + ADClientAccessRuleCollection.ContainerName);
	}
}
