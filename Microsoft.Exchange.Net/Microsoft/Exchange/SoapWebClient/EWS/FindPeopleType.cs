using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class FindPeopleType : BaseRequestType
	{
		public PersonaResponseShapeType PersonaShape;

		public IndexedPageViewType IndexedPageItemView;

		public RestrictionType Restriction;

		public RestrictionType AggregationRestriction;

		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FieldOrderType[] SortOrder;

		public TargetFolderIdType ParentFolderId;

		public string QueryString;
	}
}
