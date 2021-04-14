using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class StoreDriverAgentTransientException : TransientException
	{
		public StoreDriverAgentTransientException(LocalizedString message) : base(message)
		{
		}

		public StoreDriverAgentTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
