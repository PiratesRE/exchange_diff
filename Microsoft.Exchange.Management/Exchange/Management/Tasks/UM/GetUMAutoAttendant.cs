using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMAutoAttendant", DefaultParameterSetName = "Identity")]
	public sealed class GetUMAutoAttendant : GetMultitenancySystemConfigurationObjectTask<UMAutoAttendantIdParameter, UMAutoAttendant>
	{
		[Parameter(Mandatory = false)]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(base.DomainController) ? null : base.NetCredential, sessionSettings, 66, "CreateConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\get_umautoattendant.cs");
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.UMDialPlan != null)
			{
				UMDialPlan umdialPlan = (UMDialPlan)base.GetDataObject<UMDialPlan>(this.UMDialPlan, this.ConfigurationSession, this.RootId, new LocalizedString?(Strings.NonExistantDialPlan(this.UMDialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(this.UMDialPlan.ToString())));
				this.dialplanFilter = new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.UMDialPlan, umdialPlan.Id);
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = base.InternalFilter;
				if (this.UMDialPlan != null)
				{
					queryFilter = QueryFilter.AndTogether(new QueryFilter[]
					{
						queryFilter,
						this.dialplanFilter
					});
				}
				return queryFilter;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			UMAutoAttendant umautoAttendant = dataObject as UMAutoAttendant;
			if (umautoAttendant != null && !string.IsNullOrEmpty(umautoAttendant.DefaultMailboxLegacyDN))
			{
				IRecipientSession recipientSessionScopedToOrganization = Utility.GetRecipientSessionScopedToOrganization(umautoAttendant.OrganizationId, true);
				umautoAttendant.DefaultMailbox = (recipientSessionScopedToOrganization.FindByLegacyExchangeDN(umautoAttendant.DefaultMailboxLegacyDN) as ADUser);
			}
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		private ComparisonFilter dialplanFilter;
	}
}
