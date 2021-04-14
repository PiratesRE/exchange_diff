using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Clients.Security
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LiveExternalException : LocalizedException
	{
		public LiveExternalException(LocalizedString message) : base(message)
		{
		}

		public LiveExternalException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected LiveExternalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
