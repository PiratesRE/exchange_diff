using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class NdrItemToTransportItemCopyException : LocalizedException
	{
		public NdrItemToTransportItemCopyException(Exception innerException) : base(LocalizedString.Empty, innerException)
		{
		}

		protected NdrItemToTransportItemCopyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
