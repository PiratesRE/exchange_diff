using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "ContactsFolder", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ContactsFolderType : BaseFolderType
	{
		[IgnoreDataMember]
		[XmlElement]
		public PermissionReadAccess SharingEffectiveRights
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PermissionReadAccess>(ContactsFolderSchema.SharingEffectiveRights);
			}
			set
			{
				base.PropertyBag[ContactsFolderSchema.SharingEffectiveRights] = value;
			}
		}

		[DataMember(Name = "SharingEffectiveRights", EmitDefaultValue = false, Order = 1)]
		[XmlIgnore]
		public string SharingEffectiveRightsString
		{
			get
			{
				if (!this.SharingEffectiveRightsSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<PermissionReadAccess>(this.SharingEffectiveRights);
			}
			set
			{
				this.SharingEffectiveRights = EnumUtilities.Parse<PermissionReadAccess>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool SharingEffectiveRightsSpecified
		{
			get
			{
				return base.IsSet(ContactsFolderSchema.SharingEffectiveRights);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public PermissionSetType PermissionSet
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PermissionSetType>(FolderSchema.PermissionSet);
			}
			set
			{
				base.PropertyBag[FolderSchema.PermissionSet] = value;
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.ContactsFolder;
			}
		}
	}
}
