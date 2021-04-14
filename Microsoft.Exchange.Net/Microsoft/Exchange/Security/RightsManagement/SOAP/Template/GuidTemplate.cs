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
	public class GuidTemplate
	{
		public string Guid
		{
			get
			{
				return this.guidField;
			}
			set
			{
				this.guidField = value;
			}
		}

		public string Hash
		{
			get
			{
				return this.hashField;
			}
			set
			{
				this.hashField = value;
			}
		}

		public string Template
		{
			get
			{
				return this.templateField;
			}
			set
			{
				this.templateField = value;
			}
		}

		private string guidField;

		private string hashField;

		private string templateField;
	}
}
