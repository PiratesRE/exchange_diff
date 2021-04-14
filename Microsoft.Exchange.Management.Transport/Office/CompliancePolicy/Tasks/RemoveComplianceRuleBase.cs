using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "ComplianceRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public abstract class RemoveComplianceRuleBase : RemoveSystemConfigurationObjectTask<PolicyIdParameter, RuleStorage>
	{
		private protected PolicyScenario Scenario { protected get; private set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter ForceDeletion
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceDeletion"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceDeletion"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.GetRootId(base.DataSession);
			}
		}

		public RemoveComplianceRuleBase()
		{
		}

		protected RemoveComplianceRuleBase(PolicyScenario scenario)
		{
			this.Scenario = scenario;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveComplianceRuleConfirmation(this.Identity.ToString());
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			RuleStorage ruleStorage = base.GetDataObjects<RuleStorage>(this.Identity, base.DataSession, null).FirstOrDefault((RuleStorage p) => p.Scenario == this.Scenario);
			if (ruleStorage != null)
			{
				return ruleStorage;
			}
			IEnumerable<RuleStorage> dataObjects = base.GetDataObjects<RuleStorage>(this.Identity, base.DataSession, null);
			foreach (RuleStorage ruleStorage2 in dataObjects)
			{
				IList<PolicyStorage> source = base.GetDataObjects<PolicyStorage>(new PolicyIdParameter(ruleStorage2.ParentPolicyId), base.DataSession, null).ToList<PolicyStorage>();
				if (source.Any((PolicyStorage p) => p.Scenario == this.Scenario))
				{
					return ruleStorage2;
				}
			}
			base.WriteError(new ErrorRuleNotFoundException(this.Identity.ToString()), ErrorCategory.InvalidOperation, null);
			return null;
		}

		protected override void InternalValidate()
		{
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			base.InternalValidate();
			if (base.DataObject.Mode == Mode.PendingDeletion && this.ShouldSoftDeleteObject())
			{
				base.WriteError(new ErrorCannotRemovePendingDeletionRuleException(base.DataObject.Name), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override bool ShouldSoftDeleteObject()
		{
			return !this.ForceDeletion.ToBool();
		}

		protected override void SaveSoftDeletedObject()
		{
			base.DataObject.Mode = Mode.PendingDeletion;
			base.DataSession.Save(base.DataObject);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (this.ShouldSoftDeleteObject())
			{
				PolicySettingStatusHelpers.CheckNotificationResultsAndUpdateStatus(this, (IConfigurationSession)base.DataSession, this.OnNotifyChanges());
			}
			TaskLogger.LogExit();
		}

		protected virtual IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return AggregatedNotificationClients.NotifyChanges(this, base.DataSession as IConfigurationSession, base.DataObject, this.executionLogger, base.GetType(), null, null);
		}

		protected override void InternalStateReset()
		{
			this.DisposePolicyConfigProvider();
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.DisposePolicyConfigProvider();
		}

		private void DisposePolicyConfigProvider()
		{
			PolicyConfigProvider policyConfigProvider = base.DataSession as PolicyConfigProvider;
			if (policyConfigProvider != null)
			{
				policyConfigProvider.Dispose();
			}
		}

		protected ExecutionLog executionLogger = ExExecutionLog.CreateForCmdlet();
	}
}
