using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UserResponse : AutodiscoverResponse
	{
		[XmlElement(IsNullable = true)]
		public string RedirectTarget;

		[XmlArray(IsNullable = true)]
		public UserSettingError[] UserSettingErrors;

		[XmlArray(IsNullable = true)]
		public UserSetting[] UserSettings;
	}
}
