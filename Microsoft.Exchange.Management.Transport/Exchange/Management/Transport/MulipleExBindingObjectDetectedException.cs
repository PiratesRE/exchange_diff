using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MulipleExBindingObjectDetectedException : InvalidCompliancePolicyExBindingException
	{
		public MulipleExBindingObjectDetectedException() : base(Strings.MulipleExBindingObjectDetected)
		{
		}

		public MulipleExBindingObjectDetectedException(Exception innerException) : base(Strings.MulipleExBindingObjectDetected, innerException)
		{
		}

		protected MulipleExBindingObjectDetectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
