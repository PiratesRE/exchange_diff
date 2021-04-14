using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetPeopleICommunicateWithResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "GetPeopleICommunicateWithResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetPeopleICommunicateWithResponse : BaseResponseMessage
	{
		public GetPeopleICommunicateWithResponse() : base(ResponseType.GetPeopleICommunicateWithResponseMessage)
		{
		}
	}
}
