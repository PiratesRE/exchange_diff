using System;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public class SenderNotFoundException : Exception
	{
		public SenderNotFoundException(string message) : base(message)
		{
		}
	}
}
