using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FopePolicyRuleHasUnrecognizedActionException : LocalizedException
	{
		public FopePolicyRuleHasUnrecognizedActionException(int ruleId, int actionId) : base(CoreStrings.FopePolicyRuleHasUnrecognizedAction(ruleId, actionId))
		{
			this.ruleId = ruleId;
			this.actionId = actionId;
		}

		public FopePolicyRuleHasUnrecognizedActionException(int ruleId, int actionId, Exception innerException) : base(CoreStrings.FopePolicyRuleHasUnrecognizedAction(ruleId, actionId), innerException)
		{
			this.ruleId = ruleId;
			this.actionId = actionId;
		}

		protected FopePolicyRuleHasUnrecognizedActionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleId = (int)info.GetValue("ruleId", typeof(int));
			this.actionId = (int)info.GetValue("actionId", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleId", this.ruleId);
			info.AddValue("actionId", this.actionId);
		}

		public int RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		public int ActionId
		{
			get
			{
				return this.actionId;
			}
		}

		private readonly int ruleId;

		private readonly int actionId;
	}
}
