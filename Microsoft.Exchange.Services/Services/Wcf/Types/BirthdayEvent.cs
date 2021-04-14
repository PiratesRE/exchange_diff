using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class BirthdayEvent
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ItemId ContactId { get; set; }

		[DataMember]
		public ItemId PersonId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string Birthday { get; set; }

		[DataMember]
		public string Attribution { get; set; }

		[DataMember]
		public bool IsWritable { get; set; }
	}
}
