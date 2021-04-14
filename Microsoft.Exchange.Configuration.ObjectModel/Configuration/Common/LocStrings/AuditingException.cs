using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Common.LocStrings
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AuditingException : LocalizedException
	{
		public AuditingException(LocalizedString message) : base(message)
		{
		}

		public AuditingException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AuditingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
