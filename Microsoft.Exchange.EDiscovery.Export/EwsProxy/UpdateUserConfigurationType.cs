using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UpdateUserConfigurationType : BaseRequestType
	{
		public UserConfigurationType UserConfiguration
		{
			get
			{
				return this.userConfigurationField;
			}
			set
			{
				this.userConfigurationField = value;
			}
		}

		private UserConfigurationType userConfigurationField;
	}
}
