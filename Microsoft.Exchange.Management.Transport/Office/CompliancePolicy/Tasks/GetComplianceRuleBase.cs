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
	[Cmdlet("Get", "ComplianceRule", DefaultParameterSetName = "Identity")]
	public abstract class GetComplianceRuleBase : GetMultitenancySystemConfigurationObjectTask<ComplianceRuleIdParameter, RuleStorage>
	{
		private protected PolicyScenario Scenario { protected get; private set; }

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.GetRootId(base.DataSession);
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.Policy != null)
				{
					return new ComparisonFilter(ComparisonOperator.Equal, RuleStorageSchema.ParentPolicyId, Utils.GetUniversalIdentity(this.PolicyStorage));
				}
				return base.InternalFilter;
			}
		}

		private protected PolicyStorage PolicyStorage { protected get; private set; }

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public PolicyIdParameter Policy { get; set; }

		protected override IEnumerable<RuleStorage> GetPagedData()
		{
			if (this.Policy != null)
			{
				return from x in base.GetPagedData()
				where x.ParentPolicyId.Equals(Utils.GetUniversalIdentity(this.PolicyStorage))
				select x;
			}
			return base.GetPagedData();
		}

		protected GetComplianceRuleBase(PolicyScenario policyScenario)
		{
			this.Scenario = policyScenario;
		}

		protected override void InternalValidate()
		{
			if (this.Policy != null)
			{
				if (this.Identity != null)
				{
					throw new PolicyAndIdentityParameterUsedTogetherException(this.PolicyStorage.ToString(), this.Identity.ToString());
				}
				this.PolicyStorage = (PolicyStorage)base.GetDataObject<PolicyStorage>(this.Policy, base.DataSession, null, new LocalizedString?(Strings.ErrorPolicyNotFound(this.Policy.ToString())), new LocalizedString?(Strings.ErrorPolicyNotUnique(this.Policy.ToString())), ExchangeErrorCategory.Client);
				base.WriteVerbose(Strings.VerbosePolicyStorageObjectLoaded(this.PolicyStorage.ToString()));
			}
			base.InternalValidate();
		}

		protected override void ResolveCurrentOrgIdBasedOnIdentity(IIdentityParameter identity)
		{
			base.ResolveCurrentOrgIdBasedOnIdentity(identity);
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (base.NeedSuppressingPiiData && dataObject is PsComplianceRuleBase)
			{
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
				PsComplianceRuleBase psComplianceRuleBase = dataObject as PsComplianceRuleBase;
				psComplianceRuleBase.SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
			}
			base.WriteResult(dataObject);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
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
