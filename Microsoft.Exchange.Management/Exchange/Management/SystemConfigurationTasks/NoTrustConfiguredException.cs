using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoTrustConfiguredException : FederationException
	{
		public NoTrustConfiguredException() : base(Strings.ErrorNoTrustConfigured)
		{
		}

		public NoTrustConfiguredException(Exception innerException) : base(Strings.ErrorNoTrustConfigured, innerException)
		{
		}

		protected NoTrustConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
