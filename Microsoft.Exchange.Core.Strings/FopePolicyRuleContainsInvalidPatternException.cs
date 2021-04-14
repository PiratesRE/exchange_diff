using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FopePolicyRuleContainsInvalidPatternException : LocalizedException
	{
		public FopePolicyRuleContainsInvalidPatternException(string ruleName) : base(CoreStrings.FopePolicyRuleContainsInvalidPattern(ruleName))
		{
			this.ruleName = ruleName;
		}

		public FopePolicyRuleContainsInvalidPatternException(string ruleName, Exception innerException) : base(CoreStrings.FopePolicyRuleContainsInvalidPattern(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected FopePolicyRuleContainsInvalidPatternException(SerializationInfo info, StreamingContext context) : base(info, context)
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
