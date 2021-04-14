using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class StartAfterOrCompleteAfterCannotBeSetOnLegacyRequestsException : RecipientTaskException
	{
		public StartAfterOrCompleteAfterCannotBeSetOnLegacyRequestsException() : base(Strings.StartAfterOrCompleteAfterCannotBeSetOnLegacyRequests)
		{
		}

		public StartAfterOrCompleteAfterCannotBeSetOnLegacyRequestsException(Exception innerException) : base(Strings.StartAfterOrCompleteAfterCannotBeSetOnLegacyRequests, innerException)
		{
		}

		protected StartAfterOrCompleteAfterCannotBeSetOnLegacyRequestsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
