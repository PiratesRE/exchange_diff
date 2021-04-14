using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class GetClientExtensionUserParametersType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] UserEnabledExtensions
		{
			get
			{
				return this.userEnabledExtensionsField;
			}
			set
			{
				this.userEnabledExtensionsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UserDisabledExtensions
		{
			get
			{
				return this.userDisabledExtensionsField;
			}
			set
			{
				this.userDisabledExtensionsField = value;
			}
		}

		[XmlAttribute]
		public string UserId
		{
			get
			{
				return this.userIdField;
			}
			set
			{
				this.userIdField = value;
			}
		}

		[XmlAttribute]
		public bool EnabledOnly
		{
			get
			{
				return this.enabledOnlyField;
			}
			set
			{
				this.enabledOnlyField = value;
			}
		}

		[XmlIgnore]
		public bool EnabledOnlySpecified
		{
			get
			{
				return this.enabledOnlyFieldSpecified;
			}
			set
			{
				this.enabledOnlyFieldSpecified = value;
			}
		}

		private string[] userEnabledExtensionsField;

		private string[] userDisabledExtensionsField;

		private string userIdField;

		private bool enabledOnlyField;

		private bool enabledOnlyFieldSpecified;
	}
}
