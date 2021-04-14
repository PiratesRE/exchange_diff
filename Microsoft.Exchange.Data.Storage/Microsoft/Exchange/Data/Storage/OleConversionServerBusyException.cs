using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class OleConversionServerBusyException : StorageTransientException
	{
		public OleConversionServerBusyException(LocalizedString message) : base(message)
		{
		}

		public OleConversionServerBusyException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected OleConversionServerBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
