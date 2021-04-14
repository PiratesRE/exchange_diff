using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidCompliancePolicyExBindingException : InvalidCompliancePolicyBindingException
	{
		public InvalidCompliancePolicyExBindingException(LocalizedString message) : base(message)
		{
		}

		public InvalidCompliancePolicyExBindingException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidCompliancePolicyExBindingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
