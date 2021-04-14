using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetServiceConfigurationType : BaseRequestType
	{
		public EmailAddressType ActingAs
		{
			get
			{
				return this.actingAsField;
			}
			set
			{
				this.actingAsField = value;
			}
		}

		[XmlArrayItem("ConfigurationName", IsNullable = false)]
		public ServiceConfigurationType[] RequestedConfiguration
		{
			get
			{
				return this.requestedConfigurationField;
			}
			set
			{
				this.requestedConfigurationField = value;
			}
		}

		public ConfigurationRequestDetailsType ConfigurationRequestDetails
		{
			get
			{
				return this.configurationRequestDetailsField;
			}
			set
			{
				this.configurationRequestDetailsField = value;
			}
		}

		private EmailAddressType actingAsField;

		private ServiceConfigurationType[] requestedConfigurationField;

		private ConfigurationRequestDetailsType configurationRequestDetailsField;
	}
}
