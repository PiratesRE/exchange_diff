using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BitlockerCertificatesNotFoundException : BitlockerUtilException
	{
		public BitlockerCertificatesNotFoundException() : base(DiskManagementStrings.BitlockerCertificatesNotFoundError)
		{
		}

		public BitlockerCertificatesNotFoundException(Exception innerException) : base(DiskManagementStrings.BitlockerCertificatesNotFoundError, innerException)
		{
		}

		protected BitlockerCertificatesNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
