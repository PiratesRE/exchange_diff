using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DateRange
	{
		[DataMember]
		public Identity BeforeDate { get; set; }

		[DataMember]
		public Identity AfterDate { get; set; }
	}
}
