using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("StartFindInGALSpeechRecognitionRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class StartFindInGALSpeechRecognitionRequest : BaseRequest
	{
		[XmlElement("Culture")]
		[DataMember(Name = "Culture", IsRequired = true)]
		public string Culture { get; set; }

		[XmlElement("TimeZone")]
		[DataMember(Name = "TimeZone", IsRequired = true)]
		public string TimeZone { get; set; }

		[DataMember(Name = "UserObjectGuid", IsRequired = true)]
		[XmlElement("UserObjectGuid")]
		public Guid UserObjectGuid { get; set; }

		[XmlElement("TenantGuid")]
		[DataMember(Name = "TenantGuid", IsRequired = true)]
		public Guid TenantGuid { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new StartFindInGALSpeechRecognitionCommand(callContext, this);
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
