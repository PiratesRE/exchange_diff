using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoNextCertificateException : FederationException
	{
		public NoNextCertificateException() : base(Strings.ErrorNoNextCertificate)
		{
		}

		public NoNextCertificateException(Exception innerException) : base(Strings.ErrorNoNextCertificate, innerException)
		{
		}

		protected NoNextCertificateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
