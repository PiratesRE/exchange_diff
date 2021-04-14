using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSpecifyAccountNamespaceWithoutTrustException : FederationException
	{
		public CannotSpecifyAccountNamespaceWithoutTrustException() : base(Strings.ErrorCannotSpecifyAccountNamespaceWithoutTrust)
		{
		}

		public CannotSpecifyAccountNamespaceWithoutTrustException(Exception innerException) : base(Strings.ErrorCannotSpecifyAccountNamespaceWithoutTrust, innerException)
		{
		}

		protected CannotSpecifyAccountNamespaceWithoutTrustException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
