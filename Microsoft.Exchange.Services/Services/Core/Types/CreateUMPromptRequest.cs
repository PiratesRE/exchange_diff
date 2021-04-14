using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CreateUMPromptRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateUMPromptRequest : BaseRequest
	{
		[XmlElement("ConfigurationObject")]
		[DataMember(Name = "ConfigurationObject", IsRequired = true, Order = 1)]
		public Guid ConfigurationObject { get; set; }

		[DataMember(Name = "PromptName", IsRequired = true, Order = 2)]
		[XmlElement("PromptName")]
		public string PromptName { get; set; }

		[XmlElement(ElementName = "AudioData")]
		[DataMember(Name = "AudioData", IsRequired = true, Order = 3)]
		public string AudioData { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CreateUMPrompt(callContext, this);
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
