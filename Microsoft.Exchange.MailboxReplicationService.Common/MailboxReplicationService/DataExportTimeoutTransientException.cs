using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataExportTimeoutTransientException : MailboxReplicationTransientException
	{
		public DataExportTimeoutTransientException() : base(MrsStrings.DataExportTimeout)
		{
		}

		public DataExportTimeoutTransientException(Exception innerException) : base(MrsStrings.DataExportTimeout, innerException)
		{
		}

		protected DataExportTimeoutTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
