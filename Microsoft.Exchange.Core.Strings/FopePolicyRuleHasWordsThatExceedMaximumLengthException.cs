using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FopePolicyRuleHasWordsThatExceedMaximumLengthException : LocalizedException
	{
		public FopePolicyRuleHasWordsThatExceedMaximumLengthException(int ruleId, int maxWordLength) : base(CoreStrings.FopePolicyRuleHasWordsThatExceedMaximumLength(ruleId, maxWordLength))
		{
			this.ruleId = ruleId;
			this.maxWordLength = maxWordLength;
		}

		public FopePolicyRuleHasWordsThatExceedMaximumLengthException(int ruleId, int maxWordLength, Exception innerException) : base(CoreStrings.FopePolicyRuleHasWordsThatExceedMaximumLength(ruleId, maxWordLength), innerException)
		{
			this.ruleId = ruleId;
			this.maxWordLength = maxWordLength;
		}

		protected FopePolicyRuleHasWordsThatExceedMaximumLengthException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleId = (int)info.GetValue("ruleId", typeof(int));
			this.maxWordLength = (int)info.GetValue("maxWordLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleId", this.ruleId);
			info.AddValue("maxWordLength", this.maxWordLength);
		}

		public int RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		public int MaxWordLength
		{
			get
			{
				return this.maxWordLength;
			}
		}

		private readonly int ruleId;

		private readonly int maxWordLength;
	}
}
