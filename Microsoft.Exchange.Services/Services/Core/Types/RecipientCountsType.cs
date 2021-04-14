using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "RecipientCountsType")]
	[Serializable]
	public class RecipientCountsType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public int ToRecipientsCount { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public int CcRecipientsCount { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public int BccRecipientsCount { get; set; }
	}
}
