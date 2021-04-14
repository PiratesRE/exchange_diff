using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class GetDlpPolicyImpl : CmdletImplementation
	{
		public GetDlpPolicyImpl(GetDlpPolicy taskObject)
		{
			this.taskObject = taskObject;
			this.taskObject.Fields.ResetChangeTracking();
		}

		public override void Validate()
		{
			if (this.taskObject.IdentityData != null)
			{
				this.taskObject.IdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn(DlpUtils.TenantDlpPoliciesCollectionName);
			}
		}

		public void WriteResult(IEnumerable<ADComplianceProgram> tenantDlpPolicies, GetDlpPolicy.WriteDelegate writeDelegate)
		{
			if (this.taskObject.NeedSuppressingPiiData && this.taskObject.ExchangeRunspaceConfig != null)
			{
				this.taskObject.ExchangeRunspaceConfig.EnablePiiMap = true;
			}
			foreach (ADComplianceProgram adDlpPolicy in tenantDlpPolicies)
			{
				DlpPolicy dlpPolicy = this.TryGetDlpPolicy(adDlpPolicy);
				if (this.taskObject.NeedSuppressingPiiData)
				{
					dlpPolicy.SuppressPiiData(Utils.GetSessionPiiMap(this.taskObject.ExchangeRunspaceConfig));
				}
				writeDelegate(dlpPolicy);
			}
		}

		public ObjectId GetRootId()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			if (configurationSession == null)
			{
				return null;
			}
			return configurationSession.GetOrgContainerId().GetChildId("Transport Settings").GetChildId("Rules").GetChildId(DlpUtils.TenantDlpPoliciesCollectionName);
		}

		private DlpPolicy TryGetDlpPolicy(ADComplianceProgram adDlpPolicy)
		{
			DlpPolicy result;
			try
			{
				result = new DlpPolicy(adDlpPolicy);
			}
			catch (DlpPolicyParsingException)
			{
				DlpPolicy dlpPolicy = new DlpPolicy(null);
				dlpPolicy.SetAdDlpPolicyWithNoDlpXml(adDlpPolicy);
				this.taskObject.WriteWarning(Strings.DlpPolicyXmlInvalid);
				result = dlpPolicy;
			}
			return result;
		}

		private readonly GetDlpPolicy taskObject;
	}
}
