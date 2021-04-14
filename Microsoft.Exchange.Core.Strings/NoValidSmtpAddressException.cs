using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoValidSmtpAddressException : LocalizedException
	{
		public NoValidSmtpAddressException(int ruleId) : base(CoreStrings.NoValidSmtpAddress(ruleId))
		{
			this.ruleId = ruleId;
		}

		public NoValidSmtpAddressException(int ruleId, Exception innerException) : base(CoreStrings.NoValidSmtpAddress(ruleId), innerException)
		{
			this.ruleId = ruleId;
		}

		protected NoValidSmtpAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
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
