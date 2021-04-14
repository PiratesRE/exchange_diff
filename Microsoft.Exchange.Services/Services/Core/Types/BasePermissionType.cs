using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(CalendarPermissionType))]
	[KnownType(typeof(PermissionType))]
	[XmlInclude(typeof(PermissionType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(CalendarPermissionType))]
	[Serializable]
	public abstract class BasePermissionType
	{
		protected BasePermissionType()
		{
			this.UserId = new UserId();
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public UserId UserId { get; set; }

		[DataMember(Order = 2)]
		public bool CanCreateItems
		{
			get
			{
				return this.canCreateItems;
			}
			set
			{
				this.CanCreateItemsSpecified = true;
				this.canCreateItems = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool CanCreateItemsSpecified { get; set; }

		[DataMember(Order = 3)]
		public bool CanCreateSubFolders
		{
			get
			{
				return this.canCreateSubFolders;
			}
			set
			{
				this.CanCreateSubFoldersSpecified = true;
				this.canCreateSubFolders = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool CanCreateSubFoldersSpecified { get; set; }

		[DataMember(Order = 4)]
		public bool IsFolderOwner
		{
			get
			{
				return this.isFolderOwner;
			}
			set
			{
				this.IsFolderOwnerSpecified = true;
				this.isFolderOwner = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsFolderOwnerSpecified { get; set; }

		[DataMember(Order = 5)]
		public bool IsFolderVisible
		{
			get
			{
				return this.isFolderVisible;
			}
			set
			{
				this.IsFolderVisibleSpecified = true;
				this.isFolderVisible = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsFolderVisibleSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public bool IsFolderContact
		{
			get
			{
				return this.isFolderContact;
			}
			set
			{
				this.IsFolderContactSpecified = true;
				this.isFolderContact = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsFolderContactSpecified { get; set; }

		[IgnoreDataMember]
		[XmlElement("EditItems")]
		public PermissionActionType EditItems
		{
			get
			{
				return this.editItems;
			}
			set
			{
				this.EditItemsSpecified = true;
				this.editItems = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "EditItems", EmitDefaultValue = false, Order = 7)]
		public string EditItemsString
		{
			get
			{
				if (!this.EditItemsSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<PermissionActionType>(this.EditItems);
			}
			set
			{
				this.EditItems = EnumUtilities.Parse<PermissionActionType>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool EditItemsSpecified { get; set; }

		[XmlElement("DeleteItems")]
		[IgnoreDataMember]
		public PermissionActionType DeleteItems
		{
			get
			{
				return this.deleteItems;
			}
			set
			{
				this.DeleteItemsSpecified = true;
				this.deleteItems = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "DeleteItems", EmitDefaultValue = false, Order = 8)]
		public string DeleteItemsString
		{
			get
			{
				if (!this.DeleteItemsSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<PermissionActionType>(this.DeleteItems);
			}
			set
			{
				this.DeleteItems = EnumUtilities.Parse<PermissionActionType>(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool DeleteItemsSpecified { get; set; }

		private bool canCreateItems;

		private bool canCreateSubFolders;

		private bool isFolderOwner;

		private bool isFolderVisible;

		private bool isFolderContact;

		private PermissionActionType editItems;

		private PermissionActionType deleteItems;
	}
}
