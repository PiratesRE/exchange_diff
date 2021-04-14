using System;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal enum PartParseOption
	{
		Parse,
		ParseSkipHeaders,
		ParseRawOuterContent,
		Skip
	}
}
