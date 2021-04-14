using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AuditLogAccessDeniedException : AuditLogException
	{
		public AuditLogAccessDeniedException() : base(Strings.AuditLogAccessDenied)
		{
		}

		public AuditLogAccessDeniedException(Exception innerException) : base(Strings.AuditLogAccessDenied, innerException)
		{
		}

		protected AuditLogAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
