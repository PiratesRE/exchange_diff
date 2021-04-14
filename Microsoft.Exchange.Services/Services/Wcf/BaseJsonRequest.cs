using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class BaseJsonRequest
	{
		[DataMember(Name = "Header", IsRequired = true)]
		public JsonRequestHeaders Header { get; set; }
	}
}
