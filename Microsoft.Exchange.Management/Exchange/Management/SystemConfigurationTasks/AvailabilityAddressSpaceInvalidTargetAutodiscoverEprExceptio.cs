using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AvailabilityAddressSpaceInvalidTargetAutodiscoverEprException : LocalizedException
	{
		public AvailabilityAddressSpaceInvalidTargetAutodiscoverEprException() : base(Strings.AvailabilityAddressSpaceInvalidTargetAutodiscoverEpr)
		{
		}

		public AvailabilityAddressSpaceInvalidTargetAutodiscoverEprException(Exception innerException) : base(Strings.AvailabilityAddressSpaceInvalidTargetAutodiscoverEpr, innerException)
		{
		}

		protected AvailabilityAddressSpaceInvalidTargetAutodiscoverEprException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
