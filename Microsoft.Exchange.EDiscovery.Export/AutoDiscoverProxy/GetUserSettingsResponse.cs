using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetUserSettingsResponse : AutodiscoverResponse
	{
		[XmlArray(IsNullable = true)]
		public UserResponse[] UserResponses
		{
			get
			{
				return this.userResponsesField;
			}
			set
			{
				this.userResponsesField = value;
			}
		}

		private UserResponse[] userResponsesField;
	}
}
