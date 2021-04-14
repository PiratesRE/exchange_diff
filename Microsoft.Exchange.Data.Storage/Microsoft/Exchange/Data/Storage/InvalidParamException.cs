using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidParamException : StoragePermanentException
	{
		public InvalidParamException(LocalizedString message) : base(message)
		{
		}

		public InvalidParamException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidParamException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
