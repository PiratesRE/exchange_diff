using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class ItemId
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string ChangeKey { get; set; }
	}
}
