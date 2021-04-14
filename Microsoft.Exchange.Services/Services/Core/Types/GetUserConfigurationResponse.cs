using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetUserConfigurationResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUserConfigurationResponse : BaseResponseMessage
	{
		public GetUserConfigurationResponse() : base(ResponseType.GetUserConfigurationResponseMessage)
		{
		}

		internal ResponseMessage CreateResponseMessage(ServiceResultCode code, ServiceError error, ServiceUserConfiguration value)
		{
			return new GetUserConfigurationResponseMessage(code, error, value);
		}

		internal void ProcessServiceResult(ServiceResult<ServiceUserConfiguration> result)
		{
			base.AddResponse(this.CreateResponseMessage(result.Code, result.Error, result.Value));
		}
	}
}
