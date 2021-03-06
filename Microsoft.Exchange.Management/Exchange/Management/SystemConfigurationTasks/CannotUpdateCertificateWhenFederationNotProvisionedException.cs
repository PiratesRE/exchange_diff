using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotUpdateCertificateWhenFederationNotProvisionedException : FederationException
	{
		public CannotUpdateCertificateWhenFederationNotProvisionedException() : base(Strings.ErrorCannotUpdateCertificateWhenFederationNotProvisioned)
		{
		}

		public CannotUpdateCertificateWhenFederationNotProvisionedException(Exception innerException) : base(Strings.ErrorCannotUpdateCertificateWhenFederationNotProvisioned, innerException)
		{
		}

		protected CannotUpdateCertificateWhenFederationNotProvisionedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
