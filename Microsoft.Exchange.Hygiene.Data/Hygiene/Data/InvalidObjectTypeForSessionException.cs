using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Hygiene.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidObjectTypeForSessionException : LocalizedException
	{
		public InvalidObjectTypeForSessionException(LocalizedString message) : base(message)
		{
		}

		public InvalidObjectTypeForSessionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidObjectTypeForSessionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
