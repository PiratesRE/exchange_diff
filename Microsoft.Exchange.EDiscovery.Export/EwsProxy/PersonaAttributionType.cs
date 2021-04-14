using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PersonaAttributionType
	{
		public string Id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		public ItemIdType SourceId
		{
			get
			{
				return this.sourceIdField;
			}
			set
			{
				this.sourceIdField = value;
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

		public bool IsWritable
		{
			get
			{
				return this.isWritableField;
			}
			set
			{
				this.isWritableField = value;
			}
		}

		[XmlIgnore]
		public bool IsWritableSpecified
		{
			get
			{
				return this.isWritableFieldSpecified;
			}
			set
			{
				this.isWritableFieldSpecified = value;
			}
		}

		public bool IsQuickContact
		{
			get
			{
				return this.isQuickContactField;
			}
			set
			{
				this.isQuickContactField = value;
			}
		}

		[XmlIgnore]
		public bool IsQuickContactSpecified
		{
			get
			{
				return this.isQuickContactFieldSpecified;
			}
			set
			{
				this.isQuickContactFieldSpecified = value;
			}
		}

		public bool IsHidden
		{
			get
			{
				return this.isHiddenField;
			}
			set
			{
				this.isHiddenField = value;
			}
		}

		[XmlIgnore]
		public bool IsHiddenSpecified
		{
			get
			{
				return this.isHiddenFieldSpecified;
			}
			set
			{
				this.isHiddenFieldSpecified = value;
			}
		}

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

		private string idField;

		private ItemIdType sourceIdField;

		private string displayNameField;

		private bool isWritableField;

		private bool isWritableFieldSpecified;

		private bool isQuickContactField;

		private bool isQuickContactFieldSpecified;

		private bool isHiddenField;

		private bool isHiddenFieldSpecified;

		private FolderIdType folderIdField;
	}
}
