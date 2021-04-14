using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MessageCategory
	{
		[DataMember]
		public int Color { get; set; }

		[DataMember]
		public string Name { get; set; }

		public override string ToString()
		{
			return string.Format("Color = {0}, Name = {1}", this.Color, this.Name);
		}
	}
}
