using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMCallDataRecordsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUMCallDataRecordsRequest : BaseRequest
	{
		[DataMember(Name = "StartDateTime")]
		[XmlElement("StartDateTime")]
		public DateTime StartDateTime { get; set; }

		[XmlElement("EndDateTime")]
		[DataMember(Name = "EndDateTime")]
		public DateTime EndDateTime { get; set; }

		[DataMember(Name = "Offset")]
		[XmlElement("Offset")]
		public int Offset { get; set; }

		[DataMember(Name = "NumberOfRecords")]
		[XmlElement("NumberOfRecords")]
		public int NumberOfRecords { get; set; }

		[DataMember(Name = "UserLegacyExchangeDN")]
		[XmlElement("UserLegacyExchangeDN")]
		public string UserLegacyExchangeDN { get; set; }

		[XmlElement("FilterBy")]
		[DataMember(Name = "FilterBy")]
		public UMCDRFilterByType FilterBy { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUMCallDataRecords(callContext, this);
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
