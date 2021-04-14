using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DomainProofOwnershipException : FederationException
	{
		public DomainProofOwnershipException(LocalizedString message) : base(message)
		{
		}

		public DomainProofOwnershipException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DomainProofOwnershipException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
