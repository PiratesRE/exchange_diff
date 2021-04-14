using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data
{
	internal sealed class ByteQuantifiedSizeAppSettingsEntry : AppSettingsEntry<ByteQuantifiedSize>
	{
		public ByteQuantifiedSizeAppSettingsEntry(string name, ByteQuantifiedSize defaultValue, Trace tracer) : base(name, defaultValue, tracer)
		{
		}

		protected override bool TryParseValue(string inputValue, out ByteQuantifiedSize outputValue)
		{
			return ByteQuantifiedSize.TryParse(inputValue, out outputValue);
		}
	}
}
