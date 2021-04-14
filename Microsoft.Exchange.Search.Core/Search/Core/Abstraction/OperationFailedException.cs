using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class OperationFailedException : ComponentException
	{
		public OperationFailedException() : base(Strings.OperationFailure)
		{
		}

		public OperationFailedException(Exception innerException) : base(Strings.OperationFailure, innerException)
		{
		}

		public OperationFailedException(LocalizedString message) : base(message)
		{
		}

		public OperationFailedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected OperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
