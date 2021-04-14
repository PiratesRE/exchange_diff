using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("SyncFolderHierarchyResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SyncFolderHierarchyResponse : BaseInfoResponse
	{
		public SyncFolderHierarchyResponse() : base(ResponseType.SyncFolderHierarchyResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new SyncFolderHierarchyResponseMessage(code, error, value as SyncFolderHierarchyChangesType);
		}

		internal override void ProcessServiceResult<TValue>(ServiceResult<TValue> result)
		{
			base.AddResponse(this.CreateResponseMessage<TValue>(result.Code, result.Error, result.Value));
		}
	}
}
