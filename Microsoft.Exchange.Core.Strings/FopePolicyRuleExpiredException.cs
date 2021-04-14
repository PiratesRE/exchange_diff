using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FopePolicyRuleExpiredException : LocalizedException
	{
		public FopePolicyRuleExpiredException(int ruleId, DateTime expiredOn) : base(CoreStrings.FopePolicyRuleExpired(ruleId, expiredOn))
		{
			this.ruleId = ruleId;
			this.expiredOn = expiredOn;
		}

		public FopePolicyRuleExpiredException(int ruleId, DateTime expiredOn, Exception innerException) : base(CoreStrings.FopePolicyRuleExpired(ruleId, expiredOn), innerException)
		{
			this.ruleId = ruleId;
			this.expiredOn = expiredOn;
		}

		protected FopePolicyRuleExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleId = (int)info.GetValue("ruleId", typeof(int));
			this.expiredOn = (DateTime)info.GetValue("expiredOn", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleId", this.ruleId);
			info.AddValue("expiredOn", this.expiredOn);
		}

		public int RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		public DateTime ExpiredOn
		{
			get
			{
				return this.expiredOn;
			}
		}

		private readonly int ruleId;

		private readonly DateTime expiredOn;
	}
}
