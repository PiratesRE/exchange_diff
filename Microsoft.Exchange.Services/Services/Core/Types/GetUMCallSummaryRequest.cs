using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMCallSummaryType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUMCallSummaryRequest : BaseRequest
	{
		[XmlElement("DailPlanGuid")]
		[DataMember(Name = "DailPlanGuid", IsRequired = true)]
		public Guid DailPlanGuid { get; set; }

		[XmlElement("GatewayGuid")]
		[DataMember(Name = "GatewayGuid", IsRequired = true)]
		public Guid GatewayGuid { get; set; }

		[XmlElement("GroupRecordsBy")]
		[DataMember(Name = "GroupRecordsBy", IsRequired = true)]
		public UMCDRGroupByType GroupRecordsBy { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUMCallSummary(callContext, this);
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
