using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "ADOwaVirtualDirectoryConfig")]
	[Serializable]
	public sealed class ADOwaVirtualDirectoryConfigXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "IsPublic")]
		public bool IsPublic { get; set; }
	}
}
