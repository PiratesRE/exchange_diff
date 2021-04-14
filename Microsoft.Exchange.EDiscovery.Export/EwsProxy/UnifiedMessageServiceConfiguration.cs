using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class UnifiedMessageServiceConfiguration : ServiceConfiguration
	{
		public bool UmEnabled
		{
			get
			{
				return this.umEnabledField;
			}
			set
			{
				this.umEnabledField = value;
			}
		}

		public string PlayOnPhoneDialString
		{
			get
			{
				return this.playOnPhoneDialStringField;
			}
			set
			{
				this.playOnPhoneDialStringField = value;
			}
		}

		public bool PlayOnPhoneEnabled
		{
			get
			{
				return this.playOnPhoneEnabledField;
			}
			set
			{
				this.playOnPhoneEnabledField = value;
			}
		}

		private bool umEnabledField;

		private string playOnPhoneDialStringField;

		private bool playOnPhoneEnabledField;
	}
}
