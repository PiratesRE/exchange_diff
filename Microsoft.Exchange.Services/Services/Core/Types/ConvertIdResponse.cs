using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ConvertIdResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ConvertIdResponse : BaseResponseMessage
	{
		public ConvertIdResponse() : base(ResponseType.ConvertIdResponseMessage)
		{
		}

		internal void AddResponses(ServiceResult<AlternateIdBase>[] serviceResults)
		{
			ServiceResult<AlternateIdBase>.ProcessServiceResults(serviceResults, delegate(ServiceResult<AlternateIdBase> result)
			{
				base.AddResponse(new ConvertIdResponseMessage(result.Code, result.Error, result.Value));
			});
		}
	}
}
