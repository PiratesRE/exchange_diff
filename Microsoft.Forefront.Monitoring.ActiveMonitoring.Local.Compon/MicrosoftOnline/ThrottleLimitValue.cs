using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class ThrottleLimitValue
	{
		[XmlAttribute]
		public string CounterName
		{
			get
			{
				return this.counterNameField;
			}
			set
			{
				this.counterNameField = value;
			}
		}

		[XmlAttribute]
		public string CounterValue
		{
			get
			{
				return this.counterValueField;
			}
			set
			{
				this.counterValueField = value;
			}
		}

		private string counterNameField;

		private string counterValueField;
	}
}
