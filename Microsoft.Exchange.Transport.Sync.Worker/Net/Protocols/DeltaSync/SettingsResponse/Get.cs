using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "Get", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Get
	{
		[XmlAnyElement]
		public XmlElement Any;
	}
}
