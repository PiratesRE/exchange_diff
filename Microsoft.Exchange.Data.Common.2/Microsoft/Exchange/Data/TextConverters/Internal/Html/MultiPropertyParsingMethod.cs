using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal delegate void MultiPropertyParsingMethod(BufferString value, FormatConverter formatConverter, PropertyId basePropertyId, Property[] outputProperties, out int parsedPropertiesCount);
}
