using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "ComplianceRule", SupportsShouldProcess = true)]
	public abstract class NewComplianceRuleBase : NewMultitenancyFixedNameSystemConfigurationObjectTask<RuleStorage>
	{
		private protected PolicyScenario Scenario { protected get; private set; }

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return (string)base.Fields[ADObjectSchema.Name];
			}
			set
			{
				base.Fields[ADObjectSchema.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)base.Fields[PsComplianceRuleBaseSchema.Comment];
			}
			set
			{
				base.Fields[PsComplianceRuleBaseSchema.Comment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Disabled
		{
			get
			{
				return base.Fields["Disabled"] != null && (bool)base.Fields["Disabled"];
			}
			set
			{
				base.Fields["Disabled"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNull]
		public PolicyIdParameter Policy
		{
			get
			{
				return (PolicyIdParameter)base.Fields["Policy"];
			}
			set
			{
				base.Fields["Policy"] = value;
			}
		}

		public NewComplianceRuleBase()
		{
		}

		protected NewComplianceRuleBase(PolicyScenario scenario)
		{
			this.Scenario = scenario;
		}

		protected override void InternalValidate()
		{
			Utils.ThrowIfNotRunInEOP();
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			IEnumerable<PolicyStorage> enumerable = base.GetDataObjects<PolicyStorage>(this.Policy, base.DataSession, null).ToList<PolicyStorage>();
			if (enumerable == null || !enumerable.Any((PolicyStorage s) => s.Scenario == this.Scenario))
			{
				base.WriteError(new ErrorPolicyNotFoundException(this.Policy.ToString()), ErrorCategory.InvalidOperation, null);
			}
			enumerable = from s in enumerable
			where s.Scenario == this.Scenario
			select s;
			if (enumerable.Count<PolicyStorage>() > 1)
			{
				base.WriteError(new ErrorPolicyNotUniqueException(this.Policy.ToString()), ErrorCategory.InvalidOperation, null);
			}
			this.policyStorage = enumerable.First<PolicyStorage>();
			base.WriteVerbose(Strings.VerbosePolicyStorageObjectLoadedForCommonRule(this.policyStorage.ToString(), this.Policy.ToString()));
			if (this.policyStorage.Mode == Mode.PendingDeletion)
			{
				base.WriteError(new ErrorCannotCreateRuleUnderPendingDeletionPolicyException(this.policyStorage.Name), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
			if (this.DoesComplianceRuleExist())
			{
				throw new ComplianceRuleAlreadyExistsException(this.Name);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			PolicySettingStatusHelpers.CheckNotificationResultsAndUpdateStatus(this, (IConfigurationSession)base.DataSession, this.OnNotifyChanges());
			TaskLogger.LogExit();
		}

		protected virtual IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return AggregatedNotificationClients.NotifyChanges(this, base.DataSession as IConfigurationSession, this.DataObject, this.executionLogger, base.GetType(), null, null);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		protected override IConfigurable PrepareDataObject()
		{
			RuleStorage ruleStorage = (RuleStorage)base.PrepareDataObject();
			ruleStorage.MasterIdentity = Guid.NewGuid();
			ruleStorage.Scenario = this.Scenario;
			return ruleStorage;
		}

		private bool DoesComplianceRuleExist()
		{
			bool flag = false;
			flag = (from p in base.GetDataObjects<RuleStorage>(new ComplianceRuleIdParameter(this.Name), base.DataSession, null)
			where p.Scenario == this.Scenario
			select p).Any<RuleStorage>();
			if (!flag)
			{
				IEnumerable<RuleStorage> dataObjects = base.GetDataObjects<RuleStorage>(new ComplianceRuleIdParameter(this.Name), base.DataSession, null);
				foreach (RuleStorage ruleStorage in dataObjects)
				{
					IList<PolicyStorage> source = base.GetDataObjects<PolicyStorage>(new PolicyIdParameter(ruleStorage.ParentPolicyId), base.DataSession, null).ToList<PolicyStorage>();
					if (source.Any((PolicyStorage p) => p.Scenario == this.Scenario))
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
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

		protected PolicyStorage policyStorage;
	}
}
