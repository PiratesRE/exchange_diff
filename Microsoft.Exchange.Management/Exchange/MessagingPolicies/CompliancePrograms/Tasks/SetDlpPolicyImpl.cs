using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class SetDlpPolicyImpl : CmdletImplementation
	{
		public SetDlpPolicyImpl(SetDlpPolicy taskObject)
		{
			this.taskObject = taskObject;
		}

		public override void Validate()
		{
			base.Validate();
			DlpPolicyMetaData dlpPolicyMetaData = DlpPolicyParser.ParseDlpPolicyInstance(this.taskObject.TargetItem.TransportRulesXml);
			if (this.taskObject.Fields.IsModified("State"))
			{
				dlpPolicyMetaData.State = this.taskObject.State;
			}
			if (this.taskObject.Fields.IsModified("Mode"))
			{
				dlpPolicyMetaData.Mode = this.taskObject.Mode;
			}
			if (this.taskObject.TargetItem.IsModified(ADObjectSchema.Name))
			{
				dlpPolicyMetaData.Name = this.taskObject.TargetItem.Name;
			}
			if (this.taskObject.Fields.IsModified("Description"))
			{
				dlpPolicyMetaData.Description = this.taskObject.Description;
			}
			ADComplianceProgram adcomplianceProgram = dlpPolicyMetaData.ToAdObject();
			this.taskObject.TargetItem.State = adcomplianceProgram.State;
			this.taskObject.TargetItem.Name = adcomplianceProgram.Name;
			this.taskObject.TargetItem.Description = adcomplianceProgram.Description;
			this.taskObject.TargetItem.TransportRulesXml = adcomplianceProgram.TransportRulesXml;
		}

		public override void ProcessRecord()
		{
			Tuple<RuleState, RuleMode> tuple = DlpUtils.DlpStateToRuleState(this.taskObject.TargetItem.State);
			this.UpdateRules(tuple.Item1, tuple.Item2);
		}

		protected void UpdateRules(RuleState state, RuleMode mode)
		{
			bool flag = this.taskObject.TargetItem.IsModified(ADObjectSchema.Name);
			ADRuleStorageManager adruleStorageManager;
			IEnumerable<TransportRuleHandle> transportRuleHandles = DlpUtils.GetTransportRuleHandles(base.DataSession, out adruleStorageManager);
			foreach (TransportRule transportRule in (from handle in transportRuleHandles
			select handle.Rule).Where(new Func<TransportRule, bool>(this.RuleDlpPolicyIdMatches)))
			{
				transportRule.Enabled = state;
				transportRule.Mode = mode;
				if (flag)
				{
					transportRule.SetDlpPolicy(this.taskObject.TargetItem.ImmutableId, this.taskObject.TargetItem.Name);
				}
			}
			adruleStorageManager.UpdateRuleHandles(transportRuleHandles);
		}

		protected bool RuleDlpPolicyIdMatches(TransportRule rule)
		{
			Guid guid;
			return rule.TryGetDlpPolicyId(out guid) && guid.Equals(this.taskObject.TargetItem.ImmutableId);
		}

		public static readonly string Identity = "Identity";

		private SetDlpPolicy taskObject;
	}
}
