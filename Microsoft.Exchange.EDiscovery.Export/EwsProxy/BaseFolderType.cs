using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(FolderType))]
	[XmlInclude(typeof(ContactsFolderType))]
	[XmlInclude(typeof(CalendarFolderType))]
	[XmlInclude(typeof(SearchFolderType))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(TasksFolderType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public abstract class BaseFolderType
	{
		public FolderIdType FolderId
		{
			get
			{
				return this.folderIdField;
			}
			set
			{
				this.folderIdField = value;
			}
		}

		public FolderIdType ParentFolderId
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

		public string FolderClass
		{
			get
			{
				return this.folderClassField;
			}
			set
			{
				this.folderClassField = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public int TotalCount
		{
			get
			{
				return this.totalCountField;
			}
			set
			{
				this.totalCountField = value;
			}
		}

		[XmlIgnore]
		public bool TotalCountSpecified
		{
			get
			{
				return this.totalCountFieldSpecified;
			}
			set
			{
				this.totalCountFieldSpecified = value;
			}
		}

		public int ChildFolderCount
		{
			get
			{
				return this.childFolderCountField;
			}
			set
			{
				this.childFolderCountField = value;
			}
		}

		[XmlIgnore]
		public bool ChildFolderCountSpecified
		{
			get
			{
				return this.childFolderCountFieldSpecified;
			}
			set
			{
				this.childFolderCountFieldSpecified = value;
			}
		}

		[XmlElement("ExtendedProperty")]
		public ExtendedPropertyType[] ExtendedProperty
		{
			get
			{
				return this.extendedPropertyField;
			}
			set
			{
				this.extendedPropertyField = value;
			}
		}

		public ManagedFolderInformationType ManagedFolderInformation
		{
			get
			{
				return this.managedFolderInformationField;
			}
			set
			{
				this.managedFolderInformationField = value;
			}
		}

		public EffectiveRightsType EffectiveRights
		{
			get
			{
				return this.effectiveRightsField;
			}
			set
			{
				this.effectiveRightsField = value;
			}
		}

		public DistinguishedFolderIdNameType DistinguishedFolderId
		{
			get
			{
				return this.distinguishedFolderIdField;
			}
			set
			{
				this.distinguishedFolderIdField = value;
			}
		}

		[XmlIgnore]
		public bool DistinguishedFolderIdSpecified
		{
			get
			{
				return this.distinguishedFolderIdFieldSpecified;
			}
			set
			{
				this.distinguishedFolderIdFieldSpecified = value;
			}
		}

		public RetentionTagType PolicyTag
		{
			get
			{
				return this.policyTagField;
			}
			set
			{
				this.policyTagField = value;
			}
		}

		public RetentionTagType ArchiveTag
		{
			get
			{
				return this.archiveTagField;
			}
			set
			{
				this.archiveTagField = value;
			}
		}

		private FolderIdType folderIdField;

		private FolderIdType parentFolderIdField;

		private string folderClassField;

		private string displayNameField;

		private int totalCountField;

		private bool totalCountFieldSpecified;

		private int childFolderCountField;

		private bool childFolderCountFieldSpecified;

		private ExtendedPropertyType[] extendedPropertyField;

		private ManagedFolderInformationType managedFolderInformationField;

		private EffectiveRightsType effectiveRightsField;

		private DistinguishedFolderIdNameType distinguishedFolderIdField;

		private bool distinguishedFolderIdFieldSpecified;

		private RetentionTagType policyTagField;

		private RetentionTagType archiveTagField;
	}
}
