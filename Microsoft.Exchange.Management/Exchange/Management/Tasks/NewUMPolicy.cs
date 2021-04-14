using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "UMMailboxPolicy", SupportsShouldProcess = true)]
	public sealed class NewUMPolicy : NewMailboxPolicyBase<UMMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewUMMailboxPolicy(base.Name.ToString(), this.UMDialPlan.ToString());
			}
		}

		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter SharedUMDialPlan
		{
			get
			{
				return (SwitchParameter)(base.Fields["SharedUMDialPlan"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SharedUMDialPlan"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			UMMailboxPolicy ummailboxPolicy = (UMMailboxPolicy)base.PrepareDataObject();
			if (!base.HasErrors)
			{
				UMDialPlanIdParameter umdialPlan = this.UMDialPlan;
				IConfigurationSession dialPlanSession = this.GetDialPlanSession();
				UMDialPlan umdialPlan2 = (UMDialPlan)base.GetDataObject<UMDialPlan>(umdialPlan, dialPlanSession, null, new LocalizedString?(Strings.NonExistantDialPlan(umdialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(umdialPlan.ToString())));
				ummailboxPolicy.UMDialPlan = umdialPlan2.Id;
				ummailboxPolicy.SourceForestPolicyNames.Add(ummailboxPolicy.Name);
			}
			return ummailboxPolicy;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.CreateParentContainerIfNeeded(this.DataObject);
			UMDialPlanIdParameter umdialPlan = this.UMDialPlan;
			IConfigurationSession dialPlanSession = this.GetDialPlanSession();
			UMDialPlan umdialPlan2 = (UMDialPlan)base.GetDataObject<UMDialPlan>(umdialPlan, dialPlanSession, null, new LocalizedString?(Strings.NonExistantDialPlan(umdialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(umdialPlan.ToString())));
			if (umdialPlan2.SubscriberType == UMSubscriberType.Consumer)
			{
				this.DataObject.AllowDialPlanSubscribers = false;
				this.DataObject.AllowExtensions = false;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private IConfigurationSession GetDialPlanSession()
		{
			IConfigurationSession result = (IConfigurationSession)base.DataSession;
			if (this.SharedUMDialPlan)
			{
				ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				result = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 167, "GetDialPlanSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxPolicies\\UMMailboxPolicyTask.cs");
			}
			return result;
		}

		private const string ParameterSharedUMDialPlan = "SharedUMDialPlan";
	}
}
