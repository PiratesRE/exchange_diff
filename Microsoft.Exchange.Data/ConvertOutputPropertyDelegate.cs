using System;

namespace Microsoft.Exchange.Data
{
	internal delegate bool ConvertOutputPropertyDelegate(ConvertOutputPropertyEventArgs args, out object convertedValue);
}
