using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "CompliancePolicyBase", DefaultParameterSetName = "Identity")]
	public abstract class GetCompliancePolicyBase : GetMultitenancySystemConfigurationObjectTask<PolicyIdParameter, PolicyStorage>
	{
		private protected PolicyScenario? Scenario { protected get; private set; }

		protected override bool DeepSearch
		{
			get
			{
				return false;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.GetRootId(base.DataSession);
			}
		}

		protected GetCompliancePolicyBase()
		{
		}

		protected GetCompliancePolicyBase(PolicyScenario policyScenario)
		{
			this.Scenario = new PolicyScenario?(policyScenario);
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

		protected override IEnumerable<PolicyStorage> GetPagedData()
		{
			if (this.Scenario != null)
			{
				return from dataObj in base.GetPagedData()
				where dataObj.Scenario == this.Scenario.Value
				select dataObj;
			}
			throw new NotImplementedException("Derived Class need override GetPagedData() if Scenario is not set.");
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (base.NeedSuppressingPiiData && dataObject is PsCompliancePolicyBase)
			{
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
				PsCompliancePolicyBase psCompliancePolicyBase = dataObject as PsCompliancePolicyBase;
				psCompliancePolicyBase.SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
			}
			base.WriteResult(dataObject);
		}

		protected void PopulateDistributionStatus(PsCompliancePolicyBase psPolicy, PolicyStorage policyStorage)
		{
			if (ExPolicyConfigProvider.IsFFOOnline)
			{
				PolicySettingStatusHelpers.PopulatePolicyDistributionStatus(psPolicy, policyStorage, base.DataSession, this, this.executionLogger);
			}
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
