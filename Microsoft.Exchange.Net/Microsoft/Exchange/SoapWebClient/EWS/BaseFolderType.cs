using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlInclude(typeof(ContactsFolderType))]
	[XmlInclude(typeof(CalendarFolderType))]
	[XmlInclude(typeof(FolderType))]
	[XmlInclude(typeof(TasksFolderType))]
	[XmlInclude(typeof(SearchFolderType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public abstract class BaseFolderType
	{
		public FolderIdType FolderId;

		public FolderIdType ParentFolderId;

		public string FolderClass;

		public string DisplayName;

		public int TotalCount;

		[XmlIgnore]
		public bool TotalCountSpecified;

		public int ChildFolderCount;

		[XmlIgnore]
		public bool ChildFolderCountSpecified;

		[XmlElement("ExtendedProperty")]
		public ExtendedPropertyType[] ExtendedProperty;

		public ManagedFolderInformationType ManagedFolderInformation;

		public EffectiveRightsType EffectiveRights;

		public DistinguishedFolderIdNameType DistinguishedFolderId;

		[XmlIgnore]
		public bool DistinguishedFolderIdSpecified;

		public RetentionTagType PolicyTag;

		public RetentionTagType ArchiveTag;
	}
}
