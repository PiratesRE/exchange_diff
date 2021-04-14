using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Logging
{
	internal class ProtocolLogRowFormatter : LogRowFormatter
	{
		private static string[] InitializeEventString()
		{
			return new string[]
			{
				"+",
				"-",
				">",
				"<",
				"*"
			};
		}

		public ProtocolLogRowFormatter(LogSchema schema) : base(schema)
		{
		}

		public override bool ShouldConvertEncoding
		{
			get
			{
				return false;
			}
		}

		protected override byte[] Encode(object value)
		{
			if (value is ProtocolEvent)
			{
				return base.Encode(ProtocolLogRowFormatter.EventString[(int)value]);
			}
			return base.Encode(value);
		}

		private static readonly string[] EventString = ProtocolLogRowFormatter.InitializeEventString();
	}
}
