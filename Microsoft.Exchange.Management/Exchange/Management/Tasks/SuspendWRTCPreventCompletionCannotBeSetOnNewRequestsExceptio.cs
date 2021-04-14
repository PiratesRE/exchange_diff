using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendWRTCPreventCompletionCannotBeSetOnNewRequestsException : RecipientTaskException
	{
		public SuspendWRTCPreventCompletionCannotBeSetOnNewRequestsException() : base(Strings.SuspendWRTCPreventCompletionCannotBeSetOnNewRequests)
		{
		}

		public SuspendWRTCPreventCompletionCannotBeSetOnNewRequestsException(Exception innerException) : base(Strings.SuspendWRTCPreventCompletionCannotBeSetOnNewRequests, innerException)
		{
		}

		protected SuspendWRTCPreventCompletionCannotBeSetOnNewRequestsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
