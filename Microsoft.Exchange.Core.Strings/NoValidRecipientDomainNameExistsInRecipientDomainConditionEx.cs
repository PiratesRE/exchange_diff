using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoValidRecipientDomainNameExistsInRecipientDomainConditionException : LocalizedException
	{
		public NoValidRecipientDomainNameExistsInRecipientDomainConditionException(int ruleId) : base(CoreStrings.NoValidRecipientDomainNameExistsInRecipientDomainCondition(ruleId))
		{
			this.ruleId = ruleId;
		}

		public NoValidRecipientDomainNameExistsInRecipientDomainConditionException(int ruleId, Exception innerException) : base(CoreStrings.NoValidRecipientDomainNameExistsInRecipientDomainCondition(ruleId), innerException)
		{
			this.ruleId = ruleId;
		}

		protected NoValidRecipientDomainNameExistsInRecipientDomainConditionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleId = (int)info.GetValue("ruleId", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleId", this.ruleId);
		}

		public int RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		private readonly int ruleId;
	}
}
