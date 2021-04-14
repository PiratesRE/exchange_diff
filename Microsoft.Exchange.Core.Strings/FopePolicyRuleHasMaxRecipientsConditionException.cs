using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FopePolicyRuleHasMaxRecipientsConditionException : LocalizedException
	{
		public FopePolicyRuleHasMaxRecipientsConditionException(int ruleId, int maxRecipients) : base(CoreStrings.FopePolicyRuleHasMaxRecipientsCondition(ruleId, maxRecipients))
		{
			this.ruleId = ruleId;
			this.maxRecipients = maxRecipients;
		}

		public FopePolicyRuleHasMaxRecipientsConditionException(int ruleId, int maxRecipients, Exception innerException) : base(CoreStrings.FopePolicyRuleHasMaxRecipientsCondition(ruleId, maxRecipients), innerException)
		{
			this.ruleId = ruleId;
			this.maxRecipients = maxRecipients;
		}

		protected FopePolicyRuleHasMaxRecipientsConditionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleId = (int)info.GetValue("ruleId", typeof(int));
			this.maxRecipients = (int)info.GetValue("maxRecipients", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleId", this.ruleId);
			info.AddValue("maxRecipients", this.maxRecipients);
		}

		public int RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		public int MaxRecipients
		{
			get
			{
				return this.maxRecipients;
			}
		}

		private readonly int ruleId;

		private readonly int maxRecipients;
	}
}
