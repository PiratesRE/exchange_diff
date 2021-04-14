using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RefreshMetadataOptionNotAllowedException : FederationException
	{
		public RefreshMetadataOptionNotAllowedException() : base(Strings.ErrorRefreshMetadataOptionNotAllowed)
		{
		}

		public RefreshMetadataOptionNotAllowedException(Exception innerException) : base(Strings.ErrorRefreshMetadataOptionNotAllowed, innerException)
		{
		}

		protected RefreshMetadataOptionNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
