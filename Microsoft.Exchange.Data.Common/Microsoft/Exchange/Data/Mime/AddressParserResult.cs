using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal enum AddressParserResult
	{
		Mailbox,
		GroupStart,
		GroupInProgress,
		End
	}
}
