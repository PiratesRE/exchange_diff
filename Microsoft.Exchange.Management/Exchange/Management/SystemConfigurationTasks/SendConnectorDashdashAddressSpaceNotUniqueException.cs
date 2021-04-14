using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorDashdashAddressSpaceNotUniqueException : LocalizedException
	{
		public SendConnectorDashdashAddressSpaceNotUniqueException() : base(Strings.SendConnectorDashdashAddressSpaceNotUnique)
		{
		}

		public SendConnectorDashdashAddressSpaceNotUniqueException(Exception innerException) : base(Strings.SendConnectorDashdashAddressSpaceNotUnique, innerException)
		{
		}

		protected SendConnectorDashdashAddressSpaceNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
