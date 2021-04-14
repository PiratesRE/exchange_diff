using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SiteMailbox", DefaultParameterSetName = "TeamMailboxIW")]
	public sealed class GetSiteMailbox : GetRecipientWithAddressListBase<RecipientIdParameter, ADUser>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public new RecipientIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public new OrganizationIdParameter Organization
		{
			get
			{
				return base.Organization;
			}
			set
			{
				base.Organization = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public SwitchParameter DeletedSiteMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["DeletedSiteMailbox"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DeletedSiteMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new string Anr
		{
			get
			{
				return base.Anr;
			}
			set
			{
				base.Anr = value;
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

		private new PSCredential Credential { get; set; }

		private new string Filter { get; set; }

		private new SwitchParameter IgnoreDefaultScope { get; set; }

		private new OrganizationalUnitIdParameter OrganizationalUnit { get; set; }

		private new string SortBy { get; set; }

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<TeamMailboxSchema>();
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetSiteMailbox.SortPropertiesArray;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.ParameterSetName == "TeamMailboxIW" && !base.TryGetExecutingUserId(out this.executingUserId))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxCannotIdentifyTheUser), ExchangeErrorCategory.Client, this.Identity);
			}
			if (GetSiteMailbox.isMultiTenant && base.CurrentOrganizationId != null && base.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
			{
				ITenantConfigurationSession tenantConfigSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.CurrentOrganizationId), 164, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\TeamMailbox\\GetSiteMailbox.cs");
				ExchangeConfigurationUnit exchangeConfigurationUnit = base.ProvisioningCache.TryAddAndGetOrganizationData<ExchangeConfigurationUnit>(CannedProvisioningCacheKeys.OrganizationCUContainer, base.CurrentOrganizationId, delegate()
				{
					ADObjectId configurationUnit = this.CurrentOrganizationId.ConfigurationUnit;
					IConfigurationSession configurationSession = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(tenantConfigSession, this.CurrentOrganizationId, true);
					return configurationSession.Read<ExchangeConfigurationUnit>(configurationUnit);
				});
				if (exchangeConfigurationUnit != null)
				{
					this.isHybridTenant = exchangeConfigurationUnit.IsDirSyncRunning;
				}
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.TeamMailbox)
				});
				if (this.DeletedSiteMailbox && !this.isHybridTenant)
				{
					QueryFilter queryFilter2 = new AmbiguousNameResolutionFilter("MDEL:");
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					});
				}
				else
				{
					QueryFilter queryFilter3 = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser),
						new OrFilter(new QueryFilter[]
						{
							new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientDisplayType, RecipientDisplayType.SyncedTeamMailboxUser),
							new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientDisplayType, RecipientDisplayType.ACLableSyncedTeamMailboxUser)
						})
					});
					queryFilter = new OrFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter3
					});
					if (base.ParameterSetName == "TeamMailboxIW")
					{
						QueryFilter queryFilter4 = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.DelegateListLink, this.executingUserId);
						queryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							queryFilter4
						});
					}
				}
				if (base.InternalFilter != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						base.InternalFilter
					});
				}
				return queryFilter;
			}
		}

		protected override bool ShouldSkipObject(IConfigurable dataObject)
		{
			ADUser aduser = dataObject as ADUser;
			if (aduser == null)
			{
				return true;
			}
			if (this.DeletedSiteMailbox)
			{
				return !TeamMailbox.IsPendingDeleteSiteMailbox(aduser);
			}
			if (TeamMailbox.IsPendingDeleteSiteMailbox(aduser))
			{
				return true;
			}
			if (!TeamMailbox.IsLocalTeamMailbox(aduser) && !TeamMailbox.IsRemoteTeamMailbox(aduser))
			{
				return true;
			}
			if (base.ParameterSetName == "TeamMailboxIW")
			{
				TeamMailbox teamMailbox = TeamMailbox.FromDataObject((ADUser)dataObject);
				return !teamMailbox.OwnersAndMembers.Contains(this.executingUserId);
			}
			return false;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			TeamMailbox teamMailbox = TeamMailbox.FromDataObject((ADUser)dataObject);
			teamMailbox.SetMyRole(this.executingUserId);
			if (this.executingUserId == null)
			{
				teamMailbox.ShowInMyClient = false;
			}
			else if (base.ExecutingUserOrganizationId == base.CurrentOrganizationId)
			{
				if (teamMailbox.Active)
				{
					ADUser aduser = (ADUser)base.GetDataObject<ADUser>(new MailboxIdParameter(this.executingUserId), base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.executingUserId.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.executingUserId.ToString())));
					teamMailbox.ShowInMyClient = aduser.TeamMailboxShowInClientList.Contains(teamMailbox.Id);
				}
				else
				{
					teamMailbox.ShowInMyClient = false;
				}
			}
			return teamMailbox;
		}

		private static readonly bool isMultiTenant = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;

		private bool isHybridTenant;

		private ADObjectId executingUserId;

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			TeamMailboxSchema.DisplayName
		};
	}
}
