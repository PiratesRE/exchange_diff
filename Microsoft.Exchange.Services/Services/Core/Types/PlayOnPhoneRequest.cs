using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("PlayOnPhoneRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class PlayOnPhoneRequest : BaseRequest
	{
		[XmlElement("ItemId")]
		[DataMember(Name = "ItemId", IsRequired = true, Order = 1)]
		public ItemId ItemId { get; set; }

		[XmlElement("DialString")]
		[DataMember(Name = "DialString", IsRequired = true, Order = 2)]
		public string DialString { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new PlayOnPhone(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
