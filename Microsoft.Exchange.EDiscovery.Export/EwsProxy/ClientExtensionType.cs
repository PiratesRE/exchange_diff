using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ClientExtensionType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] SpecificUsers
		{
			get
			{
				return this.specificUsersField;
			}
			set
			{
				this.specificUsersField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] Manifest
		{
			get
			{
				return this.manifestField;
			}
			set
			{
				this.manifestField = value;
			}
		}

		[XmlAttribute]
		public bool IsAvailable
		{
			get
			{
				return this.isAvailableField;
			}
			set
			{
				this.isAvailableField = value;
			}
		}

		[XmlIgnore]
		public bool IsAvailableSpecified
		{
			get
			{
				return this.isAvailableFieldSpecified;
			}
			set
			{
				this.isAvailableFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool IsMandatory
		{
			get
			{
				return this.isMandatoryField;
			}
			set
			{
				this.isMandatoryField = value;
			}
		}

		[XmlIgnore]
		public bool IsMandatorySpecified
		{
			get
			{
				return this.isMandatoryFieldSpecified;
			}
			set
			{
				this.isMandatoryFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool IsEnabledByDefault
		{
			get
			{
				return this.isEnabledByDefaultField;
			}
			set
			{
				this.isEnabledByDefaultField = value;
			}
		}

		[XmlIgnore]
		public bool IsEnabledByDefaultSpecified
		{
			get
			{
				return this.isEnabledByDefaultFieldSpecified;
			}
			set
			{
				this.isEnabledByDefaultFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public ClientExtensionProvidedToType ProvidedTo
		{
			get
			{
				return this.providedToField;
			}
			set
			{
				this.providedToField = value;
			}
		}

		[XmlIgnore]
		public bool ProvidedToSpecified
		{
			get
			{
				return this.providedToFieldSpecified;
			}
			set
			{
				this.providedToFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public ClientExtensionTypeType Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		[XmlIgnore]
		public bool TypeSpecified
		{
			get
			{
				return this.typeFieldSpecified;
			}
			set
			{
				this.typeFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public ClientExtensionScopeType Scope
		{
			get
			{
				return this.scopeField;
			}
			set
			{
				this.scopeField = value;
			}
		}

		[XmlIgnore]
		public bool ScopeSpecified
		{
			get
			{
				return this.scopeFieldSpecified;
			}
			set
			{
				this.scopeFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string MarketplaceAssetId
		{
			get
			{
				return this.marketplaceAssetIdField;
			}
			set
			{
				this.marketplaceAssetIdField = value;
			}
		}

		[XmlAttribute]
		public string MarketplaceContentMarket
		{
			get
			{
				return this.marketplaceContentMarketField;
			}
			set
			{
				this.marketplaceContentMarketField = value;
			}
		}

		[XmlAttribute]
		public string AppStatus
		{
			get
			{
				return this.appStatusField;
			}
			set
			{
				this.appStatusField = value;
			}
		}

		[XmlAttribute]
		public string Etoken
		{
			get
			{
				return this.etokenField;
			}
			set
			{
				this.etokenField = value;
			}
		}

		private string[] specificUsersField;

		private byte[] manifestField;

		private bool isAvailableField;

		private bool isAvailableFieldSpecified;

		private bool isMandatoryField;

		private bool isMandatoryFieldSpecified;

		private bool isEnabledByDefaultField;

		private bool isEnabledByDefaultFieldSpecified;

		private ClientExtensionProvidedToType providedToField;

		private bool providedToFieldSpecified;

		private ClientExtensionTypeType typeField;

		private bool typeFieldSpecified;

		private ClientExtensionScopeType scopeField;

		private bool scopeFieldSpecified;

		private string marketplaceAssetIdField;

		private string marketplaceContentMarketField;

		private string appStatusField;

		private string etokenField;
	}
}
