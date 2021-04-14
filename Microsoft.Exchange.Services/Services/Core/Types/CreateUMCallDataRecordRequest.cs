using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CreateUMCallDataRecordType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateUMCallDataRecordRequest : BaseRequest
	{
		[XmlElement("CDRData")]
		[DataMember(Name = "CDRData")]
		public CDRData CDRData { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CreateUMCallDataRecord(callContext, this);
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
