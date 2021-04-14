using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidDocumentInTrainingSetException : OperationFailedException
	{
		public InvalidDocumentInTrainingSetException() : base(Strings.InvalidDocumentInTrainingSet)
		{
		}

		public InvalidDocumentInTrainingSetException(Exception innerException) : base(Strings.InvalidDocumentInTrainingSet, innerException)
		{
		}

		protected InvalidDocumentInTrainingSetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
