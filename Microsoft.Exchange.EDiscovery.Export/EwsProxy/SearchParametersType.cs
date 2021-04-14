using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SearchParametersType
	{
		public RestrictionType Restriction
		{
			get
			{
				return this.restrictionField;
			}
			set
			{
				this.restrictionField = value;
			}
		}

		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderIdType), IsNullable = false)]
		public BaseFolderIdType[] BaseFolderIds
		{
			get
			{
				return this.baseFolderIdsField;
			}
			set
			{
				this.baseFolderIdsField = value;
			}
		}

		[XmlAttribute]
		public SearchFolderTraversalType Traversal
		{
			get
			{
				return this.traversalField;
			}
			set
			{
				this.traversalField = value;
			}
		}

		[XmlIgnore]
		public bool TraversalSpecified
		{
			get
			{
				return this.traversalFieldSpecified;
			}
			set
			{
				this.traversalFieldSpecified = value;
			}
		}

		private RestrictionType restrictionField;

		private BaseFolderIdType[] baseFolderIdsField;

		private SearchFolderTraversalType traversalField;

		private bool traversalFieldSpecified;
	}
}
