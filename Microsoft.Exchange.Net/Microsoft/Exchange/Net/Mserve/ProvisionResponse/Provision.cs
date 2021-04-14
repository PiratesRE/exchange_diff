using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.ProvisionResponse
{
	[XmlRoot(Namespace = "DeltaSyncV2:", IsNullable = false)]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class Provision
	{
		public int Status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		public Fault Fault
		{
			get
			{
				return this.faultField;
			}
			set
			{
				this.faultField = value;
			}
		}

		public ProvisionResponses Responses
		{
			get
			{
				return this.responsesField;
			}
			set
			{
				this.responsesField = value;
			}
		}

		private int statusField;

		private Fault faultField;

		private ProvisionResponses responsesField;
	}
}
