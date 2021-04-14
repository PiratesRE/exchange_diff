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
	[Cmdlet("Remove", "CompliancePolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public abstract class RemoveCompliancePolicyBase : RemoveSystemConfigurationObjectTask<PolicyIdParameter, PolicyStorage>
	{
		protected IList<RuleStorage> RuleStorages { get; set; }

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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveCompliancePolicyConfirmation(this.Identity.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.GetRootId(base.DataSession);
			}
		}

		public RemoveCompliancePolicyBase()
		{
		}

		protected RemoveCompliancePolicyBase(PolicyScenario scenario)
		{
			this.Scenario = scenario;
		}

		protected override IConfigurable ResolveDataObject()
		{
			IEnumerable<PolicyStorage> dataObjects = base.GetDataObjects<PolicyStorage>(this.Identity, base.DataSession, null);
			foreach (PolicyStorage policyStorage in dataObjects)
			{
				if (policyStorage.Scenario == this.Scenario)
				{
					return policyStorage;
				}
			}
			base.WriteError(new ErrorPolicyNotFoundException(this.Identity.ToString()), ErrorCategory.InvalidOperation, null);
			return null;
		}

		protected override void InternalValidate()
		{
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			base.InternalValidate();
			if (base.DataObject.Mode == Mode.PendingDeletion && this.ShouldSoftDeleteObject())
			{
				base.WriteError(new ErrorCannotRemovePendingDeletionPolicyException(base.DataObject.Name), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override bool ShouldSoftDeleteObject()
		{
			return !this.ForceDeletion.ToBool();
		}

		protected override void SaveSoftDeletedObject()
		{
			base.DataObject.Mode = Mode.PendingDeletion;
			base.DataSession.Save(base.DataObject);
			if (this.RuleStorages != null)
			{
				foreach (RuleStorage ruleStorage in this.RuleStorages)
				{
					if (ruleStorage.Mode != Mode.PendingDeletion)
					{
						ruleStorage.Mode = Mode.PendingDeletion;
						base.DataSession.Save(ruleStorage);
					}
				}
			}
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.RuleStorages = Utils.LoadRuleStoragesByPolicy(base.DataSession, base.DataObject, this.RootId);
			IList<BindingStorage> list = Utils.LoadBindingStoragesByPolicy(base.DataSession, base.DataObject);
			foreach (RuleStorage ruleStorage in this.RuleStorages)
			{
				base.WriteVerbose(Strings.VerboseLoadRuleStorageObjectsForPolicy(ruleStorage.ToString(), base.DataObject.ToString()));
			}
			if (!this.ShouldSoftDeleteObject())
			{
				Utils.RemovePolicyStorageBase(base.DataSession, new WriteVerboseDelegate(base.WriteVerbose), this.RuleStorages);
				Utils.RemovePolicyStorageBase(base.DataSession, new WriteVerboseDelegate(base.WriteVerbose), list);
			}
			else
			{
				list = null;
			}
			base.InternalProcessRecord();
			if (this.ShouldSoftDeleteObject())
			{
				PolicySettingStatusHelpers.CheckNotificationResultsAndUpdateStatus(this, (IConfigurationSession)base.DataSession, this.OnNotifyChanges(list, this.RuleStorages));
			}
			TaskLogger.LogExit();
		}

		protected virtual IEnumerable<ChangeNotificationData> OnNotifyChanges(IEnumerable<UnifiedPolicyStorageBase> bindingStorageObjects, IEnumerable<UnifiedPolicyStorageBase> ruleStorageObjects)
		{
			return AggregatedNotificationClients.NotifyChanges(this, (IConfigurationSession)base.DataSession, base.DataObject, this.executionLogger, base.GetType(), bindingStorageObjects, ruleStorageObjects);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
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
