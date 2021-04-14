using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class TeamMailboxDiagnosticsBase : DataAccessTask<ADUser>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public RecipientIdParameter Identity
		{
			get
			{
				return (RecipientIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public SwitchParameter BypassOwnerCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassOwnerCheck"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BypassOwnerCheck"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		internal Dictionary<ADUser, ExchangePrincipal> TMPrincipals
		{
			get
			{
				return this.tmPrincipals;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 121, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\TeamMailbox\\TeamMailboxDiagnosticsBase.cs");
		}

		protected override void InternalValidate()
		{
			OptionalIdentityData optionalIdentityData = new OptionalIdentityData();
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.TeamMailbox);
			queryFilter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			if (base.ParameterSetName != "TeamMailboxITPro")
			{
				ADObjectId propertyValue = null;
				if (!base.TryGetExecutingUserId(out propertyValue))
				{
					base.WriteError(new InvalidOperationException(Strings.CouldNotGetExecutingUser), ErrorCategory.InvalidOperation, null);
				}
				QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.DelegateListLink, propertyValue);
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter3
				});
				if (this.additionalConstrainedIdentity != null)
				{
					queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.DelegateListLink, this.additionalConstrainedIdentity);
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter3
					});
				}
			}
			optionalIdentityData.AdditionalFilter = queryFilter;
			LocalizedString? localizedString = null;
			IEnumerable<ADUser> dataObjects = base.GetDataObjects<ADUser>(this.Identity, base.DataSession, null, optionalIdentityData, out localizedString);
			foreach (ADUser aduser in dataObjects)
			{
				ExchangePrincipal value = ExchangePrincipal.FromADUser(((IRecipientSession)base.DataSession).SessionSettings, aduser, RemotingOptions.AllowCrossSite);
				this.tmPrincipals.Add(aduser, value);
			}
			if (this.tmPrincipals.Count == 0)
			{
				base.WriteError(new InvalidOperationException(Strings.CouldNotLocateAnyTeamMailbox), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			TaskLogger.LogExit();
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 228, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\TeamMailbox\\TeamMailboxDiagnosticsBase.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId;
		}

		private Dictionary<ADUser, ExchangePrincipal> tmPrincipals = new Dictionary<ADUser, ExchangePrincipal>();

		protected ADObjectId additionalConstrainedIdentity;

		public enum TargetType : uint
		{
			All,
			Document,
			Membership,
			Maintenance
		}
	}
}
