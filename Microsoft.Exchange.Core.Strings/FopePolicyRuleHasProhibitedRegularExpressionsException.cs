using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FopePolicyRuleHasProhibitedRegularExpressionsException : LocalizedException
	{
		public FopePolicyRuleHasProhibitedRegularExpressionsException(int ruleId, string reason) : base(CoreStrings.FopePolicyRuleHasProhibitedRegularExpressions(ruleId, reason))
		{
			this.ruleId = ruleId;
			this.reason = reason;
		}

		public FopePolicyRuleHasProhibitedRegularExpressionsException(int ruleId, string reason, Exception innerException) : base(CoreStrings.FopePolicyRuleHasProhibitedRegularExpressions(ruleId, reason), innerException)
		{
			this.ruleId = ruleId;
			this.reason = reason;
		}

		protected FopePolicyRuleHasProhibitedRegularExpressionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleId = (int)info.GetValue("ruleId", typeof(int));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleId", this.ruleId);
			info.AddValue("reason", this.reason);
		}

		public int RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly int ruleId;

		private readonly string reason;
	}
}
