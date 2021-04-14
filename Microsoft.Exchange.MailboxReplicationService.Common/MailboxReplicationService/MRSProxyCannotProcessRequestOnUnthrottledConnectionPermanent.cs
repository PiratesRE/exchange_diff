using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MRSProxyCannotProcessRequestOnUnthrottledConnectionPermanentException : MailboxReplicationPermanentException
	{
		public MRSProxyCannotProcessRequestOnUnthrottledConnectionPermanentException() : base(MrsStrings.MRSProxyConnectionNotThrottledError)
		{
		}

		public MRSProxyCannotProcessRequestOnUnthrottledConnectionPermanentException(Exception innerException) : base(MrsStrings.MRSProxyConnectionNotThrottledError, innerException)
		{
		}

		protected MRSProxyCannotProcessRequestOnUnthrottledConnectionPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
