using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NullDocumentProcessingContextException : OperationFailedException
	{
		public NullDocumentProcessingContextException() : base(Strings.NullDocumentProcessingContext)
		{
		}

		public NullDocumentProcessingContextException(Exception innerException) : base(Strings.NullDocumentProcessingContext, innerException)
		{
		}

		protected NullDocumentProcessingContextException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
