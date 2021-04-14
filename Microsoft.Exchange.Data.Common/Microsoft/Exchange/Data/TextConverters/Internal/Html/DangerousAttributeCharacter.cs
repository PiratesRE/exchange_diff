using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal enum DangerousAttributeCharacter : byte
	{
		None,
		Backquote,
		Backslash
	}
}
