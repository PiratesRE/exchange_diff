using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	[XmlType(Namespace = "HMSETTINGS:")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[Serializable]
	public enum FilterOperatorType
	{
		Contains,
		[XmlEnum("Does not contain")]
		Doesnotcontain,
		[XmlEnum("Contains word")]
		Containsword,
		[XmlEnum("Starts with")]
		Startswith,
		[XmlEnum("Ends with")]
		Endswith,
		Equals
	}
}
