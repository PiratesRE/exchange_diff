using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class AcceptedDomain : ADConfigurationObject, IProvisioningCacheInvalidation
	{
		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				return (SmtpDomainWithSubdomains)this[AcceptedDomainSchema.DomainName];
			}
			set
			{
				this[AcceptedDomainSchema.DomainName] = value;
			}
		}

		public ADObjectId CatchAllRecipientID
		{
			get
			{
				return (ADObjectId)this[AcceptedDomainSchema.CatchAllRecipient];
			}
			set
			{
				this[AcceptedDomainSchema.CatchAllRecipient] = value;
			}
		}

		[Parameter]
		public AcceptedDomainType DomainType
		{
			get
			{
				return (AcceptedDomainType)((int)this[AcceptedDomainSchema.AcceptedDomainType]);
			}
			set
			{
				this[AcceptedDomainSchema.AcceptedDomainType] = (int)value;
			}
		}

		public bool MatchSubDomains
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.MatchSubDomains];
			}
			internal set
			{
				this[AcceptedDomainSchema.MatchSubDomains] = value;
			}
		}

		[Parameter]
		public bool AddressBookEnabled
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.AddressBookEnabled];
			}
			set
			{
				this[AcceptedDomainSchema.AddressBookEnabled] = value;
			}
		}

		public bool Default
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.Default];
			}
			internal set
			{
				this[AcceptedDomainSchema.Default] = value;
			}
		}

		internal AuthenticationType RawAuthenticationType
		{
			get
			{
				return (AuthenticationType)this[AcceptedDomainSchema.RawAuthenticationType];
			}
			set
			{
				this[AcceptedDomainSchema.RawAuthenticationType] = value;
			}
		}

		public AuthenticationType? AuthenticationType
		{
			get
			{
				return (AuthenticationType?)this[AcceptedDomainSchema.AuthenticationType];
			}
		}

		internal LiveIdInstanceType RawLiveIdInstanceType
		{
			get
			{
				return (LiveIdInstanceType)this[AcceptedDomainSchema.RawLiveIdInstanceType];
			}
			set
			{
				this[AcceptedDomainSchema.RawLiveIdInstanceType] = value;
			}
		}

		public LiveIdInstanceType? LiveIdInstanceType
		{
			get
			{
				return (LiveIdInstanceType?)this[AcceptedDomainSchema.LiveIdInstanceType];
			}
		}

		[Parameter]
		public bool PendingRemoval
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.PendingRemoval];
			}
			set
			{
				this[AcceptedDomainSchema.PendingRemoval] = value;
			}
		}

		[Parameter]
		public bool PendingCompletion
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.PendingCompletion];
			}
			set
			{
				this[AcceptedDomainSchema.PendingCompletion] = value;
			}
		}

		[Parameter]
		public bool DualProvisioningEnabled
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.DualProvisioningEnabled];
			}
			set
			{
				this[AcceptedDomainSchema.DualProvisioningEnabled] = value;
			}
		}

		public ADObjectId FederatedOrganizationLink
		{
			get
			{
				return (ADObjectId)this[AcceptedDomainSchema.FederatedOrganizationLink];
			}
			internal set
			{
				this[AcceptedDomainSchema.FederatedOrganizationLink] = value;
			}
		}

		public ADObjectId MailFlowPartner
		{
			get
			{
				return (ADObjectId)this[AcceptedDomainSchema.MailFlowPartner];
			}
			set
			{
				this[AcceptedDomainSchema.MailFlowPartner] = value;
			}
		}

		[Parameter]
		public bool OutboundOnly
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.OutboundOnly];
			}
			set
			{
				this[AcceptedDomainSchema.OutboundOnly] = value;
			}
		}

		public bool PendingFederatedAccountNamespace
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.PendingFederatedAccountNamespace];
			}
			internal set
			{
				this[AcceptedDomainSchema.PendingFederatedAccountNamespace] = value;
			}
		}

		public bool PendingFederatedDomain
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.PendingFederatedDomain];
			}
			internal set
			{
				this[AcceptedDomainSchema.PendingFederatedDomain] = value;
			}
		}

		public bool IsCoexistenceDomain
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.IsCoexistenceDomain];
			}
			internal set
			{
				this[AcceptedDomainSchema.IsCoexistenceDomain] = value;
			}
		}

		public bool PerimeterDuplicateDetected
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.PerimeterDuplicateDetected];
			}
		}

		public bool IsDefaultFederatedDomain
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.IsDefaultFederatedDomain];
			}
			set
			{
				this[AcceptedDomainSchema.IsDefaultFederatedDomain] = value;
			}
		}

		public bool EnableNego2Authentication
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.EnableNego2Authentication];
			}
			set
			{
				this[AcceptedDomainSchema.EnableNego2Authentication] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return AcceptedDomain.SchemaObject;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return AcceptedDomain.AcceptedDomainContainer;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchAcceptedDomain";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new AndFilter(new QueryFilter[]
				{
					base.ImplicitFilter,
					X400AuthoritativeDomain.NonX400DomainsFilter
				});
			}
		}

		public bool InitialDomain
		{
			get
			{
				return (bool)this[AcceptedDomainSchema.InitialDomain];
			}
			internal set
			{
				this[AcceptedDomainSchema.InitialDomain] = value;
			}
		}

		internal static string FormatEhfOutboundOnlyDomainName(string domainName, Guid domainGuid)
		{
			int num = 38 + "DuplicateDomain".Length;
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			int num2 = 255 - num;
			if (domainName.Length > num2)
			{
				int num3 = domainName.Length - num2;
				if (domainName[num3] == '.')
				{
					num3++;
				}
				domainName = domainName.Substring(num3);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1}.{2}", new object[]
			{
				"DuplicateDomain",
				domainGuid,
				domainName
			});
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.DomainName != null && this.DomainName.Equals(SmtpDomainWithSubdomains.StarDomain))
			{
				if (this.DomainType == AcceptedDomainType.Authoritative)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.StarAcceptedDomainCannotBeAuthoritative, AcceptedDomainSchema.AcceptedDomainType, this.DomainType));
				}
				if (this.Default)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.StarAcceptedDomainCannotBeDefault, AcceptedDomainSchema.Default, this.Default));
				}
				if (this.InitialDomain)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.StarAcceptedDomainCannotBeInitialDomain, AcceptedDomainSchema.InitialDomain, this.InitialDomain));
				}
			}
			if (this.EnableNego2Authentication && this.DomainName.IncludeSubDomains)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorAcceptedDomainCannotContainWildcardAndNegoConfig, AcceptedDomainSchema.EnableNego2Authentication, this.EnableNego2Authentication));
			}
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			bool flag = false;
			if (base.OrganizationId == null)
			{
				return flag;
			}
			if (base.ObjectState == ObjectState.New || base.ObjectState == ObjectState.Deleted)
			{
				flag = true;
			}
			if (!flag && base.ObjectState == ObjectState.Changed && (base.IsChanged(AcceptedDomainSchema.DomainName) || base.IsChanged(AcceptedDomainSchema.AcceptedDomainType) || base.IsChanged(AcceptedDomainSchema.AcceptedDomainFlags) || base.IsChanged(ADObjectSchema.OrganizationalUnitRoot) || base.IsChanged(ADObjectSchema.ConfigurationUnit)))
			{
				flag = true;
			}
			if (flag)
			{
				orgId = base.OrganizationId;
				keys = new Guid[]
				{
					CannedProvisioningCacheKeys.OrganizationAcceptedDomains,
					CannedProvisioningCacheKeys.NamespaceAuthenticationTypeCacheKey
				};
			}
			return flag;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		internal static IEnumerable<QueryFilter> ConflictingDomainFilters(AcceptedDomain domain, bool ignoreStars)
		{
			if (domain != null && domain.DomainName != null)
			{
				if (domain.DomainName.Equals(SmtpDomainWithSubdomains.StarDomain))
				{
					yield return new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, domain.Guid);
				}
				else
				{
					if (ignoreStars || domain.DomainName.IncludeSubDomains)
					{
						yield return new AndFilter(new QueryFilter[]
						{
							new TextFilter(AcceptedDomainSchema.DomainName, '.' + domain.DomainName.Domain, MatchOptions.Suffix, MatchFlags.IgnoreCase),
							new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, domain.Guid)
						});
					}
					string s = domain.DomainName.SmtpDomain.Domain;
					for (int i = s.IndexOf('.', 0); i != -1; i = s.IndexOf('.', i + 1))
					{
						yield return new TextFilter(AcceptedDomainSchema.DomainName, ignoreStars ? s.Substring(i + 1) : ("*" + s.Substring(i)), MatchOptions.FullString, MatchFlags.IgnoreCase);
					}
					yield return new TextFilter(AcceptedDomainSchema.DomainName, "*", MatchOptions.FullString, MatchFlags.IgnoreCase);
				}
			}
			yield break;
		}

		internal const string LdapName = "msExchAcceptedDomain";

		internal static readonly ADObjectId AcceptedDomainContainer = new ADObjectId("CN=Accepted Domains,CN=Transport Settings");

		private static readonly AcceptedDomainSchema SchemaObject = ObjectSchema.GetInstance<AcceptedDomainSchema>();
	}
}
