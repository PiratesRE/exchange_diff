using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "GetUserPhotoResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetUserPhotoResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUserPhotoResponse : BaseResponseMessage
	{
		public GetUserPhotoResponse() : base(ResponseType.GetUserPhotoResponseMessage)
		{
		}
	}
}
