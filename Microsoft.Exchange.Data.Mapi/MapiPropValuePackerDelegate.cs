using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal delegate PropValue MapiPropValuePackerDelegate(object value, MapiPropertyDefinition propertyDefinition);
}
