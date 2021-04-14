using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[Serializable]
	public class Admin
	{
		public string NetId
		{
			get
			{
				return this.netIdField;
			}
			set
			{
				this.netIdField = value;
			}
		}

		public string ByodAuthToken
		{
			get
			{
				return this.byodAuthTokenField;
			}
			set
			{
				this.byodAuthTokenField = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabledField;
			}
			set
			{
				this.isEnabledField = value;
			}
		}

		private string netIdField;

		private string byodAuthTokenField;

		private bool isEnabledField;
	}
}
