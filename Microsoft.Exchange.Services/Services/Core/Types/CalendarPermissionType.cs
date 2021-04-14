using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "CalendarPermission")]
	[Serializable]
	public class CalendarPermissionType : BasePermissionType
	{
		[IgnoreDataMember]
		[XmlElement]
		public CalendarPermissionReadAccess? ReadItems { get; set; }

		[DataMember(Name = "ReadItems", EmitDefaultValue = false, Order = 1)]
		[XmlIgnore]
		public string ReadItemsString
		{
			get
			{
				if (this.ReadItems == null)
				{
					return null;
				}
				return EnumUtilities.ToString<CalendarPermissionReadAccess>(this.ReadItems.Value);
			}
			set
			{
				this.ReadItems = new CalendarPermissionReadAccess?(EnumUtilities.Parse<CalendarPermissionReadAccess>(value));
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ReadItemsSpecified
		{
			get
			{
				return this.ReadItems != null;
			}
			set
			{
			}
		}

		[XmlElement("CalendarPermissionLevel")]
		[IgnoreDataMember]
		public CalendarPermissionLevelType CalendarPermissionLevel { get; set; }

		[XmlIgnore]
		[DataMember(Name = "CalendarPermissionLevel", EmitDefaultValue = false, Order = 2)]
		public string CalendarPermissionLevelString
		{
			get
			{
				return EnumUtilities.ToString<CalendarPermissionLevelType>(this.CalendarPermissionLevel);
			}
			set
			{
				this.CalendarPermissionLevel = EnumUtilities.Parse<CalendarPermissionLevelType>(value);
			}
		}
	}
}
