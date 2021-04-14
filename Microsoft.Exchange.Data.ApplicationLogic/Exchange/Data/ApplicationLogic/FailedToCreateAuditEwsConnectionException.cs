using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToCreateAuditEwsConnectionException : AuditLogException
	{
		public FailedToCreateAuditEwsConnectionException() : base(Strings.FailedToCreateAuditEwsConnection)
		{
		}

		public FailedToCreateAuditEwsConnectionException(Exception innerException) : base(Strings.FailedToCreateAuditEwsConnection, innerException)
		{
		}

		protected FailedToCreateAuditEwsConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
