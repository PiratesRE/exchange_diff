using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.OutlookProtectionRules
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OutlookProtectionRuleInvalidPriorityException : LocalizedException
	{
		public OutlookProtectionRuleInvalidPriorityException() : base(Strings.OutlookProtectionRuleInvalidPriority)
		{
		}

		public OutlookProtectionRuleInvalidPriorityException(Exception innerException) : base(Strings.OutlookProtectionRuleInvalidPriority, innerException)
		{
		}

		protected OutlookProtectionRuleInvalidPriorityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
