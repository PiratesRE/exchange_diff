using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoAccountNamespaceException : FederationException
	{
		public NoAccountNamespaceException() : base(Strings.ErrorNoAccountNamespace)
		{
		}

		public NoAccountNamespaceException(Exception innerException) : base(Strings.ErrorNoAccountNamespace, innerException)
		{
		}

		protected NoAccountNamespaceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
