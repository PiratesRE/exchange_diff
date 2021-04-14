using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class HygieneIPRange
	{
		public byte IPA { get; set; }

		public byte IPB { get; set; }

		public byte IPC { get; set; }

		public byte IPD { get; set; }

		public byte? CIDR { get; set; }
	}
}
