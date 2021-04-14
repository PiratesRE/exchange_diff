using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ApplyConversationActionResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ApplyConversationActionResponse : BaseResponseMessage
	{
		public ApplyConversationActionResponse() : base(ResponseType.ApplyConversationActionResponseMessage)
		{
		}

		internal void AddResponses(ServiceResult<ApplyConversationActionResponseMessage>[] results)
		{
			ServiceResult<ApplyConversationActionResponseMessage>.ProcessServiceResults(results, delegate(ServiceResult<ApplyConversationActionResponseMessage> result)
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.ExchangeV2_1))
				{
					if (result.Value != null)
					{
						base.AddResponse(result.Value);
						return;
					}
					base.AddResponse(new ApplyConversationActionResponseMessage(result.Code, result.Error));
					return;
				}
				else
				{
					if (result.Value != null)
					{
						base.AddResponse(result.Value);
						return;
					}
					base.AddResponse(new ApplyConversationActionResponseMessage());
					return;
				}
			});
		}
	}
}
