using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class TimeZoneEntry
	{
		public TimeZoneEntry(string value, string name)
		{
			this.Value = value;
			this.Name = name;
		}

		public TimeZoneEntry(ExTimeZone timezone) : this(timezone.Id, timezone.LocalizableDisplayName.ToString(CultureInfo.CurrentUICulture))
		{
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Value { get; set; }
	}
}
