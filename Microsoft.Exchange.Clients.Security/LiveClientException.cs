using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Clients.Security
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LiveClientException : LocalizedException
	{
		public LiveClientException(LocalizedString message) : base(message)
		{
		}

		public LiveClientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected LiveClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
