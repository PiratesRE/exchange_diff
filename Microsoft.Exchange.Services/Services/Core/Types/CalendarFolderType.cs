using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "CalendarFolder", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class CalendarFolderType : BaseFolderType
	{
		[IgnoreDataMember]
		[XmlElement]
		public CalendarPermissionReadAccess SharingEffectiveRights
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<CalendarPermissionReadAccess>(CalendarFolderSchema.SharingEffectiveRights);
			}
			set
			{
				base.PropertyBag[ContactsFolderSchema.SharingEffectiveRights] = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "SharingEffectiveRights", EmitDefaultValue = false, Order = 1)]
		public string SharingEffectiveRightsString
		{
			get
			{
				if (!this.SharingEffectiveRightsSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<CalendarPermissionReadAccess>(this.SharingEffectiveRights);
			}
			set
			{
				this.SharingEffectiveRights = EnumUtilities.Parse<CalendarPermissionReadAccess>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool SharingEffectiveRightsSpecified
		{
			get
			{
				return base.IsSet(CalendarFolderSchema.SharingEffectiveRights);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public CalendarPermissionSetType PermissionSet
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<CalendarPermissionSetType>(CalendarFolderSchema.PermissionSet);
			}
			set
			{
				base.PropertyBag[CalendarFolderSchema.PermissionSet] = value;
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.CalendarFolder;
			}
		}
	}
}
