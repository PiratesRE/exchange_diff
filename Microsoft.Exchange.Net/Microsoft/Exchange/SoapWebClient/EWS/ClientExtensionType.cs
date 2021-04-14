using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ClientExtensionType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] SpecificUsers;

		[XmlElement(DataType = "base64Binary")]
		public byte[] Manifest;

		[XmlAttribute]
		public bool IsAvailable;

		[XmlIgnore]
		public bool IsAvailableSpecified;

		[XmlAttribute]
		public bool IsMandatory;

		[XmlIgnore]
		public bool IsMandatorySpecified;

		[XmlAttribute]
		public bool IsEnabledByDefault;

		[XmlIgnore]
		public bool IsEnabledByDefaultSpecified;

		[XmlAttribute]
		public ClientExtensionProvidedToType ProvidedTo;

		[XmlIgnore]
		public bool ProvidedToSpecified;

		[XmlAttribute]
		public ClientExtensionTypeType Type;

		[XmlIgnore]
		public bool TypeSpecified;

		[XmlAttribute]
		public ClientExtensionScopeType Scope;

		[XmlIgnore]
		public bool ScopeSpecified;

		[XmlAttribute]
		public string MarketplaceAssetId;

		[XmlAttribute]
		public string MarketplaceContentMarket;

		[XmlAttribute]
		public string AppStatus;

		[XmlAttribute]
		public string Etoken;
	}
}
