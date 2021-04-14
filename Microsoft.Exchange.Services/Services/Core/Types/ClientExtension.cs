using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ClientExtensionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ClientExtension
	{
		[XmlAttribute]
		public bool IsAvailable { get; set; }

		[XmlAttribute]
		public bool IsMandatory { get; set; }

		[XmlAttribute]
		public bool IsEnabledByDefault { get; set; }

		[XmlAttribute]
		public ClientExtensionProvidedTo ProvidedTo { get; set; }

		[XmlAttribute]
		public ExtensionType Type { get; set; }

		[XmlAttribute]
		public ExtensionInstallScope Scope { get; set; }

		[XmlAttribute]
		public string MarketplaceAssetId { get; set; }

		[XmlAttribute]
		public string MarketplaceContentMarket { get; set; }

		[XmlAttribute]
		public string AppStatus { get; set; }

		[XmlAttribute]
		public string Etoken { get; set; }

		[XmlArrayItem("String", IsNullable = false)]
		public string[] SpecificUsers { get; set; }

		[XmlElement]
		public string Manifest { get; set; }
	}
}
