using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class GetUserSettingsRequest : AutodiscoverRequest
	{
		[XmlArray(IsNullable = true)]
		public User[] Users
		{
			get
			{
				return this.usersField;
			}
			set
			{
				this.usersField = value;
			}
		}

		[XmlArrayItem("Setting")]
		[XmlArray(IsNullable = true)]
		public string[] RequestedSettings
		{
			get
			{
				return this.requestedSettingsField;
			}
			set
			{
				this.requestedSettingsField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public ExchangeVersion? RequestedVersion
		{
			get
			{
				return this.requestedVersionField;
			}
			set
			{
				this.requestedVersionField = value;
			}
		}

		private User[] usersField;

		private string[] requestedSettingsField;

		private ExchangeVersion? requestedVersionField;
	}
}
