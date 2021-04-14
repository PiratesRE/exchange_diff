using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.PolicyNudges
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetPolicyTipConfigUrlUsedAsActionFilterException : LocalizedException
	{
		public GetPolicyTipConfigUrlUsedAsActionFilterException() : base(Strings.GetPolicyTipConfigUrlUsedAsActionFilter)
		{
		}

		public GetPolicyTipConfigUrlUsedAsActionFilterException(Exception innerException) : base(Strings.GetPolicyTipConfigUrlUsedAsActionFilter, innerException)
		{
		}

		protected GetPolicyTipConfigUrlUsedAsActionFilterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
