using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetUserConfigurationType : BaseRequestType
	{
		public UserConfigurationNameType UserConfigurationName
		{
			get
			{
				return this.userConfigurationNameField;
			}
			set
			{
				this.userConfigurationNameField = value;
			}
		}

		public UserConfigurationPropertyType UserConfigurationProperties
		{
			get
			{
				return this.userConfigurationPropertiesField;
			}
			set
			{
				this.userConfigurationPropertiesField = value;
			}
		}

		private UserConfigurationNameType userConfigurationNameField;

		private UserConfigurationPropertyType userConfigurationPropertiesField;
	}
}
