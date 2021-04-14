using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CertificateDoesNotHavePKCS10Exception : LocalizedException
	{
		public CertificateDoesNotHavePKCS10Exception() : base(Strings.CertificateDoesNotHavePKCS10)
		{
		}

		public CertificateDoesNotHavePKCS10Exception(Exception innerException) : base(Strings.CertificateDoesNotHavePKCS10, innerException)
		{
		}

		protected CertificateDoesNotHavePKCS10Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
