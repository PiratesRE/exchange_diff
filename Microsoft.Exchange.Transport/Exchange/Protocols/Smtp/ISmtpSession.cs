using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpSession
	{
		string HelloDomain { get; }
	}
}
