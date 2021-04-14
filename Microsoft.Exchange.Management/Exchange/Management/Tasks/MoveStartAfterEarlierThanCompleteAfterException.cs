using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoveStartAfterEarlierThanCompleteAfterException : RecipientTaskException
	{
		public MoveStartAfterEarlierThanCompleteAfterException() : base(Strings.ErrorStartAfterEarlierThanCompleteAfter)
		{
		}

		public MoveStartAfterEarlierThanCompleteAfterException(Exception innerException) : base(Strings.ErrorStartAfterEarlierThanCompleteAfter, innerException)
		{
		}

		protected MoveStartAfterEarlierThanCompleteAfterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
