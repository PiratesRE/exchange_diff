using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Transport
{
	[Serializable]
	internal sealed class SubmissionQueueBlockedException : ApplicationException
	{
		public SubmissionQueueBlockedException() : base("The Process Manager indicated that submission queue is blocked.")
		{
		}

		public SubmissionQueueBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private const string SubmissionQueueBlockedMessage = "The Process Manager indicated that submission queue is blocked.";
	}
}
