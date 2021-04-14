using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal interface IMessageConverter
	{
		string Description { get; }

		bool IsOutbound { get; }

		Trace Tracer { get; }

		void LogMessage(Exception exception);
	}
}
