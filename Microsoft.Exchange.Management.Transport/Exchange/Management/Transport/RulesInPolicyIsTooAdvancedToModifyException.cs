using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RulesInPolicyIsTooAdvancedToModifyException : TaskObjectIsTooAdvancedException
	{
		public RulesInPolicyIsTooAdvancedToModifyException(string policy, string rule) : base(Strings.ErrorRulesInPolicyIsTooAdvancedToModify(policy, rule))
		{
			this.policy = policy;
			this.rule = rule;
		}

		public RulesInPolicyIsTooAdvancedToModifyException(string policy, string rule, Exception innerException) : base(Strings.ErrorRulesInPolicyIsTooAdvancedToModify(policy, rule), innerException)
		{
			this.policy = policy;
			this.rule = rule;
		}

		protected RulesInPolicyIsTooAdvancedToModifyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policy = (string)info.GetValue("policy", typeof(string));
			this.rule = (string)info.GetValue("rule", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policy", this.policy);
			info.AddValue("rule", this.rule);
		}

		public string Policy
		{
			get
			{
				return this.policy;
			}
		}

		public string Rule
		{
			get
			{
				return this.rule;
			}
		}

		private readonly string policy;

		private readonly string rule;
	}
}
