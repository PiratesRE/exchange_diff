using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncrementalSyncIntervalCannotBeSetOnNonIncrementalRequestsException : RecipientTaskException
	{
		public IncrementalSyncIntervalCannotBeSetOnNonIncrementalRequestsException() : base(Strings.ErrorIncrementalSyncIntervalCannotBeSetOnNonIncrementalRequests)
		{
		}

		public IncrementalSyncIntervalCannotBeSetOnNonIncrementalRequestsException(Exception innerException) : base(Strings.ErrorIncrementalSyncIntervalCannotBeSetOnNonIncrementalRequests, innerException)
		{
		}

		protected IncrementalSyncIntervalCannotBeSetOnNonIncrementalRequestsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
