using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class NonNdrItemToTransportItemCopyException : LocalizedException
	{
		public NonNdrItemToTransportItemCopyException(Exception innerException) : base(LocalizedString.Empty, innerException)
		{
		}

		protected NonNdrItemToTransportItemCopyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
