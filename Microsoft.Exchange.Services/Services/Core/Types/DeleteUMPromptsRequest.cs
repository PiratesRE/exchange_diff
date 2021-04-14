using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DeleteUMPromptsRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DeleteUMPromptsRequest : BaseRequest
	{
		[XmlElement("ConfigurationObject")]
		[DataMember(Name = "ConfigurationObject", IsRequired = true, Order = 1)]
		public Guid ConfigurationObject { get; set; }

		[DataMember(Name = "PromptNames", IsRequired = false, Order = 2)]
		[XmlArray(ElementName = "PromptNames", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] PromptNames { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new DeleteUMPrompts(callContext, this);
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
