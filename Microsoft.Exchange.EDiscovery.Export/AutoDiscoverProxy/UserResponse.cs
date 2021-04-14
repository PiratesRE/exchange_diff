using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UserResponse : AutodiscoverResponse
	{
		[XmlElement(IsNullable = true)]
		public string RedirectTarget
		{
			get
			{
				return this.redirectTargetField;
			}
			set
			{
				this.redirectTargetField = value;
			}
		}

		[XmlArray(IsNullable = true)]
		public UserSettingError[] UserSettingErrors
		{
			get
			{
				return this.userSettingErrorsField;
			}
			set
			{
				this.userSettingErrorsField = value;
			}
		}

		[XmlArray(IsNullable = true)]
		public UserSetting[] UserSettings
		{
			get
			{
				return this.userSettingsField;
			}
			set
			{
				this.userSettingsField = value;
			}
		}

		private string redirectTargetField;

		private UserSettingError[] userSettingErrorsField;

		private UserSetting[] userSettingsField;
	}
}
