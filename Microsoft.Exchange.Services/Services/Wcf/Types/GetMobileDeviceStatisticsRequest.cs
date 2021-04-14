using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMobileDeviceStatisticsRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public MobileDeviceStatisticsOptions Options { get; set; }

		public override string ToString()
		{
			return string.Format("GetMobileDeviceStatisticsRequest: {0}", this.Options);
		}
	}
}
