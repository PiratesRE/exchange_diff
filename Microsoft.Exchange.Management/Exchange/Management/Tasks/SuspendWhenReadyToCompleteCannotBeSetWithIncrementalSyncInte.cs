using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncIntervalException : RecipientTaskException
	{
		public SuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncIntervalException() : base(Strings.ErrorSuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncInterval)
		{
		}

		public SuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncIntervalException(Exception innerException) : base(Strings.ErrorSuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncInterval, innerException)
		{
		}

		protected SuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncIntervalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
