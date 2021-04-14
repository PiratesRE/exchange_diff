using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class OleConversionFailedException : StoragePermanentException
	{
		public OleConversionFailedException(LocalizedString message) : base(message)
		{
		}

		public OleConversionFailedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected OleConversionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
