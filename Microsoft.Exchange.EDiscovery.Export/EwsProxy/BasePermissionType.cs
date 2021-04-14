using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(CalendarPermissionType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(PermissionType))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public abstract class BasePermissionType
	{
		public UserIdType UserId
		{
			get
			{
				return this.userIdField;
			}
			set
			{
				this.userIdField = value;
			}
		}

		public bool CanCreateItems
		{
			get
			{
				return this.canCreateItemsField;
			}
			set
			{
				this.canCreateItemsField = value;
			}
		}

		[XmlIgnore]
		public bool CanCreateItemsSpecified
		{
			get
			{
				return this.canCreateItemsFieldSpecified;
			}
			set
			{
				this.canCreateItemsFieldSpecified = value;
			}
		}

		public bool CanCreateSubFolders
		{
			get
			{
				return this.canCreateSubFoldersField;
			}
			set
			{
				this.canCreateSubFoldersField = value;
			}
		}

		[XmlIgnore]
		public bool CanCreateSubFoldersSpecified
		{
			get
			{
				return this.canCreateSubFoldersFieldSpecified;
			}
			set
			{
				this.canCreateSubFoldersFieldSpecified = value;
			}
		}

		public bool IsFolderOwner
		{
			get
			{
				return this.isFolderOwnerField;
			}
			set
			{
				this.isFolderOwnerField = value;
			}
		}

		[XmlIgnore]
		public bool IsFolderOwnerSpecified
		{
			get
			{
				return this.isFolderOwnerFieldSpecified;
			}
			set
			{
				this.isFolderOwnerFieldSpecified = value;
			}
		}

		public bool IsFolderVisible
		{
			get
			{
				return this.isFolderVisibleField;
			}
			set
			{
				this.isFolderVisibleField = value;
			}
		}

		[XmlIgnore]
		public bool IsFolderVisibleSpecified
		{
			get
			{
				return this.isFolderVisibleFieldSpecified;
			}
			set
			{
				this.isFolderVisibleFieldSpecified = value;
			}
		}

		public bool IsFolderContact
		{
			get
			{
				return this.isFolderContactField;
			}
			set
			{
				this.isFolderContactField = value;
			}
		}

		[XmlIgnore]
		public bool IsFolderContactSpecified
		{
			get
			{
				return this.isFolderContactFieldSpecified;
			}
			set
			{
				this.isFolderContactFieldSpecified = value;
			}
		}

		public PermissionActionType EditItems
		{
			get
			{
				return this.editItemsField;
			}
			set
			{
				this.editItemsField = value;
			}
		}

		[XmlIgnore]
		public bool EditItemsSpecified
		{
			get
			{
				return this.editItemsFieldSpecified;
			}
			set
			{
				this.editItemsFieldSpecified = value;
			}
		}

		public PermissionActionType DeleteItems
		{
			get
			{
				return this.deleteItemsField;
			}
			set
			{
				this.deleteItemsField = value;
			}
		}

		[XmlIgnore]
		public bool DeleteItemsSpecified
		{
			get
			{
				return this.deleteItemsFieldSpecified;
			}
			set
			{
				this.deleteItemsFieldSpecified = value;
			}
		}

		private UserIdType userIdField;

		private bool canCreateItemsField;

		private bool canCreateItemsFieldSpecified;

		private bool canCreateSubFoldersField;

		private bool canCreateSubFoldersFieldSpecified;

		private bool isFolderOwnerField;

		private bool isFolderOwnerFieldSpecified;

		private bool isFolderVisibleField;

		private bool isFolderVisibleFieldSpecified;

		private bool isFolderContactField;

		private bool isFolderContactFieldSpecified;

		private PermissionActionType editItemsField;

		private bool editItemsFieldSpecified;

		private PermissionActionType deleteItemsField;

		private bool deleteItemsFieldSpecified;
	}
}
