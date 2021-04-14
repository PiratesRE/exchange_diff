using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal delegate PropertyValue PropertyValueParsingMethod(BufferString value, FormatConverter formatConverter);
}
