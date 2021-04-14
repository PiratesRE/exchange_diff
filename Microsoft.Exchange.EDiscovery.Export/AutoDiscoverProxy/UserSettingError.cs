using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class UserSettingError
	{
		public ErrorCode ErrorCode
		{
			get
			{
				return this.errorCodeField;
			}
			set
			{
				this.errorCodeField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string ErrorMessage
		{
			get
			{
				return this.errorMessageField;
			}
			set
			{
				this.errorMessageField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string SettingName
		{
			get
			{
				return this.settingNameField;
			}
			set
			{
				this.settingNameField = value;
			}
		}

		private ErrorCode errorCodeField;

		private string errorMessageField;

		private string settingNameField;
	}
}
