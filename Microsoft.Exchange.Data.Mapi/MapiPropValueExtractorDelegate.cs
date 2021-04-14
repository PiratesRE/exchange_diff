using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal delegate object MapiPropValueExtractorDelegate(PropValue value, MapiPropertyDefinition propertyDefinition);
}
