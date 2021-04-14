using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetServerTimeZonesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetServerTimeZonesRequest : BaseRequest
	{
		[XmlAttribute("ReturnFullTimeZoneData")]
		[DataMember(Name = "ReturnFullTimeZoneData", IsRequired = false, Order = 1)]
		public bool ReturnFullTimeZoneData
		{
			get
			{
				return this.returnFullTimeZoneData;
			}
			set
			{
				this.returnFullTimeZoneData = value;
			}
		}

		[XmlArray(ElementName = "Ids", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "Ids", IsRequired = false, Order = 2)]
		[XmlArrayItem("Id", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetServerTimeZones(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		private bool returnFullTimeZoneData = true;

		private string[] id;
	}
}
