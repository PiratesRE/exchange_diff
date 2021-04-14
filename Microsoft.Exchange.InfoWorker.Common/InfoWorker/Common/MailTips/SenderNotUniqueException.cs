using System;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public class SenderNotUniqueException : Exception
	{
		public SenderNotUniqueException(string message) : base(message)
		{
		}
	}
}
