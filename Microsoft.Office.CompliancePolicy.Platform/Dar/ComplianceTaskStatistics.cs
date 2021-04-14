using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	[DataContract]
	public class ComplianceTaskStatistics
	{
		[DataMember]
		public int Completed { get; set; }

		[DataMember]
		public int Yielded { get; set; }

		[DataMember]
		public int TransientFailed { get; set; }

		[DataMember]
		public int Failed { get; set; }
	}
}
