using System;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal enum PartContentParseOption
	{
		Parse,
		ParseRawContent,
		ParseEmbeddedMessage,
		Skip
	}
}
