using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SpBindingWithoutSpWorkloadException : InvalidCompliancePolicySpBindingException
	{
		public SpBindingWithoutSpWorkloadException() : base(Strings.ErrorSpBindingWithoutSpWorkload)
		{
		}

		public SpBindingWithoutSpWorkloadException(Exception innerException) : base(Strings.ErrorSpBindingWithoutSpWorkload, innerException)
		{
		}

		protected SpBindingWithoutSpWorkloadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
