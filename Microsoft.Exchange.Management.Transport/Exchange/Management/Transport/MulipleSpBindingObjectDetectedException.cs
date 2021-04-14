using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MulipleSpBindingObjectDetectedException : InvalidCompliancePolicySpBindingException
	{
		public MulipleSpBindingObjectDetectedException() : base(Strings.MulipleSpBindingObjectDetected)
		{
		}

		public MulipleSpBindingObjectDetectedException(Exception innerException) : base(Strings.MulipleSpBindingObjectDetected, innerException)
		{
		}

		protected MulipleSpBindingObjectDetectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
