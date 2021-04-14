using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal enum PartParseOptionInternal
	{
		Parse,
		ParseSkipHeaders,
		ParseRawOuterContent,
		Skip
	}
}
