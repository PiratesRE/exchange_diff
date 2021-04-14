using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidADRecipientTypeException : OperationFailedException
	{
		public InvalidADRecipientTypeException() : base(Strings.InvalidAdRecipient)
		{
		}

		public InvalidADRecipientTypeException(Exception innerException) : base(Strings.InvalidAdRecipient, innerException)
		{
		}

		protected InvalidADRecipientTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
