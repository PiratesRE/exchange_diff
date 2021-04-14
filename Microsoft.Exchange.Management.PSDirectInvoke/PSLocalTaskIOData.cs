using System;
using System.Globalization;

namespace Microsoft.Exchange.Management.PSDirectInvoke
{
	internal sealed class PSLocalTaskIOData
	{
		public PSLocalTaskIOData(PSLocalTaskIOType type, DateTime when, string data)
		{
			this.Type = type;
			this.When = when;
			this.Data = data;
		}

		public PSLocalTaskIOType Type { get; private set; }

		public DateTime When { get; private set; }

		public string Data { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.Type,
				":",
				this.When.ToLocalTime().ToString("yyyy/MM/dd:HH:mm:ss.fff", CultureInfo.InvariantCulture),
				" ",
				this.Data
			});
		}
	}
}
