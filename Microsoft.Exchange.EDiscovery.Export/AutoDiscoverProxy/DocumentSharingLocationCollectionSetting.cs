using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class DocumentSharingLocationCollectionSetting : UserSetting
	{
		[XmlArrayItem(IsNullable = false)]
		public DocumentSharingLocation[] DocumentSharingLocations
		{
			get
			{
				return this.documentSharingLocationsField;
			}
			set
			{
				this.documentSharingLocationsField = value;
			}
		}

		private DocumentSharingLocation[] documentSharingLocationsField;
	}
}
