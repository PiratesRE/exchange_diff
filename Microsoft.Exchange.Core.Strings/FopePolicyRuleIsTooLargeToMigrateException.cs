using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FopePolicyRuleIsTooLargeToMigrateException : LocalizedException
	{
		public FopePolicyRuleIsTooLargeToMigrateException(string ruleName, ulong ruleSize, ulong maxRuleSize) : base(CoreStrings.FopePolicyRuleIsTooLargeToMigrate(ruleName, ruleSize, maxRuleSize))
		{
			this.ruleName = ruleName;
			this.ruleSize = ruleSize;
			this.maxRuleSize = maxRuleSize;
		}

		public FopePolicyRuleIsTooLargeToMigrateException(string ruleName, ulong ruleSize, ulong maxRuleSize, Exception innerException) : base(CoreStrings.FopePolicyRuleIsTooLargeToMigrate(ruleName, ruleSize, maxRuleSize), innerException)
		{
			this.ruleName = ruleName;
			this.ruleSize = ruleSize;
			this.maxRuleSize = maxRuleSize;
		}

		protected FopePolicyRuleIsTooLargeToMigrateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleName = (string)info.GetValue("ruleName", typeof(string));
			this.ruleSize = (ulong)info.GetValue("ruleSize", typeof(ulong));
			this.maxRuleSize = (ulong)info.GetValue("maxRuleSize", typeof(ulong));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleName", this.ruleName);
			info.AddValue("ruleSize", this.ruleSize);
			info.AddValue("maxRuleSize", this.maxRuleSize);
		}

		public string RuleName
		{
			get
			{
				return this.ruleName;
			}
		}

		public ulong RuleSize
		{
			get
			{
				return this.ruleSize;
			}
		}

		public ulong MaxRuleSize
		{
			get
			{
				return this.maxRuleSize;
			}
		}

		private readonly string ruleName;

		private readonly ulong ruleSize;

		private readonly ulong maxRuleSize;
	}
}
