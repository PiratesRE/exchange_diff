using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToObtainServersInLocalSitePermanentException : MailboxReplicationPermanentException
	{
		public UnableToObtainServersInLocalSitePermanentException() : base(MrsStrings.UnableToObtainServersInLocalSite)
		{
		}

		public UnableToObtainServersInLocalSitePermanentException(Exception innerException) : base(MrsStrings.UnableToObtainServersInLocalSite, innerException)
		{
		}

		protected UnableToObtainServersInLocalSitePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
