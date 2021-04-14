using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse
{
	[XmlRoot(Namespace = "HMSYNC:", IsNullable = false)]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "HMSYNC:")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DesignerCategory("code")]
	[Serializable]
	public class DeletesAsMoves
	{
	}
}
