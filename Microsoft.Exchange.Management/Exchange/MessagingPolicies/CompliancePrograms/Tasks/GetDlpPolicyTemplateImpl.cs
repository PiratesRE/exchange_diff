using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class GetDlpPolicyTemplateImpl : CmdletImplementation
	{
		public GetDlpPolicyTemplateImpl(GetDlpPolicyTemplate taskObject)
		{
			this.taskObject = taskObject;
			this.taskObject.Fields.ResetChangeTracking();
		}

		public override void Validate()
		{
			if (this.taskObject.IdentityData != null)
			{
				this.taskObject.IdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn(DlpUtils.OutOfBoxDlpPoliciesCollectionName);
			}
		}

		public void WriteResult(IEnumerable<ADComplianceProgram> tenantDlpPolicyTemplates, GetDlpPolicy.WriteDelegate writeDelegate)
		{
			IEnumerable<ADComplianceProgram> outOfBoxDlpTemplates;
			if (this.taskObject.Identity == null)
			{
				outOfBoxDlpTemplates = DlpUtils.GetOutOfBoxDlpTemplates(base.DataSession);
			}
			else
			{
				outOfBoxDlpTemplates = DlpUtils.GetOutOfBoxDlpTemplates(base.DataSession, this.taskObject.Identity.ToString());
			}
			foreach (ADComplianceProgram dlpPolicy in outOfBoxDlpTemplates)
			{
				writeDelegate(new DlpPolicyTemplate(dlpPolicy, this.taskObject.CommandRuntime.Host.CurrentCulture));
			}
		}

		public ObjectId GetRootId()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			if (configurationSession == null)
			{
				return null;
			}
			return configurationSession.GetOrgContainerId().GetChildId("Transport Settings").GetChildId("Rules").GetChildId(DlpUtils.OutOfBoxDlpPoliciesCollectionName);
		}

		private GetDlpPolicyTemplate taskObject;
	}
}
