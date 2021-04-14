using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RuleContainsNoActionsException : InvalidComplianceRuleActionException
	{
		public RuleContainsNoActionsException(string ruleName) : base(Strings.ErrorRuleContainsNoActions(ruleName))
		{
			this.ruleName = ruleName;
		}

		public RuleContainsNoActionsException(string ruleName, Exception innerException) : base(Strings.ErrorRuleContainsNoActions(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected RuleContainsNoActionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleName = (string)info.GetValue("ruleName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleName", this.ruleName);
		}

		public string RuleName
		{
			get
			{
				return this.ruleName;
			}
		}

		private readonly string ruleName;
	}
}
