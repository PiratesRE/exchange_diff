using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class SetEncryptionConfigurationType : BaseRequestType
	{
		public string ImageBase64
		{
			get
			{
				return this.imageBase64Field;
			}
			set
			{
				this.imageBase64Field = value;
			}
		}

		public string EmailText
		{
			get
			{
				return this.emailTextField;
			}
			set
			{
				this.emailTextField = value;
			}
		}

		public string PortalText
		{
			get
			{
				return this.portalTextField;
			}
			set
			{
				this.portalTextField = value;
			}
		}

		public string DisclaimerText
		{
			get
			{
				return this.disclaimerTextField;
			}
			set
			{
				this.disclaimerTextField = value;
			}
		}

		public bool OTPEnabled
		{
			get
			{
				return this.oTPEnabledField;
			}
			set
			{
				this.oTPEnabledField = value;
			}
		}

		[XmlIgnore]
		public bool OTPEnabledSpecified
		{
			get
			{
				return this.oTPEnabledFieldSpecified;
			}
			set
			{
				this.oTPEnabledFieldSpecified = value;
			}
		}

		private string imageBase64Field;

		private string emailTextField;

		private string portalTextField;

		private string disclaimerTextField;

		private bool oTPEnabledField;

		private bool oTPEnabledFieldSpecified;
	}
}
