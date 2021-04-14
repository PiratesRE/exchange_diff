using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class TimeZoneConfiguration
	{
		[DataMember(IsRequired = false)]
		public string CurrentTimeZone { get; set; }

		[DataMember]
		public TimeZoneEntry[] TimeZoneList { get; set; }
	}
}
