using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TranslatableProvisioningException : ProvisioningException
	{
		public TranslatableProvisioningException(LocalizedString message) : base(message)
		{
		}

		public TranslatableProvisioningException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TranslatableProvisioningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
