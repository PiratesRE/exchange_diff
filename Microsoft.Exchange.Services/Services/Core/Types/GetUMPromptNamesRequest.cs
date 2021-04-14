using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMPromptNamesRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUMPromptNamesRequest : BaseRequest
	{
		[XmlElement("ConfigurationObject")]
		[DataMember(Name = "ConfigurationObject", IsRequired = true, Order = 1)]
		public Guid ConfigurationObject { get; set; }

		[DataMember(Name = "HoursElapsedSinceLastModified", IsRequired = true, Order = 2)]
		[XmlElement("HoursElapsedSinceLastModified")]
		public int HoursElapsedSinceLastModified { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUMPromptNames(callContext, this);
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
