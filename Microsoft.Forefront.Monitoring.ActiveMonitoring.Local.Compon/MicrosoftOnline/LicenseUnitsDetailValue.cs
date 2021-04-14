using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class LicenseUnitsDetailValue
	{
		[XmlAttribute]
		public int Enabled
		{
			get
			{
				return this.enabledField;
			}
			set
			{
				this.enabledField = value;
			}
		}

		[XmlAttribute]
		public int Warning
		{
			get
			{
				return this.warningField;
			}
			set
			{
				this.warningField = value;
			}
		}

		[XmlAttribute]
		public int Suspended
		{
			get
			{
				return this.suspendedField;
			}
			set
			{
				this.suspendedField = value;
			}
		}

		private int enabledField;

		private int warningField;

		private int suspendedField;
	}
}
