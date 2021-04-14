using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetServiceConfigurationRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetServiceConfigurationRequest : BaseRequest
	{
		[XmlElement("ActingAs")]
		public EmailAddressWrapper ActingAs
		{
			get
			{
				return this.actingAs;
			}
			set
			{
				this.actingAs = value;
			}
		}

		[XmlAnyElement("ConfigurationRequestDetails")]
		public XmlElement ConfigurationRequestDetails { get; set; }

		[XmlArrayItem("ConfigurationName", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArray("RequestedConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string[] ConfigurationTypes
		{
			get
			{
				return this.configurationTypes;
			}
			set
			{
				this.configurationTypes = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetServiceConfiguration(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.configurationTypes == null)
			{
				return null;
			}
			if (Array.Exists<string>(this.configurationTypes, (string compare) => compare == "UnifiedMessagingConfiguration"))
			{
				return callContext.GetServerInfoForEffectiveCaller();
			}
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		internal const string ActingAsElementName = "ActingAs";

		internal const string RequestedConfigurationElementName = "RequestedConfiguration";

		internal const string ConfigurationTypeElementName = "ConfigurationName";

		internal const string ConfigurationRequestDetailsElementName = "ConfigurationRequestDetails";

		private EmailAddressWrapper actingAs;

		private string[] configurationTypes;
	}
}
