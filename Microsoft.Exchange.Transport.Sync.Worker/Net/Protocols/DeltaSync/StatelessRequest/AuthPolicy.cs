using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DesignerCategory("code")]
	[XmlRoot(Namespace = "HMSYNC:", IsNullable = false)]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "HMSYNC:")]
	[Serializable]
	public class AuthPolicy
	{
		public string SAP
		{
			get
			{
				return this.sAPField;
			}
			set
			{
				this.sAPField = value;
			}
		}

		public string Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		private string sAPField;

		private string versionField;
	}
}
