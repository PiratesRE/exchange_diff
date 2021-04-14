using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ResolveNamesType : BaseRequestType
	{
		public ResolveNamesType()
		{
			this.searchScopeField = ResolveNamesSearchScopeType.ActiveDirectoryContacts;
		}

		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderIdType[] ParentFolderIds
		{
			get
			{
				return this.parentFolderIdsField;
			}
			set
			{
				this.parentFolderIdsField = value;
			}
		}

		public string UnresolvedEntry
		{
			get
			{
				return this.unresolvedEntryField;
			}
			set
			{
				this.unresolvedEntryField = value;
			}
		}

		[XmlAttribute]
		public bool ReturnFullContactData
		{
			get
			{
				return this.returnFullContactDataField;
			}
			set
			{
				this.returnFullContactDataField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(ResolveNamesSearchScopeType.ActiveDirectoryContacts)]
		public ResolveNamesSearchScopeType SearchScope
		{
			get
			{
				return this.searchScopeField;
			}
			set
			{
				this.searchScopeField = value;
			}
		}

		[XmlAttribute]
		public DefaultShapeNamesType ContactDataShape
		{
			get
			{
				return this.contactDataShapeField;
			}
			set
			{
				this.contactDataShapeField = value;
			}
		}

		[XmlIgnore]
		public bool ContactDataShapeSpecified
		{
			get
			{
				return this.contactDataShapeFieldSpecified;
			}
			set
			{
				this.contactDataShapeFieldSpecified = value;
			}
		}

		private BaseFolderIdType[] parentFolderIdsField;

		private string unresolvedEntryField;

		private bool returnFullContactDataField;

		private ResolveNamesSearchScopeType searchScopeField;

		private DefaultShapeNamesType contactDataShapeField;

		private bool contactDataShapeFieldSpecified;
	}
}
