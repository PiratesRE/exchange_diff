using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Clients.Security
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LiveConfigurationException : LocalizedException
	{
		public LiveConfigurationException(LocalizedString message) : base(message)
		{
		}

		public LiveConfigurationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected LiveConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
