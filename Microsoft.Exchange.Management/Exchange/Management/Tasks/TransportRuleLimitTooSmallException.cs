using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TransportRuleLimitTooSmallException : LocalizedException
	{
		public TransportRuleLimitTooSmallException(int newValue, int ruleCount) : base(Strings.ErrorTransportRuleLimitTooSmall(newValue, ruleCount))
		{
			this.newValue = newValue;
			this.ruleCount = ruleCount;
		}

		public TransportRuleLimitTooSmallException(int newValue, int ruleCount, Exception innerException) : base(Strings.ErrorTransportRuleLimitTooSmall(newValue, ruleCount), innerException)
		{
			this.newValue = newValue;
			this.ruleCount = ruleCount;
		}

		protected TransportRuleLimitTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.newValue = (int)info.GetValue("newValue", typeof(int));
			this.ruleCount = (int)info.GetValue("ruleCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("newValue", this.newValue);
			info.AddValue("ruleCount", this.ruleCount);
		}

		public int NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		public int RuleCount
		{
			get
			{
				return this.ruleCount;
			}
		}

		private readonly int newValue;

		private readonly int ruleCount;
	}
}
