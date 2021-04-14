using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "UnseenData")]
	[Serializable]
	public class UnseenDataType
	{
		public UnseenDataType()
		{
		}

		public UnseenDataType(int unseenCount, string lastVisitedTimeString)
		{
			this.UnseenCount = unseenCount;
			this.LastVisitedTime = lastVisitedTimeString;
		}

		[DataMember(Name = "UnseenCount", IsRequired = true)]
		public int UnseenCount { get; set; }

		[DateTimeString]
		[DataMember(IsRequired = true)]
		public string LastVisitedTime { get; set; }
	}
}
