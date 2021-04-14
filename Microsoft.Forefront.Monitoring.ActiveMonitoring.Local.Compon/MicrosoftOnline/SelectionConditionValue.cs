using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class SelectionConditionValue
	{
		[XmlArrayItem("Value", IsNullable = false)]
		public string[] Values
		{
			get
			{
				return this.valuesField;
			}
			set
			{
				this.valuesField = value;
			}
		}

		[XmlAttribute]
		public int Claim
		{
			get
			{
				return this.claimField;
			}
			set
			{
				this.claimField = value;
			}
		}

		[XmlAttribute]
		public int Operator
		{
			get
			{
				return this.operatorField;
			}
			set
			{
				this.operatorField = value;
			}
		}

		private string[] valuesField;

		private int claimField;

		private int operatorField;
	}
}
