using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateCalendarEventRequest : BaseRequest
	{
		[DataMember]
		public TargetFolderId CalendarId { get; set; }

		public Event[] Events { get; set; }

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			BaseServerIdInfo baseServerIdInfo = (this.CalendarId == null || this.CalendarId.BaseFolderId == null) ? null : BaseRequest.GetServerInfoForFolderId(callContext, this.CalendarId.BaseFolderId);
			return BaseRequest.ServerInfosToResourceKeys(true, new BaseServerIdInfo[]
			{
				baseServerIdInfo
			});
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.CalendarId != null)
			{
				return BaseRequest.GetServerInfoForFolderId(callContext, this.CalendarId.BaseFolderId);
			}
			return null;
		}
	}
}
