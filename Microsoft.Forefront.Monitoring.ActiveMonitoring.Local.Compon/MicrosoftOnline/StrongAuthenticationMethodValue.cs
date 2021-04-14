using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class StrongAuthenticationMethodValue
	{
		public StrongAuthenticationMethodValue()
		{
			this.defaultField = false;
		}

		[XmlAttribute]
		public int MethodType
		{
			get
			{
				return this.methodTypeField;
			}
			set
			{
				this.methodTypeField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool Default
		{
			get
			{
				return this.defaultField;
			}
			set
			{
				this.defaultField = value;
			}
		}

		private int methodTypeField;

		private bool defaultField;
	}
}
