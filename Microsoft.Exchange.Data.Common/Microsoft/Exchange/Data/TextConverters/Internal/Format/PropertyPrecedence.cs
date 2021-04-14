using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal enum PropertyPrecedence : byte
	{
		InlineStyle,
		StyleBase,
		NonStyle = 9,
		TagDefault,
		Inherited
	}
}
