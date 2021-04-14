using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class PoisonHandlerNdrGenerationErrorException : LocalizedException
	{
		public PoisonHandlerNdrGenerationErrorException(Exception innerException) : base(LocalizedString.Empty, innerException)
		{
		}

		protected PoisonHandlerNdrGenerationErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
