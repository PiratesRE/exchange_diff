using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidPropertyTypeException : StoragePermanentException
	{
		public InvalidPropertyTypeException(LocalizedString message) : base(message)
		{
		}

		public InvalidPropertyTypeException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidPropertyTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
