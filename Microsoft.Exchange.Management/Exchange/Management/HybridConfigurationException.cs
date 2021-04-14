using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class HybridConfigurationException : LocalizedException
	{
		public HybridConfigurationException(LocalizedString message) : base(message)
		{
		}

		public HybridConfigurationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected HybridConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
