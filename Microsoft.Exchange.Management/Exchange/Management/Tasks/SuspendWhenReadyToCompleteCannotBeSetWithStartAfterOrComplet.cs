using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendWhenReadyToCompleteCannotBeSetWithStartAfterOrCompleteAfterException : RecipientTaskException
	{
		public SuspendWhenReadyToCompleteCannotBeSetWithStartAfterOrCompleteAfterException() : base(Strings.ErrorSuspendWhenReadyToCompleteCannotBeSetWithStartAfterOrCompleteAfter)
		{
		}

		public SuspendWhenReadyToCompleteCannotBeSetWithStartAfterOrCompleteAfterException(Exception innerException) : base(Strings.ErrorSuspendWhenReadyToCompleteCannotBeSetWithStartAfterOrCompleteAfter, innerException)
		{
		}

		protected SuspendWhenReadyToCompleteCannotBeSetWithStartAfterOrCompleteAfterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
