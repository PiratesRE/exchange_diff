using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WeatherDailyConditions
	{
		[DataMember]
		public int High { get; set; }

		[DataMember]
		public int Low { get; set; }

		[DataMember]
		public int SkyCode { get; set; }

		[DataMember]
		public string SkyText { get; set; }

		[DataMember]
		public string PrecipitationChance { get; set; }
	}
}
