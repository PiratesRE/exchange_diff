using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NestedDocumentCountZeroException : OperationFailedException
	{
		public NestedDocumentCountZeroException() : base(Strings.NestedDocumentCountZero)
		{
		}

		public NestedDocumentCountZeroException(Exception innerException) : base(Strings.NestedDocumentCountZero, innerException)
		{
		}

		protected NestedDocumentCountZeroException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
