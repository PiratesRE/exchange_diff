using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotAuthorizedException : ThirdPartyReplicationException
	{
		public NotAuthorizedException() : base(ThirdPartyReplication.NotAuthorizedError)
		{
		}

		public NotAuthorizedException(Exception innerException) : base(ThirdPartyReplication.NotAuthorizedError, innerException)
		{
		}

		protected NotAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
