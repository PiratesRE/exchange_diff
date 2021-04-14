using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class WriteOptions
	{
		[DataMember]
		public bool IsPerformTestUpdate { get; set; }

		[DataMember]
		public double PercentageOfNodesToSucceed { get; set; }

		[DataMember]
		public string[] WaitForNodes { get; set; }

		public bool IsWaitRequired()
		{
			return this.PercentageOfNodesToSucceed > 0.0 || (this.WaitForNodes != null && this.WaitForNodes.Length > 0);
		}
	}
}
