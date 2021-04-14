using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class DiscoverySearchConfigurationType
	{
		public string SearchId
		{
			get
			{
				return this.searchIdField;
			}
			set
			{
				this.searchIdField = value;
			}
		}

		public string SearchQuery
		{
			get
			{
				return this.searchQueryField;
			}
			set
			{
				this.searchQueryField = value;
			}
		}

		[XmlArrayItem("SearchableMailbox", IsNullable = false)]
		public SearchableMailboxType[] SearchableMailboxes
		{
			get
			{
				return this.searchableMailboxesField;
			}
			set
			{
				this.searchableMailboxesField = value;
			}
		}

		public string InPlaceHoldIdentity
		{
			get
			{
				return this.inPlaceHoldIdentityField;
			}
			set
			{
				this.inPlaceHoldIdentityField = value;
			}
		}

		public string ManagedByOrganization
		{
			get
			{
				return this.managedByOrganizationField;
			}
			set
			{
				this.managedByOrganizationField = value;
			}
		}

		public string Language
		{
			get
			{
				return this.languageField;
			}
			set
			{
				this.languageField = value;
			}
		}

		private string searchIdField;

		private string searchQueryField;

		private SearchableMailboxType[] searchableMailboxesField;

		private string inPlaceHoldIdentityField;

		private string managedByOrganizationField;

		private string languageField;
	}
}
