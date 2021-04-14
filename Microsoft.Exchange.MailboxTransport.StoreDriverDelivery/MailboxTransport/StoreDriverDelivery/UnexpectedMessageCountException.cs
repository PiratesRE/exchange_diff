using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	[Serializable]
	internal class UnexpectedMessageCountException : Exception
	{
		public UnexpectedMessageCountException(string message) : base(message)
		{
		}
	}
}
