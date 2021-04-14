using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class WrongObjectTypeException : StoragePermanentException
	{
		public WrongObjectTypeException(LocalizedString message) : base(message)
		{
		}

		public WrongObjectTypeException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WrongObjectTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
