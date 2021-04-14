using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataExportCanceledPermanentException : MailboxReplicationPermanentException
	{
		public DataExportCanceledPermanentException() : base(MrsStrings.DataExportCanceled)
		{
		}

		public DataExportCanceledPermanentException(Exception innerException) : base(MrsStrings.DataExportCanceled, innerException)
		{
		}

		protected DataExportCanceledPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
