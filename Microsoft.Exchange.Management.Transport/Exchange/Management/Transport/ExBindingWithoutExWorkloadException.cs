using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ExBindingWithoutExWorkloadException : InvalidCompliancePolicyExBindingException
	{
		public ExBindingWithoutExWorkloadException() : base(Strings.ErrorExBindingWithoutExWorkload)
		{
		}

		public ExBindingWithoutExWorkloadException(Exception innerException) : base(Strings.ErrorExBindingWithoutExWorkload, innerException)
		{
		}

		protected ExBindingWithoutExWorkloadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
