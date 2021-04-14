using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public enum ParsingStatus
	{
		ProtocolError,
		Error,
		MoreDataRequired,
		Complete
	}
}
