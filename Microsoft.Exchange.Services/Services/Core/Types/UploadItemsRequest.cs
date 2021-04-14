using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UploadItemsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UploadItemsRequest : BaseRequest
	{
		[XmlElement("Items", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public XmlNodeArray Items
		{
			get
			{
				return this.items;
			}
			set
			{
				this.items = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UploadItems(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.items == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForIdList(callContext, this.items.Nodes);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			BaseServerIdInfo serverInfoForSingleId = BaseRequest.GetServerInfoForSingleId(callContext, this.items.Nodes[taskStep]);
			return BaseRequest.ServerInfoToResourceKeys(true, serverInfoForSingleId);
		}

		private XmlNodeArray items;
	}
}
