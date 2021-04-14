using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCalendarViewRequest : BaseRequest
	{
		public TargetFolderId CalendarId { get; set; }

		public ExDateTime StartRange { get; set; }

		public ExDateTime EndRange { get; set; }

		public bool? ReturnMasterItems { get; set; }

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.CalendarId == null)
			{
				return null;
			}
			BaseServerIdInfo serverInfoForFolderId = BaseRequest.GetServerInfoForFolderId(callContext, this.CalendarId.BaseFolderId);
			return BaseRequest.ServerInfoToResourceKeys(false, serverInfoForFolderId);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.CalendarId == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderId(callContext, this.CalendarId.BaseFolderId);
		}
	}
}
