using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMobileDeviceStatisticsResponse : OptionsResponseBase
	{
		public GetMobileDeviceStatisticsResponse()
		{
			this.MobileDeviceStatisticsCollection = new MobileDeviceStatisticsCollection();
		}

		[DataMember(IsRequired = true)]
		public MobileDeviceStatisticsCollection MobileDeviceStatisticsCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetMobileDeviceStatisticsResponse: {0}", this.MobileDeviceStatisticsCollection);
		}
	}
}
