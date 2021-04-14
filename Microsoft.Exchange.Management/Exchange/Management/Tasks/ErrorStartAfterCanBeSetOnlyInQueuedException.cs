using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorStartAfterCanBeSetOnlyInQueuedException : RecipientTaskException
	{
		public ErrorStartAfterCanBeSetOnlyInQueuedException() : base(Strings.ErrorStartAfterCanBeSetOnlyInQueued)
		{
		}

		public ErrorStartAfterCanBeSetOnlyInQueuedException(Exception innerException) : base(Strings.ErrorStartAfterCanBeSetOnlyInQueued, innerException)
		{
		}

		protected ErrorStartAfterCanBeSetOnlyInQueuedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
