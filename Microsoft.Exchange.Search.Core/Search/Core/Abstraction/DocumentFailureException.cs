using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class DocumentFailureException : OperationFailedException
	{
		public DocumentFailureException() : base(Strings.DocumentFailure)
		{
		}

		public DocumentFailureException(Exception innerException) : base(Strings.DocumentFailure, innerException)
		{
		}

		public DocumentFailureException(LocalizedString message) : base(message)
		{
		}

		public DocumentFailureException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DocumentFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
