using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidCompliancePolicyWorkloadException : LocalizedException
	{
		public InvalidCompliancePolicyWorkloadException() : base(Strings.InvalidCompliancePolicyWorkload)
		{
		}

		public InvalidCompliancePolicyWorkloadException(Exception innerException) : base(Strings.InvalidCompliancePolicyWorkload, innerException)
		{
		}

		protected InvalidCompliancePolicyWorkloadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
