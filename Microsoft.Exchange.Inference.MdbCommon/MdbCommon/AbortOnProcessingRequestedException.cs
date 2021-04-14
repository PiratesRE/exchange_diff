using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AbortOnProcessingRequestedException : OperationFailedException
	{
		public AbortOnProcessingRequestedException() : base(Strings.AbortOnProcessingRequested)
		{
		}

		public AbortOnProcessingRequestedException(Exception innerException) : base(Strings.AbortOnProcessingRequested, innerException)
		{
		}

		protected AbortOnProcessingRequestedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
