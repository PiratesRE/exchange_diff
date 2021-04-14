using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorRuleNotFoundException : LocalizedException
	{
		public ErrorRuleNotFoundException(string ruleId) : base(Strings.ErrorRuleNotFound(ruleId))
		{
			this.ruleId = ruleId;
		}

		public ErrorRuleNotFoundException(string ruleId, Exception innerException) : base(Strings.ErrorRuleNotFound(ruleId), innerException)
		{
			this.ruleId = ruleId;
		}

		protected ErrorRuleNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleId = (string)info.GetValue("ruleId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleId", this.ruleId);
		}

		public string RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		private readonly string ruleId;
	}
}
