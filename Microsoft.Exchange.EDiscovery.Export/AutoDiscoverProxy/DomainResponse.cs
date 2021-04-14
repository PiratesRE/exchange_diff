using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class DomainResponse : AutodiscoverResponse
	{
		[XmlArray(IsNullable = true)]
		public DomainSettingError[] DomainSettingErrors
		{
			get
			{
				return this.domainSettingErrorsField;
			}
			set
			{
				this.domainSettingErrorsField = value;
			}
		}

		[XmlArray(IsNullable = true)]
		public DomainSetting[] DomainSettings
		{
			get
			{
				return this.domainSettingsField;
			}
			set
			{
				this.domainSettingsField = value;
			}
		}

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

		private DomainSettingError[] domainSettingErrorsField;

		private DomainSetting[] domainSettingsField;

		private string redirectTargetField;
	}
}
