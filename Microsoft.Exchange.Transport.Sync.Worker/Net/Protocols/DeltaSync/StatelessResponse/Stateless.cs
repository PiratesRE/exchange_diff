using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse
{
	[XmlRoot(Namespace = "DeltaSyncV2:", IsNullable = false)]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	public class Stateless
	{
		[XmlElement(Namespace = "HMSYNC:")]
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

		[XmlIgnore]
		public bool StatusSpecified
		{
			get
			{
				return this.statusFieldSpecified;
			}
			set
			{
				this.statusFieldSpecified = value;
			}
		}

		[XmlElement(Namespace = "HMSYNC:")]
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

		[XmlElement(Namespace = "HMSYNC:")]
		public AuthPolicy AuthPolicy
		{
			get
			{
				return this.authPolicyField;
			}
			set
			{
				this.authPolicyField = value;
			}
		}

		[XmlArrayItem("Collection", IsNullable = false)]
		public StatelessCollection[] Collections
		{
			get
			{
				return this.collectionsField;
			}
			set
			{
				this.collectionsField = value;
			}
		}

		private int statusField;

		private bool statusFieldSpecified;

		private Fault faultField;

		private AuthPolicy authPolicyField;

		private StatelessCollection[] collectionsField;
	}
}
