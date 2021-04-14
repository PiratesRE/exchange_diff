using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal enum PartContentParseOptionInternal
	{
		Parse,
		ParseRawContent,
		ParseEmbeddedMessage,
		Skip
	}
}
