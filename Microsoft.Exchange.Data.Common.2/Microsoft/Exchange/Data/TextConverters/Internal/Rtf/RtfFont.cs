using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal struct RtfFont
	{
		public RtfFont(string fname)
		{
			this.Name = fname;
			this.Value = default(PropertyValue);
		}

		public string Name;

		public PropertyValue Value;
	}
}
