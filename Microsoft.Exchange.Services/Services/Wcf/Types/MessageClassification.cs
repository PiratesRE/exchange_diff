using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MessageClassification
	{
		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public Guid Guid { get; set; }

		public override string ToString()
		{
			return string.Format("Guid = {0}, DisplayName = {1}", this.Guid, this.DisplayName);
		}
	}
}
