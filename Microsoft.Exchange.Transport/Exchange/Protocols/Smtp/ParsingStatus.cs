using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum ParsingStatus
	{
		ProtocolError,
		Error,
		MoreDataRequired,
		Complete,
		IgnorableProtocolError
	}
}
