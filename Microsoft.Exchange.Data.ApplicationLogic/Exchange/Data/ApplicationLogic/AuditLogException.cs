using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AuditLogException : LocalizedException
	{
		public AuditLogException(LocalizedString message) : base(message)
		{
		}

		public AuditLogException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AuditLogException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
