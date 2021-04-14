using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ProvisioningDataCorruptException : ProvisioningException
	{
		public ProvisioningDataCorruptException(LocalizedString message) : base(message)
		{
		}

		public ProvisioningDataCorruptException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ProvisioningDataCorruptException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
