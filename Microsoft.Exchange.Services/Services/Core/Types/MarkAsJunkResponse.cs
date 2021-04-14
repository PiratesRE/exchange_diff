using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("MarkAsJunkResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class MarkAsJunkResponse : BaseResponseMessage
	{
		public MarkAsJunkResponse() : base(ResponseType.MarkAsJunkResponseMessage)
		{
		}

		internal void AddResponses(ServiceResult<ItemId>[] results)
		{
			ServiceResult<ItemId>.ProcessServiceResults(results, delegate(ServiceResult<ItemId> result)
			{
				base.AddResponse(new MarkAsJunkResponseMessage(result.Code, result.Error, result.Value));
			});
		}
	}
}
