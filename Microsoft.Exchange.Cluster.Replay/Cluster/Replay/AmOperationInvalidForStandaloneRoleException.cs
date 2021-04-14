using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmOperationInvalidForStandaloneRoleException : AmCommonException
	{
		public AmOperationInvalidForStandaloneRoleException() : base(ReplayStrings.AmOperationInvalidForStandaloneRoleException)
		{
		}

		public AmOperationInvalidForStandaloneRoleException(Exception innerException) : base(ReplayStrings.AmOperationInvalidForStandaloneRoleException, innerException)
		{
		}

		protected AmOperationInvalidForStandaloneRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
