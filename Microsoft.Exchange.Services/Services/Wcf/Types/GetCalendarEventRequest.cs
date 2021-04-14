using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCalendarEventRequest : BaseRequest
	{
		[DataMember]
		public TargetFolderId CalendarId { get; set; }

		[DataMember]
		public ItemId[] EventIds { get; set; }

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.EventIds == null || this.EventIds.Length < taskStep)
			{
				return null;
			}
			BaseServerIdInfo serverInfoForItemId = BaseRequest.GetServerInfoForItemId(callContext, this.EventIds[taskStep]);
			return BaseRequest.ServerInfoToResourceKeys(false, serverInfoForItemId);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.EventIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForItemIdList(callContext, this.EventIds);
		}
	}
}
