using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "ComplianceRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public abstract class SetComplianceRuleBase : SetSystemConfigurationObjectTask<ComplianceRuleIdParameter, PsComplianceRuleBase, RuleStorage>
	{
		private protected PolicyScenario Scenario { protected get; private set; }

		protected PsComplianceRuleBase PsRulePresentationObject { get; set; }

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)base.Fields["Comment"];
			}
			set
			{
				base.Fields["Comment"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Disabled
		{
			get
			{
				return (bool)base.Fields["Disabled"];
			}
			set
			{
				base.Fields["Disabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ContentMatchQuery
		{
			get
			{
				return (string)base.Fields["ContentMatchQuery"];
			}
			set
			{
				base.Fields["ContentMatchQuery"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.GetRootId(base.DataSession);
			}
		}

		public SetComplianceRuleBase()
		{
		}

		protected SetComplianceRuleBase(PolicyScenario scenario)
		{
			this.Scenario = scenario;
		}

		protected virtual void CopyExplicitParameters()
		{
			if (base.Fields.IsModified("Comment"))
			{
				this.PsRulePresentationObject.Comment = this.Comment;
			}
			if (base.Fields.IsModified("Disabled"))
			{
				this.PsRulePresentationObject.Disabled = this.Disabled;
			}
			if (base.Fields.IsModified("ContentMatchQuery"))
			{
				this.PsRulePresentationObject.ContentMatchQuery = this.ContentMatchQuery;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			RuleStorage ruleStorage = base.GetDataObjects<RuleStorage>(this.Identity, base.DataSession, null).FirstOrDefault((RuleStorage r) => r.Scenario == this.Scenario);
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
			Utils.ThrowIfNotRunInEOP();
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			base.InternalValidate();
			if (this.DataObject.IsModified(ADObjectSchema.Name) && this.DoesComplianceRuleExist())
			{
				throw new ComplianceRuleAlreadyExistsException((string)this.DataObject[ADObjectSchema.Name]);
			}
			if (base.Fields.IsModified("Disabled") && this.Disabled)
			{
				PolicyStorage policyStorage = (PolicyStorage)base.GetDataObject<PolicyStorage>(new PolicyIdParameter(this.DataObject.ParentPolicyId), base.DataSession, null, new LocalizedString?(Strings.ErrorPolicyNotFound(this.DataObject.ParentPolicyId.ToString())), new LocalizedString?(Strings.ErrorPolicyNotUnique(this.DataObject.ParentPolicyId.ToString())), ExchangeErrorCategory.Client);
				if (policyStorage.IsEnabled)
				{
					this.WriteWarning(Strings.WarningDisabledRuleInEnabledPolicy(this.DataObject.Name));
				}
			}
		}

		private bool DoesComplianceRuleExist()
		{
			bool flag = false;
			flag = (from p in base.GetDataObjects<RuleStorage>(new ComplianceRuleIdParameter((string)this.DataObject[ADObjectSchema.Name]), base.DataSession, null)
			where p.Scenario == this.Scenario
			select p).Any<RuleStorage>();
			if (!flag)
			{
				IEnumerable<RuleStorage> dataObjects = base.GetDataObjects<RuleStorage>(new ComplianceRuleIdParameter((string)this.DataObject[ADObjectSchema.Name]), base.DataSession, null);
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

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
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
