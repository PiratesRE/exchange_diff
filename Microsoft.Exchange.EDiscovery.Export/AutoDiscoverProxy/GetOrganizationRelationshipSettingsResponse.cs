using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetOrganizationRelationshipSettingsResponse : AutodiscoverResponse
	{
		[XmlArray(IsNullable = true)]
		public OrganizationRelationshipSettings[] OrganizationRelationshipSettingsCollection
		{
			get
			{
				return this.organizationRelationshipSettingsCollectionField;
			}
			set
			{
				this.organizationRelationshipSettingsCollectionField = value;
			}
		}

		private OrganizationRelationshipSettings[] organizationRelationshipSettingsCollectionField;
	}
}
