using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetTimeZoneOffsetsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetTimeZoneOffsetsRequest : BaseRequest
	{
		[XmlElement("StartTime", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(IsRequired = true, Order = 1)]
		public string StartTime { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		[XmlElement("EndTime", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string EndTime { get; set; }

		[XmlElement("TimeZoneId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
		public string TimeZoneId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetTimeZoneOffsets(callContext, this);
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
