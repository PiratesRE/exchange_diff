using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	[Serializable]
	internal class MessageDepotPermanentException : DataSourceOperationException
	{
		public MessageDepotPermanentException(LocalizedString errorMessage, Exception innerException = null) : base(errorMessage, innerException)
		{
		}

		protected MessageDepotPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
