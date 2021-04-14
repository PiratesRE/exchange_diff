using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class SmtpDomain
	{
		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlAttribute]
		public bool IncludeSubdomains
		{
			get
			{
				return this.includeSubdomainsField;
			}
			set
			{
				this.includeSubdomainsField = value;
			}
		}

		[XmlIgnore]
		public bool IncludeSubdomainsSpecified
		{
			get
			{
				return this.includeSubdomainsFieldSpecified;
			}
			set
			{
				this.includeSubdomainsFieldSpecified = value;
			}
		}

		private string nameField;

		private bool includeSubdomainsField;

		private bool includeSubdomainsFieldSpecified;
	}
}
