using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlRoot(Namespace = "DeltaSyncV2:", IsNullable = false)]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[Serializable]
	public class Stateless
	{
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

		private StatelessCollection[] collectionsField;
	}
}
