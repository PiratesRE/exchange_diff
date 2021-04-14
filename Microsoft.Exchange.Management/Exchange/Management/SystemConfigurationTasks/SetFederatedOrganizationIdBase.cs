using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetFederatedOrganizationIdBase : SetMultitenancySingletonSystemConfigurationObjectTask<FederatedOrganizationId>
	{
		protected MultiValuedProperty<ADObjectId> FederatedAcceptedDomains
		{
			get
			{
				if (this.federatedAcceptedDomains == null)
				{
					this.federatedAcceptedDomains = this.GetFederatedAcceptedDomains();
				}
				return this.federatedAcceptedDomains;
			}
		}

		private MultiValuedProperty<ADObjectId> GetFederatedAcceptedDomains()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, this.DataObject.Guid);
			ADRawEntry[] array = this.ConfigurationSession.Find(base.CurrentOrgContainerId, QueryScope.SubTree, filter, null, 1, new ADPropertyDefinition[]
			{
				FederatedOrganizationIdSchema.AcceptedDomainsBackLink
			});
			if (array == null || array.Length == 0)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotLocateFedOrgId), ErrorCategory.InvalidOperation, null);
			}
			return array[0][FederatedOrganizationIdSchema.AcceptedDomainsBackLink] as MultiValuedProperty<ADObjectId>;
		}

		protected AcceptedDomain GetAcceptedDomain(SmtpDomain domain, bool suppressChecks)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, domain.Domain);
			AcceptedDomain[] array = base.DataSession.Find<AcceptedDomain>(filter, base.CurrentOrgContainerId, true, null) as AcceptedDomain[];
			if (array == null || array.Length == 0)
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, "*." + domain.Domain);
				array = (base.DataSession.Find<AcceptedDomain>(filter, base.CurrentOrgContainerId, true, null) as AcceptedDomain[]);
				if (array == null || array.Length == 0)
				{
					base.WriteError(new DomainNameNotAcceptedDomainException(domain.Domain), ErrorCategory.InvalidOperation, null);
				}
				else
				{
					base.WriteError(new AcceptedDomainsWithSubdomainsException(domain.Domain), ErrorCategory.InvalidOperation, null);
				}
			}
			AcceptedDomain acceptedDomain = array[0];
			if (suppressChecks)
			{
				return acceptedDomain;
			}
			if (acceptedDomain.DomainName.IncludeSubDomains)
			{
				base.WriteError(new AcceptedDomainsWithSubdomainsException(domain.Domain), ErrorCategory.InvalidOperation, null);
			}
			if (acceptedDomain.DomainType != AcceptedDomainType.Authoritative && AcceptedDomainType.InternalRelay != acceptedDomain.DomainType)
			{
				base.WriteError(new AcceptedDomainsInvalidTypeException(domain.Domain), ErrorCategory.InvalidOperation, null);
			}
			return acceptedDomain;
		}

		protected void ZapDanglingDomainTrusts()
		{
			foreach (ADObjectId identity in this.FederatedAcceptedDomains)
			{
				AcceptedDomain acceptedDomain = (AcceptedDomain)base.DataSession.Read<AcceptedDomain>(identity);
				if (acceptedDomain != null && acceptedDomain.FederatedOrganizationLink != null)
				{
					base.WriteVerbose(Strings.SetFedAcceptedDomainCleanup(acceptedDomain.DomainName.Domain));
					acceptedDomain.FederatedOrganizationLink = null;
					base.DataSession.Save(acceptedDomain);
				}
			}
		}

		private MultiValuedProperty<ADObjectId> federatedAcceptedDomains;
	}
}
