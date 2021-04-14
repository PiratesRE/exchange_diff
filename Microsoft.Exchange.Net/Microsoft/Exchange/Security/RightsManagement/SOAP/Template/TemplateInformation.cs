using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Template
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://microsoft.com/DRM/TemplateDistributionService")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class TemplateInformation
	{
		public string ServerPublicKey
		{
			get
			{
				return this.serverPublicKeyField;
			}
			set
			{
				this.serverPublicKeyField = value;
			}
		}

		public int GuidHashCount
		{
			get
			{
				return this.guidHashCountField;
			}
			set
			{
				this.guidHashCountField = value;
			}
		}

		[XmlElement("GuidHash")]
		public GuidHash[] GuidHash
		{
			get
			{
				return this.guidHashField;
			}
			set
			{
				this.guidHashField = value;
			}
		}

		private string serverPublicKeyField;

		private int guidHashCountField;

		private GuidHash[] guidHashField;
	}
}
