using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "AttendeeCountsType")]
	[Serializable]
	public class AttendeeCountsType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public int RequiredAttendeesCount { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public int OptionalAttendeesCount { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public int ResourcesCount { get; set; }
	}
}
