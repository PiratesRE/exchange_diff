using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class FindPeopleType : BaseRequestType
	{
		public PersonaResponseShapeType PersonaShape
		{
			get
			{
				return this.personaShapeField;
			}
			set
			{
				this.personaShapeField = value;
			}
		}

		public IndexedPageViewType IndexedPageItemView
		{
			get
			{
				return this.indexedPageItemViewField;
			}
			set
			{
				this.indexedPageItemViewField = value;
			}
		}

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

		public RestrictionType AggregationRestriction
		{
			get
			{
				return this.aggregationRestrictionField;
			}
			set
			{
				this.aggregationRestrictionField = value;
			}
		}

		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FieldOrderType[] SortOrder
		{
			get
			{
				return this.sortOrderField;
			}
			set
			{
				this.sortOrderField = value;
			}
		}

		public TargetFolderIdType ParentFolderId
		{
			get
			{
				return this.parentFolderIdField;
			}
			set
			{
				this.parentFolderIdField = value;
			}
		}

		public string QueryString
		{
			get
			{
				return this.queryStringField;
			}
			set
			{
				this.queryStringField = value;
			}
		}

		private PersonaResponseShapeType personaShapeField;

		private IndexedPageViewType indexedPageItemViewField;

		private RestrictionType restrictionField;

		private RestrictionType aggregationRestrictionField;

		private FieldOrderType[] sortOrderField;

		private TargetFolderIdType parentFolderIdField;

		private string queryStringField;
	}
}
