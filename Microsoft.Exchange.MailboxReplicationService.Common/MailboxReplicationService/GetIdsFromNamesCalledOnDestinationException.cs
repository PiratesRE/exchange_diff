using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetIdsFromNamesCalledOnDestinationException : MailboxReplicationPermanentException
	{
		public GetIdsFromNamesCalledOnDestinationException() : base(MrsStrings.GetIdsFromNamesCalledOnDestination)
		{
		}

		public GetIdsFromNamesCalledOnDestinationException(Exception innerException) : base(MrsStrings.GetIdsFromNamesCalledOnDestination, innerException)
		{
		}

		protected GetIdsFromNamesCalledOnDestinationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
