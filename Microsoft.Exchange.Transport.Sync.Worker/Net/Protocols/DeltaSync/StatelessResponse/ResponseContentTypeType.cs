using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse
{
	[XmlRoot("ResponseContentType", Namespace = "HMMAIL:", IsNullable = false)]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(Namespace = "HMMAIL:")]
	[Serializable]
	public enum ResponseContentTypeType
	{
		mtom
	}
}
