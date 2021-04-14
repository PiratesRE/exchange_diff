using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class BaseJsonResponse
	{
		public BaseJsonResponse()
		{
			this.Header = new JsonResponseHeaders();
		}

		[DataMember(Name = "Header")]
		public JsonResponseHeaders Header;
	}
}
