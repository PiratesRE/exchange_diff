using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("MarkAsJunkType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MarkAsJunkRequest : BaseRequest
	{
		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "ItemIds", IsRequired = true, Order = 1)]
		[XmlArray("ItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ItemId[] ItemIds { get; set; }

		[XmlAttribute("IsJunk", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "IsJunk", IsRequired = true, Order = 2)]
		public bool IsJunk { get; set; }

		[XmlAttribute("MoveItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "MoveItem", IsRequired = true, Order = 3)]
		public bool MoveItem { get; set; }

		[DataMember(Name = "SendCopy", IsRequired = false, Order = 4)]
		[XmlIgnore]
		public bool SendCopy { get; set; }

		[DataMember(Name = "JunkMessageHeader", IsRequired = false, Order = 5)]
		[XmlIgnore]
		public string JunkMessageHeader { get; set; }

		[DataMember(Name = "JunkMessageBody", IsRequired = false, Order = 6)]
		[XmlIgnore]
		public string JunkMessageBody { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new MarkAsJunk(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.ItemIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForItemIdList(callContext, this.ItemIds);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.ItemIds == null || this.ItemIds.Length < taskStep)
			{
				return null;
			}
			BaseServerIdInfo serverInfoForItemId = BaseRequest.GetServerInfoForItemId(callContext, this.ItemIds[taskStep]);
			return BaseRequest.ServerInfoToResourceKeys(true, serverInfoForItemId);
		}
	}
}
