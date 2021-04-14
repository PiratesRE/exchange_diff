using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[XmlType(Namespace = "HMSETTINGS:")]
	[Serializable]
	public enum HeaderDisplayType
	{
		[XmlEnum("No Header")]
		NoHeader,
		Basic,
		Full,
		Advanced
	}
}
